using System.ComponentModel.DataAnnotations;

namespace Strata.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed {1} characters")]
        public required string Name { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        [Required]
        public string CreatedBy { get; set; } = string.Empty;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        [Required]
        public string UpdatedBy { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
