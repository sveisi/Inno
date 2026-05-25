using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Inno.Helper
{
    public interface IUserContextService
    {
        string UserId { get; }
        string UserName { get; }
        int? CustomerId { get; }
    }

    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string UserId
        {
            get
            {
                // دسترسی به HttpContext و User
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null) return null;

                var user = httpContext.User;
                if (user == null || !user.Identity.IsAuthenticated) return null;

                // تلاش برای یافتن ClaimTypes.NameIdentifier
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

                return userIdClaim;
            }
        }

        public string UserName => _httpContextAccessor.HttpContext?.User?.Identity?.Name;

        public int? CustomerId
        {
            get
            {
                var claimValue = _httpContextAccessor.HttpContext?.User?.FindFirst("CustomerId")?.Value;

                if (int.TryParse(claimValue, out int result))
                {
                    return result;
                }

                return null;
            }
        }
    }
}