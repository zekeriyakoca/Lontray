

using Catalog.API.AppServices;
using Catalog.API.Dtos;
using Catalog.API.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly ILogger<CatalogController> logger;
        private readonly CatalogContext context;
        private readonly ICatalogAppService catalogAppService;

        public CatalogController(ILogger<CatalogController> logger, CatalogContext context, ICatalogAppService catalogAppService)
        {
            this.logger = logger;
            this.context = context;
            this.catalogAppService = catalogAppService;
        }

        [HttpGet("/Test")]
        public IActionResult Index()
        {
            return Ok("");
        }

        [HttpPost("/Create")]
        public async Task<IActionResult> AddCatalogItem([FromBody] CreateCatalogItemDto dto)
        {
            if (dto == null)
                return BadRequest();

            await catalogAppService.CreateCatalogItem(dto);
            return Ok("");
        }

        [HttpPut("/Update")]
        public async Task<IActionResult> UpdateCatalogItem([FromBody] UpdateCatalogItemDto dto)
        {
            if (dto == null)
                return BadRequest();

            await catalogAppService.UpdateCatalogItem(dto);
            return Ok("");
        }

    }
}
