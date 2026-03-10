using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models
{
    public class FoodImage
    {
        public int FoodImageId { get; set; }

        public string? UrlImage { get; set; }

        [ForeignKey("Menu")]
        public int MenuId { get; set; }
        public Menu Menu { get; set; }

        public int SortOrder { get; set; }// Thứ tự ảnh

        public bool MainImage { get; set; }
    }
}
