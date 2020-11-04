using Microsoft.EntityFrameworkCore;
using tomware.OpenIddict.UI.Core;
using tomware.OpenIddict.UI.Infrastructure.Configuration;
using OpenIddict.EntityFrameworkCore.Models;

namespace tomware.OpenIddict.UI.Infrastructure
{
  public class OpenIddictUIContext : DbContext
  {
    // further configs see
    // https://github.com/IdentityServer/IdentityServer4/blob/main/src/EntityFramework.Storage/src/DbContexts/ConfigurationDbContext.cs

    public OpenIddictUIContext(DbContextOptions options) : base(options)
    { }

    public DbSet<ClaimType> ClaimTypes { get; set; }

    public DbSet<OpenIddictEntityFrameworkCoreApplication> Applications { get; set; }

    public DbSet<OpenIddictEntityFrameworkCoreAuthorization> Authorizations { get; set; }

    public DbSet<OpenIddictEntityFrameworkCoreScope> Scopes { get; set; }

    public DbSet<OpenIddictEntityFrameworkCoreToken> Tokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      builder.ApplyConfiguration(new ClaimTypeEntityConfiguration());
    }
  }
}
