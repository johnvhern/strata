using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Strata.Data;
using Strata.Helpers;
using Strata.Models;
using Strata.ViewModel.SoftwareLicense;

namespace Strata.Controllers
{
    [Authorize]
    public class SoftwareLicenseController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public SoftwareLicenseController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager
        )
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(
            string searchString,
            bool? isActive,
            int pageNumber = 1
        )
        {
            int pageSize = 25;

            var query = _context
                .SoftwareLicenses.AsNoTracking()
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

            var pagedSoftwareLicense = await PaginatedList<SoftwareLicense>.CreateAsync(
                query,
                pageNumber,
                pageSize
            );

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentStatus"] = isActive;

            return View(pagedSoftwareLicense);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new SoftwareLicenseCreateViewModel
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
        public async Task<IActionResult> Create(SoftwareLicenseCreateViewModel model)
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
                await _context.SoftwareLicenses.AnyAsync(c =>
                    c.Name.ToLower() == model.Name.ToLower().Trim()
                )
            )
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "A license with the same name already exists."
                );
            }

            var license = new SoftwareLicense
            {
                Name = model.Name,
                Description = model.Description,
                ProductKey = model.ProductKey,
                ExpirationDate = model.ExpirationDate,
                BrandId = model.BrandId,
                CategoryId = model.CategoryId,
                IsActive = model.IsActive,
            };

            _context.SoftwareLicenses.Add(license);
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

            var license = await _context.SoftwareLicenses.FirstOrDefaultAsync(l => l.Id == id);

            if (license == null)
            {
                return NotFound();
            }

            var licenseVM = new SoftwareLicenseEditViewModel
            {
                Id = license.Id,
                Name = license.Name,
                Description = license.Description,
                ProductKey = license.ProductKey,
                ExpirationDate = license.ExpirationDate,
                CategoryId = license.CategoryId,
                BrandId = license.BrandId,
                IsActive = license.IsActive,
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

            return View(licenseVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, SoftwareLicenseEditViewModel model)
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
                await _context.SoftwareLicenses.AnyAsync(l =>
                    l.Id != model.Id && l.Name.ToLower() == model.Name.ToLower().Trim()
                )
            )
            {
                ModelState.AddModelError(
                    nameof(model.Name),
                    "Another license with the same name already exists."
                );
            }

            var license = await _context.SoftwareLicenses.FindAsync(id);

            if (license == null)
            {
                return NotFound();
            }

            license.Name = model.Name;
            license.Description = model.Description;
            license.ProductKey = model.ProductKey;
            license.ExpirationDate = model.ExpirationDate;
            license.CategoryId = model.CategoryId;
            license.BrandId = model.BrandId;
            license.IsActive = model.IsActive;

            _context.SoftwareLicenses.Update(license);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> DetailsConfirmation(int id)
        {
            var license = await _context.SoftwareLicenses.FirstOrDefaultAsync(x => x.Id == id);

            if (license == null)
                return NotFound();

            var vm = new SoftwareLicenseDetailsViewModel { Id = license.Id, Name = license.Name };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DetailsConfirmation(SoftwareLicenseDetailsViewModel model)
        {
            var license = await _context.SoftwareLicenses.FirstOrDefaultAsync(x =>
                x.Id == model.Id
            );

            if (license == null)
                return NotFound();

            model.Name = license.Name;

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var validPassword = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!validPassword)
            {
                ModelState.AddModelError(nameof(model.Password), "Incorrect password.");
                return View(model);
            }

            model.ProductKey = license.ProductKey;
            model.Password = string.Empty;

            return View(model);
        }
    }
}
