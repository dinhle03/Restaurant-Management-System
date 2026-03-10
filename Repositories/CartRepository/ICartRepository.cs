using Ecommerce.DTO;
using Ecommerce.Models;
using System.Security.Claims;

namespace Ecommerce.Repositories.CartRepository
{
    public interface ICartRepository
    {
        Task<int> CreateAsync(Cart model);
        Task AddToCart(int foodsizeId, int cartId);
        Task RemoveToCart(int foodsizeId, int cartId);
        Task<IEnumerable<CartItemDTO>> getCartItem(string userId);
        Task<Cart> GetById(int id);
        Task<(int,decimal)> GetQuantityCartItem(string userId);
        Task<Cart?> GetActiveCartByUserIdAsync(string userId);

        Task<(int distinctFoodCount, decimal totalPrice)> GetCartSummaryAsync(int cartId);
    }
}
