using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Strata.Interfaces;
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
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Consumable> Consumables { get; set; }

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
                .Entries<IAuditableEntity>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in auditedEntries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = timestamp;
                    entry.Entity.CreatedBy = username;
                }

                entry.Entity.UpdatedAt = timestamp;
                entry.Entity.UpdatedBy = username;

                if (entry.State == EntityState.Modified)
                {
                    entry.Property(x => x.CreatedAt).IsModified = false;
                    entry.Property(x => x.CreatedBy).IsModified = false;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
