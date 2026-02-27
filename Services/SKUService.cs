using AutoMapper;
using AutoMapper.QueryableExtensions;
using Gridify;
using Inno.Data;
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

        public async Task<SKU> CreateAsync(SKUView v)
        {
            var n = mapper.Map<SKU>(v);
            var res = await AddAsync(n);

            return res;
        }

        public async Task<SKU> UpdateAsync(SKUView v)
        {
            var n = mapper.Map<SKU>(v);
            var res = await UpdateAsync(n);

            return res;
        }
    }
}