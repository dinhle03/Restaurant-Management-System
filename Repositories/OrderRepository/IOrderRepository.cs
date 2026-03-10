using Ecommerce.DTO;
using Ecommerce.Models;

namespace Ecommerce.Repositories.OrderRepository
{
    public interface IOrderRepository
    {
        
        Task<Order?> GetById(int orderId);

        //Hàm để hiện đơn hàng phía nhà hàng
        Task<IEnumerable<OrderDTO>> GetAllOrderAsync();

        //Hiện thông tin chi tiết của 1 đơn phía nhà hàng
        Task<IEnumerable<OrderDetailDTO>> GetOrderDetailsByIdAsync(int orderId);

        //Hàm cập nhật OrderStatus(Trạng thái đơn hàng) phía bếp
        Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus);

        //Hàm yêu cầu thanh toán
        Task<bool> RequestPaymentAsync(int orderId);

        //Hàm để người dùng lấy danh sách đơn hàng cùng chi tiết đơn của họ
        Task<IEnumerable<OrderUserDTO>> GetOrdersByUserIdAsync(string customerId);

        //Hàm cập nhật trạng thái thanh toán của đơn hàng sau khi thanh toán thành công
        Task UpdatePaymentStatusOrderAsync(Order order);

        //Hàm lấy danh sách đơn hàng trả bằng tiền mặt
        Task<IEnumerable<OrderDTO>> GetCashOrdersAsync();

        //Hàm lấy danh sách lịch sử đơn hàng đã thanh toán
        Task<IEnumerable<Order>> GetOrderHasPayment();
    }
}
