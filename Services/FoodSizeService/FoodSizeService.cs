using Ecommerce.DTO;
using Ecommerce.Models;
using Ecommerce.Repositories.FoodSizeRepository;
using Ecommerce.Repositories.MenuRepository;
using Ecommerce.ViewModels;

namespace Ecommerce.Services.FoodSizeService
{
    public class FoodSizeService : IFoodSizeService
    {
        private readonly IFoodSizeRepository foodsizeRepository;
        private readonly IMenuRepository menuRepository;

        public FoodSizeService(IFoodSizeRepository foodsizeRepository, IMenuRepository menuRepository)
        {
            this.foodsizeRepository = foodsizeRepository;
            this.menuRepository = menuRepository;
        }

        // =====================================
        // GET ALL
        // =====================================
        public async Task<IEnumerable<FoodSizeViewModel>> GetAll()
        {
            return await foodsizeRepository.GetAll();
        }
        // =====================================
        // SEARCH
        // =====================================
        public async Task<IEnumerable<FoodSizeViewModel>> Search(string? keyword)
        {
            return await foodsizeRepository.Search(keyword);
        }

        // =====================================
        // GET BY ID
        // =====================================
        public async Task<FoodSizeViewModel> GetById(int id)
        {
            return await foodsizeRepository.GetById(id);
        }

        // =====================================
        // CREATE
        // =====================================
        public async Task<StatusDTO> Create(FoodSize model)
        {
            // 1️⃣ Kiểm tra giá bán
            if (model.Price < 0)
                return new StatusDTO { IsSuccess = false, Message = "Giá bán không được âm" };



            // 3️⃣ Kiểm tra Menu tồn tại
            var menu = await menuRepository.GetById(model.MenuId);
            if (menu == null)
                return new StatusDTO { IsSuccess = false, Message = "Không tìm thấy món ăn tương ứng" };

            // 4️⃣ Tạo mới
            await foodsizeRepository.Create(model);
            return new StatusDTO
            {
                IsSuccess = true,
                Message = $"Thêm biến thể {model.FoodName} cho món {menu.MenuName} thành công"
            };
        }

        // =====================================
        // UPDATE
        // =====================================
        public async Task<StatusDTO> Update(FoodSize model)
        {
            // 1️⃣ Kiểm tra tồn tại
            var current = await foodsizeRepository.GetById(model.FoodSizeId);
            if (current == null)
                return new StatusDTO { IsSuccess = false, Message = "Không tìm thấy biến thể của món ăn cần cập nhật" };

            // 2️⃣ Kiểm tra Menu tồn tại
            var menu = await menuRepository.GetById(model.MenuId);
            if (menu == null)
                return new StatusDTO { IsSuccess = false, Message = "Không tìm thấy món ăn (Menu) tương ứng" };

            // 3️⃣ Kiểm tra giá bán
            if (model.Price < 0)
                return new StatusDTO { IsSuccess = false, Message = "Giá bán không được âm" };


            // 5️⃣ Cập nhật
            await foodsizeRepository.Update(model);
            return new StatusDTO
            {
                IsSuccess = true,
                Message = $"Cập nhật biến thể {model.FoodName} thành công"
            };
        }

        // =====================================
        // DELETE
        // =====================================
        public async Task<StatusDTO> Delete(int id)
        {
            // 1️⃣ Kiểm tra tồn tại
            var current = await foodsizeRepository.GetById(id);
            if (current == null)
                return new StatusDTO { IsSuccess = false, Message = "Không tìm thấy biến thể của món ăn cần xóa" };

            // 2️⃣ Thực hiện xóa
            await foodsizeRepository.Delete(id);
            return new StatusDTO
            {
                IsSuccess = true,
                Message = $"Đã xóa biến thể {current.FoodName} thành công"
            };
        }
    }
}