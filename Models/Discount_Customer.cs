namespace Ecommerce.Models
{
    public class Discount_Customer
    {
        public string DiscountId { get; set; }
        public Discount Discount { get; set; }

        public string CustomerId { get; set; }
        public Customer Customer { get; set; }

        public bool isUsed { get; set; }
    }
}
