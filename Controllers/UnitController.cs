using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Strata.Data;
using Strata.Helpers;
using Strata.Models.Catalog;
using Strata.ViewModel.Catalog.UnitOfMeasure;

namespace Strata.Controllers;

[Authorize]
public class UnitController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public UnitController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(
        string searchString,
        string sortOrder,
        int pageNumber = 1
    )
    {
        int pageSize = 25;
        
        ViewData["CurrentSort"] = sortOrder;
        ViewData["NameSort"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        ViewData["CreatedSort"] = sortOrder == "CreatedAt" ? "created_desc" : "CreatedAt";

        var query = _context.UnitOfMeasures.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            query = query.Where(c => c.Name.Contains(searchString));
        }

        query = sortOrder switch
        {
            "name_desc" => query.OrderByDescending(c => c.Name).ThenBy(c => c.Id),
            "CreatedAt" => query.OrderBy(c => c.CreatedAt).ThenBy(c => c.Id),
            "created_desc" => query.OrderByDescending(c => c.CreatedAt).ThenBy(c => c.Id),
            _ => query.OrderBy(c => c.Name).ThenBy(c => c.Id)
        };

        var pagedBrand = await PaginatedList<UnitOfMeasure>.CreateAsync(query, pageNumber, pageSize);

        ViewData["CurrentFilter"] = searchString;
        return View("~/Views/Catalog/Unit/Index.cshtml", pagedBrand);
    }

    public async Task<IActionResult> Create()
    {
        return PartialView("~/Views/Catalog/Unit/_CreatePartial.cshtml");
    }

    [HttpPost]
    public async Task<IActionResult> Create(UnitCreateViewModal model)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("~/Views/Catalog/Unit/_CreatePartial.cshtml");
        }

        if (await _context.UnitOfMeasures.AnyAsync(u => u.Name.ToLower() == model.Name.ToLower().Trim()))
        {
            ModelState.AddModelError(nameof(model.Name), "A unit with the same name already exists.");
            
            return PartialView("~/Views/Catalog/Unit/_CreatePartial.cshtml");
        }

        var unit = new UnitOfMeasure
        {
            Name = model.Name,
            Abbreviation = model.Abbreviation
        };
        
        _context.UnitOfMeasures.Add(unit);
        await _context.SaveChangesAsync();
        
        return Json(new { success = true });
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
        
        return PartialView("~/Views/Catalog/Unit/_EditPartial.cshtml", model);
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
            return PartialView("~/Views/Catalog/Unit/_EditPartial.cshtml");
        }

        if (await _context.UnitOfMeasures.AnyAsync(u => u.Id != model.Id && u.Name.ToLower() == model.Name.ToLower().Trim()))
        {
            ModelState.AddModelError(nameof(model.Name), "Another unit with the same name already exists.");
            
            return PartialView("~/Views/Catalog/Unit/_EditPartial.cshtml", model);
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
        
        return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var unit = await _context.UnitOfMeasures.FindAsync(id);

        if (unit == null)
        {
            return NotFound();
        }

        var model = new UnitDeleteViewModel
        {
            Id = unit.Id,
            Name = unit.Name,
        };
        
        return PartialView("~/Views/Catalog/Unit/_DeletePartial.cshtml", model);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> Remove(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var unit = await _context.UnitOfMeasures.FindAsync(id);

        if (unit == null)
        {
            return NotFound();
        }
        
        _context.UnitOfMeasures.Remove(unit);
        await _context.SaveChangesAsync();
        
        return Json(new { success = true });
    }
}