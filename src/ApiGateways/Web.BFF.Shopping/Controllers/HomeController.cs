using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using CatalogGrpc;
using Grpc.Core;
using System.Threading.Tasks;

namespace Web.BFF.Shopping.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {

        private readonly ILogger<HomeController> logger;
        private readonly Catalog.CatalogClient catalogClient;

        public HomeController(ILogger<HomeController> logger, Catalog.CatalogClient catalogClient)
        {
            this.logger = logger;
            this.catalogClient = catalogClient;
        }

        [HttpGet]
        [Authorize]
        public IEnumerable<string> Get()
        {
            return new string[2] { "Get it!", "Get it!" };
        }

        [HttpGet("ValidateBasket")]
        public async Task<IActionResult> ValidateBasket() // Test method
        {
            var request = new CheckAvailibilityRequest();
            //{
            //    Items = new List<CatalogItemAvailibilityRequest>() {
            //        new CatalogItemAvailibilityRequest()
            //        {
            //            Id = 2,
            //            Quantity = 1
            //        }
            //    }
            //};

            var response = await catalogClient.ValidateBasketAsync(request);
            return Ok();
        }
    }
}
