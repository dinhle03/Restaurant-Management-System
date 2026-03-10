using Ecommerce.DTO;
using Ecommerce.Models;
using Ecommerce.Repositories.MenuRepository;
using Ecommerce.ViewModels;
using static Ecommerce.DTO.MenuOrderDTO;

namespace Ecommerce.Services.MenuService
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository menuRepository;

        public MenuService(IMenuRepository menuRepository)
        {
            this.menuRepository = menuRepository;
        }

        public async Task<IEnumerable<MenuViewModel>> GetAll(string? search = null)
        {
            return await menuRepository.GetAll(search);
        }

        public async Task<MenuViewModel> GetById(int id)
        {
            return await menuRepository.GetById(id);
        }

        public async Task<StatusDTO> Create(Menu model)
        {
            var checkName = await menuRepository.ValidName(model.MenuName);
            if (!string.IsNullOrEmpty(checkName))
                return new StatusDTO { IsSuccess = false, Message = checkName };

            await menuRepository.Create(model);

            return new StatusDTO
            {
                IsSuccess = true,
                Message = $"Tạo món {model.MenuName} thành công"
            };
        }

        public async Task<StatusDTO> Update(Menu model)
        {
            var checkName = await menuRepository.ValidName(model.MenuName);
            if (!string.IsNullOrEmpty(checkName))
            {
                var current = await menuRepository.GetById(model.MenuId);
                if (current != null && current.MenuName != model.MenuName)
                    return new StatusDTO { IsSuccess = false, Message = "Tên món ăn bị trùng" };
            }

            await menuRepository.Update(model);
            return new StatusDTO
            {
                IsSuccess = true,
                Message = $"Cập nhật món {model.MenuName} thành công"
            };
        }

        public async Task<StatusDTO> Delete(int id)
        {
            var menu = await menuRepository.GetById(id);
            if (menu == null)
                return new StatusDTO { IsSuccess = false, Message = "Không tìm thấy món ăn cần xóa" };

            await menuRepository.Delete(id);
            return new StatusDTO
            {
                IsSuccess = true,
                Message = $"Xóa món {menu.MenuName} thành công"
            };
        }

        public async Task<Menu?> GetEntityById(int id)
        {
            return await menuRepository.GetEntityById(id);
        }



        public async Task<List<MenuDto>> GetAvailableMenusByCategoryAsync(int? categoryId = null)
        {
            return await menuRepository.GetAvailableMenusByCategoryAsync(categoryId);
        }

    }
}
