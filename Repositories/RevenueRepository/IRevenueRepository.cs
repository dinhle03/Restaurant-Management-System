using Ecommerce.Models;

namespace Ecommerce.Repositories.RevenueRepository
{
    public interface IRevenueRepository
    {
        Task<Revenue> GetRevenueByIdAsync(int revenueId);
        Task UpdateRevenueAsync(Revenue revenue);
        Task<int> CheckRevenue();
        Task addRevenue_Orders(int orderId, int revenueId);
        Task<IEnumerable<Revenue>> GetAllRevenues();
        Task<object> GetNumberOfUserandRevenueOfDay();
    }
}
