using Ecommerce.DTO;
using Ecommerce.Models;
using Ecommerce.Services.CartService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    public class CartApiController : ControllerBase
    {
        private readonly ICartService cartService;
        private readonly UserManager<Users> userManager;

        public CartApiController(ICartService cartService, UserManager<Users> userManager)
        {
            this.cartService = cartService;
            this.userManager = userManager;
        }

        [HttpGet("GetCartItem")]
        public async Task<IActionResult> GetCartItem()
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new StatusDTO { IsSuccess = false, Message = "Người dùng chưa đăng nhập" });

            var result = await cartService.GetCartItem(userId);
            return Ok(result);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] Cart model)
        {

            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Không xác định được người dùng");

            model.CartStatus = "Chưa xác nhận";
            model.CustomerId = userId;

            var result = await cartService.Create(model);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromQuery] int foodSizeId, [FromQuery] int cartId)
        {
            if (foodSizeId <= 0 || cartId <= 0)
                return BadRequest(new StatusDTO { IsSuccess = false, Message = "Dữ liệu không hợp lệ" });

            var result = await cartService.AddToCart(foodSizeId, cartId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("RemoveToCart")]
        public async Task<IActionResult> RemoveToCart([FromQuery] int foodSizeId, [FromQuery] int cartId)
        {
            if (foodSizeId <= 0 || cartId <= 0)
                return BadRequest(new StatusDTO { IsSuccess = false, Message = "Dữ liệu không hợp lệ" });

            var result = await cartService.RemoveToCart(foodSizeId, cartId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("GetQuantityCartItem")]
        public async Task<IActionResult> GetQuantityCartItem()
        {
            // 🔹 Lấy userId từ token JWT
            var userId = User.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new StatusDTO { IsSuccess = false, Message = "Người dùng chưa đăng nhập" });

            // 🔹 Gọi service
            var (quantity, total) = await cartService.GetQuantityCartItem(userId);

            return Ok(new
            {
                IsSuccess = true,
                Message = "Lấy thông tin giỏ hàng thành công",
                Quantity = quantity,
                TotalAmount = total
            });
        }

        [HttpGet("Summary/{cartId}")]
        public async Task<IActionResult> GetCartSummary(int cartId)
        {
            var (distinctCount, totalPrice) = await cartService.GetCartSummaryAsync(cartId);

            return Ok(new
            {
                CartId = cartId,
                DistinctFoodCount = distinctCount,
                TotalPrice = totalPrice
            });
        }

    }
}
