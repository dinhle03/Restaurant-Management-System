using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Ecommerce.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email không hợp lệ!")]
        [EmailAddress]
        [DisplayName("Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không hợp lệ!")]
        [DataType(DataType.Password)]
        [DisplayName("Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Remember Me?")]
        public bool RememberMe { get; set; }
    }
}
