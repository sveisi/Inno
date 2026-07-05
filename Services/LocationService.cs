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
    public class LocationService : BaseService<Location>, ILocationService
    {
        public LocationService(InnoContext ctx, IMapper mapper) : base(ctx, mapper)
        {
        }

        public Paging<LocationView> Get(GridifyQuery gridify)
        {
            var res = entities.AsNoTracking().ProjectTo<LocationView>(mapper.ConfigurationProvider).Gridify(gridify);

            return res;
        }

        public async Task<LocationView> GetLocationAsync(string id)
        {
            var res = await entities.ProjectTo<LocationView>(mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x.Id == id);

            return res;
        }

        public async Task<Location> CreateAsync(LocationView v)
        {
            var n = mapper.Map<Location>(v);
            n.Id = n.Id.ToUpper();
            var res = await AddAsync(n);

            return res;
        }

        public async Task<Location> UpdateAsync(LocationView v)
        {
            var n = mapper.Map<Location>(v);
            var res = await UpdateAsync(n);

            return res;
        }
    }
}