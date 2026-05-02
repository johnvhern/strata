using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Strata.ViewModel.SparePart
{
    public class SparePartEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than {1} characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Description cannot be longer than {1} characters.")]
        public string? Description { get; set; }

        [StringLength(50, ErrorMessage = "Serial Number cannot be longer than {1} characters.")]
        public string? SerialNumber { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public IEnumerable<SelectListItem> BrandOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> CategoryOptions { get; set; } =
            new List<SelectListItem>();
        public bool IsActive { get; set; }
    }
}
