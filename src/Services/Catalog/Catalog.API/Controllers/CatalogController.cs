

using Catalog.API.Infrastructure;
using Catalog.API.IntegrationEvents.Events;
using EventBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly ILogger<CatalogController> logger;
        private readonly CatalogContext context;
        private readonly IEventBus eventBus;

        public CatalogController(ILogger<CatalogController> logger, CatalogContext context, IEventBus eventBus)
        {
            this.logger = logger;
            this.context = context;
            this.eventBus = eventBus;
        }

        [HttpGet("/Test")]
        public IActionResult Index()
        {
            eventBus.Publish(new SomethingDoneIntegrationEvent("Test"));
            return Ok("");
        }
    }
}
