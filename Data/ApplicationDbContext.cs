using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Strata.Models;
using Strata.Services;

namespace Strata.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly ICurrentUserService _currentUserService;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ICurrentUserService currentUserService
        )
            : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<Category> Categories { get; set; }

        public override int SaveChanges()
        {
            return SaveChangesAsync(default).GetAwaiter().GetResult();
        }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default
        )
        {
            ChangeTracker.DetectChanges();

            var timestamp = DateTimeOffset.UtcNow;
            var username = _currentUserService.GetCurrentUsername();

            var auditedEntries = ChangeTracker
                .Entries<Category>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in auditedEntries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = timestamp;
                        entry.Entity.CreatedBy = username;
                        entry.Entity.UpdatedAt = timestamp;
                        entry.Entity.UpdatedBy = username;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = timestamp;
                        entry.Entity.UpdatedBy = username;
                        // Prevent overwriting CreatedAt/CreatedBy
                        entry.Property(c => c.CreatedAt).IsModified = false;
                        entry.Property(c => c.CreatedBy).IsModified = false;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
