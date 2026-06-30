using Gridify;
using Inno.Models;
using Inno.Helper;
using Inno.ViewModels;
using System.Threading.Tasks;

namespace Inno.Services.Interfaces
{
    public interface IProductService : IBaseService<Product>
    {
        Paging<ProductListView> Get(GridifyQuery gridify);
        Paging<InventoryView> GetInventory(GridifyQuery gridify, int storageId);
        Task<ProductView> GetProductAsync(string code);
        Task<Product> CreateAsync(ProductView v);
        Task<Result> UpdateAsync(ProductView v);
    }
}