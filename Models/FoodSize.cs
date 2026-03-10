using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Ecommerce.Models
{
    public class FoodSize
    {
        public int FoodSizeId { get; set; }

        public int MenuId { get; set; }
        [ValidateNever]
        public Menu Menu { get; set; }

        public string FoodName { get; set; }// S,M,L

        [Column(TypeName = "decimal(18,2)")]
        [Required(ErrorMessage = "Không được để trống giá bán!")]
        public decimal Price { get; set; } 

        public int SortOrder { get; set; }// Thứ tự
        [ValidateNever]
        public virtual ICollection<CartDetails> CartDetails { get; set; }
        [ValidateNever]
        public virtual ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
