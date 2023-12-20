using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Server;
using Server.Models;
using tomware.OpenIddict.UI.Identity.Infrastructure;
using tomware.OpenIddict.UI.Infrastructure;
using tomware.OpenIddict.UI.Suite.Core;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static OpenIddict.Server.OpenIddictServerEvents;
using static OpenIddict.Server.OpenIddictServerHandlers;

namespace tomware.OpenIddict.UI.Tests;

public class IntegrationApplicationFactory<TEntryPoint>
  : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
{
  public string AccessToken { get; set; }

  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    builder.UseEnvironment("Testing");

    builder.ConfigureServices(services =>
    {
      FixDbContext<ApplicationDbContext>(services);
      FixDbContext<OpenIddictUIContext>(services);
      FixDbContext<OpenIddictUIIdentityContext>(services);

      var sp = services.BuildServiceProvider();
      AccessToken = GetAccessToken(sp);
    });
  }

  public void FixDbContext<T>(IServiceCollection services)
    where T : DbContext
  {
    var descriptor = services.SingleOrDefault(d =>
    {
      return d.ServiceType == typeof(DbContextOptions<T>);
    });
    services.Remove(descriptor);
    services.AddDbContext<T>(options =>
    {
      options.UseInMemoryDatabase("InMemoryDbForTesting");
    });
  }

  public string GetAccessToken(IServiceProvider sp)
  {
    try
    {
      var claims = new List<Claim>
      {
        new(Claims.Role, Roles.Administrator),
        new(Claims.Issuer, "https://localhost:5001/")
      };
      var identity = new ClaimsIdentity(claims);

      var options = (IOptions<OpenIddictServerOptions>)sp
        .GetService(typeof(IOptions<OpenIddictServerOptions>));
      var logger = (ILogger<OpenIddictServerDispatcher>)sp
        .GetService(typeof(ILogger<OpenIddictServerDispatcher>));

      var transaction = new OpenIddictServerTransaction
      {
        Options = options.Value,
        Logger = logger,
      };

      var context = new ProcessSignInContext(transaction)
      {
        AccessTokenPrincipal = new ClaimsPrincipal(identity)
      };

      var dispatcher = sp.GetRequiredService<IOpenIddictServerDispatcher>();
      var generator = new GenerateAccessToken(dispatcher);
#pragma warning disable CA2012
      generator.HandleAsync(context).GetAwaiter().GetResult();
#pragma warning restore CA2012
      return context.AccessToken;
    }
    catch (Exception)
    {
      throw;
    }
  }
}
