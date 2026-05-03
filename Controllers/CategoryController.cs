using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Strata.Data;
using Strata.Helpers;
using Strata.Models;
using Strata.Models.Catalog;
using Strata.ViewModel.Catalog.Category;

namespace Strata.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            string searchString,
            int pageNumber = 1
        )
        {
            int pageSize = 25;

            var query = _context.Categories.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(c => c.Name.Contains(searchString));
            }

            query = query.OrderBy(c => c.Name).ThenBy(c => c.Id);

            var pagedCategory = await PaginatedList<Category>.CreateAsync(
                query,
                pageNumber,
                pageSize
            );

            ViewData["CurrentFilter"] = searchString;
            return View("~/Views/Catalog/Category/Index.cshtml", pagedCategory);
        }

        public IActionResult Create()
        {
            return PartialView("~/Views/Catalog/Category/_CreatePartial.cshtml", new CategoryCreateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("~/Views/Catalog/Category/_CreatePartial.cshtml", model);
            }

            if (
                await _context.Categories.AnyAsync(c =>
                    c.Name.ToLower().Trim() == model.Name.ToLower().Trim()
                )
            )
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "A category with the same name already exists."
                );
                return PartialView("~/Views/Catalog/Category/_CreatePartial.cshtml", model);
            }

            var category = new Category { Name = model.Name, Description = model.Description };

            _context.Categories.Add(category);
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

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
    
            if (category == null)
            {
                return NotFound();
            }

            var model = new CategoryEditViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description =  category.Description
            };

            return PartialView("~/Views/Catalog/Category/_EditPartial.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CategoryEditViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return PartialView("~/Views/Catalog/Category/_EditPartial.cshtml", model);
            }

            if (
                await _context.Categories.AnyAsync(c =>
                    c.Id != model.Id && c.Name.ToLower() == model.Name.ToLower().Trim()
                )
            )
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "Another category with the same name already exists."
                );
                return PartialView("~/Views/Catalog/Category/_EditPartial.cshtml", model);
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            category.Name = model.Name;
            category.Description = model.Description;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}
