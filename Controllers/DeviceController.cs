using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Strata.Data;
using Strata.Helpers;
using Strata.Models;
using Strata.ViewModel.Device;

namespace Strata.Controllers
{
    [Authorize]
    public class DeviceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DeviceController(ApplicationDbContext context)
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
                .Devices.AsNoTracking()
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

            var pagedDevices = await PaginatedList<Device>.CreateAsync(
                query,
                pageNumber,
                pageSize
            );

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentStatus"] = isActive;

            return View(pagedDevices);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new DeviceCreateViewModel
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
        public async Task<IActionResult> Create(DeviceCreateViewModel deviceModel)
        {
            if (!ModelState.IsValid)
            {
                deviceModel.BrandOptions = await _context
                    .Brands.Where(b => b.IsActive)
                    .OrderBy(b => b.Name)
                    .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name })
                    .ToListAsync();

                deviceModel.CategoryOptions = await _context
                    .Categories.Where(c => c.IsActive)
                    .OrderBy(c => c.Name)
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToListAsync();

                return View(deviceModel);
            }

            if (await _context.Devices.AnyAsync(d => d.Name.ToLower() == deviceModel.Name.ToLower().Trim()))
            {
                ModelState.AddModelError(
                    nameof(deviceModel.Name),
                    "A device with the same name already exists."
                );
            }

            var device = new Device
            {
                Name = deviceModel.Name,
                Description = deviceModel.Description,
                Model = deviceModel.Model,
                SerialNumber = deviceModel.SerialNumber,
                BrandId = deviceModel.BrandId,
                CategoryId = deviceModel.CategoryId,
                IsActive = deviceModel.IsActive,
            };

            _context.Devices.Add(device);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
