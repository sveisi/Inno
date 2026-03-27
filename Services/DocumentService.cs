using AutoMapper;
using AutoMapper.QueryableExtensions;
using Gridify;
using Inno.Data;
using Inno.Helper;
using Inno.Models;
using Inno.Services.Interfaces;
using Inno.Types;
using Inno.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inno.Services
{
    public class DocumentService : BaseService<Document>, IDocumentService
    {
        private readonly IUserContextService userContextSrv;

        public DocumentService(InnoContext ctx, IMapper mapper, IUserContextService userContextSrv) : base(ctx, mapper)
        {
            this.userContextSrv = userContextSrv;
        }

        public Paging<DocumentListView> Get(GridifyQuery gridify)
        {
            var res = entities.ProjectTo<DocumentListView>(mapper.ConfigurationProvider).Gridify(gridify);

            return res;
        }

        public async Task<Result<PurchaseReceiptCreateView>> GetPurchaseReceiptAsync(int id)
        {
            var doc = await entities.Where(x => x.Id == id)
                .Include(x => x.DocumentItems)
                .ProjectTo<PurchaseReceiptCreateView>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            var msg = GetChangeDocumentMsg(new Document() { ConfirmedAt = doc.ConfirmedAt, CreatedBy = doc.CreatedBy });
            if (msg != null)
                return Result<PurchaseReceiptCreateView>.Failure(msg);

            return Result<PurchaseReceiptCreateView>.Ok(doc);
        }

        public async Task<Result<DocumentItemView>> AddToPurchaseReceiptAsync(PurchaseReceiptCreateView newItem)
        {
            var prd = await ctx.SKUs.Where(x => x.Id == newItem.SKUId)
                .Select(x => new
                {
                    x.ProductId,
                    x.Product.Name,
                    x.Product.EnName,
                    x.Product.UnitId,
                    x.InitQty
                }).FirstOrDefaultAsync();
            if (prd == null)
                return Result<DocumentItemView>.Failure(string.Format(Resources.SharedResource._0_NotFoundMsg, Resources.SharedResource.SKUId));
            
            //لوکیشن مورد نظر وجود دارد
            var loc = await ctx.Locations.FirstOrDefaultAsync(x => x.Id.ToUpper() == newItem.LocationId.ToUpper());
            if (loc == null)
                return Result<DocumentItemView>.Failure(string.Format(Resources.SharedResource._0_NotFoundMsg, Resources.SharedResource.Location));
            newItem.LocationId = loc.Id;

            //سند جدید است یا اینکه به سند موجود قلمی اضافه میشود
            Document doc = null;
            if (newItem.DocumentId > 0)
            {
                doc = await entities.FirstOrDefaultAsync(x => x.Id == newItem.DocumentId);
                var msg = GetChangeDocumentMsg(doc);
                if (msg != null)
                    return Result<DocumentItemView>.Failure(msg);
            }
            else
            {
                doc = new Document
                {
                    DocumentTypeId = Types.DocumentType.PurchaseReceipt.GetHashCode(),
                    StorageId = newItem.StorageId
                };
                entities.Add(doc);
            }
            //طاقه قبلا در سند دیگر یا در همان سند رسید نشده است
            var itemExist = await ctx.DocumentItems.FirstOrDefaultAsync(x => x.SKUId == newItem.SKUId);
            if (itemExist != null)
            {
                if (itemExist.DocumentId == newItem.DocumentId)
                    return Result<DocumentItemView>.Failure(string.Format(Resources.SharedResource.Duplicate_0_Msg, Resources.SharedResource.SKUId));
                else
                {
                    if (prd.UnitId == UnitMeasurement.Meter.GetHashCode())
                        return Result<DocumentItemView>.Failure($" طاقه مورد نظر قبلا در سند {itemExist.DocumentId} رسید شده است");
                }
            }
            //برای طاقه همان مقدار قبلی در تعریف طاقه رسید میشود و برای عدد مقدار صحیح
            var qty = prd.UnitId == UnitMeasurement.Meter.GetHashCode() ? prd.InitQty : ((int)newItem.Qty);

            var docItem = new DocumentItem
            {
                RowNo = 1,
                SKUId = newItem.SKUId,
                Qty = qty,
                LocationId = newItem.LocationId
            };
            doc.DocumentItems ??= new List<DocumentItem>();
            doc.DocumentItems.Add(docItem);

            await ctx.SaveChangesAsync();

            var res = new DocumentItemView
            {
                Id = docItem.Id,
                DocumentId = docItem.DocumentId,
                RowNo = docItem.RowNo,
                SKUId = docItem.SKUId,
                ProductId = prd.ProductId,
                ProductName = prd.Name,
                Qty = docItem.Qty,
                LocationId = docItem.LocationId
            };
            return Result<DocumentItemView>.Ok(res);
        }

        public async Task<Result<int>> DeleteAsync(int id)
        {
            var doc = await entities.Where(x => x.Id == id).FirstOrDefaultAsync();

            var msg = GetChangeDocumentMsg(doc);
            if (msg != null)
                return Result<int>.Failure(msg);

            entities.Remove(doc);
            await ctx.SaveChangesAsync();

            return Result<int>.Ok(1);
        }

        public async Task<Result<DocumentListView>> ConfirmAsync(int id)
        {
            var doc = await entities.Where(x => x.Id == id)
                .ProjectTo<DocumentListView>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
            if (doc == null)
                return Result<DocumentListView>.Failure(Resources.SharedResource.RecordNotFoundMsg);
            if (doc.ConfirmedAt.HasValue)
                return Result<DocumentListView>.Failure("سند قبلا تایید شده است.");

            doc.ConfirmedAt = System.DateTime.Now;
            await entities.Where(x => x.Id == id)
                .ExecuteUpdateAsync(x => x.SetProperty(p => p.ConfirmedAt, System.DateTime.Now));

            return Result<DocumentListView>.Ok(doc);
        }

        public async Task<Result<DocumentItem>> DeleteItemAsync(int id)
        {
            var item = await ctx.DocumentItems.Where(x => x.Id == id)
                .Include(x => x.Document)
                .FirstOrDefaultAsync();

            if (item == null)
                return Result<DocumentItem>.Failure(Resources.SharedResource.RecordNotFoundMsg);
            var msg = GetChangeDocumentMsg(item.Document);
            if (msg != null)
                return Result<DocumentItem>.Failure(msg);

            ctx.DocumentItems.Remove(item);
            await ctx.SaveChangesAsync();

            return Result<DocumentItem>.Ok(item);
        }

        private string GetChangeDocumentMsg(Document doc)
        {
            if (doc == null)
                return Resources.SharedResource.RecordNotFoundMsg;
            if (doc.ConfirmedAt.HasValue)
                return Resources.SharedResource.DocumentConfirmedMsg;
            if (doc.CreatedBy != userContextSrv.UserId)
                return Resources.SharedResource.AccessDeniedMsg;

            return null;
        }
    }
}