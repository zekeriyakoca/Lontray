using Microsoft.AspNetCore.Mvc;

namespace Ordering.API.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index() => new RedirectResult("~/swagger");
    }
}
