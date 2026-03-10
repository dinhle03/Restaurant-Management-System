using Ecommerce.DTO;
using Ecommerce.Models;

namespace Ecommerce.Services.MenuCategoryService
{
    public interface IMenuCategoryService
    {
        Task<MenuCategory> GetByIdAsync(int id);
        Task<IEnumerable<MenuCategory>> GetAllAsync();
        Task<StatusDTO> CreateAsync(MenuCategory model);
        Task<StatusDTO> UpdateAsync(MenuCategory model);
        Task<StatusDTO> DeleteAsync(int id);
    }
}
