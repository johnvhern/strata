using Microsoft.AspNetCore.Mvc;
using Strata.Data;

namespace Strata.Controllers
{
    public class SparePartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SparePartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }
    }
}
