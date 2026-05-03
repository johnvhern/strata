using System.ComponentModel.DataAnnotations;

namespace Strata.ViewModel.Catalog.UnitOfMeasure;

public class UnitEditViewModal
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Name is required")]
    [StringLength(50, ErrorMessage = "{0} must not exceed {1} characters")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Abbreviation is required")]
    [StringLength(20, ErrorMessage = "{0} must not exceed {1} characters")]
    public string Abbreviation { get; set; } = string.Empty;
}