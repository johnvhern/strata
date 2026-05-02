using System.ComponentModel.DataAnnotations;
using Strata.Interfaces;

namespace Strata.Models
{
    public class Category : IAuditableEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed {1} characters")]
        public required string Name { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<Consumable>? Consumables { get; set; } = new List<Consumable>();
        public ICollection<SparePart>? SpareParts { get; set; } = new List<SparePart>();
        public ICollection<SoftwareLicense>? SoftwareLicenses { get; set; } =
            new List<SoftwareLicense>();
        public ICollection<Device>? Devices { get; set; } =
            new List<Device>();
    }
}
