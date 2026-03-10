using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models
{
    public class Revenue
    {
        public int RevenueId { get; set; }
        public DateTime Date { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public virtual ICollection<Revenue_Order> Revenue_Order { get; set; }
    }
}
