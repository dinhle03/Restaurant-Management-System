using System.ComponentModel.DataAnnotations;

namespace Ecommerce.DTO
{
    public class CartItemDTO
    {
        public int FoodSizeId { get; set; }

        public int CartId { get; set; }

        public string MenuName { get; set; }

        public string FoodSizeName  { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng không được là 1 số âm và lớn hơn 0.")]
        public int Count { get; set; }

        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }
    }
}
