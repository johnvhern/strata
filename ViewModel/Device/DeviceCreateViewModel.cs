using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Strata.ViewModel.Device;

public class DeviceCreateViewModel
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "{0} cannot be longer than {1} characters.")]
    public string Name { get; set; } = string.Empty;
    [StringLength(255, ErrorMessage = "{0} cannot be longer than {1} characters.")]
    public string? Description { get; set; }
    [StringLength(100, ErrorMessage = "{0} cannot be longer than {1} characters.")]
    public string? Model { get; set; }
    [StringLength(50, ErrorMessage = "{0} cannot be longer than {1} characters.")]
    public string? SerialNumber { get; set; }
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public IEnumerable<SelectListItem> BrandOptions { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> CategoryOptions { get; set; } =
        new List<SelectListItem>();
    public bool IsActive { get; set; }
}