using Gridify;
using Inno.Helper;
using Inno.Models;
using Inno.ViewModels;
using System.Threading.Tasks;

namespace Inno.Services.Interfaces
{
    public interface IAccountService : IBaseService<User>
    {
        Paging<UserListView> Get(GridifyQuery gridify);
        Task<Result> CreateAsync(UserView userView);
        Task<Result> UpdateAsync(UserEditView userView);
        Task<Result> ChangePasswordAsync(string loggedInUserName, string userName, string newPass);
        Task<Result> DeleteAsync(string userName);
    }
}