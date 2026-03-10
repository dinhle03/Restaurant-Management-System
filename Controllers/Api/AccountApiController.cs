using Ecommerce.Repositories.AccountRepository;
using Ecommerce.Services.Account;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountApiController : ControllerBase
    {
        private readonly IAccountService accountService;

        public AccountApiController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] RegisterViewModel model, IFormFile? image)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await accountService.RegisterAsync(model, image);
            return result.isSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await accountService.LoginAsync(model);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpPost("SendOTP")]
        public async Task<IActionResult> SendOTP([FromBody] SendOTPViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await accountService.SendOTPAsync(model);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpPost("VerifyOTP")]
        public async Task<IActionResult> VerifyOTP(VerifyOTPViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await accountService.VerifyOTPAsync(model);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await accountService.ChangePasswordAsync(model);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await accountService.Logout();
            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }
    }
}
