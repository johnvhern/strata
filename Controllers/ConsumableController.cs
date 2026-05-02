using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Strata.Data;
using Strata.Helpers;
using Strata.Models;
using Strata.ViewModel.Consumable;

namespace Strata.Controllers
{
    [Authorize]
    public class ConsumableController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConsumableController(ApplicationDbContext context)
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
                .Consumables.AsNoTracking()
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

            var pagedConsumable = await PaginatedList<Consumable>.CreateAsync(
                query,
                pageNumber,
                pageSize
            );

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentStatus"] = isActive;

            return View(pagedConsumable);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new ConsumableCreateViewModel
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
        public async Task<IActionResult> Create(ConsumableCreateViewModel model)
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

            if (
                await _context.Consumables.AnyAsync(c =>
                    c.Name.ToLower() == model.Name.ToLower().Trim()
                )
            )
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "A consumable with the same name already exists."
                );
            }

            var consumable = new Consumable
            {
                Name = model.Name,
                Description = model.Description,
                BrandId = model.BrandId,
                CategoryId = model.CategoryId,
                IsActive = model.IsActive,
            };

            _context.Consumables.Add(consumable);
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

            var consumable = await _context.Consumables.FirstOrDefaultAsync(c => c.Id == id);

            if (consumable == null)
            {
                return NotFound();
            }

            var consumableEditVM = new ConsumableEditViewModel
            {
                Id = consumable.Id,
                Name = consumable.Name,
                Description = consumable.Description,
                BrandId = consumable.BrandId,
                CategoryId = consumable.CategoryId,
                IsActive = consumable.IsActive,
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

            return View(consumableEditVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ConsumableEditViewModel model)
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

            if (
                await _context.Consumables.AnyAsync(c =>
                    c.Id != model.Id && c.Name.ToLower() == model.Name.ToLower().Trim()
                )
            )
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "Another consumable with the same name already exists."
                );
            }

            var consumable = await _context.Consumables.FindAsync(id);

            if (consumable == null)
            {
                return NotFound();
            }

            consumable.Name = model.Name;
            consumable.Description = model.Description;
            consumable.BrandId = model.BrandId;
            consumable.CategoryId = model.CategoryId;
            consumable.IsActive = model.IsActive;

            _context.Consumables.Update(consumable);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
