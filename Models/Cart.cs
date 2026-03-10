using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Ecommerce.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        [DisplayName("Trạng thái")]
        public string CartStatus { get; set; }

        public string CustomerId { get; set; }

        [ValidateNever]
        public Customer Customer { get; set; }

        [Required]
        public int TableId { get; set; }

        [ValidateNever]
        public Table Table { get; set; }

        [ValidateNever]
        public virtual ICollection<CartDetails> CartDetails { get; set; }
    }
}
