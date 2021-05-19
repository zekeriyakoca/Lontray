using Basket.API.Infrastructure.Repositories;
using Basket.API.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository basketRepository;
        private readonly ILogger<BasketController> logger;

        public BasketController(IBasketRepository basketRepository, ILogger<BasketController> logger)
        {
            this.basketRepository = basketRepository;
            this.logger = logger;
        }

        [HttpGet("{customerId}")]
        [ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK)]
        public async Task<ActionResult<CustomerBasket>> GetBasket([FromQuery] string customerId)
        {
            return Ok(await basketRepository.GetBasketAsync(customerId) ?? new CustomerBasket(customerId));
        }

        [HttpPost("Update")]
        [ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK)]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket([FromBody] CustomerBasket basket)
        {
            return Ok(await basketRepository.UpdateBasketAsync(basket));
        }

        [HttpPost("checkout/{customerId}")]
        [ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK)]
        public async Task<ActionResult<CustomerBasket>> CheckoutBasket([FromQuery] string customerId, [FromHeader(Name = "x-requestid")] string requestId)
        {
            var basket = await basketRepository.GetBasketAsync(customerId);

            if (basket == null)
            {
                return BadRequest();
            }

            throw new NotImplementedException();
        }

        [HttpGet("Delete/{customerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> DeleteBasket([FromQuery] string customerId)
        {
            await basketRepository.DeleteBasketAsync(customerId);
            return Ok();
        }
    }

}
