using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Services.DiscountCustomerService;
using Ecommerce.Models;
using Ecommerce.DTO;
using System.Security.Claims;
using Ecommerce.Data;

namespace Ecommerce.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DiscountCustomerApiController : ControllerBase
    {
        private readonly IDiscountCustomerService _discountCustomerService;
        private readonly AppDbContext _context;

        public DiscountCustomerApiController(
            IDiscountCustomerService discountCustomerService,
            AppDbContext context)
        {
            _discountCustomerService = discountCustomerService;
            _context = context;
        }

        // 🔹 1. Lấy danh sách mã giảm giá của user hiện tại
        [HttpGet]
        public async Task<IActionResult> GetUserDiscounts()
        {
            // ✅ FIX: đọc cả "userId" (token cũ Flutter) hoặc "nameid" (token chuẩn .NET)
            var customerId = User.FindFirstValue("userId")
                           ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(customerId))
            {
                Console.WriteLine("⚠️ Không tìm thấy thông tin userId trong token!");
                return Unauthorized("Không tìm thấy thông tin người dùng trong token.");
            }

            Console.WriteLine($"📥 Lấy mã giảm giá cho userId = {customerId}");

            var discounts = await _context.Discount_Customers
                .Include(dc => dc.Discount)
                .Where(dc => dc.CustomerId == customerId && dc.isUsed == false)
                .Select(dc => new
                {
                    dc.DiscountId,
                    dc.CustomerId,
                    dc.isUsed,
                    discount = new
                    {
                        dc.Discount.DiscountId,
                        dc.Discount.DiscountName,
                        dc.Discount.DiscountCategory,
                        dc.Discount.DiscountPrice,
                        dc.Discount.DateStart,
                        dc.Discount.DateEnd,
                        dc.Discount.DiscountStatus
                    }
                })
                .ToListAsync();

            Console.WriteLine($"✅ Trả về {discounts.Count} mã giảm giá cho CustomerId = {customerId}");
            return Ok(discounts);
        }

        // 🔹 2. Đánh dấu mã là đã dùng
        [HttpPut("use/{discountId}")]
        public async Task<IActionResult> UseDiscount(string discountId)
        {
            // ✅ Cũng sửa tương tự ở đây
            var customerId = User.FindFirstValue("userId")
                           ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(customerId))
            {
                Console.WriteLine("⚠️ Không tìm thấy customerId trong token khi sử dụng mã!");
                return Unauthorized("Không tìm thấy người dùng trong token.");
            }

            var result = await _discountCustomerService.UseDiscountAsync(customerId, discountId);
            if (!result)
            {
                Console.WriteLine($"❌ Không thể sử dụng mã {discountId} cho userId={customerId}");
                return BadRequest("Không thể sử dụng mã này (đã dùng hoặc không tồn tại).");
            }

            Console.WriteLine($"✅ UserId={customerId} đã sử dụng mã {discountId}");
            return Ok(new { message = "✅ Đã sử dụng mã thành công." });
        }

        // 🔹 3. Admin gán mã giảm giá cho user
        [HttpPost("assign")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignDiscountToUser([FromBody] AssignDiscountDTO dto)
        {
            try
            {
                Console.WriteLine("📥 Nhận yêu cầu gán mã giảm giá:");
                Console.WriteLine($"   DiscountId = {dto.DiscountId}");
                Console.WriteLine($"   CustomerId = {dto.CustomerId}");

                if (string.IsNullOrEmpty(dto.DiscountId) || string.IsNullOrEmpty(dto.CustomerId))
                {
                    Console.WriteLine("⚠️ Thiếu discountId hoặc customerId trong body.");
                    return BadRequest(new { message = "Thiếu thông tin discountId hoặc customerId." });
                }

                var exists = await _context.Discount_Customers
                    .AnyAsync(dc => dc.CustomerId == dto.CustomerId && dc.DiscountId == dto.DiscountId);

                if (exists)
                {
                    Console.WriteLine("⚠️ Người dùng này đã có mã này.");
                    return BadRequest(new { message = "Người dùng này đã có mã giảm giá này." });
                }

                var newRecord = new Discount_Customer
                {
                    DiscountId = dto.DiscountId,
                    CustomerId = dto.CustomerId,
                    isUsed = false
                };

                _context.Discount_Customers.Add(newRecord);
                await _context.SaveChangesAsync();

                Console.WriteLine("✅ Gán mã thành công!");
                return Ok(new { message = "✅ Đã gán mã giảm giá cho người dùng thành công." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("🔥 Lỗi khi gán mã:");
                Console.WriteLine(ex.ToString());
                return StatusCode(500, new { message = $"Lỗi nội bộ server: {ex.Message}" });
            }
        }
        [HttpPost("exchange")]
        public async Task<IActionResult> ExchangeDiscount([FromBody] ExchangeDiscountDTO dto)
        {
            var customerId = User.FindFirstValue("userId")
                           ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(customerId))
                return Unauthorized("Không tìm thấy thông tin người dùng.");

            if (string.IsNullOrEmpty(dto.DiscountId))
                return BadRequest("Thiếu DiscountId.");

            var result = await _discountCustomerService
                .ExchangeDiscountAsync(customerId, dto.DiscountId);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }



    }
}
