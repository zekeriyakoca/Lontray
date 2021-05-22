using Microsoft.AspNetCore.Mvc;

namespace Ordering.API.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => new RedirectResult("~/swagger");
    }
}
