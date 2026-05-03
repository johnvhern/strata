using Strata.Interfaces;

namespace Strata.Models.Catalog;

public class UnitOfMeasure : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Abbreviation { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    
    public ICollection<Item> Items { get; set; } = new List<Item>();
}