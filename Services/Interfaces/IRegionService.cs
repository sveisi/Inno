using Gridify;
using Inno.Models;
using Inno.Types;
using Inno.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inno.Services.Interfaces
{
    public interface IRegionService : IBaseService<Region>
    {
        Paging<RegionView> Get(GridifyQuery gridify);
        Task<RegionView> GetRegionAsync(int id);
        Task<List<LookupView<int>>> GetLookupAsync(RegionType type);
        Task<Region> CreateAsync(RegionView v);
        Task<Region> UpdateAsync(RegionView v);
    }
}