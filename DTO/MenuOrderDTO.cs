using Ecommerce.Models;

namespace Ecommerce.DTO
{
    public class MenuOrderDTO
    {
        public class FoodSizeDto
        {
            public int FoodSizeId { get; set; }
            public string FoodSizeName { get; set; }
            public decimal Price { get; set; }
        }

        public class MenuDto
        {
            public int MenuId { get; set; }
            public string MenuName { get; set; }
            public MenuCategory MenuCategory { get; set; }

            public List<string>? Images { get; set; } = new();
            public List<FoodSizeDto> Variants { get; set; } = new();
        }
    }
}
