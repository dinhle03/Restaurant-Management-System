using Ecommerce.Services.RevenueService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RevenueApiController : ControllerBase
    {
        private readonly IRevenueService revenueService;

        public RevenueApiController(IRevenueService revenueService)
        {
            this.revenueService = revenueService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRevenues()
        {
            var result = await revenueService.GetAllRevenues();
            return Ok(result);
        }

        [HttpGet("GetNumberOfUserandRevenueOfDay")]
        public async Task<IActionResult> GetNumberOfUserandRevenueOfDay()
        {
            var result = await revenueService.GetNumberOfUserandRevenueOfDay();
            return Ok(result);
        } 
    }
}
