using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mvc.Server;
using Mvc.Server.Helpers;
using Mvc.Server.Services;
using OpenIddict.Server;
using Xunit;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static OpenIddict.Server.OpenIddictServerEvents;
using static OpenIddict.Server.OpenIddictServerHandlers;

namespace tomware.OpenIddict.UI.Test.Helpers
{
  public class IntegrationContext
    : IClassFixture<IntegrationApplicationFactory<Startup>>
  {
    private IntegrationApplicationFactory<Mvc.Server.Startup> _factory;

    private readonly HttpClient _client;
    private readonly string _accessToken;

    protected IServiceProvider Services => _factory.Services;

    protected IntegrationContext(IntegrationApplicationFactory<Mvc.Server.Startup> factory)
    {
      _factory = factory;

      _client = factory.CreateClient(new WebApplicationFactoryClientOptions
      {
        BaseAddress = new Uri("https://localhost:5000"),
        AllowAutoRedirect = false
      });

      _accessToken = _factory.AccessToken;
    }

    private HttpClient GetClient(bool authorized = true)
    {
      if (this._client.DefaultRequestHeaders.Authorization != null)
      {
        this._client.DefaultRequestHeaders.Authorization = null;
      }

      if (authorized)
      {
        if (this._client.DefaultRequestHeaders.Authorization == null)
        {
          this._client.DefaultRequestHeaders.Authorization
            = new AuthenticationHeaderValue("Bearer", _accessToken);
        }
      }

      return this._client;
    }

    protected T Deserialize<T>(string responseBody)
    {
      return JsonSerializer.Deserialize<T>(responseBody, new JsonSerializerOptions
      {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
      });
    }

    protected async Task<HttpResponseMessage> GetAsync(
      string endpoint,
      bool authorized = true
    )
    {
      return await GetClient(authorized)
        .GetAsync(endpoint);
    }

    protected async Task<HttpResponseMessage> PostAsync<T>(
      string endpoint,
      T content,
      bool authorized = true
    )
    {
      var payload = JsonSerializer.Serialize<T>(content);
      var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

      return await GetClient(authorized)
        .PostAsync(endpoint, httpContent);
    }

    protected async Task<HttpResponseMessage> PutAsync<T>(
      string endpoint,
      T content,
      bool authorized = true
    )
    {
      var payload = JsonSerializer.Serialize<T>(content);
      var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

      return await GetClient(authorized)
        .PutAsync(endpoint, httpContent);
    }

  protected async Task<HttpResponseMessage> DeleteAsync(
     string endpoint,
     bool authorized = true
   )
    {
      return await GetClient(authorized)
        .DeleteAsync(endpoint);
    }
  }

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

        this.AccessToken = GenerateAccessToken(sp);
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