using AutoMapper;
using AutoMapper.QueryableExtensions;
using Gridify;
using Inno.Data;
using Inno.Helper;
using Inno.Models;
using Inno.Services.Interfaces;
using Inno.Types;
using Inno.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Inno.Services
{
    public class AccountService : BaseService<User>, IAccountService
    {
        private readonly UserManager<User> userMgr;
        private readonly RoleManager<Role> roleMgr;
        private readonly IUserContextService userContextSrv;

        public AccountService(InnoContext ctx, IMapper mapper, UserManager<User> userMgr, 
            RoleManager<Role> roleMgr, IUserContextService userContextSrv)
            : base(ctx, mapper)
        {
            this.userMgr = userMgr;
            this.roleMgr = roleMgr;
            this.userContextSrv = userContextSrv;
        }

        public Paging<UserListView> Get(GridifyQuery gridify)
        {
            var res = entities.ProjectTo<UserListView>(mapper.ConfigurationProvider).Gridify(gridify);

            return res;
        }

        public async Task<Result> CreateAsync(UserView userView)
        {
            var user = await userMgr.FindByNameAsync(userView.UserName);
            if (user != null)
                return Result.Failure(string.Format(Resources.SharedResource.Duplicate_0_Msg, userView.UserName));

            var u = new User()
            {
                UserName = userView.UserName,
                EmailConfirmed = true,
                IsActive = true,
            };
            if (userView.UserRole == Types.UserRoleType.Customer)
            {
                if (!userView.CustomerId.HasValue)
                    return Result.Failure("مشتری مشخص نشده است");

                u.CustomerId = userView.CustomerId;

                var userCust = await entities.FirstOrDefaultAsync(x => x.CustomerId == u.CustomerId.Value);
                if (userCust != null)
                    return Result.Failure($"قبلا برای این مشتری کاربری به نام '{userCust.UserName}' تعریف شده است");

                var cust = await ctx.Customers.FirstOrDefaultAsync(x => x.Id == u.CustomerId.Value);
                if (cust == null)
                    return Result.Failure(string.Format(Resources.SharedResource._0_NotFoundMsg, Resources.SharedResource.Customer));

                u.FullName = cust.FullName;
            }

            var res = await userMgr.CreateAsync(u, userView.Password);
            if (!res.Succeeded)
                return Result.Failure(res.Errors.First().Description);

            var roleRes = await userMgr.AddToRoleAsync(u, userView.UserRole.ToString());
            if (!roleRes.Succeeded)
                return Result.Failure(roleRes.Errors.First().Description);

            return Result.Ok();
        }

        public async Task<Result> UpdateAsync(UserEditView userView)
        {
            var user = await userMgr.FindByNameAsync(userView.OrigUserName);
            if (user == null)
                return Result.Failure(Resources.SharedResource.RecordNotFoundMsg);

            if (user.CustomerId.HasValue)
            {
                var cust = await ctx.Customers.FirstOrDefaultAsync(x => x.Id == user.CustomerId.Value);
                if (cust == null)
                    return Result.Failure(string.Format(Resources.SharedResource._0_NotFoundMsg, Resources.SharedResource.Customer));
                //نباید نام کاربر با نام مشتری متفاوت باشد
                user.FullName = cust.FullName;
            }
            else
            {
                user.FullName = userView.FullName;
            }
            user.UserName = userView.UserName;
            user.IsActive = userView.IsActive;

            var res = await userMgr.UpdateAsync(user);
            if (!res.Succeeded)
                return Result.Failure(res.Errors.First().Description);

            return Result.Ok();
        }

        public async Task<Result> ChangePasswordAsync(string loggedInUserName, string userName, string newPass)
        {
            //اگر کاربر ادمین نباشد حق تغییر کلمه عبور دیگری را ندارد
            if (loggedInUserName != userName)
            {
                var loggedInUser = await userMgr.FindByNameAsync(loggedInUserName);
                if (loggedInUser == null)
                    return Result.Failure(Resources.SharedResource.RecordNotFoundMsg);

                var isAdmin = await userMgr.IsInRoleAsync(loggedInUser, UserRoleName.Admin);
                if (!isAdmin)
                    return Result.Failure(Resources.SharedResource.AccessDeniedMsg);
            }

            var user = await userMgr.FindByNameAsync(userName);
            if (user == null)
                return Result.Failure(Resources.SharedResource.RecordNotFoundMsg);

            //تغییر پسورد
            var removeResult = await userMgr.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
                return Result.Failure(removeResult.Errors.First().Description);

            var addResult = await userMgr.AddPasswordAsync(user, newPass);
            if (!addResult.Succeeded)
                return Result.Failure(addResult.Errors.First().Description);

            return Result.Ok();
        }

        public async Task<Result> ChangePasswordAsync(string currentPass, string newPass)
        {
            var user = await userMgr.FindByIdAsync(userContextSrv.UserId.Value.ToString());
            if (user == null)
                return Resources.SharedResource.RecordNotFoundMsg;

            var isCorrect = await userMgr.CheckPasswordAsync(user, currentPass);
            if (!isCorrect)
                return "گذرواژه فعلی اشتباه است.";

            var res = await userMgr.ChangePasswordAsync(user, currentPass, newPass);
            if (!res.Succeeded)
                return Result.Failure(res.Errors.First().Description);

            return Result.Ok();
        }

        public async Task<Result> DeleteAsync(string userName)
        {
            var user = await userMgr.FindByNameAsync(userName);
            if (user == null)
                return Resources.SharedResource.RecordNotFoundMsg;

            // check if user is admin
            if (await userMgr.IsInRoleAsync(user, UserRoleName.Admin))
            {
                var admins = await userMgr.GetUsersInRoleAsync(UserRoleName.Admin);

                if (admins.Count <= 1)
                    return Result.Failure("حداقل یک کاربر ادمین باید در سیستم وجود داشته باشد.");
            }

            var rolesForUser = await userMgr.GetRolesAsync(user);
            if (rolesForUser.Count > 0)
                await userMgr.RemoveFromRolesAsync(user, rolesForUser);

            var identityRes = await userMgr.DeleteAsync(user);
            if (!identityRes.Succeeded)
                return identityRes.Errors.First().Description;

            return Result.Ok();
        }
    }
}