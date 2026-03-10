using Ecommerce.DTO;
using Ecommerce.Models;
using Ecommerce.Services.OrderService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderApiController : ControllerBase
    {
        private readonly IOrderService orderService;
        private readonly UserManager<Users> userManager;

        public OrderApiController(IOrderService orderService, UserManager<Users> userManager)
        {
            this.orderService = orderService;
            this.userManager = userManager;
        }

        [Authorize(Roles = "User")]
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder()
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new StatusDTO { IsSuccess = false, Message = "Người dùng chưa đăng nhập." });

            var result = await orderService.CreateOrderAsync(userId);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // 🔹 [ADMIN/STAFF] Lấy danh sách tất cả đơn hàng (chưa hoàn thành)
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        // Lấy chi tiết 1 đơn hàng theo ID
        [Authorize(Roles = "Admin,Staff,User")]
        [HttpGet("GetOrderDetails/{orderId}")]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            var order = await orderService.GetOrderDetailsByIdAsync(orderId);
            if (order == null)
                return NotFound(new StatusDTO { IsSuccess = false, Message = "Không tìm thấy đơn hàng." });

            return Ok(order);
        }

        // 🔹 [USER] Lấy danh sách đơn hàng của người dùng (bao gồm chi tiết)
        [Authorize(Roles = "User")]
        [HttpGet("GetMyOrders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new StatusDTO { IsSuccess = false, Message = "Người dùng chưa đăng nhập." }); ;

            var orders = await orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }

        // 🔹 [ADMIN/STAFF] Cập nhật trạng thái đơn hàng (Tiếp nhận đơn, Đang chuẩn bị, Hoàn thành)
        [Authorize(Roles = "Admin,Staff")]
        [HttpPut("UpdateOrderStatus/{orderId}")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromQuery] string newStatus)
        {
            var result = await orderService.UpdateOrderStatusAsync(orderId, newStatus);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // 🔹 [USER] Gửi yêu cầu thanh toán (chỉ khi đơn hàng đã hoàn thành)
        [Authorize(Roles = "User")]
        [HttpPut("RequestPayment/{orderId}")]
        public async Task<IActionResult> RequestPayment(int orderId)
        {
            var result = await orderService.RequestPaymentAsync(orderId);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "User")]
        [HttpPost("UpdatePaymentMethodandTotal")]
        public async Task<IActionResult> UpdatePaymentMethodandTotal(ConfirmPaymentDTO model)
        {
            var result = await orderService.UpdatePaymentMethodandTotal(model);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetOrdersPayByCash")]
        public async Task<IActionResult> GetOrdersPayByCash()
        {
            var orders = await orderService.GetCashOrdersAsync();
            return Ok(orders);
        }

        [Authorize(Roles = "User")]
        [HttpGet("GetOrderHasPayment")]
        public async Task<IActionResult> GetOrderHasPayment()
        {
            var orders = await orderService.GetOrderHasPayment();
            return Ok(orders);
        }
    }
}
