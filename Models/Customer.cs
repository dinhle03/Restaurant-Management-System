using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class Customer : Users
    {
        [Range(0, int.MaxValue, ErrorMessage = "Điểm thưởng không được là 1 số âm.")]
        [Required(ErrorMessage = "Vui lòng nhập điểm thưởng của khách hàng")]
        public int Point { get; set; }

        public int RankId { get; set; }
        public CustomerRank CustomerRank { get; set; }

        public virtual ICollection<Discount_Customer> Discount_Customer { get; set; }
    }
}
