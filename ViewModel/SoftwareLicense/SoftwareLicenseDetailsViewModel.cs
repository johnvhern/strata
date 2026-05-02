using System.ComponentModel.DataAnnotations;

namespace Strata.ViewModel.SoftwareLicense
{
    public class SoftwareLicenseDetailsViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? ProductKey { get; set; }
    }
}
