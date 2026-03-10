using Ecommerce.Data;
using Ecommerce.Models;
using Ecommerce.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.FoodSizeRepository
{
    public class FoodSizeRepository : IFoodSizeRepository
    {
        private readonly AppDbContext db;

        public FoodSizeRepository(AppDbContext db)
        {
            this.db = db;
        }

        // =====================================
        // GET ALL - Lấy tất cả kích cỡ món ăn
        // =====================================
        public async Task<IEnumerable<FoodSizeViewModel>> GetAll()
        {
            var lstFoodSize = await db.FoodSizes
                .Include(fs => fs.Menu)
                .OrderBy(fs => fs.SortOrder)   // 👈 Sắp xếp tăng dần
                .Select(fs => new FoodSizeViewModel
                {
                    FoodSizeId = fs.FoodSizeId,
                    MenuName = fs.Menu.MenuName,
                    FoodName = fs.FoodName,
                    Price = fs.Price,
                    SortOrder = fs.SortOrder
                })
                .ToListAsync();

            return lstFoodSize;
        }
        // =====================================
        // SEARCH - Tìm kiếm theo tên món ăn hoặc kích cỡ
        // =====================================
        public async Task<IEnumerable<FoodSizeViewModel>> Search(string? keyword)
        {
            var query = db.FoodSizes
                .Include(fs => fs.Menu)
                .Select(fs => new FoodSizeViewModel
                {
                    FoodSizeId = fs.FoodSizeId,
                    MenuName = fs.Menu.MenuName,
                    FoodName = fs.FoodName,
                    Price = fs.Price,
                    SortOrder = fs.SortOrder
                });

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(fs =>
                    fs.MenuName.Contains(keyword) ||
                    fs.FoodName.Contains(keyword));
            }

            // Giới hạn trả về 20 bản ghi để tránh quá tải
            return await query.Take(20).ToListAsync();
        }


        // =====================================
        // GET BY ID
        // =====================================
        public async Task<FoodSizeViewModel> GetById(int id)
        {
            var foodSize = await db.FoodSizes
                .Include(fs => fs.Menu)
                .Select(fs => new FoodSizeViewModel
                {
                    FoodSizeId = fs.FoodSizeId,
                    MenuName = fs.Menu.MenuName,
                    FoodName = fs.FoodName,
                    Price = fs.Price,
                    SortOrder = fs.SortOrder
                })
                .FirstOrDefaultAsync(fs => fs.FoodSizeId == id);

            return foodSize;
        }

        // =====================================
        // CREATE
        // =====================================
        public async Task Create(FoodSize model)
        {
            await db.FoodSizes.AddAsync(model);          
            await db.SaveChangesAsync();
            var inventoryItem = new Inventory()
            {
                FoodSizeId = model.FoodSizeId,
                Quantity = 0,
                Unit = "Phần"
            };
            await db.Inventories.AddAsync(inventoryItem);
            await db.SaveChangesAsync();
        }

        // =====================================
        // UPDATE
        // =====================================
        public async Task Update(FoodSize model)
        {
            db.FoodSizes.Update(model);
            await db.SaveChangesAsync();
        }

        // =====================================
        // DELETE
        // =====================================
        public async Task Delete(int id)
        {
            var foodSize = await db.FoodSizes.FindAsync(id);
            if (foodSize != null)
            {
                db.FoodSizes.Remove(foodSize);
                await db.SaveChangesAsync();
            }
        }

        // =====================================
        // VALID NAME (kiểm tra trùng tên FoodName)
        // =====================================
        public async Task<string> ValidName(string foodName)
        {
            var result = await db.FoodSizes.FirstOrDefaultAsync(fs => fs.FoodName == foodName);
            if (result == null) return "";
            return "Tên kích cỡ món ăn bị trùng";
        }
    }
}
