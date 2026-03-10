using Ecommerce.Data;
using Ecommerce.Models;
using Ecommerce.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.InventoryRepository
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext db;

        public InventoryRepository(AppDbContext db)
        {
            this.db = db;
        }

        // 🟢 Lấy toàn bộ danh sách tồn kho (kèm MenuName và FoodSizeName)
        public async Task<IEnumerable<InventoryViewModel>> GetAll()
        {
            var list = await db.Inventories
                .Include(i => i.FoodSize)
                    .ThenInclude(fs => fs.Menu) // đi qua FoodSize -> Menu
                .Select(i => new InventoryViewModel
                {
                    InventoryId = i.InventoryId,
                    MenuName = i.FoodSize.Menu.MenuName, // lấy từ Menu
                    FoodSizeName = i.FoodSize.FoodName,
                    Unit = i.Unit,
                    Quantity = i.Quantity
                })
                .ToListAsync();

            return list;
        }
        public async Task<IEnumerable<InventoryViewModel>> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return await GetAll();
            }

            keyword = keyword.ToLower();

            return await db.Inventories
                .Include(i => i.FoodSize)
                .ThenInclude(fs => fs.Menu)
                .Where(i =>
                    i.FoodSize.Menu.MenuName.ToLower().Contains(keyword) ||
                    i.FoodSize.FoodName.ToLower().Contains(keyword) ||
                    i.Unit.ToLower().Contains(keyword))
                .Select(i => new InventoryViewModel
                {
                    InventoryId = i.InventoryId,
                    MenuName = i.FoodSize.Menu.MenuName,
                    FoodSizeName = i.FoodSize.FoodName,
                    Quantity = i.Quantity,
                    Unit = i.Unit
                })
                .Take(50)
                .ToListAsync();
        }


        // 🟢 Lấy tồn kho theo ID
        public async Task<InventoryViewModel> GetById(int id)
        {
            var inventory = await db.Inventories
                .Include(i => i.FoodSize)
                    .ThenInclude(fs => fs.Menu)
                .Select(i => new InventoryViewModel
                {
                    InventoryId = i.InventoryId,
                    MenuName = i.FoodSize.Menu.MenuName,
                    FoodSizeName = i.FoodSize.FoodName,
                    Unit = i.Unit,
                    Quantity = i.Quantity
                })
                .FirstOrDefaultAsync(i => i.InventoryId == id);

            return inventory;
        }

        // 🟢 Tạo mới tồn kho
        public async Task Create(Inventory model)
        {
            await db.Inventories.AddAsync(model);
            await db.SaveChangesAsync();
        }

        // 🟢 Cập nhật tồn kho
        public async Task Update(Inventory model)
        {
            db.Inventories.Update(model);
            await db.SaveChangesAsync();
        }

        // 🟢 Xóa tồn kho
        public async Task Delete(int id)
        {
            var inventory = await db.Inventories.FindAsync(id);
            if (inventory != null)
            {
                db.Inventories.Remove(inventory);
                await db.SaveChangesAsync();
            }
        }
        public async Task<Inventory> GetEntityById(int id)
        {
            return await db.Inventories.FindAsync(id);
        }


    }
}
