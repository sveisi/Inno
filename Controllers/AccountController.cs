using AutoMapper;
using Inno.Helper;
using Inno.Models;
using Inno.Services.Interfaces;
using Inno.Types;
using Inno.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Inno.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IMapper mapper;
        private readonly IAccountService accSrv;
        private readonly UserManager<User> userMgr;
        private readonly SignInManager<User> signInMgr;
        private readonly ICustomerService customerSrv;
        private readonly RoleManager<Role> roleMgr;

        public AccountController(IMapper mapper, IAccountService accSrv, UserManager<User> userMgr, SignInManager<User> signInMgr,
            ICustomerService customerSrv, RoleManager<Role> roleMgr)
        {
            this.mapper = mapper;
            this.accSrv = accSrv;
            this.userMgr = userMgr;
            this.signInMgr = signInMgr;
            this.customerSrv = customerSrv;
            this.roleMgr = roleMgr;
        }

        [Authorize(Roles = UserRoleName.Admin)]
        public IActionResult Index() => View();

        [Authorize(Roles = UserRoleName.Admin)]
        [HttpPost]
        public IActionResult GetData(DataTablePostModel dt)
        {
            var list = accSrv.Get(dt.Gridify());

            var jsonData = new
            {
                draw = dt.draw,
                recordsFiltered = list.Count,
                recordsTotal = list.Count,
                data = list.Data
            };

            return Ok(jsonData);
        }

        [Authorize(Roles = UserRoleName.Admin)]
        public async Task<IActionResult> Create()
        {
            var v = new UserView { IsActive = true };

            v.Customers = await customerSrv.GetLookupAsync();
            return View("_Create", v);
        }

        [Authorize(Roles = UserRoleName.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserView userView)
        {
            if (!ModelState.IsValid)
                return GetModelError();

            var res = await accSrv.CreateAsync(userView);
            return res.ToActionResult();
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return AjaxFail(Resources.SharedResource.RecordNotFoundMsg);

            var user = await userMgr.FindByNameAsync(id);
            if (user == null) return AjaxFail(Resources.SharedResource.RecordNotFoundMsg);

            return View("_Edit", mapper.Map<UserEditView>(user));
        }

        [Authorize(Roles = UserRoleName.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditView userView)
        {
            if (!ModelState.IsValid)
                return GetModelError();

            var res = await accSrv.UpdateAsync(userView);
            return res.ToActionResult();
        }

        public IActionResult ChangePassword(string id)
        {
            var user = new ChangePasswordView() { UserName = id };
            return View("_ChangePassword", user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordView userModel)
        {
            if (!ModelState.IsValid)
                return GetModelError();

            var res = await accSrv.ChangePasswordAsync(User.Identity.Name, userModel.UserName, userModel.Password);
            return res.ToActionResult();
        }

        [Authorize(Roles = UserRoleName.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (!ModelState.IsValid)
                return GetModelError();

            var res = await accSrv.DeleteAsync(id);
            return res.ToActionResult();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (signInMgr.IsSignedIn(User))
                return RedirectToAction("Index", "Home");

            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginView model, string returnUrl = null)
        {
            if (signInMgr.IsSignedIn(User))
                return RedirectToAction("Index", "Home");

            ViewData["returnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await userMgr.FindByNameAsync(model.UserName);
                if (user != null && !user.IsActive)
                {
                    ViewData["ErrorMessage"] = Resources.SharedResource.AccountIsLockedOutMsg;
                    return View(model);
                }

                var result = await signInMgr.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, true);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    if (result.IsLockedOut)
                    {
                        ViewData["ErrorMessage"] = Resources.SharedResource.AccountIsLockedOutMsg;
                        return View(model);
                    }

                    var admin = await userMgr.FindByNameAsync("admin");
                    if (admin == null)
                    {
                        await CreateRoleIfNotExists(roleMgr, UserRoleName.Admin);
                        await CreateRoleIfNotExists(roleMgr, UserRoleName.Storekeeper);
                        await CreateRoleIfNotExists(roleMgr, UserRoleName.Customer);

                        var u = new User()
                        {
                            UserName = "Admin",
                            FullName = "امیر",
                            EmailConfirmed = true,
                        };
                        await userMgr.CreateAsync(u, "amir");
                        await userMgr.AddToRoleAsync(u, UserRoleName.Admin);
                    }
                }

                ModelState.AddModelError("", Resources.SharedResource.LoginFaildMsg);
            }
            return View(model);
        }

        async Task CreateRoleIfNotExists(RoleManager<Role> roleMgr, string roleName)
        {
            if (!await roleMgr.RoleExistsAsync(roleName))
            {
                await roleMgr.CreateAsync(new Role(roleName));
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await signInMgr.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public async Task<IActionResult> IsUserNameInUse(string userName, string OrigUserName)
        {
            if (userName == OrigUserName) return Json(true);
            var user = await userMgr.FindByNameAsync(userName);
            if (user == null) return Json(true);
            return Json(Resources.SharedResource.UsernameNotAvailableMsg);
        }
    }
}