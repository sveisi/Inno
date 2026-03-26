using Inno.Types;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Inno.ViewModels
{
    public class UserEditView
    {
        public string Id { get; set; }
        [Required]
        [Remote("IsUserNameInUse", "Account", AdditionalFields = nameof(OrigUserName))]
        public string UserName { get; set; }

        /// <summary>برای کنترل در حالت ویرایش</summary>
        public string OrigUserName { get; set; }

        [Required]
        public UserRoleType UserRole { get; set; }

        [Required]
        [DisplayName("Name")]
        public string FullName { get; set; }

        [DisplayName("Contact")]
        public int? ContactId { get; set; }

        [Required]
        [DisplayName("Active")]
        public bool IsActive { get; set; }
    }
}