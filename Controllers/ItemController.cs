using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Strata.Data;
using Strata.ViewModel.Catalog.Item;

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

    public async Task<IActionResult> Create()
    {
        var vm = new ItemCreateViewModel
        {
            BrandsOptions = await _context.Brands.Where(b => !b.IsDeleted).OrderBy(b => b.Name).Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name }).ToListAsync(),
                
            CategoryOptions = await _context.Categories.Where(c => !c.IsDeleted).OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToListAsync(),
            
            UnitOptions = await _context.UnitOfMeasures.Where(u => !u.IsDeleted).OrderBy(u => u.Name).Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Name }).ToListAsync(),
        };
        
        return PartialView("~/Views/Catalog/Item/_CreateItemPartial.cshtml", vm);
    }
}