using Swashbuckle.AspNetCore.Annotations;

namespace Ecommerce.ViewModels
{
    public class UpdateCustomerViewModel
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Gender { get; set; }

        [SwaggerIgnore]
        public string? UrlImage { get; set; }
        public int Point { get; set; }
    }
}
