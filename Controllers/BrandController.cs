using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Strata.Data;
using Strata.Helpers;
using Strata.Models;
using Strata.ViewModel;

namespace Strata.Controllers
{
    public class BrandController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BrandController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            string searchString,
            bool? isActive,
            int pageNumber = 1
        )
        {
            int pageSize = 25;

            var query = _context.Brands.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(c => c.Name.Contains(searchString));
            }

            if (isActive.HasValue)
            {
                query = query.Where(c => c.IsActive == isActive.Value);
            }

            query = query.OrderBy(c => c.Name).ThenBy(c => c.Id);

            var pagedBrand = await PaginatedList<Brand>.CreateAsync(query, pageNumber, pageSize);

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentStatus"] = isActive;
            return View(pagedBrand);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(BrandViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var brand = new Brand { Name = model.Name, IsActive = model.IsActive };

            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var brand = await _context.Brands.FirstOrDefaultAsync(c => c.Id == id);

            if (brand == null)
            {
                return NotFound();
            }

            var model = new BrandViewModel
            {
                Id = brand.Id,
                Name = brand.Name,
                IsActive = brand.IsActive,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BrandViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var brand = await _context.Brands.FindAsync(model.Id);
            if (brand == null)
            {
                return NotFound();
            }

            brand.Name = model.Name;
            brand.IsActive = model.IsActive;
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
