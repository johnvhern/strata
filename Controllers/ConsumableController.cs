using Microsoft.AspNetCore.Mvc;

namespace Strata.Controllers
{
    public class ConsumableController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
