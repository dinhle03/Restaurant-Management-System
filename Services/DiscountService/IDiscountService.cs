using Ecommerce.DTO;
using Ecommerce.Models;

namespace Ecommerce.Services.DiscountService
{
    public interface IDiscountService
    {
        Task<IEnumerable<Discount>> GetAll(string? search = null);
        Task<Discount?> GetById(string id);
        Task<StatusDTO> Create(Discount model);
        Task<StatusDTO> Update(Discount model);
        Task<StatusDTO> Delete(string id);
        Task<StatusDTO> AddDiscountToCustomer(string discountId, string customerId);
        Task<StatusDTO> ExchangeDiscount(string discountId, string customerId);

    }
}
