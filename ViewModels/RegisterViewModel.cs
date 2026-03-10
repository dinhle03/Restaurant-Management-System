using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Ecommerce.ViewModels
{
    public class RegisterViewModel
    {
        [DisplayName("Họ Tên")]
        [Required(ErrorMessage = "Vui lòng nhập Họ Tên!")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Họ tên phải từ 5 đến 50 ký tự!")]
        public string Name { get; set; }

        [DisplayName("Số điện thoại")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Số điện thoại chỉ chứa các ký tự số!.")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại!")]
        public string? PhoneNumber { get; set; }

        [DisplayName("Ngày sinh")]
        public DateTime? DateOfBirth { get; set; }

        [DisplayName("Giới tính")]
        [Range(0, 1, ErrorMessage = "Lựa chọn giới tính không hợp lệ!")]
        public int Gender { get; set; }

        public string? UrlImage { get; set; }

        [Required(ErrorMessage = "Email không hợp lệ!")]
        [EmailAddress]
        [DisplayName("Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không hợp lệ!")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "{0} must be at {2} at {1} character long.")]
        [DataType(DataType.Password)]
        [DisplayName("Mật khẩu")]
        [Compare("ConfirmPassword", ErrorMessage = "Xác nhận mật khẩu chưa trùng khớp.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu không hợp lệ!")]
        [DataType(DataType.Password)]
        [DisplayName("Xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; }

        [DisplayName("Điểm thưởng")]
        [Range(0, int.MaxValue, ErrorMessage = "Điểm thưởng không được là 1 số âm.")]
        public int? RankPoint { get; set; }
    }
}
