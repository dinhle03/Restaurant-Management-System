namespace Ecommerce.DTO
{
    public class LoginResultDTO
    {
        public bool IsSuccess { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
    }
}
