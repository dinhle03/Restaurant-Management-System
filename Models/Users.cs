using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Ecommerce.Models
{
    public class Users : IdentityUser
    {
        [DisplayName("Họ Tên")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Họ tên phải từ 5 đến 50 ký tự!")]
        [Required(ErrorMessage = "Vui lòng nhập Họ Tên!")]
        public string Name { get; set; }

        [DisplayName("Số điện thoại")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Số điện thoại chỉ chứa các ký tự số!.")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại!")]
        public string? PhoneNumber { get; set; }

        [DisplayName("Ngày sinh")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Vui lòng chọn ngày sinh!")]
        public DateTime? DateOfBirth { get; set; }

        [DisplayName("Giới tính")]
        public int Gender { get; set; }

        public string? UrlImage { get; set; }
    }
}
