using System;
using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;
using tomware.OpenIddict.UI.Core;
using tomware.OpenIddict.UI.Infrastructure.Configuration;

namespace tomware.OpenIddict.UI.Infrastructure
{
  public class OpenIddictUIStoreOptions
  {
    public Action<DbContextOptionsBuilder> OpenIddictUIContext { get; set; }
    public Action<IServiceProvider, DbContextOptionsBuilder> ResolveDbContextOptions { get; set; }
  }

  public interface IOpenIddictUIContext
  {
    DbSet<ClaimType> ClaimTypes { get; set; }

    DbSet<OpenIddictEntityFrameworkCoreApplication> Applications { get; set; }

    DbSet<OpenIddictEntityFrameworkCoreAuthorization> Authorizations { get; set; }

    DbSet<OpenIddictEntityFrameworkCoreScope> Scopes { get; set; }

    DbSet<OpenIddictEntityFrameworkCoreToken> Tokens { get; set; }
  }

  public class OpenIddictUIContext : OpenIddictUIContext<OpenIddictUIContext>
  {
    public OpenIddictUIContext(
      DbContextOptions<OpenIddictUIContext> options,
      OpenIddictUIStoreOptions storeOptions
    ) : base(options, storeOptions)
    {
    }
  }

  public class OpenIddictUIContext<TContext> : DbContext, IOpenIddictUIContext
    where TContext : DbContext, IOpenIddictUIContext
  {
    private readonly OpenIddictUIStoreOptions _storeOptions;

    public OpenIddictUIContext(
      DbContextOptions<TContext> options,
      OpenIddictUIStoreOptions storeOptions
    )
      : base(options)
    {
      _storeOptions = storeOptions
        ?? throw new ArgumentNullException(nameof(storeOptions));
    }

    public DbSet<ClaimType> ClaimTypes { get; set; }

    public DbSet<OpenIddictEntityFrameworkCoreApplication> Applications { get; set; }

    public DbSet<OpenIddictEntityFrameworkCoreAuthorization> Authorizations { get; set; }

    public DbSet<OpenIddictEntityFrameworkCoreScope> Scopes { get; set; }

    public DbSet<OpenIddictEntityFrameworkCoreToken> Tokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      builder
        .ApplyConfiguration(new ClaimTypeEntityConfiguration())
        .UseOpenIddict();
      // .ApplyConfiguration(new OpenIddictEntityFrameworkCoreApplicationConfiguration<TApplication, TAuthorization, TToken, TKey>())
      // .ApplyConfiguration(new OpenIddictEntityFrameworkCoreAuthorizationConfiguration<TAuthorization, TApplication, TToken, TKey>())
      // .ApplyConfiguration(new OpenIddictEntityFrameworkCoreScopeConfiguration<TScope, TKey>())
      // .ApplyConfiguration(new OpenIddictEntityFrameworkCoreTokenConfiguration<TToken, TApplication, TAuthorization, TKey>());
    }
  }
}
