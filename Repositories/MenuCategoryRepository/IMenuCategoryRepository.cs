using Ecommerce.DTO;
using Ecommerce.Models;

namespace Ecommerce.Repositories.MenuCategoryRepository
{
    public interface IMenuCategoryRepository
    {
        Task<MenuCategory> GetByIdAsync(int id);
        Task<IEnumerable<MenuCategory>> GetAllAsync();
        Task<MenuCategory> CreateAsync(MenuCategory model);
        Task<bool> UpdateAsync(MenuCategory model);
        Task<bool> DeleteAsync(int id);

        Task<bool> HasMenuInCategoryAsync(int categoryId);
    }
}
