using Ecommerce.Models;
using Ecommerce.Services.TableService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableApiController : ControllerBase
    {
        private readonly ITableService tableService;

        public TableApiController(ITableService tableService)
        {
            this.tableService = tableService;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var items = await tableService.GetById(id);
            return Ok(items);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var items = await tableService.GetAll();
            return Ok(items);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] Table model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dữ liệu không hợp lệ.");

            var result = await tableService.Create(model);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] Table model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dữ liệu không hợp lệ.");

            var result = await tableService.Update(model);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await tableService.Delete(id);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpGet("CreateQR")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateQR(int tableId)
        {
            var result = await tableService.CreateQRTable(tableId);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpGet("ValidTableToken")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ValidTableToken(string token)
        {
            var result = await tableService.ValidTableToken(token, User);
            return result.IsValid ? Ok(result) : BadRequest(result.Message);
        }

    }
}
