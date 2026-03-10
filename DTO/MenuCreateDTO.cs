using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTO
{
    public class MenuCreateDTO
    {
        [Required]
        public string MenuName { get; set; }

        [Required]
        public int MenuCategoryId { get; set; }

        public string? Detail { get; set; }

        // Danh sách ảnh upload
        public List<IFormFile>? Images { get; set; }
    }
}
