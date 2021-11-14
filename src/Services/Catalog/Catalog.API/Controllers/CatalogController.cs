﻿

using Catalog.API.AppServices;
using Catalog.API.Dtos;
using Catalog.API.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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

        [HttpGet("{id}")]
        public async Task<ActionResult<CatalogItemDto>> GetItem([FromRoute] int id)
        {

            throw new NotImplementedException();
        }

        [HttpPost("")]
        public async Task<ActionResult<IEnumerable<CatalogItemDto>>> GetItems([FromBody] CatalogItemSearchParamsDto dto)
        {
            throw new NotImplementedException();
        }

        [HttpPost("")]
        public async Task<ActionResult> AddCatalogItem([FromBody] CreateCatalogItemDto dto)
        {
            if (dto == null)
                return BadRequest();

            await catalogAppService.CreateCatalogItem(dto);
            return Ok("");
        }

        [HttpPut("")]
        public async Task<ActionResult> UpdateCatalogItem([FromBody] UpdateCatalogItemDto dto)
        {
            if (dto == null)
                return BadRequest();

            await catalogAppService.UpdateCatalogItem(dto);
            return Ok("");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveCatalogItem([FromRoute] int id)
        {
            throw new NotImplementedException();
        }

    }
}
