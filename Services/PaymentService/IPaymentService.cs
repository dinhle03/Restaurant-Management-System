using Ecommerce.DTO;

namespace Ecommerce.Services.PaymentService
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentUrlMomo(OrderInfo model);
        Task<object> PaymentCallbackAsync(IQueryCollection query);
        Task<object> ConfirmOrderPayByCash(int orderId);
    }
}
