using Ecommerce.DTO;

namespace Ecommerce.Services.DiscountCustomerService
{
    public interface IDiscountCustomerService
    {
        Task<IEnumerable<object>> GetUserDiscountsAsync(string customerId);
        Task<bool> UseDiscountAsync(string customerId, string discountId);
        Task<bool> AssignDiscountToUserAsync(string discountId, string customerId);
        Task<StatusDTO> ExchangeDiscountAsync(string customerId, string discountId);
    }
}
