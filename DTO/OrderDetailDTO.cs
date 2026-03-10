namespace Ecommerce.DTO
{
    public class OrderDetailDTO
    {
        public string FoodName { get; set; }
        public int Quantity { get; set; }
        public string? UrlImage { get; set; }
        public decimal Price { get; set; }
    }
}
