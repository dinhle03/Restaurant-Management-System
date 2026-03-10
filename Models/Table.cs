using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Ecommerce.Models
{
    public class Table
    {
        [Key]
        public int TableId { get; set; }

        [DisplayName("Vị trí bàn")]
        [Required(ErrorMessage = "Không được để trống tên bàn")]
        public string TableName { get; set; }

        [DisplayName("Số ghế")]
        [Required(ErrorMessage = "Vui lòng nhập số ghế")]
        public int NumberOfSeats { get; set; }

        [DisplayName("Trạng thái")]
        [Required(ErrorMessage = "Vui lòng chọn trạng thái bàn")]
        [RegularExpression("Trống|Đang sử dụng|Đã đặt", ErrorMessage = "Trạng thái chỉ có thể là 'Trống', 'Đang sử dụng' hoặc 'Đã đặt'")]
        public string TableStatus { get; set; }

        public string? QRCodePath { get; set; }

        public string? OwnerTable { get; set; }
    }
}
