using Ecommerce.Models;

namespace Ecommerce.Repositories.DiscountCustomerRepository
{
    public interface IDiscountCustomerRepository
    {
        Task<bool> AddDiscountToCustomer(string discountId, string customerId);
        Task<bool> CheckCustomerHasDiscount(string discountId, string customerId);
        Task<IEnumerable<Discount_Customer>> GetByCustomerIdAsync(string customerId);
        Task<Discount_Customer?> GetByCustomerAndDiscountAsync(string customerId, string discountId);
        Task UpdateAsync(Discount_Customer entity);
        Task<Customer?> GetCustomerByIdAsync(string customerId);
        Task<Discount?> GetDiscountByIdAsync(string discountId);
        Task<int> GetCustomerPoint(string customerId);
        Task DeductPoint(string customerId, int point);
    }
}
