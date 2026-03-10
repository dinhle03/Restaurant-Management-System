namespace Ecommerce.DTO
{
    public class CustomerResultDTO
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public CustomerDTO? Customer { get; set; }
    }
}
