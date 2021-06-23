using Microsoft.EntityFrameworkCore;
using System;
using tomware.OpenIddict.UI.Identity.Core;

namespace tomware.OpenIddict.UI.Identity.Infrastructure
{
  public class OpenIddictUIIdentityStoreOptions
  {
    public Action<DbContextOptionsBuilder> OpenIddictUIIdentityContext { get; set; }
    public Action<IServiceProvider, DbContextOptionsBuilder> ResolveDbContextOptions { get; set; }
  }

  public interface IOpenIddictUIIdentityContext
  {
    DbSet<ClaimType> ClaimTypes { get; set; }
  }

  public class OpenIddictUIIdentityContext : OpenIddictUIIdentityContext<OpenIddictUIIdentityContext>
  {
    public OpenIddictUIIdentityContext(DbContextOptions<OpenIddictUIIdentityContext> options)
      : base(options)
    { }
  }

  public class OpenIddictUIIdentityContext<TContext> : DbContext, IOpenIddictUIIdentityContext
    where TContext : DbContext, IOpenIddictUIIdentityContext
  {
    public OpenIddictUIIdentityContext(DbContextOptions<TContext> options)
      : base(options)
    { }

    public DbSet<ClaimType> ClaimTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      builder.ApplyConfiguration(new ClaimTypeEntityConfiguration());
    }
  }
}
