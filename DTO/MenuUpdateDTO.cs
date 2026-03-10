using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTO
{
    public class MenuUpdateDTO
    {
        [Required]
        public int MenuId { get; set; }

        [Required]
        public string MenuName { get; set; }

        public string? Detail { get; set; }

        [Required]
        public int MenuCategoryId { get; set; }

        // JSON: { "keep": [...], "main": "..." }
        public string? ExistingImages { get; set; }

        public List<IFormFile>? Images { get; set; }
    }
}
