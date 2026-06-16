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
    public class ProductService : BaseService<Product>, IProductService
    {
        public ProductService(InnoContext ctx, IMapper mapper) : base(ctx, mapper)
        {
        }

        public Paging<ProductListView> Get(GridifyQuery gridify)
        {
            var res = entities.AsNoTracking().ProjectTo<ProductListView>(mapper.ConfigurationProvider).Gridify(gridify);

            return res;
        }

        public async Task<ProductView> GetProductAsync(string code)
        {
            var res = await entities.ProjectTo<ProductView>(mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x.Id == code);

            return res;
        }

        public async Task<Product> CreateAsync(ProductView v)
        {
            var n = mapper.Map<Product>(v);
            var res = await AddAsync(n);

            return res;
        }

        public async Task<Result> UpdateAsync(ProductView v)
        {
            var n = mapper.Map<Product>(v);
            var res = await UpdateAsync(n);

            return Result.Ok();
        }
    }
}