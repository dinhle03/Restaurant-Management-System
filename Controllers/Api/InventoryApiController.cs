using Ecommerce.DTO;
using Ecommerce.Models;
using Ecommerce.Services.InventoryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class InventoryApiController : ControllerBase
    {
        private readonly IInventoryService inventoryService;

        public InventoryApiController(IInventoryService inventoryService)
        {
            this.inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                var filtered = await inventoryService.Search(search);
                return Ok(filtered);
            }

            var result = await inventoryService.GetAll();
            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await inventoryService.GetById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Inventory model)
        {
            var result = await inventoryService.Create(model);
            if (!result.IsSuccess)
                return BadRequest(result);

            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Inventory model)
        {
            if (id != model.InventoryId)
                return Ok(new StatusDTO { IsSuccess = false, Message = "ID không khớp" });

            var result = await inventoryService.Update(model);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await inventoryService.Delete(id);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
