namespace Ecommerce.DTO
{
    public class VerifyOTPResult
    {
        public string UserId { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
