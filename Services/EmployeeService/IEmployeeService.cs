using Ecommerce.DTO;
using Ecommerce.Models;
using Ecommerce.ViewModels;

namespace Ecommerce.Services.EmployeeService
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDTO>> GetAllAsync(string? search);
        Task<EmployeeDTO?> GetByIdAsync(string id);
        Task<StatusDTO> CreateAsync(RegisterViewModel model, IFormFile? imageFile);
        Task<StatusDTO> UpdateAsync(string id, Users updated, IFormFile? imageFile);
        Task<StatusDTO> DeleteAsync(string id);
    }
}
