using System.ComponentModel.DataAnnotations;

namespace Strata.ViewModel.Category
{
    public class CategoryCreateViewModel
    {
        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed {1} characters")]
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
