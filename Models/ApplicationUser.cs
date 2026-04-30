using Microsoft.AspNetCore.Identity;

namespace Strata.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
