namespace Ecommerce.Models
{
    public class OTP
    {
        public int OTPId { get; set; }

        public string OTPCode { get; set; }

        public string UserId { get; set; }
        public Users Users { get; set; }

        public DateTime TimeCreate { get; set; }

        public string Status { get; set; }
    }
}
