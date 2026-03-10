namespace Ecommerce.Services.Mail
{
    public interface IMailService
    {
        Task<(bool IsSuccess, string Message)> SendMailAsync(string toEmail, string subject, string body);
    }
}
