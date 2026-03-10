using Ecommerce.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Ecommerce.ViewModels
{
    public class MenuViewModel
    {
        public int MenuId { get; set; }

        public string MenuName { get; set; }

        public string? Detail { get; set; }

        public string MenuCategoryName { get; set; }
        // 👇 Thêm 2 dòng này:
        public string? MainImageUrl { get; set; }          // ảnh chính hiển thị ở danh sách
        public List<string>? ImageUrls { get; set; }
    }
}
