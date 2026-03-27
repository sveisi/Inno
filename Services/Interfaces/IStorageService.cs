using Gridify;
using Inno.Models;
using Inno.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inno.Services.Interfaces
{
    public interface IStorageService : IBaseService<Storage>
    {
        Paging<StorageView> Get(GridifyQuery gridify);
        Task<List<StorageView>> GetAllStorageAsync();
        Task<StorageView> GetStorageAsync(string id);
        Task<Storage> CreateAsync(StorageView v);
        Task<Storage> UpdateAsync(StorageView v);
    }
}