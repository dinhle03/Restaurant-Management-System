using Ecommerce.Data;
using Ecommerce.DTO;
using Ecommerce.Services.MomoService;
using Ecommerce.Services.PaymentService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentApiController : ControllerBase
    {
        private readonly IPaymentService paymentService;

        public PaymentApiController(IMomoService momoService, IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }


        [HttpPost("CreatePaymentMomo")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreatePaymentMomo([FromBody] OrderInfo model)
        {
            var url = await paymentService.CreatePaymentUrlMomo(model);
            return Ok(new { url });
        }


        [HttpGet("PaymentCallback")]
        [HttpPost("PaymentCallback")]
        public async Task<IActionResult> PaymentCallback()
        {
            try
            {
                var result = await paymentService.PaymentCallbackAsync(HttpContext.Request.Query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Đã xảy ra lỗi khi xử lý callback thanh toán.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("ConfirmOrderPayByCash")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmOrderPayByCash(int orderId)
        {
            var result = await paymentService.ConfirmOrderPayByCash(orderId);
            return Ok(result);
        }
    }
}