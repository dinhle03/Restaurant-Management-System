
using Ecommerce.Data;
using Ecommerce.DTO;
using Ecommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Ecommerce.Repositories.CartRepository
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext db;
        private readonly UserManager<Users> userManager;

        public CartRepository(AppDbContext db, UserManager<Users> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        //1.Nếu thêm lần đầu sẽ tạo mới 1 cartItem
        //2.Nếu có thì tăng số lượng 
        public async Task AddToCart(int foodsizeId, int cartId)
        {
            var cartItem = await db.Cart_Menus.FirstOrDefaultAsync(c => c.FoodSizeId == foodsizeId && c.CartId == cartId);
            if (cartItem == null)
            {
                var newCartItem = new CartDetails
                {
                    FoodSizeId = foodsizeId,
                    CartId = cartId,
                    Count = 1,
                };
                await db.Cart_Menus.AddAsync(newCartItem);
            }
            else
            {
                cartItem.Count += 1;
                db.Cart_Menus.Update(cartItem);
            }
            await db.SaveChangesAsync();
        }

        public async Task<int> CreateAsync(Cart model)
        {
            var existingCart = await db.Carts
                .FirstOrDefaultAsync(c => c.CustomerId == model.CustomerId && c.CartStatus == "Chưa xác nhận");

            if (existingCart != null)
                return existingCart.CartId;

            await db.Carts.AddAsync(model);
            await db.SaveChangesAsync();

            return model.CartId;
        }

        public async Task RemoveToCart(int foodsizeId, int cartId)
        {
            var cartItem = await db.Cart_Menus
                .FirstOrDefaultAsync(c => c.FoodSizeId == foodsizeId && c.CartId == cartId);

            if (cartItem == null)
                return;

            if (cartItem.Count > 1)
            {
                cartItem.Count -= 1;
                db.Cart_Menus.Update(cartItem);
            }
            else
            {
                db.Cart_Menus.Remove(cartItem);
            }

            await db.SaveChangesAsync();
        }

        public async Task<IEnumerable<CartItemDTO>> getCartItem(string userId)
        {
            var cart = await db.Carts.FirstOrDefaultAsync(c => c.CustomerId == userId && c.CartStatus == "Chưa xác nhận");

            if (cart == null) return new List<CartItemDTO>();

            var cartItems = db.Cart_Menus.Include(c => c.FoodSize)
                .Include(c => c.FoodSize.Menu)
                .Where(c => c.CartId == cart.CartId).Select(c => new CartItemDTO
                {
                FoodSizeId = c.FoodSizeId,
                CartId = c.CartId,
                MenuName = c.FoodSize.Menu.MenuName,
                FoodSizeName = c.FoodSize.FoodName,
                Count = c.Count,
                Price = (c.Count * c.FoodSize.Price),
                ImageUrl = db.FoodImages
                        .Where(fi => fi.MenuId == c.FoodSize.MenuId && fi.MainImage)
                        .Select(fi => fi.UrlImage)
                        .FirstOrDefault()
                });

            return cartItems;
        }

        public async Task<Cart> GetById(int id)
        {
            return db.Carts.Find(id);
        }

        public async Task<(int, decimal)> GetQuantityCartItem(string userId)
        {
            var cartItem = await getCartItem(userId);
            decimal total = 0;

            foreach (var item in cartItem)
            {
                total += item.Price;
            }

            if (cartItem == null || !cartItem.Any())
                return (0, 0);

            return (cartItem.Count(), total);
        }

        public async Task<Cart?> GetActiveCartByUserIdAsync(string userId)
        {
            return await db.Carts
                    .Include(c => c.Table)
                    .Include(c => c.CartDetails)
                        .ThenInclude(cd => cd.FoodSize)
                        .ThenInclude(fs => fs.Menu)
                    .FirstOrDefaultAsync(c => c.CustomerId == userId && c.CartStatus == "Chưa xác nhận");

        }

        public async Task<(int distinctFoodCount, decimal totalPrice)> GetCartSummaryAsync(int cartId)
        {
            var query = db.Cart_Menus
                .Include(cm => cm.FoodSize)
                .Where(cm => cm.CartId == cartId);

            var distinctFoodCount = await query
                .Select(cm => cm.FoodSizeId)
                .Distinct()
                .CountAsync();

            var totalPrice = await query
                .SumAsync(cm => cm.FoodSize.Price * cm.Count);

            return (distinctFoodCount, totalPrice);
        }

    }
}
