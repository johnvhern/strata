using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Strata.ViewModel.Catalog.Item;

public class ItemCreateViewModel
{
    public string? ItemCode { get; set; }
    
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
    
    [Range(typeof(int), "0", "999999")]
    public int MinimumStockLevel { get; set; }
    
    [Range(typeof(int), "0", "999999")]
    public int ReorderLevel { get; set; }
    
    [Range(typeof(decimal), "0.00", "99999999.99")]
    public decimal? StandardCost { get; set; }
    public bool IsActive { get; set; }
    
    
    
}