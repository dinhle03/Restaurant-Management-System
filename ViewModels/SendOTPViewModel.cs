using System.ComponentModel.DataAnnotations;

namespace Ecommerce.ViewModels
{
    public class SendOTPViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
