using System.ComponentModel.DataAnnotations;

namespace Strata.ViewModel.Catalog.Category
{
    public class CategoryCreateViewModel
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(100, ErrorMessage = "{0} cannot exceed {1} characters")]
        public string? Description { get; set; }
    }
}
