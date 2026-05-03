using Microsoft.AspNetCore.Mvc;
using Strata.Data;

namespace Strata.Controllers;

public class UnitController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public UnitController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View();
    }
}