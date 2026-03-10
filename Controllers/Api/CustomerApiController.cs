using Ecommerce.DTO;
using Ecommerce.Services.Account;
using Ecommerce.Services.Customer;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerApiController : ControllerBase
    {
        private readonly ICustomerService customerService;

        public CustomerApiController(ICustomerService customerService)
        {
            this.customerService = customerService;
        }

        [HttpGet("GetAllCustomer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllCustomer(string? search = null)
        {
            var lstCustomer = await customerService.GetAll(search);
            return Ok(lstCustomer);
        }

        [HttpGet("GetCustomerById")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetCustomerById(string userId)
        {
            var customer = await customerService.GetById(userId);
            if (customer.IsSuccess == false)
            {
                return BadRequest(customer.Message);
            }

            return Ok(customer);
        }

        [HttpPut("UpdateCustomer")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> UpdateCustomer(string userId, [FromForm] UpdateCustomerViewModel model, IFormFile? UrlImage)
        {
            var result = await customerService.Update(model, userId, UrlImage);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpDelete("DeleteCustomer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCustomer(string userId)
        {
            var result = await customerService.Delete(userId);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpPut("LockUser/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> LockUser(string userId)
        {
            var result = await customerService.LockUserAsync(userId);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpPut("UnlockUser/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnlockUser(string userId)
        {
            var result = await customerService.UnlockUserAsync(userId);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpGet("GetLockedCustomers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetLockedCustomers()
        {
            var lockedCustomers = await customerService.GetLockedCustomersAsync();
            return Ok(lockedCustomers);
        }


    }
}