using Ecommerce.DTO;
using Ecommerce.Models;
using Ecommerce.Repositories.CartRepository;
using Ecommerce.Repositories.FoodSizeRepository;
using Ecommerce.Repositories.TableRepository;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ICartRepository cartRepository;
        private readonly IFoodSizeRepository foodSizeRepository;
        private readonly ITableRepository tableRepository;
        private readonly UserManager<Users> userManager;

        public CartService(ICartRepository cartRepository, IFoodSizeRepository foodSizeRepository, ITableRepository tableRepository, UserManager<Users> userManager)
        {
            this.cartRepository = cartRepository;
            this.foodSizeRepository = foodSizeRepository;
            this.tableRepository = tableRepository;
            this.userManager = userManager;
        }

        public async Task<StatusDTO> AddToCart(int foodsizeId, int cartId)
        {
            var foodsize = await foodSizeRepository.GetById(foodsizeId);
            if(foodsize == null) return new StatusDTO { IsSuccess = false, Message = "Không tìm thấy món ăn" };

            var cart = await cartRepository.GetById(cartId);
            if (cart == null) return new StatusDTO { IsSuccess = false, Message = "Không tìm thấy giỏ hàng" };

            await cartRepository.AddToCart(foodsizeId, cartId);
            return new StatusDTO { IsSuccess = true, Message = "Thêm món vào giỏ hàng thành công" };
        }

        public async Task<CreateCartDTO> Create(Cart model)
        {
            var cart = await tableRepository.GetById(model.TableId);
            if (cart == null) return new CreateCartDTO { IsSuccess = false, Message = "Bàn không tồn tại" };

            var cartId = await cartRepository.CreateAsync(model);
            return new CreateCartDTO { CartId = cartId, IsSuccess = true, Message = "Tạo giỏ hàng thành công" };
        }

        public async Task<IEnumerable<CartItemDTO>> GetCartItem(string userId)
        {
            var cartItem = await cartRepository.getCartItem(userId);
            return cartItem;
        }

        public async Task<StatusDTO> RemoveToCart(int foodsizeId, int cartId)
        {
            var foodsize = await foodSizeRepository.GetById(foodsizeId);
            if (foodsize == null) return new StatusDTO { IsSuccess = false, Message = "Không tìm thấy món ăn" };

            var cart = await cartRepository.GetById(cartId);
            if (cart == null) return new StatusDTO { IsSuccess = false, Message = "Không tìm thấy giỏ hàng" };

            await cartRepository.RemoveToCart(foodsizeId, cartId);
            return new StatusDTO { IsSuccess = true, Message = "Giảm món trong giỏ hàng thành công" };
        }

        public async Task<(int quantity, decimal total)> GetQuantityCartItem(string userId)
        {
            var (quantity, total) = await cartRepository.GetQuantityCartItem(userId);
            return (quantity, total);
        }

        public async Task<(int distinctFoodCount, decimal totalPrice)> GetCartSummaryAsync(int cartId)
        {
            return await cartRepository.GetCartSummaryAsync(cartId);
        }
    }
}
