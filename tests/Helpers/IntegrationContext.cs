using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Server;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace tomware.OpenIddict.UI.Tests.Helpers
{
  public class IntegrationContext : IClassFixture<IntegrationApplicationFactory<Startup>>
  {
    private readonly IntegrationApplicationFactory<Startup> _factory;
    private readonly HttpClient _client;
    private readonly string _accessToken;

    protected IntegrationContext(IntegrationApplicationFactory<Server.Startup> factory)
    {
      _factory = factory;

      _client = factory.CreateClient(new WebApplicationFactoryClientOptions
      {
        BaseAddress = new Uri("https://localhost:5000"),
        AllowAutoRedirect = false
      });

      _accessToken = _factory.AccessToken;
    }

    protected static T Deserialize<T>(string responseBody)
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

    protected T GetRequiredService<T>()
    {
      return _factory.Services.GetRequiredService<T>();
    }

    private HttpClient GetClient(bool authorized = true)
    {
      if (_client.DefaultRequestHeaders.Authorization != null)
      {
        _client.DefaultRequestHeaders.Authorization = null;
      }

      if (authorized)
      {
        if (_client.DefaultRequestHeaders.Authorization == null)
        {
          _client.DefaultRequestHeaders.Authorization
            = new AuthenticationHeaderValue("Bearer", _accessToken);
        }
      }

      return _client;
    }
  }
}