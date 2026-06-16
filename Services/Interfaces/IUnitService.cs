using Gridify;
using Inno.Models;
using Inno.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inno.Services.Interfaces
{
    public interface IUnitService : IBaseService<Unit>
    {
        Task<List<LookupView<int>>> GetLookupAsync();
    }
}