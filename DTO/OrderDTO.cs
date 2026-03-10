namespace Ecommerce.DTO
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public string TableName { get; set; }
    }
}
