using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace tomware.Microip.Web
{
  public class STSContext : IdentityDbContext<ApplicationUser>
  {
    public STSContext(DbContextOptions options) : base(options)
    {
      this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      builder.Entity<ClaimType>()
          .HasIndex(c => c.Name)
          .IsUnique();
    }
  }
}