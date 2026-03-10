using Ecommerce.Models;
using Ecommerce.Services.EmployeeService;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class EmployeeApiController : ControllerBase
    {
        private readonly IEmployeeService employeeService;

        public EmployeeApiController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string? search)
            => Ok(await employeeService.GetAllAsync(search));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await employeeService.GetByIdAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] RegisterViewModel model, IFormFile? imageFile)
        {
            var result = await employeeService.CreateAsync(model, imageFile);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromForm] Users model, IFormFile? imageFile)
        {
            var result = await employeeService.UpdateAsync(id, model, imageFile);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await employeeService.DeleteAsync(id);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }
    }
}
