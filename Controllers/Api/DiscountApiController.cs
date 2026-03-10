using Ecommerce.DTO;
using Ecommerce.Models;
using Ecommerce.Services.DiscountService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountApiController : ControllerBase
    {
        private readonly IDiscountService discountService;

        public DiscountApiController(IDiscountService discountService)
        {
            this.discountService = discountService;
        }

        // 🔹 GET all + search
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var result = await discountService.GetAll(search);
            return Ok(result);
        }

        // 🔹 GET by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await discountService.GetById(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        // 🔹 POST create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Discount model)
        {
            if (model.RequiredPoints < 0)
                return BadRequest(new StatusDTO { IsSuccess = false, Message = "Điểm cần đổi không hợp lệ" });

            var result = await discountService.Create(model);
            if (!result.IsSuccess)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }

        // 🔹 PUT update
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Discount model)
        {
            if (id != model.DiscountId)
                return BadRequest(new StatusDTO { IsSuccess = false, Message = "ID không khớp" });

            if (model.RequiredPoints < 0)
                return BadRequest(new StatusDTO { IsSuccess = false, Message = "Điểm cần đổi không hợp lệ" });

            var result = await discountService.Update(model);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // 🔹 DELETE
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await discountService.Delete(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        // 🔹 Người dùng nhận mã (cũ)
        [Authorize(Roles = "User")]
        [HttpPost("receive")]
        public async Task<IActionResult> ReceiveDiscount([FromQuery] string discountId, [FromQuery] string customerId)
        {
            var result = await discountService.AddDiscountToCustomer(discountId, customerId);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        // ✅ 🔹 NEW FEATURE: Đổi mã bằng điểm
        [Authorize] // chỉ cần đăng nhập, không cần Role cứng
        [HttpPost("exchange")]
        public async Task<IActionResult> ExchangeDiscount([FromBody] ExchangeDiscountDTO dto)
        {
            try
            {
                // 🧩 Debug claim để kiểm tra token có gì
                Console.WriteLine("🧾 ======= CLAIMS FROM TOKEN =======");
                foreach (var c in User.Claims)
                    Console.WriteLine($"🔹 {c.Type} = {c.Value}");
                Console.WriteLine("===================================");

                // 🧩 Lấy customerId từ claim do JWT hiện tại đang dùng key 'userId'
                var customerId = User.FindFirstValue("userId");

                // fallback nếu token không có userId thì lấy từ DTO
                if (string.IsNullOrEmpty(customerId))
                    customerId = dto.CustomerId;

                if (string.IsNullOrEmpty(customerId))
                    return Unauthorized("Không tìm thấy thông tin người dùng.");

                // 🧩 Kiểm tra role thủ công (vì JWT của bạn không theo chuẩn ClaimTypes.Role)
                var role = User.FindFirstValue("role") ?? "User";
                Console.WriteLine($"🔸 Role hiện tại trong token: {role}");

                if (role != "User" && role != "Customer")
                    return Forbid("Bạn không có quyền đổi mã này.");

                // 🧩 Gọi service
                var result = await discountService.ExchangeDiscount(dto.DiscountId, customerId);
                if (!result.IsSuccess)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new StatusDTO
                {
                    IsSuccess = false,
                    Message = $"Lỗi khi đổi mã giảm giá: {ex.Message}"
                });
            }
        }
    }
}
