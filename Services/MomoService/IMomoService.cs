using Ecommerce.DTO;

namespace Ecommerce.Services.MomoService
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfo model);
        MomoExecuteResponseModel PaymentExecute(IQueryCollection collection);
    }
}
