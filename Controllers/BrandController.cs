using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Strata.Data;
using Strata.Helpers;
using Strata.Models.Catalog;
using Strata.ViewModel.Brand;

namespace Strata.Controllers
{
    [Authorize]
    public class BrandController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BrandController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            string searchString,
            int pageNumber = 1
        )
        {
            int pageSize = 25;

            var query = _context.Brands.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(c => c.Name.Contains(searchString));
            }

            query = query.OrderBy(c => c.Name).ThenBy(c => c.Id);

            var pagedBrand = await PaginatedList<Brand>.CreateAsync(query, pageNumber, pageSize);

            ViewData["CurrentFilter"] = searchString;
            return View("~/Views/Catalog/Brand/Index.cshtml", pagedBrand);
        }

        public IActionResult Create()
        {
            return PartialView("~/Views/Catalog/Brand/_CreatePartial.cshtml", new BrandCreateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(BrandCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("~/Views/Catalog/Brand/_CreatePartial.cshtml", model);
            }

            if (
                await _context.Brands.AnyAsync(b => b.Name.ToLower() == model.Name.ToLower().Trim())
            )
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "A brand with the same name already exists."
                );
                return PartialView("~/Views/Catalog/Brand/_CreatePartial.cshtml", model);
            }

            var brand = new Brand { Name = model.Name};

            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true
            });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands.FirstOrDefaultAsync(c => c.Id == id);

            if (brand == null)
            {
                return NotFound();
            }

            var model = new BrandEditViewModel
            {
                Id = brand.Id,
                Name = brand.Name,
            };

            return PartialView("~/Views/Catalog/Brand/_EditPartial.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, BrandEditViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return PartialView("~/Views/Catalog/Brand/_EditPartial.cshtml", model);
            }

            if (
                await _context.Brands.AnyAsync(b =>
                    b.Id != model.Id && b.Name.ToLower() == model.Name.ToLower().Trim()
                )
            )
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "Another brand with the same name already exists."
                );
                return PartialView("~/Views/Catalog/Brand/_EditPartial.cshtml", model);
            }

            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            brand.Name = model.Name;
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true
            });
        }
    }
}
