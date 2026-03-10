using Ecommerce.Models;

namespace Ecommerce.Services.RevenueService
{
    public interface IRevenueService
    {
        Task<IEnumerable<Revenue>> GetAllRevenues();
        Task<object> GetNumberOfUserandRevenueOfDay();
    }
}
