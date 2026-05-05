using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Strata.Data;
using Strata.Helpers;
using Strata.Models.Catalog;
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

    public async Task<IActionResult> Index(
        string searchString,
        bool? isActive,
        string sortOrder,
        int pageNumber = 1
    )
    {
        int pageSize = 25;
        
        ViewData["CurrentSort"] = sortOrder;
        ViewData["NameSort"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        ViewData["ItemSort"] = sortOrder == "ItemCode" ? "item_desc" : "ItemCode";
        ViewData["CostSort"] = sortOrder == "StandardCost" ? "cost_desc" : "StandardCost";

        var query = _context
            .Items.AsNoTracking()
            .Include(i => i.Brand)
            .Include(i => i.Category)
            .Include(i => i.UnitOfMeasure)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            query = query.Where(i => i.Name.Contains(searchString));
        }

        if (isActive.HasValue)
        {
            query = query.Where(i => i.IsActive == isActive.Value);
        }

        query = sortOrder switch
        {
            "name_desc" => query.OrderByDescending(i => i.Name).ThenBy(i => i.Id),
            "ItemCode" => query.OrderBy(i => i.ItemCode).ThenBy(i => i.Id),
            "item_desc" => query.OrderByDescending(i => i.ItemCode).ThenBy(i => i.Id),
            "StandardCost" => query.OrderBy(i => i.StandardCost).ThenBy(i => i.Id),
            "cost_desc" => query.OrderByDescending(i => i.StandardCost).ThenBy(i => i.Id),
            _ => query.OrderBy(i => i.Name).ThenBy(i => i.Id)
        };

        var pagedItems = await PaginatedList<Item>.CreateAsync(
            query,
            pageNumber,
            pageSize
        );

        ViewData["CurrentFilter"] = searchString;
        ViewData["CurrentStatus"] = isActive;

        return View("~/Views/Catalog/Item/Index.cshtml", pagedItems);
    }

    public async Task<IActionResult> Create()
    {
        var vm = new ItemCreateViewModel
        {
            BrandsOptions = await _context.Brands.Where(b => !b.IsDeleted).OrderBy(b => b.Name).Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name }).ToListAsync(),
                
            CategoryOptions = await _context.Categories.Where(c => !c.IsDeleted).OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToListAsync(),
            
            UnitOptions = await _context.UnitOfMeasures.Where(u => !u.IsDeleted).OrderBy(u => u.Name).Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Name }).ToListAsync(),
        };
        
        return PartialView("~/Views/Catalog/Item/_CreatePartial.cshtml", vm);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ItemCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.BrandsOptions = await _context.Brands.Where(b => !b.IsDeleted).OrderBy(b => b.Name)
                .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name }).ToListAsync();

            model.CategoryOptions = await _context.Categories.Where(c => !c.IsDeleted).OrderBy(c => c.Name)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToListAsync();

            model.UnitOptions = await _context.UnitOfMeasures.Where(u => !u.IsDeleted).OrderBy(u => u.Name)
                .Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Name }).ToListAsync();
            
            return PartialView("~/Views/Catalog/Item/_CreatePartial.cshtml", model);
        }

        if (await _context.Items.AnyAsync(i => i.Name.ToLower() == model.Name.ToLower().Trim()))
        {
            ModelState.AddModelError(nameof(model.Name), "An item with the same name already exists.");
            return PartialView("~/Views/Catalog/Item/_CreatePartial.cshtml", model);
        }

        var item = new Item
        {
            ItemCode =  model.ItemCode,
            Name = model.Name,
            Description = model.Description,
            BrandId = model.BrandId,
            CategoryId = model.CategoryId,
            UnitOfMeasureId = model.UnitOfMeasureId,
            IsSerialized = model.IsSerialized,
            IsConsumable = model.IsConsumable,
            IsSparePart = model.IsSparePart,
            RequiresMaintenance = model.RequiresMaintenance,
            MinimumStockLevel = model.MinimumStockLevel,
            ReorderLevel = model.ReorderLevel,
            StandardCost = model.StandardCost,
            IsActive =  model.IsActive
        };
        
        _context.Items.Add(item);
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
        
        var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == id);

        if (item == null)
        {
            return NotFound();
        }
        
        var model = new ItemEditViewModel
        {
            BrandsOptions = await _context.Brands.Where(b => !b.IsDeleted).OrderBy(b => b.Name).Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name }).ToListAsync(),
                
            CategoryOptions = await _context.Categories.Where(c => !c.IsDeleted).OrderBy(c => c.Name).Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToListAsync(),
            
            UnitOptions = await _context.UnitOfMeasures.Where(u => !u.IsDeleted).OrderBy(u => u.Name).Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Name }).ToListAsync(),
            
            ItemCode =  item.ItemCode,
            Name = item.Name,
            Description = item.Description,
            BrandId = item.BrandId,
            CategoryId = item.CategoryId,
            UnitOfMeasureId = item.UnitOfMeasureId,
            IsSerialized = item.IsSerialized,
            IsConsumable = item.IsConsumable,
            IsSparePart = item.IsSparePart,
            RequiresMaintenance = item.RequiresMaintenance,
            MinimumStockLevel = item.MinimumStockLevel,
            ReorderLevel = item.ReorderLevel,
            StandardCost = item.StandardCost,
            IsActive =  item.IsActive,
        };

        return PartialView("~/Views/Catalog/Item/_EditPartial.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, ItemEditViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return PartialView("~/Views/Catalog/Item/_EditPartial.cshtml", model);
        }

        if (await _context.Items.AnyAsync(i => i.Id != model.Id && i.Name.ToLower() == model.Name.ToLower().Trim()))
        {
            ModelState.AddModelError(nameof(model.Name), "An item with the same name already exists.");
            return PartialView("~/Views/Catalog/Item/_EditPartial.cshtml", model);
        }

        var item = await _context.Items.FindAsync(id);

        if (item == null)
        {
            return NotFound();
        }
        
        item.ItemCode = model.ItemCode;
        item.Name = model.Name;
        item.Description = model.Description;
        item.BrandId = model.BrandId;
        item.CategoryId = model.CategoryId;
        item.UnitOfMeasureId = model.UnitOfMeasureId;
        item.IsSerialized = model.IsSerialized;
        item.IsConsumable = model.IsConsumable;
        item.IsSparePart = model.IsSparePart;
        item.RequiresMaintenance = model.RequiresMaintenance;
        item.MinimumStockLevel = model.MinimumStockLevel;
        item.ReorderLevel = model.ReorderLevel;
        item.StandardCost = model.StandardCost;
        item.IsActive = model.IsActive;

        _context.Items.Update(item);
        await _context.SaveChangesAsync();
        
        return Json(new { success = true });
    }
}