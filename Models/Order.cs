using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Ecommerce.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [DisplayName("Tổng tiền")]
        [Column(TypeName = "decimal(37,1)")]
        public decimal TotalAmount { get; set; }

        [DisplayName("Trạng thái đơn")]
        public string OrderStatus { get; set; }//Tiếp nhận đơn, Đang chuẩn bị, Hoàn thành

        [DisplayName("Ngày đặt")]
        public DateTime OrderDate { get; set; }

        [DisplayName("Trạng thái thanh toán")]
        public string PaymentStatus { get; set; }//Chưa thanh toán, Yêu cầu thanh toán, Chờ xác nhận thanh toán, Đã thanh toán

        public string? PaymentMethod { get; set; }//Tiền mặt, Chuyển khoản VNPay

        public DateTime? PaymentDate { get; set; }

        public int TableId { get; set; }
        [ValidateNever]
        public Table Table { get; set; }

        public string CustomerId { get; set; }
        [ValidateNever]
        public Customer Customer { get; set; }

        [ValidateNever]
        public virtual ICollection<OrderDetails> OrderDetails { get; set; }
        [ValidateNever]
        public virtual ICollection<Revenue_Order> Revenue_Order { get; set; }
    }
}
