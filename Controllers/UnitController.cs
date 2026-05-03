using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Strata.Data;
using Strata.Helpers;
using Strata.Models.Catalog;
using Strata.ViewModel.Catalog.UnitOfMeasure;

namespace Strata.Controllers;

public class UnitController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public UnitController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(
        string searchString,
        int pageNumber = 1
    )
    {
        int pageSize = 25;

        var query = _context.UnitOfMeasures.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            query = query.Where(c => c.Name.Contains(searchString));
        }

        query = query.OrderBy(c => c.Name).ThenBy(c => c.Id);

        var pagedBrand = await PaginatedList<UnitOfMeasure>.CreateAsync(query, pageNumber, pageSize);

        ViewData["CurrentFilter"] = searchString;
        return View("~/Views/Catalog/Unit/Index.cshtml", pagedBrand);
    }

    public async Task<IActionResult> Create()
    {
        return View("~/Views/Catalog/Unit/Create.cshtml");
    }

    [HttpPost]
    public async Task<IActionResult> Create(UnitCreateViewModal model)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Catalog/Unit/Create.cshtml");
        }

        if (await _context.UnitOfMeasures.AnyAsync(u => u.Name.ToLower() == model.Name.ToLower().Trim()))
        {
            ModelState.AddModelError(nameof(model.Name), "A unit with the same name already exists.");
            
            return View("~/Views/Catalog/Unit/Create.cshtml");
        }

        var unit = new UnitOfMeasure
        {
            Name = model.Name,
            Abbreviation = model.Abbreviation
        };
        
        _context.UnitOfMeasures.Add(unit);
        await _context.SaveChangesAsync();
        
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        
        var unit = await _context.UnitOfMeasures.FirstOrDefaultAsync(u => u.Id == id);

        if (unit == null)
        {
            return NotFound();
        }

        var model = new UnitEditViewModal
        {
            Name = unit.Name,
            Abbreviation =  unit.Abbreviation
        };
        
        return View("~/Views/Catalog/Unit/Edit.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, UnitEditViewModal model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View("~/Views/Catalog/Unit/Edit.cshtml");
        }

        if (await _context.UnitOfMeasures.AnyAsync(u => u.Id != model.Id && u.Name.ToLower() == model.Name.ToLower().Trim()))
        {
            ModelState.AddModelError(nameof(model.Name), "Another unit with the same name already exists.");
            
            return View("~/Views/Catalog/Unit/Edit.cshtml", model);
        }

        var unit = await _context.UnitOfMeasures.FindAsync(id);

        if (unit == null)
        {
            return NotFound();
        }
        
        unit.Name = model.Name;
        unit.Abbreviation = model.Abbreviation;
        
        _context.UnitOfMeasures.Update(unit);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}