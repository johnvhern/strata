using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Strata.Data;
using Strata.Helpers;
using Strata.Models;
using Strata.ViewModel.SparePart;

namespace Strata.Controllers
{
    [Authorize]
    public class SparePartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SparePartController(ApplicationDbContext context)
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

            var query = _context
                .SpareParts.AsNoTracking()
                .Include(c => c.Brand)
                .Include(c => c.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(c => c.Name.Contains(searchString));
            }

            if (isActive.HasValue)
            {
                query = query.Where(c => c.IsActive == isActive.Value);
            }

            query = query.OrderBy(c => c.Name).ThenBy(c => c.Id);

            var pageSparePart = await PaginatedList<SparePart>.CreateAsync(
                query,
                pageNumber,
                pageSize
            );

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentStatus"] = isActive;

            return View(pageSparePart);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new SparePartCreateViewModel
            {
                BrandOptions = await _context
                    .Brands.Where(b => b.IsActive)
                    .OrderBy(b => b.Name)
                    .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name })
                    .ToListAsync(),

                CategoryOptions = await _context
                    .Categories.Where(c => c.IsActive)
                    .OrderBy(c => c.Name)
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToListAsync(),
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SparePartCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.BrandOptions = await _context
                    .Brands.Where(b => b.IsActive)
                    .OrderBy(b => b.Name)
                    .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name })
                    .ToListAsync();

                model.CategoryOptions = await _context
                    .Categories.Where(c => c.IsActive)
                    .OrderBy(c => c.Name)
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToListAsync();

                return View(model);
            }

            var sparePart = new SparePart
            {
                Name = model.Name,
                Description = model.Description,
                SerialNumber = model.SerialNumber,
                BrandId = model.BrandId,
                CategoryId = model.CategoryId,
                IsActive = model.IsActive,
            };

            _context.SpareParts.Add(sparePart);
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

            var sp = await _context.SpareParts.FirstOrDefaultAsync(c => c.Id == id);

            if (sp == null)
            {
                return NotFound();
            }

            var sparePart = new SparePartEditViewModel
            {
                Id = sp.Id,
                Name = sp.Name,
                Description = sp.Description,
                SerialNumber = sp.SerialNumber,
                BrandId = sp.BrandId,
                CategoryId = sp.CategoryId,
                IsActive = sp.IsActive,
                BrandOptions = await _context
                    .Brands.Where(b => b.IsActive)
                    .OrderBy(b => b.Name)
                    .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name })
                    .ToListAsync(),
                CategoryOptions = await _context
                    .Categories.Where(c => c.IsActive)
                    .OrderBy(c => c.Name)
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToListAsync(),
            };

            return View(sparePart);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, SparePartEditViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                model.BrandOptions = await _context
                    .Brands.Where(b => b.IsActive)
                    .OrderBy(b => b.Name)
                    .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name })
                    .ToListAsync();
                model.CategoryOptions = await _context
                    .Categories.Where(c => c.IsActive)
                    .OrderBy(c => c.Name)
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToListAsync();

                return View(model);
            }

            var sp = await _context.SpareParts.FindAsync(id);

            if (sp == null)
            {
                return NotFound();
            }

            sp.Name = model.Name;
            sp.Description = model.Description;
            sp.SerialNumber = model.SerialNumber;
            sp.BrandId = model.BrandId;
            sp.CategoryId = model.CategoryId;
            sp.IsActive = model.IsActive;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
