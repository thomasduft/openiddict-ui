using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mvc.Server.Helpers;
using Mvc.Server.Services;
using OpenIddict.Server;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static OpenIddict.Server.OpenIddictServerEvents;
using static OpenIddict.Server.OpenIddictServerHandlers;

namespace tomware.OpenIddict.UI.Tests
{
  public class IntegrationApplicationFactory<TStartup>
  : WebApplicationFactory<TStartup> where TStartup : class
  {
    public string AccessToken { get; set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
      builder.UseEnvironment("Testing");

      builder.ConfigureServices(services =>
      {
        var sp = services.BuildServiceProvider();
        EnsureMigration(sp);

        AccessToken = GenerateAccessToken(sp);
      });
    }

    public void EnsureMigration(IServiceProvider sp)
    {
      using (var scope = sp.CreateScope())
      {
        var services = scope.ServiceProvider;
        try
        {
          var migrator = services.GetRequiredService<IMigrationService>();
          migrator.EnsureMigrationAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
          var logger = services.GetRequiredService<ILogger<IntegrationApplicationFactory<TStartup>>>();
          logger.LogError(ex, "An error occurred while migrating the testing database.");
        }
      }
    }

    public string GenerateAccessToken(IServiceProvider sp)
    {
      try
      {
        var claims = new List<Claim>
        {
          new Claim(Claims.Role, Roles.ADMINISTRATOR_ROLE)
        };
        var identity = new ClaimsIdentity(claims);

        IOptions<OpenIddictServerOptions> options
          = (IOptions<OpenIddictServerOptions>)sp.GetService(typeof(IOptions<OpenIddictServerOptions>));
        ILogger<OpenIddictServerDispatcher> logger
          = (ILogger<OpenIddictServerDispatcher>)sp.GetService(typeof(ILogger<OpenIddictServerDispatcher>));

        var transaction = new OpenIddictServerTransaction
        {
          Options = options.Value,
          Logger = logger
        };

        var context = new ProcessSignInContext(transaction)
        {
          Issuer = new Uri("https://localhost:5000/"),
          AccessTokenPrincipal = new ClaimsPrincipal(identity)
        };

        var generator = new GenerateIdentityModelAccessToken();
        generator.HandleAsync(context).GetAwaiter().GetResult();

        return context.AccessToken;
      }
      catch (Exception)
      {
        throw;
      }
    }
  }
}