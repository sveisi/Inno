using System.ComponentModel.DataAnnotations;

namespace Inno.ViewModels
{
    public class LoginView
    {
        [Required]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe{ get; set; }
    }
}