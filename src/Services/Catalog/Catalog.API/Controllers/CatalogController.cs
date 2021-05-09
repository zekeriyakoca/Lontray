using Catalog.API.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ILogger<CatalogController> loggerl;
        private readonly CatalogContext context;

        public CatalogController(ILogger<CatalogController> logger, CatalogContext context)
        {
            this.loggerl = logger;
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
