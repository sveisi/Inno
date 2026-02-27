using AutoMapper;
using Inno.Models;
using Inno.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Inno.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMapper mapper;
        private readonly UserManager<User> userMgr;
        private readonly SignInManager<User> signInMgr;
        private readonly RoleManager<IdentityRole> roleMgr;

        public AccountController(IMapper mapper, UserManager<User> userMgr, SignInManager<User> signInMgr, RoleManager<IdentityRole> roleMgr)
        {
            this.mapper = mapper;
            this.userMgr = userMgr;
            this.signInMgr = signInMgr;
            this.roleMgr = roleMgr;
        }

        [Authorize(Roles = UserRoleName.Admin)]
        public IActionResult Index()
        {
            var users = userMgr.Users.Select(x => new UserView() { Id = x.Id, UserName = x.UserName });
            return View(users);
        }

        [Authorize(Roles = UserRoleName.Admin)]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = UserRoleName.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserView userView)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var u = new User()
                    {
                        UserName = userView.UserName,
                        EmailConfirmed = true,
                    };
                    
                    var res = await userMgr.CreateAsync(u, userView.Password);
                    if (res.Succeeded)
                    {
                        var roleRes = await userMgr.AddToRoleAsync(u, userView.UserRole.ToString());
                        if (!roleRes.Succeeded)
                        {
                            foreach (var err in roleRes.Errors)
                            {
                                ModelState.AddModelError("", err.Description);
                            }
                        }
                        else
                        {
                            return RedirectToAction(nameof(Index));
                        }
                    }
                    else
                    {
                        foreach (var err in res.Errors)
                        {
                            ModelState.AddModelError("", err.Description);
                        }
                    }
                }
                catch (SysException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(userView);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await userMgr.FindByIdAsync(id);
            if (user == null) return NotFound();

            return View(mapper.Map<UserEditView>(user));
        }

        [Authorize(Roles = UserRoleName.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditView userView)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await userMgr.FindByIdAsync(userView.Id);
                    user.UserName = userView.UserName;

                    var res = await userMgr.UpdateAsync(user);
                    if (res.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        foreach (var err in res.Errors)
                        {
                            ModelState.AddModelError("", err.Description);
                        }
                    }
                }
                catch (SysException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(userView);
        }

        public IActionResult ChangePassword(string userName)
        {
            var user = new ChangePasswordView() { UserName = userName };
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordView userModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //غیر از ادمین باشد فقط میتواند پسورد خودش را تغییر دهد
                    User user = null;
                    if (User.IsInRole(UserRoleName.Admin))
                        user = await userMgr.FindByNameAsync(userModel.UserName);
                    else
                        user = await userMgr.FindByNameAsync(User.Identity.Name);

                    if (user == null)
                        ModelState.AddModelError("", "Not Found");
                    else
                    {
                        user.PasswordHash = userMgr.PasswordHasher.HashPassword(user, userModel.Password);
                        var res = await userMgr.UpdateAsync(user);
                        if (res.Succeeded)
                        {
                            ViewData["PasswordChangedMsg"] = Resources.SharedResource.PasswordChangedMsg;
                        }
                        else
                        {
                            foreach (var err in res.Errors)
                            {
                                ModelState.AddModelError("", err.Description);
                            }
                        }
                    }
                }
                catch (SysException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(userModel);
        }

        [Authorize(Roles = UserRoleName.Admin)]
        [HttpGet]
        public async Task<IActionResult> ChangeActive(string id)
        {
            var res = false;
            try
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                var user = await userMgr.FindByIdAsync(id);
                if (user.NormalizedUserName != "ADMIN")
                {
                    var identityRes = await userMgr.UpdateAsync(user);
                }
            }
            catch (SysException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Internal error!");
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = UserRoleName.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var res = false;
            try
            {
                if (string.IsNullOrEmpty(id))
                    return NotFound();

                var user = await userMgr.FindByIdAsync(id);
                if (user.NormalizedUserName != "ADMIN")
                {
                    var rolesForUser = await userMgr.GetRolesAsync(user);
                    //var logins =await  userMgr.GetLoginsAsync(user);

                    if (rolesForUser.Count > 0)
                    {
                        await userMgr.RemoveFromRolesAsync(user, rolesForUser);
                    }

                    var identityRes = await userMgr.DeleteAsync(user);
                    res = identityRes.Succeeded;
                }
            }
            catch (SysException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Internal error!");
            }
            //return new JsonResult(new { success = res });
            return RedirectToAction("Index");
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
                        // سپس در متد اصلی خود اینگونه استفاده کنید:
                        await CreateRoleIfNotExists(roleMgr, UserRoleName.Admin);
                        await CreateRoleIfNotExists(roleMgr, UserRoleName.Customer);

                        var u = new User()
                        {
                            UserName = "Admin",
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

        async Task CreateRoleIfNotExists(RoleManager<IdentityRole> roleMgr, string roleName)
        {
            if (!await roleMgr.RoleExistsAsync(roleName))
            {
                await roleMgr.CreateAsync(new IdentityRole(roleName));
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
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await userMgr.FindByEmailAsync(email);
            if (user == null) return Json(true);
            return Json("ایمیل وارد شده از قبل موجود است");
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