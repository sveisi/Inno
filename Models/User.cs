using Microsoft.AspNetCore.Identity;

namespace Inno.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public int? CustomerId { get; set; }
        public bool IsActive { get; set; }

        public Customer Customer { get; set; }
    }
}