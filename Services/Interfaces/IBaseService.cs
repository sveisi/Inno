using Inno.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inno.Services.Interfaces
{
    public interface IBaseService<TEntity> where TEntity : class
    {
        Task<IList<TEntity>> GetAsync();
        //Task<int> SaveChangesAsync();
        //Task<TEntity> AddAsync(TEntity entity);
        //Task<TEntity> UpdateAsync(TEntity entity);
        //Task DeleteAsync(TEntity entity);
        Task<TEntity> DeleteAsync(object id);
    }
}