using System.ComponentModel.DataAnnotations;

namespace Strata.ViewModel.Brand;

public class BrandCreateViewModel
{
    [Required(ErrorMessage = "Brand name is required")]
    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string Name { get; set; } = string.Empty;
}
