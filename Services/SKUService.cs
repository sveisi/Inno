using AutoMapper;
using AutoMapper.QueryableExtensions;
using Gridify;
using Inno.Data;
using Inno.Helper;
using Inno.Models;
using Inno.Services.Interfaces;
using Inno.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Inno.Services
{
    public class SKUService : BaseService<SKU>, ISKUService
    {
        public SKUService(InnoContext ctx, IMapper mapper) : base(ctx, mapper)
        {
        }

        public Paging<SKUListView> Get(GridifyQuery gridify)
        {
            var res = entities.AsNoTracking().ProjectTo<SKUListView>(mapper.ConfigurationProvider).Gridify(gridify);

            return res;
        }

        public async Task<SKUView> GetSKUAsync(string id)
        {
            var res = await entities.ProjectTo<SKUView>(mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x.Id == id);

            return res;
        }

        public async Task<Result<SKU>> CreateAsync(SKUView v)
        {
            if (v.InitQty <= 0)
                return Result<SKU>.Failure(Resources.SharedResource.InitQtyMsg);

            var dup = await entities.AnyAsync(x => x.Id == v.Id);
            if (dup)
                return Result<SKU>.Failure(string.Format(Resources.SharedResource.Duplicate_0_Msg, Resources.SharedResource.SKUId));

            var n = mapper.Map<SKU>(v);
            n.CurrentQty = 0;
            n.ReservedQty = 0;
            var res = await AddAsync(n);

            return Result<SKU>.Success(res);
        }

        public async Task<SKU> UpdateAsync(SKUView v)
        {
            var entity = await entities.FirstAsync(x => x.Id == v.Id);
            mapper.Map(v, entity);
            await ctx.SaveChangesAsync();

            return entity;
        }
    }
}