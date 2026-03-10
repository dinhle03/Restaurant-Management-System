using Ecommerce.DTO;
using Ecommerce.Models;
using Ecommerce.ViewModels;
using static Ecommerce.DTO.MenuOrderDTO;

namespace Ecommerce.Services.MenuService
{
    public interface IMenuService
    {
        Task<IEnumerable<MenuViewModel>> GetAll(string? search = null);

        Task<MenuViewModel> GetById(int id);
        Task<StatusDTO> Create(Menu model);
        Task<StatusDTO> Update(Menu model);
        Task<StatusDTO> Delete(int id);
        Task<Menu?> GetEntityById(int id);
        Task<List<MenuDto>> GetAvailableMenusByCategoryAsync(int? categoryId = null);

    }
}
