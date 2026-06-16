using AutoMapper;
using AutoMapper.QueryableExtensions;
using Gridify;
using Inno.Data;
using Inno.Models;
using Inno.Services.Interfaces;
using Inno.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inno.Services
{
    public class ColorService : BaseService<Color>, IColorService
    {
        public ColorService(InnoContext ctx, IMapper mapper) : base(ctx, mapper)
        {
        }

        public Paging<ColorView> Get(GridifyQuery gridify)
        {
            var res = entities.AsNoTracking().ProjectTo<ColorView>(mapper.ConfigurationProvider).Gridify(gridify);

            return res;
        }

        public async Task<ColorView> GetColorAsync(int id)
        {
            var res = await entities.ProjectTo<ColorView>(mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x.Id == id);

            return res;
        }

        public async Task<List<LookupView<int>>> GetLookupAsync()
        {
            return await entities.Select(x => new LookupView<int>(x.Id, x.Name)).ToListAsync();
        }

        public async Task<Color> CreateAsync(ColorView v)
        {
            var n = mapper.Map<Color>(v);
            var res = await AddAsync(n);

            return res;
        }

        public async Task<Color> UpdateAsync(ColorView v)
        {
            var n = mapper.Map<Color>(v);
            var res = await UpdateAsync(n);

            return res;
        }
    }
}