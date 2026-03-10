using Ecommerce.DTO;
using Ecommerce.Models;
using Ecommerce.Repositories.MenuCategoryRepository;

namespace Ecommerce.Services.MenuCategoryService
{
    public class MenuCategoryService : IMenuCategoryService
    {
        private readonly IMenuCategoryRepository menuCategoryRepository;

        public MenuCategoryService(IMenuCategoryRepository menuCategoryRepository)
        {
            this.menuCategoryRepository = menuCategoryRepository;
        }

        public async Task<StatusDTO> CreateAsync(MenuCategory model)
        {
            var item = await menuCategoryRepository.CreateAsync(model);
            if (item == null) return new StatusDTO { IsSuccess = false, Message = "Tạo không thành công" };

            return new StatusDTO {IsSuccess = true, Message = "Tạo thành công"};
        }

        public async Task<StatusDTO> DeleteAsync(int id)
        {
            if (id == 0) return new StatusDTO { IsSuccess = false, Message = "Id không hợp lệ" };
            var check = await menuCategoryRepository.HasMenuInCategoryAsync(id);
            if (check == true) return new StatusDTO { IsSuccess = false, 
                Message = "Không thể xóa loại món ăn vì có món đã có loại món ăn này" };

            var result = await menuCategoryRepository.DeleteAsync(id);
            if(!result) return new StatusDTO { IsSuccess = false, Message = "Xóa không thành công" };
            return new StatusDTO { IsSuccess = true, Message = "Xóa thành công" };
        }

        public async Task<IEnumerable<MenuCategory>> GetAllAsync() => await menuCategoryRepository.GetAllAsync();

        public async Task<MenuCategory> GetByIdAsync(int id) => await menuCategoryRepository.GetByIdAsync(id);

        public async Task<StatusDTO> UpdateAsync(MenuCategory model)
        {
            if (model.MenuCategoryId <= 0)
                return new StatusDTO { IsSuccess = false, Message = "Mã hạng không hợp lệ" };
            var item = await menuCategoryRepository.UpdateAsync(model);
            if (item == false) return new StatusDTO { IsSuccess = false, Message = "Cập nhật không thành công" };

            return new StatusDTO { IsSuccess = true, Message = "Cập nhật thành công" };
        }
    }
}
