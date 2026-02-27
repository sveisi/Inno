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
    public class CategoryService : BaseService<Category>, ICategoryService
    {
        public CategoryService(InnoContext ctx, IMapper mapper) : base(ctx, mapper)
        {
        }

        public Paging<CategoryView> Get(GridifyQuery gridify)
        {
            var res = entities.AsNoTracking().ProjectTo<CategoryView>(mapper.ConfigurationProvider).Gridify(gridify);

            return res;
        }

        public async Task<CategoryView> GetCategoryAsync(int id)
        {
            var res = await entities.ProjectTo<CategoryView>(mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x.Id == id);

            return res;
        }

        public async Task<Category> CreateAsync(string name, string enName)
        {
            var n = new Category() { Name = name, EnName = enName };
            var res = await AddAsync(n);

            return res;
        }

        public async Task UpdateAsync(CategoryView category)
        {
            /* var entity = await entities.FirstAsync(x => x.Id == v.Id); // tracked
             mapper.Map(v, entity); //بهتر است خوانده شود سپس مپ شود تا همه پراپرتی ها تغییر نکنند و باگ‌های خاموش نسازه.
             await ctx.SaveChangesAsync();
             return entity;*/
            await entities.Where(x => x.Id == category.Id)
               .ExecuteUpdateAsync(s => s
               .SetProperty(x => x.Name, category.Name)
               .SetProperty(x => x.EnName, category.EnName));
        }
    }
}