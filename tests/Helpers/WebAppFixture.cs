using Alba;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mvc.Server;
using OpenIddict.Server;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using tomware.OpenIddict.UI.Api;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static OpenIddict.Server.OpenIddictServerEvents;
using static OpenIddict.Server.OpenIddictServerHandlers;

namespace tomware.OpenIddict.UI.Tests.Helpers
{
  public class WebAppFixture : IDisposable
  {
    private string AccessToken { get; set; }

    public SystemUnderTest System { get; }

    public bool IssueAccessToken { get; set; } = true;

    // see: https://jeremydmiller.com/2020/04/13/using-alba-for-integration-testing-asp-net-core-web-services/
    public WebAppFixture()
    {
      // Use the application configuration the way that it is in the real application
      // project
      var builder = Program.CreateHostBuilder(new string[0])

          // You may need to do this for any static
          // content or files in the main application including
          // appsettings.json files

          // DirectoryFinder is an Alba helper
          // .UseContentRoot(DirectoryFinder.FindParallelFolder("WebApplication"))

          // Override the hosting environment to "Testing"
          .UseEnvironment("Testing");

      // This is the Alba scenario wrapper around
      // TestServer and an active .Net Core IHost
      System = new SystemUnderTest(builder);

      // There's also a BeforeEachAsync() signature
      System.BeforeEach(httpContext =>
      {
        // Take any kind of setup action before
        // each simulated HTTP request

        // In this case, I'm setting a fake JWT token on each request
        // as a demonstration
        if (this.IssueAccessToken)
        {
          if (!httpContext.Request.Headers.ContainsKey("Authorization"))
          {
            httpContext.Request.Headers["Authorization"] = $"Bearer {AccessToken}";
          }
        }
      });

      System.AfterEach(httpContext =>
      {
        // Take any kind of teardown action after
        // each simulated HTTP request
      });

      AccessToken = GenerateAccessToken();
    }

    public void Dispose()
    {
      System?.Dispose();
    }

    private string GenerateAccessToken()
    {
      try
      {
        var claims = new List<Claim>
        {
          new Claim(Claims.Role, Roles.ADMINISTRATOR_ROLE)
        };
        var identity = new ClaimsIdentity(claims);

        IOptions<OpenIddictServerOptions> options
          = (IOptions<OpenIddictServerOptions>)this.System.Services.GetService(typeof(IOptions<OpenIddictServerOptions>));
        ILogger<OpenIddictServerDispatcher> logger
          = (ILogger<OpenIddictServerDispatcher>)this.System.Services.GetService(typeof(ILogger<OpenIddictServerDispatcher>));

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