using Gridify;
using Inno.Models;
using Inno.ViewModels;
using System.Threading.Tasks;

namespace Inno.Services.Interfaces
{
    public interface ILocationService : IBaseService<Location>
    {
        Paging<LocationView> Get(GridifyQuery gridify);
        Task<LocationView> GetLocationAsync(string id);
        Task<Location> CreateAsync(LocationView v);
        Task<Location> UpdateAsync(LocationView v);
    }
}