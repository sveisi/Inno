using System.ComponentModel.DataAnnotations;

namespace Inno.ViewModels
{
    public class ChangePasswordView
    {
        [Required]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}