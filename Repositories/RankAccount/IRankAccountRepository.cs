using Ecommerce.Models;

namespace Ecommerce.Repositories.RankAccount
{
    public interface IRankAccountRepository
    {
        Task<int> SelectLastestRank();
        Task CreateAsync(CustomerRank model);
        Task UpdateAsync(CustomerRank model);
        Task DeleteAsync(int id);
        Task<IEnumerable<CustomerRank>> GetAllAsync();
        Task<CustomerRank> GetByIdAsync(int id);
        Task<int?> GetRankPointByUserIdAsync(string userId);
    }
}
