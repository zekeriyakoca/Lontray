using Basket.API.Infrastructure.Repositories;
using Basket.API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // TODO : Add Authorization/Authentication
    public class BasketController : ControllerBase
    {
        [FromHeader(Name = "x-requestid")]
        private string RequestId { get; set; }

        private readonly IBasketRepository basketRepository;
        private readonly ILogger<BasketController> logger;

        public BasketController(IBasketRepository basketRepository, ILogger<BasketController> logger)
        {
            this.basketRepository = basketRepository;
            this.logger = logger;
        }

        [HttpGet("{customerId}")]
        [Authorize]
        [ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK)]
        public async Task<ActionResult<CustomerBasket>> GetBasket([FromRoute] string customerId)
        {
            return Ok(await basketRepository.GetBasketAsync(customerId) ?? new CustomerBasket(customerId));
        }

        [HttpPut("")]
        [ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK)]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket([FromBody] CustomerBasket basket)
        {
            return Ok(await basketRepository.UpdateBasketAsync(basket));
        }

        [HttpPost("merge/{oldCustomerId}/into/{customerId}")]
        [Authorize]
        [ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK)]
        public async Task<ActionResult<CustomerBasket>> MergeBaskets([FromRoute] string oldCustomerId, [FromRoute] string customerId)
        {
            if (String.IsNullOrWhiteSpace(oldCustomerId) || String.IsNullOrWhiteSpace(customerId))
                    return BadRequest("BasketIds cannot be null");

            var baseBasket = await basketRepository.GetBasketAsync(customerId) ?? new CustomerBasket(customerId);
            var oldBasket = await basketRepository.GetBasketAsync(oldCustomerId);
            baseBasket = baseBasket.MergeFrom(oldBasket);
            
            baseBasket = await basketRepository.UpdateBasketAsync(baseBasket);
            
            await basketRepository.DeleteBasketAsync(oldBasket.CustomerId);
            
            return Ok(baseBasket);
        }

        [HttpPost("checkout/{customerId}")]
        [ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK)]
        public async Task<ActionResult<CustomerBasket>> CheckoutBasket([FromRoute] string customerId)
        {
            var basket = await basketRepository.GetBasketAsync(customerId);

            if (basket == null)
            {
                return BadRequest();
            }

            throw new NotImplementedException();
        }

        [HttpDelete("{customerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> DeleteBasket([FromRoute] string customerId)
        {
            await basketRepository.DeleteBasketAsync(customerId);
            return Ok();
        }

        [HttpPost("ApplyCode/{code}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> ApplyCoupon([FromRoute] string code)
        {
            throw new NotImplementedException();
        }
    }

}
