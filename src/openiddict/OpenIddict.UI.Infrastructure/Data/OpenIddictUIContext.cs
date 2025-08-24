using System;
using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;

namespace tomware.OpenIddict.UI.Infrastructure;

public class OpenIddictUIStoreOptions
{
  public Action<DbContextOptionsBuilder> OpenIddictUIContext { get; set; }
  public Action<IServiceProvider, DbContextOptionsBuilder> ResolveDbContextOptions { get; set; }
}

public interface IOpenIddictUIContext
{
  public DbSet<OpenIddictEntityFrameworkCoreApplication> Applications { get; set; }

  public DbSet<OpenIddictEntityFrameworkCoreAuthorization> Authorizations { get; set; }

  public DbSet<OpenIddictEntityFrameworkCoreScope> Scopes { get; set; }

  public DbSet<OpenIddictEntityFrameworkCoreToken> Tokens { get; set; }
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

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.UseOpenIddict();
  }
}
