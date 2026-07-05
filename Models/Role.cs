using System;
using Microsoft.AspNetCore.Identity;

namespace Inno.Models
{
    public class Role : IdentityRole<Guid>
    {
        public Role()
        { }
        public Role(string roleName) : base(roleName)
        { }
    }
}