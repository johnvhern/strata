using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Strata.ViewModel.SoftwareLicense
{
    public class SoftwareLicenseEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed {1} characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Description cannot exceed {1} characters.")]
        public string? Description { get; set; }

        [StringLength(50, ErrorMessage = "Product Key cannot exceed {1} characters.")]
        public string? ProductKey { get; set; }

        [Range(
            typeof(DateOnly),
            "01/01/2000",
            "12/31/2099",
            ErrorMessage = "Expiration Date must be between {1} and {2}."
        )]
        public DateOnly? ExpirationDate { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public IEnumerable<SelectListItem> BrandOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> CategoryOptions { get; set; } =
            new List<SelectListItem>();
        public bool IsActive { get; set; }
    }
}
