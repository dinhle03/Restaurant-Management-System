using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System;

namespace Ecommerce.Models
{
    public class MenuCategory
    {
        [Key]
        [DisplayName("Mã loai ")]
        public int MenuCategoryId { get; set; }

        [Required(ErrorMessage = "Tên loại không được để trống")]
        public string MenuCategoryName { get; set; }
    }
}
