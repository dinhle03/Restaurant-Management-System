namespace Ecommerce.DTO
{
    public class ConfirmPaymentDTO
    {
        public int OrderId { get; set; }
        public decimal Total {  get; set; }
        public string DiscountId { get; set; }
        public string PaymentMethod { get; set; }
    }
}
