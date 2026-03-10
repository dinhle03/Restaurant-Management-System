using Ecommerce.DTO;
using Ecommerce.Models;
using Ecommerce.Services.FoodSizeService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Có thể bỏ nếu bạn muốn public cho khách xem
    public class FoodSizeApiController : ControllerBase
    {
        private readonly IFoodSizeService foodsizeService;

        public FoodSizeApiController(IFoodSizeService foodsizeService)
        {
            this.foodsizeService = foodsizeService;
        }

        // ===========================
        // GET: api/FoodSizeApi
        // ===========================
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                var result = await foodsizeService.Search(search);
                return Ok(result);
            }

            var all = await foodsizeService.GetAll();
            return Ok(all);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await foodsizeService.GetById(id);
            if (result == null)
            {
                return NotFound(new StatusDTO
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy biến thể"
                });
            }
            return Ok(result);
        }


        // ===========================
        // POST: api/FoodSizeApi
        // ===========================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FoodSize model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new StatusDTO
                {
                    IsSuccess = false,
                    Message = "Dữ liệu không hợp lệ"
                });
            }

            var result = await foodsizeService.Create(model);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // ===========================
        // PUT: api/FoodSizeApi/{id}
        // ===========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FoodSize model)
        {
            if (id != model.FoodSizeId)
            {
                return BadRequest(new StatusDTO
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy biến thể cần cập nhật"
                });
            }

            var result = await foodsizeService.Update(model);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // ===========================
        // DELETE: api/FoodSizeApi/{id}
        // ===========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await foodsizeService.Delete(id);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
