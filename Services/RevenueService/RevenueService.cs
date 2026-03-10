using Ecommerce.Models;
using Ecommerce.Repositories.RevenueRepository;

namespace Ecommerce.Services.RevenueService
{
    public class RevenueService : IRevenueService
    {
        private readonly IRevenueRepository revenueRepository;

        public RevenueService(IRevenueRepository revenueRepository)
        {
            this.revenueRepository = revenueRepository;
        }

        public Task<IEnumerable<Revenue>> GetAllRevenues()
        {
            return revenueRepository.GetAllRevenues();
        }

        public Task<object> GetNumberOfUserandRevenueOfDay()
        {
            return revenueRepository.GetNumberOfUserandRevenueOfDay();
        }
    }
}
