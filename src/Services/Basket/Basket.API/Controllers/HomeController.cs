using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers
{
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index() => new RedirectResult("~/swagger");
    }
}
