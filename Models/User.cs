using System;
using Microsoft.AspNetCore.Identity;

namespace Inno.Models
{
    public class User : IdentityUser<Guid>
    {
        public string FullName { get; set; }
        public int? CustomerId { get; set; }
        public bool IsActive { get; set; } = true;

        public Customer Customer { get; set; }
    }
}