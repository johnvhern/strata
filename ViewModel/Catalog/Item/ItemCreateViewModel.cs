using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Strata.ViewModel.Catalog.Item;

public class ItemCreateViewModel : IValidatableObject
{
    public string ItemCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Item name is required")]
    [StringLength(100, ErrorMessage = "Name must not exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Description must not exceed 100 characters")]
    public string? Description { get; set; }

    public int? BrandId { get; set; }
    public IEnumerable<SelectListItem> BrandsOptions { get; set; } = new List<SelectListItem>();

    public int? CategoryId { get; set; }
    public IEnumerable<SelectListItem> CategoryOptions { get; set; } = new List<SelectListItem>();

    public int? UnitOfMeasureId { get; set; }
    public IEnumerable<SelectListItem> UnitOptions { get; set; } = new List<SelectListItem>();

    public bool IsSerialized { get; set; }
    public bool IsConsumable { get; set; }
    public bool IsSparePart { get; set; }
    public bool RequiresMaintenance { get; set; }

    [Display(Name = "Min. Stock Level")]
    [Range(typeof(int), "0", "999999")]
    public int MinimumStockLevel { get; set; }

    [Display(Name = "Reorder Level")]
    [Range(typeof(int), "0", "999999")]
    public int ReorderLevel { get; set; }

    [Display(Name = "Standard Cost")]
    [Range(typeof(decimal), "0.00", "99999999.99")]
    public decimal? StandardCost { get; set; }

    public bool IsActive { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (IsSerialized && IsConsumable)
        {
            yield return new ValidationResult(
                "An item cannot be both serialized and consumable.", new [] { nameof(IsSerialized), nameof(IsConsumable) }
                );
        }

        if (IsConsumable && RequiresMaintenance)
        {
            yield return new ValidationResult(
                "Consumables items do not need maintenance.",
                new[] { nameof(IsConsumable), nameof(RequiresMaintenance) }
            );
        }

        if (IsConsumable && IsSparePart)
        {
            yield return new ValidationResult(
                "Consumable items cannot be treated as spare part.", new []{ nameof(IsSparePart), nameof(IsSparePart) }
            );
        }
    }

}