using AutoMapper;
using AutoMapper.QueryableExtensions;
using Gridify;
using Inno.Data;
using Inno.Helper;
using Inno.Models;
using Inno.Services.Interfaces;
using Inno.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inno.Services
{
    public class ProductService : BaseService<Product>, IProductService
    {
        private readonly IAttachmentService attachmentSrv;

        public ProductService(InnoContext ctx, IMapper mapper, IAttachmentService attachmentSrv)
            : base(ctx, mapper)
        {
            this.attachmentSrv = attachmentSrv;
        }

        public Paging<ProductListView> Get(GridifyQuery gridify)
        {
            var res = entities.AsNoTracking().ProjectTo<ProductListView>(mapper.ConfigurationProvider).Gridify(gridify);

            return res;
        }

        public Paging<InventoryView> GetInventory(GridifyQuery gridify, int storageId)
        {
            var query = entities.AsNoTracking()
                .Select(p => new InventoryView
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    ProductEnName = p.EnName,
                    UnitName = p.Unit.Name,

                    StorageId = storageId,

                    Qty = p.SKUs.Where(x => x.Location.StorageId == storageId)
                        .Sum(x => (decimal?)(x.CurrentQty - x.ReservedQty)) ?? 0
                })
                .Where(x => x.Qty > 0);

            return query.Gridify(gridify);
        }

        public async Task<ProductView> GetProductAsync(string code)
        {
            var res = await entities.Include(x => x.Images).ProjectTo<ProductView>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == code);

            return res;
        }

        public async Task<Product> CreateAsync(ProductView v)
        {
            var entity = mapper.Map<Product>(v);
            entity.Images = v.Images?
                .Select((x, i) => new ProductImage
                {
                    AttachmentId = x.Id,
                    SortOrder = i
                }).ToList() ?? new List<ProductImage>();

            await ExecuteInTransactionAsync(async () =>
            {
                await AddAsync(entity);

                var newIds = entity.Images.Select(x => x.AttachmentId).Distinct().ToList();

                await ctx.Attachments.Where(x => newIds.Contains(x.Id))
                    .ExecuteUpdateAsync(x => x.SetProperty(a => a.IsTemporary, false));

                await ctx.SaveChangesAsync();
            });

            return entity;
        }

        public async Task<Result> UpdateAsync(ProductView prdView)
        {
            var entity = await entities.Include(x => x.Images)
                .ThenInclude(x => x.Attachment)
                .FirstOrDefaultAsync(x => x.Id == prdView.Id);

            if (entity == null)
                return Result.Failure(Resources.SharedResource.RecordNotFoundMsg);

            mapper.Map(prdView, entity);

            await ExecuteInTransactionAsync(async () =>
            {
                var newIds = prdView.Images.Select(x => x.Id).Distinct().ToList();

                var currentIds = entity.Images.Select(x => x.AttachmentId).ToList();

                var deleted = entity.Images.Where(x => !newIds.Contains(x.AttachmentId)).ToList();
                //تصاویر کالا بصورت آبشاری هم حذف میشن و باید دقت داشت جهت حذف از ضمیمه به کالا است
                await attachmentSrv.DeleteAttachmentAsync(deleted.Select(x => x.AttachmentId).ToArray());

                //از کالکشن حذف شود تا عملیات حذف و مرتب سازی صحیح شود
                foreach (var img in deleted)
                    entity.Images.Remove(img);

                var added = newIds.Except(currentIds)
                    .Select(id => new ProductImage
                    {
                        ProductId = entity.Id,
                        AttachmentId = id
                    });

                foreach (var item in added)
                    entity.Images.Add(item);

                // مرتب‌سازی همه تصاویر
                foreach (var img in entity.Images)
                {
                    img.SortOrder = newIds.IndexOf(img.AttachmentId);
                }

                await ctx.Attachments
                    .Where(x => newIds.Contains(x.Id))
                    .ExecuteUpdateAsync(x => x.SetProperty(a => a.IsTemporary, false));

                await ctx.SaveChangesAsync();
            });

            return Result.Ok();
        }

        public override async Task<Product> DeleteAsync(object id)
        {
            var entity = await entities.Include(x => x.Images).ThenInclude(x => x.Attachment)
                .FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (entity == null)
                return null;

            var attachmentIds = entity.Images.Select(x => x.AttachmentId).ToArray();

            await ExecuteInTransactionAsync(async () =>
            {
                await attachmentSrv.DeleteAttachmentAsync(attachmentIds);

                entities.Remove(entity);

                await ctx.SaveChangesAsync();
            });

            return entity;
        }
    }
}