using Ecommerce.DTO;
using Ecommerce.Models;

namespace Ecommerce.Services.OrderService
{
    public interface IOrderService
    {
        Task<StatusDTO> CreateOrderAsync(string userId);
        Task<IEnumerable<OrderDetailDTO>?> GetOrderDetailsByIdAsync(int orderId);
        Task<IEnumerable<OrderUserDTO>> GetOrdersByUserIdAsync(string customerId);
        Task<StatusDTO> UpdateOrderStatusAsync(int orderId, string newStatus);
        Task<StatusDTO> RequestPaymentAsync(int orderId);
        Task<IEnumerable<OrderDTO>> GetAllOrdersAsync();
        //Cập nhật giá tiền và phương thức thanh toán
        Task<UpdateOrderDTO> UpdatePaymentMethodandTotal(ConfirmPaymentDTO model);

        Task<IEnumerable<OrderDTO>> GetCashOrdersAsync();
        Task<IEnumerable<Order>> GetOrderHasPayment();
    }
}
