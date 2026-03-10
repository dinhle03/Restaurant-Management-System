using Ecommerce.DTO;
using Ecommerce.ViewModels;

namespace Ecommerce.Services.Customer
{
    public interface ICustomerService
    {
        Task<List<CustomerDTO>> GetAll(string? search = null);
        Task<CustomerResultDTO> GetById(string userId);
        Task<StatusDTO> Update(UpdateCustomerViewModel model, string userId, IFormFile? UrlImage);
        Task<StatusDTO> Delete(string userId);
        Task<StatusDTO> LockUserAsync(string userId);
        Task<StatusDTO> UnlockUserAsync(string userId);
        Task<List<CustomerDTO>> GetLockedCustomersAsync();
    }
}
