using Ecommerce.DTO;
using Ecommerce.Models;

namespace Ecommerce.Services.CartService
{
    public interface ICartService
    {
        Task<IEnumerable<CartItemDTO>> GetCartItem(string userId);
        Task<CreateCartDTO> Create(Cart model);
        Task<StatusDTO> AddToCart(int foodsizeId, int cartId);
        Task<StatusDTO> RemoveToCart(int foodsizeId, int cartId);
        Task<(int quantity, decimal total)> GetQuantityCartItem(string userId);
        Task<(int distinctFoodCount, decimal totalPrice)> GetCartSummaryAsync(int cartId);
    }
}
