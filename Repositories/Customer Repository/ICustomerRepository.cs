using Ecommerce.DTO;
using Ecommerce.Models;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Repositories.CustomerRepository
{
    public interface ICustomerRepository
    {
        Task<List<CustomerDTO>> GetAll(string? search = null);
        Task<Customer> GetById(string userId);
        Task<Customer> Update(UpdateCustomerViewModel model, string userId, IFormFile? UrlImage);
        Task<StatusDTO> Delete(Customer customer);
        Task<IdentityResult> CreateAsync(RegisterViewModel model, string urlImage, int rankId, string role);

        Task<Customer?> GetCustomerByPhoneAsync(string PhoneNumber, string userId);
        Task<CustomerRank?> GetRankByPointAsync(int point);
        Task<bool> IsCustomerCart(string userId);
        Task<List<CustomerDTO>> GetLockedCustomersAsync();
        Task<bool> UpdatePointAsync(string userId, int newPoint);
    }
}