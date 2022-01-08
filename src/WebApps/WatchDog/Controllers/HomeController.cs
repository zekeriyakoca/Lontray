using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace WatchDog.Controllers
{
    public class HomeController : Controller
    {
        public IConfiguration Configuration { get; }

        public HomeController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IActionResult Index()
        {
            var model = Configuration.GetSection("Urls").GetChildren()
                .ToDictionary(u => u.GetValue<string>("Name"), u => u.GetValue<string>("Url"));

            return View(model);
        }

        // List all configs including ovirriden config
        [Route("/config")]
        public IActionResult Config()
        {
            var allHealthCheckConfigs = Configuration.GetSection("HealthChecksUI:HealthChecks").GetChildren()
                                 .Union(Configuration.GetSection("HealthChecks-UI:HealthChecks").GetChildren());

            var children = allHealthCheckConfigs
                .SelectMany(cs => cs.GetChildren())
                .Select(c => new { Path = c.Path, Value = c.Value })
                .GroupBy(c => c.Path.Substring(0, c.Path.LastIndexOf(":")))
                //.OrderBy(c => c.ElementAt(0).Value)
                .ToDictionary(c => c.ElementAt(0).Value, c => c.ElementAt(1).Value);


            return View(children);
        }



    }
}
