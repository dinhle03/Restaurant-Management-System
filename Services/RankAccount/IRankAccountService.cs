using Ecommerce.DTO;
using Ecommerce.Models;

namespace Ecommerce.Services.RankAccount
{
    public interface IRankAccountService
    {
        Task<IEnumerable<CustomerRank>> GetAllAsync();
        Task<StatusDTO> CreateAsync(CustomerRank customerRank);
        Task<StatusDTO> UpdateAsync(CustomerRank customerRank);
        Task<StatusDTO> DeleteAsync(int id);
        Task<CustomerRank> GetById(int id);
    }
}
