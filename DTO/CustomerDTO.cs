namespace Ecommerce.DTO
{
    public class CustomerDTO
    {
        public string CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int Gender { get; set; }
        public string? UrlImage { get; set; }
        public string RankName { get; set; }
        public int Point { get; set; }
        public int RankId { get; set; }
        public bool IsLocked { get; set; }

    }
}