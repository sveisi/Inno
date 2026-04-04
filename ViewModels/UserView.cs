using Inno.Types;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Inno.ViewModels
{
    public class UserView
    {
        public string Id { get; set; }

        [Required]
        [Remote("IsUserNameInUse", "Account")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        [Required]
        public UserRoleType UserRole { get; set; }

        [Required]
        [DisplayName("Name")]
        public string FullName { get; set; }

        [DisplayName("Customer")]
        public int? CustomerId { get; set; }

        [Required]
        [DisplayName("Active")]
        public bool IsActive { get; set; }

        public List<LookupView<int>> Customers { get; set; }
    }
}