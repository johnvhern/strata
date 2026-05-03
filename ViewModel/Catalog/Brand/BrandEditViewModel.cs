using System.ComponentModel.DataAnnotations;

namespace Strata.ViewModel.Catalog.Brand;

public class BrandEditViewModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Brand name is required")]
    [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string Name { get; set; } = string.Empty;
}