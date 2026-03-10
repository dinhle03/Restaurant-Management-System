using Ecommerce.Models;
using Ecommerce.Services.MenuCategoryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuCategoryApiController : ControllerBase
    {
        private readonly IMenuCategoryService menuCategoryService;

        public MenuCategoryApiController(IMenuCategoryService menuCategoryService)
        {
            this.menuCategoryService = menuCategoryService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetAll()
        {
            var items = await menuCategoryService.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await menuCategoryService.GetByIdAsync(id);
            if (item == null)
                return NotFound("Không tìm thấy loại món ăn.");

            return Ok(item);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] MenuCategory model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dữ liệu không hợp lệ.");

            var result = await menuCategoryService.CreateAsync(model);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] MenuCategory model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dữ liệu không hợp lệ.");

            var result = await menuCategoryService.UpdateAsync(model);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await menuCategoryService.DeleteAsync(id);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }
    }
}
