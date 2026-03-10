using System.Net.Mail;
using System.Net;
using Ecommerce.Services.Mail;

public class MailService : IMailService
{
    private readonly SmtpClient smtpClient;

    public MailService()
    {
        smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("luuquangphu125@gmail.com", "jzfh oiee aevq ufyx"),
            EnableSsl = true
        };
    }

    public async Task<(bool IsSuccess, string Message)> SendMailAsync(string toEmail, string subject, string body)
    {
        try
        {
            using var mailMessage = new MailMessage("luuquangphu125@gmail.com", toEmail, subject, body)
            {
                IsBodyHtml = true
            };

            await smtpClient.SendMailAsync(mailMessage);
            return (true, "Gửi mail thành công");
        }
        catch (Exception ex)
        {
            return (false, $"Lỗi khi gửi mail: {ex.Message}");
        }
    }
}
