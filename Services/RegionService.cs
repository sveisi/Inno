using AutoMapper;
using AutoMapper.QueryableExtensions;
using Gridify;
using Inno.Data;
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
    public class RegionService : BaseService<Region>, IRegionService
    {
        public RegionService(InnoContext ctx, IMapper mapper) : base(ctx, mapper)
        {
        }

        public Paging<RegionView> Get(GridifyQuery gridify)
        {
            var res = entities.AsNoTracking().ProjectTo<RegionView>(mapper.ConfigurationProvider).Gridify(gridify);

            return res;
        }

        public async Task<RegionView> GetRegionAsync(int id)
        {
            var res = await entities.ProjectTo<RegionView>(mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x.Id == id);

            return res;
        }

        public async Task<List<LookupView<int>>> GetLookupAsync(RegionType type)
        {
            return await entities.Where(x => x.Type == type)
                .Select(x => new LookupView<int>(x.Id, x.Name, x.EnName))
                .ToListAsync();
        }

        public override async Task<IList<Region>> GetAsync()
        {
            return await entities.OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<Region> CreateAsync(RegionView v)
        {
            var n = mapper.Map<Region>(v);
            var res = await AddAsync(n);

            return res;
        }

        public async Task<Region> UpdateAsync(RegionView v)
        {
            var n = mapper.Map<Region>(v);
            var res = await UpdateAsync(n);

            return res;
        }
    }
}