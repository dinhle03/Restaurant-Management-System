using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Ecommerce.Models
{
    public class Discount
    {
        [Key]
        public string DiscountId { get; set; }

        public string DiscountName { get; set; }
        
        public required string DiscountCategory { get; set; }//Phần trăm, tiền

        [Column(TypeName = "decimal(18,2)")]
        public required decimal DiscountPrice { get; set; }

        public DateTime DateEnd { get; set; }

        public DateTime DateStart { get; set; }

        //False: ngừng áp dụng, true: áp dụng
        public bool DiscountStatus { get; set; }
        // ⚡ Thêm mới:
        [Range(0, int.MaxValue, ErrorMessage = "Điểm quy đổi phải >= 0")]
        [ValidateNever]
        public int RequiredPoints { get; set; }   // ✅ Điểm cần để đổi mã

        [JsonIgnore]
        public virtual ICollection<Discount_Customer>? Discount_Customer { get; set; }
    }
}
