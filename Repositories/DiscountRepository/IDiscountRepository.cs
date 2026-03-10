using Ecommerce.Models;

namespace Ecommerce.Repositories.DiscountRepository
{
    public interface IDiscountRepository
    {
        Task<IEnumerable<Discount>> GetAll(string? search = null);
        Task<Discount?> GetById(string id);
        Task Create(Discount model);
        Task Update(Discount model);
        Task Delete(string id);
        Task<bool> ExchangeDiscountAsync(string discountId, string customerId);
        Task<Discount_Customer?> GetDiscountByUserId(string userId, string discountId);
    }
}
