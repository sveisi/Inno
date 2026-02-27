using Gridify;
using Inno.Models;
using Inno.ViewModels;
using System.Threading.Tasks;

namespace Inno.Services.Interfaces
{
    public interface ICategoryService : IBaseService<Category>
    {
        Paging<CategoryView> Get(GridifyQuery gridify);
        Task<CategoryView> GetCategoryAsync(int id);
        Task<Category> CreateAsync(string name, string enName);
        Task UpdateAsync(CategoryView category);
    }
}