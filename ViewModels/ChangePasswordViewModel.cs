using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Ecommerce.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "UserId không hợp lệ!")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Mật khẩu không hợp lệ!")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "{0} must be at {2} at {1} character long.")]
        [DataType(DataType.Password)]
        [DisplayName("Mật khẩu")]
        [Compare("ConfirmNewPassword", ErrorMessage = "Mật khẩu chưa trùng khớp.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu không hợp lệ!")]
        [DataType(DataType.Password)]
        [DisplayName("Xác nhận mật khẩu")]
        public string ConfirmNewPassword { get; set; }
    }
}
