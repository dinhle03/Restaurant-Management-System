using Ecommerce.Models;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Repositories.EmployeeRepository
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Users>> GetAllAsync(string? search);
        Task<Users?> GetByIdAsync(string id);
        Task<IList<string>> GetRolesAsync(Users user);
        Task<IdentityResult> CreateAsync(RegisterViewModel model, string urlImage, string role);
        Task<IdentityResult> UpdateAsync(Users user);
        Task<IdentityResult> DeleteAsync(Users user);

    }
}
