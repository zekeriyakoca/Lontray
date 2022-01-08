using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WatchDog.Models;

namespace WatchDog.Controllers
{
    public class HomeController : Controller
    {
        public IConfiguration Configuration { get; }

        public HomeController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        //public IActionResult Index() => RedirectToRoute("/hc-ui");
        public IActionResult Index() => View();

        // List all configs including ovirriden config
        [HttpGet("/config")]
        public IActionResult Config()
        {
            var allHealthCheckConfigs = Configuration.GetSection("HealthChecksUI:HealthChecks").GetChildren()
                                 .Union(Configuration.GetSection("HealthChecks-UI:HealthChecks").GetChildren());

            var children = allHealthCheckConfigs
                .SelectMany(cs => cs.GetChildren())
                .Select(c => new { Path = c.Path, Value = c.Value })
                .GroupBy(c => c.Path.Substring(0, c.Path.LastIndexOf(":")))
                .ToDictionary(c => c.ElementAt(0).Value, c => c.ElementAt(1).Value);


            return Json(children);
        }



    }
}
