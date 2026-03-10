namespace Ecommerce.DTO
{
    public class UpdateOrderDTO
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int OrderId { get; set; }
        public decimal Total { get; set; }
    }
}
