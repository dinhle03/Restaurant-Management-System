using Ecommerce.DTO;
using Ecommerce.Models;
using Ecommerce.Repositories.FoodSizeRepository;
using Ecommerce.Repositories.InventoryRepository;
using Ecommerce.ViewModels;

namespace Ecommerce.Services.InventoryService
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository inventoryRepository;
        private readonly IFoodSizeRepository foodSizeRepository;

        public InventoryService(IInventoryRepository inventoryRepository, IFoodSizeRepository foodSizeRepository)
        {
            this.inventoryRepository = inventoryRepository;
            this.foodSizeRepository = foodSizeRepository;
        }

        // 🟢 Lấy danh sách tất cả tồn kho
        public async Task<IEnumerable<InventoryViewModel>> GetAll()
        {
            return await inventoryRepository.GetAll();
        }

        // 🔍 Tìm kiếm tồn kho
        public async Task<IEnumerable<InventoryViewModel>> Search(string keyword)
        {
            return await inventoryRepository.Search(keyword);
        }

        // 🟣 Lấy tồn kho theo ID
        public async Task<InventoryViewModel> GetById(int id)
        {
            return await inventoryRepository.GetById(id);
        }

        // 🟢 Tạo tồn kho mới
        public async Task<StatusDTO> Create(Inventory model)
        {
            // Kiểm tra biến thể món ăn (FoodSize)
            var foodSize = await foodSizeRepository.GetById(model.FoodSizeId);
            if (foodSize == null)
                return new StatusDTO { IsSuccess = false, Message = "Không tìm thấy biến thể món ăn tương ứng" };

            if (model.Quantity < 0)
                return new StatusDTO { IsSuccess = false, Message = "Số lượng không được âm" };

            await inventoryRepository.Create(model);
            return new StatusDTO { IsSuccess = true, Message = "Thêm tồn kho mới thành công" };
        }

        // 🟠 Cập nhật tồn kho
        public async Task<StatusDTO> Update(Inventory model)
        {
            // Tìm bản ghi hiện có
            var existing = await inventoryRepository.GetEntityById(model.InventoryId);

            if (existing == null)
                return new StatusDTO { IsSuccess = false, Message = "Không tìm thấy tồn kho cần cập nhật" };

            // ⚠️ Giữ nguyên FoodSizeId hiện tại nếu model gửi lên là 0 hoặc null
            if (model.FoodSizeId == 0)
                model.FoodSizeId = existing.FoodSizeId;

            // Kiểm tra biến thể tồn tại
            var foodSize = await foodSizeRepository.GetById(model.FoodSizeId);
            if (foodSize == null)
                return new StatusDTO { IsSuccess = false, Message = "Không tìm thấy biến thể món ăn tương ứng" };

            // Kiểm tra số lượng âm
            if (model.Quantity < 0)
                return new StatusDTO { IsSuccess = false, Message = "Số lượng không được âm" };

            // 🧩 Cập nhật dữ liệu — chỉ cập nhật các field thay đổi
            existing.Unit = model.Unit ?? existing.Unit;
            existing.Quantity = model.Quantity;
            existing.FoodSizeId = model.FoodSizeId; // Giữ nguyên khóa ngoại

            try
            {
                await inventoryRepository.Update(existing);
                return new StatusDTO { IsSuccess = true, Message = "Cập nhật tồn kho thành công" };
            }
            catch (Exception ex)
            {
                // Ghi log chi tiết nếu cần
                return new StatusDTO
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi cập nhật tồn kho: {ex.InnerException?.Message ?? ex.Message}"
                };
            }
        }

        // 🔴 Xóa tồn kho
        public async Task<StatusDTO> Delete(int id)
        {
            var existing = await inventoryRepository.GetById(id);
            if (existing == null)
                return new StatusDTO { IsSuccess = false, Message = "Không tìm thấy tồn kho cần xóa" };

            await inventoryRepository.Delete(id);
            return new StatusDTO { IsSuccess = true, Message = "Xóa tồn kho thành công" };
        }

    }
}
