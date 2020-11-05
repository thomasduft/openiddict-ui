using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace tomware.Microip.Web
{
  public class STSContext : IdentityDbContext<ApplicationUser>
  {
    public STSContext(DbContextOptions<STSContext> options) : base(options)
    {
      this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }
  }
}