using Inno.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;

public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, IdentityRole>
{
    public CustomUserClaimsPrincipalFactory(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IOptions<IdentityOptions> options)
        : base(userManager, roleManager, options)
    {
    }

    public override async Task<ClaimsPrincipal> CreateAsync(User user)
    {
        // ساخت پرنسیپال اصلی توسط کلاس پایه، شامل نقش‌ها، نام کاربری و سایر کلیم‌های استاندارد
        var principal = await base.CreateAsync(user);

        // اضافه کردن کلیم دلخواه
        if (user.CustomerId != null)
        {
            var identity = (ClaimsIdentity)principal.Identity;
            identity.AddClaim(new Claim("CustomerId", user.CustomerId.ToString()));
        }

        return principal;
    }
}
//میتوان در متد لاگین با این کد اینکار را کرد، ولی روش بالا حرفه ای تر و بدون خطاتر است
/*
    //لیست کلیم‌ها  
    var claims = new List<Claim>
    {
        //کلیم‌های استاندارد که نیاز داره که معمولا خودش اضافه می‌کنه ولی برای اطمینان
        new Claim(ClaimTypes.NameIdentifier, fullUser.Id),
        new Claim(ClaimTypes.Name, fullUser.UserName),                
        //اضافه کردن کلیم دلخواه 
        new Claim("CustomerId", fullUser.CustomerId?.ToString() ?? string.Empty)
    };

    //اضافه کردن نقش‌های کاربر به کلیم‌ها،اگر این کار رو نکنیم، ممکنه نقش‌ها در کوکی ذخیره نشن. 
    var roles = await userMgr.GetRolesAsync(fullUser);
    foreach (var role in roles)
    {
        claims.Add(new Claim(ClaimTypes.Role, role));
    }

    await signInMgr.SignInWithClaimsAsync(fullUser, model.RememberMe, claims);
*/