using Strata.Interfaces;

namespace Strata.ViewModel.Catalog.Item;

public class ItemDetailsViewModel : IAuditableEntity
{
    public int Id { get; set; }
    
    public string? ItemCode { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public string? Category { get; set; }
    public string? UnitOfMeasure { get; set; }

    public bool IsSerialized { get; set; }
    public bool IsConsumable { get; set; }
    public bool IsSparePart { get; set; }
    public bool RequiresMaintenance { get; set; }
    
    public int MinimumStockLevel { get; set; }
    public int ReorderLevel { get; set; }
    
    public decimal? StandardCost { get; set; }

    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}