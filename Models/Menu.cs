using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Ecommerce.Models
{
    public class Menu
    {
        [Key]
        [DisplayName("Mã món ăn")]
        public int MenuId { get; set; }

        [DisplayName("Tên món ăn")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Tên món ăn phải từ 5 đến 100 ký tự!")]
        [Required(ErrorMessage = "Không được để trống tên món ăn!")]
        public string MenuName { get; set; }

        [DisplayName("Mô tả món ăn")]
        public string? Detail { get; set; }


        [Required(ErrorMessage = "Vui lòng chọn loại món ăn!")]
        public int MenuCategoryId { get; set; }
        [ValidateNever]
        public MenuCategory MenuCategory { get; set; }
        public ICollection<FoodImage>? FoodImages { get; set; }
    }
}
