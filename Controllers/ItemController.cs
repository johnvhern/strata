using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Strata.Data;

namespace Strata.Controllers;

[Authorize]
public class ItemController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public ItemController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View("~/Views/Catalog/Item/Index.cshtml");
    }
}