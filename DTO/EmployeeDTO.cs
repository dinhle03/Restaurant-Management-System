namespace Ecommerce.DTO
{
    public class EmployeeDTO
    {
        public string Id { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? Gender { get; set; }
        public string? UrlImage { get; set; }
    }
}
