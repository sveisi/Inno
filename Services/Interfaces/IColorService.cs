using Gridify;
using Inno.Models;
using Inno.ViewModels;
using System.Threading.Tasks;

namespace Inno.Services.Interfaces
{
    public interface IColorService : IBaseService<Color>
    {
        Paging<ColorView> Get(GridifyQuery gridify);
        Task<ColorView> GetColorAsync(int id);
        Task<Color> CreateAsync(ColorView v);
        Task<Color> UpdateAsync(ColorView v);
    }
}