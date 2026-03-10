using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Ecommerce.Models
{
    public class CustomerRank
    {
        [Key]
        [DisplayName("Mã hạng")]
        public int RankId { get; set; }

        [DisplayName("Tên Hạng")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Tên của hạng phải từ 1 đến 200 ký tự!")]
        [Required(ErrorMessage = "Vui lòng nhập tên hạng")]
        public string RankName { get; set; }

        [DisplayName("Điểm hạng")]
        [Range(0, int.MaxValue, ErrorMessage = "Điểm của 1 hạng không được là 1 số âm.")]
        [Required(ErrorMessage = "Hãy nhập điểm hạng của bạn!")]
        public int RankPoint { get; set; }

        public int? RankDiscount { get; set; }
    }
}
