using System.ComponentModel.DataAnnotations;
using Strata.Interfaces;

namespace Strata.Models
{
    public class Brand : IAuditableEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public ICollection<Consumable>? Consumables { get; set; } = new List<Consumable>();
        public ICollection<SparePart>? SpareParts { get; set; } = new List<SparePart>();
        public ICollection<SoftwareLicense>? SoftwareLicenses { get; set; } =
            new List<SoftwareLicense>();
        public ICollection<Device>? Devices { get; set; } =
            new List<Device>();
    }
}
