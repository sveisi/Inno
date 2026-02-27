using Gridify;
using Inno.Models;
using Inno.ViewModels;
using System.Threading.Tasks;

namespace Inno.Services.Interfaces
{
    public interface ISKUService : IBaseService<SKU>
    {
        Paging<SKUListView> Get(GridifyQuery gridify);
        Task<SKUView> GetSKUAsync(string id);
        Task<SKU> CreateAsync(SKUView v);
        Task<SKU> UpdateAsync(SKUView v);
    }
}