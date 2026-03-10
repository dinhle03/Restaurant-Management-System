using Ecommerce.Data;
using Ecommerce.DTO;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.MenuCategoryRepository
{
    public class MenuCategoryRepository : IMenuCategoryRepository
    {
        private readonly AppDbContext db;

        public MenuCategoryRepository(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<MenuCategory> CreateAsync(MenuCategory model)
        {
            var item = new MenuCategory
            {
                MenuCategoryId = model.MenuCategoryId,
                MenuCategoryName = model.MenuCategoryName,
            };
            await db.MenuCategories.AddAsync(item);
            await db.SaveChangesAsync();

            return item;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await GetByIdAsync(id);
            if(item == null) return false;

            db.MenuCategories.Remove(item);
            await db.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<MenuCategory>> GetAllAsync()
        {
            var lst = await db.MenuCategories.ToListAsync();
            return lst;
        }

        public async Task<MenuCategory> GetByIdAsync(int id)
        {
            var item = await db.MenuCategories.FindAsync(id);
            return item;
        }

        public async Task<bool> UpdateAsync(MenuCategory model)
        {
            var item = await GetByIdAsync(model.MenuCategoryId);
            if(item == null) return false;

            item.MenuCategoryName = model.MenuCategoryName;

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasMenuInCategoryAsync(int categoryId)
        {
            return await db.Menus.AnyAsync(m => m.MenuCategoryId == categoryId);
        }
    }
}
