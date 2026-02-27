using Microsoft.AspNetCore.Identity;

namespace Inno.Models
{
    public class User : IdentityUser
    {
        public int? CustomerId { get; set; }
    }
}