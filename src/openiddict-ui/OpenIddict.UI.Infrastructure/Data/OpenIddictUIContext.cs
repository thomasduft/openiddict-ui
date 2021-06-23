using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;
using System;

namespace tomware.OpenIddict.UI.Infrastructure
{
  public class OpenIddictUIStoreOptions
  {
    public Action<DbContextOptionsBuilder> OpenIddictUIContext { get; set; }
    public Action<IServiceProvider, DbContextOptionsBuilder> ResolveDbContextOptions { get; set; }
  }

  public interface IOpenIddictUIContext
  {
    DbSet<OpenIddictEntityFrameworkCoreApplication> Applications { get; set; }

    DbSet<OpenIddictEntityFrameworkCoreAuthorization> Authorizations { get; set; }

    DbSet<OpenIddictEntityFrameworkCoreScope> Scopes { get; set; }

    DbSet<OpenIddictEntityFrameworkCoreToken> Tokens { get; set; }
  }

  public class OpenIddictUIContext : OpenIddictUIContext<OpenIddictUIContext>
  {
    public OpenIddictUIContext(DbContextOptions<OpenIddictUIContext> options)
      : base(options)
    { }
  }

  public class OpenIddictUIContext<TContext> : DbContext, IOpenIddictUIContext
    where TContext : DbContext, IOpenIddictUIContext
  {
    public OpenIddictUIContext(DbContextOptions<TContext> options)
      : base(options)
    { }

    public DbSet<OpenIddictEntityFrameworkCoreApplication> Applications { get; set; }

    public DbSet<OpenIddictEntityFrameworkCoreAuthorization> Authorizations { get; set; }

    public DbSet<OpenIddictEntityFrameworkCoreScope> Scopes { get; set; }

    public DbSet<OpenIddictEntityFrameworkCoreToken> Tokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);

      builder.UseOpenIddict();
      // .ApplyConfiguration(new OpenIddictEntityFrameworkCoreApplicationConfiguration<TApplication, TAuthorization, TToken, TKey>())
      // .ApplyConfiguration(new OpenIddictEntityFrameworkCoreAuthorizationConfiguration<TAuthorization, TApplication, TToken, TKey>())
      // .ApplyConfiguration(new OpenIddictEntityFrameworkCoreScopeConfiguration<TScope, TKey>())
      // .ApplyConfiguration(new OpenIddictEntityFrameworkCoreTokenConfiguration<TToken, TApplication, TAuthorization, TKey>());
    }
  }
}
