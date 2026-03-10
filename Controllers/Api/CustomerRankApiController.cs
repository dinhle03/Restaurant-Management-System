using Ecommerce.Models;
using Ecommerce.Services.RankAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CustomerRankApiController : ControllerBase
    {
        private readonly IRankAccountService rankAccountService;
        public CustomerRankApiController(IRankAccountService rankAccountService) 
        {
            this.rankAccountService = rankAccountService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await rankAccountService.GetById(id);
            return Ok(item);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var lst = await rankAccountService.GetAllAsync();
            return Ok(lst);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await rankAccountService.DeleteAsync(id);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerRank model)
        {
            if (model == null) return NotFound();

            var result = await rankAccountService.CreateAsync(model);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CustomerRank model)
        {
            if(model == null) return NotFound();
            var result = await rankAccountService.UpdateAsync(model);
            return result.IsSuccess ? Ok(result.Message) : BadRequest(result.Message);
        }
    }
}
