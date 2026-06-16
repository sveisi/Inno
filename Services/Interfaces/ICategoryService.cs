using Gridify;
using Inno.Helper;
using Inno.Models;
using Inno.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inno.Services.Interfaces
{
    public interface ICategoryService : IBaseService<Category>
    {
        Paging<CategoryView> Get(GridifyQuery gridify);
        Task<CategoryView> GetCategoryAsync(int id);
        Task<List<LookupView<int>>> GetLookupAsync();
        Task<Result<Category>> CreateAsync(string name, string enName);
        Task UpdateAsync(CategoryView category);
    }
}