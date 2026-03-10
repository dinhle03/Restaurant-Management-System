namespace Ecommerce.Models
{
    public class Revenue_Order
    {
        public int RevenueId { get; set; }
        public Revenue Revenue { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}