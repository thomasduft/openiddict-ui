using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Server;
using Xunit;

namespace tomware.OpenIddict.UI.Tests.Helpers;

public class IntegrationContext : IClassFixture<IntegrationApplicationFactory<Testing>>
{
  private static readonly JsonSerializerOptions JsonSerializerOptions = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
  };

  private readonly IntegrationApplicationFactory<Testing> _factory;
  private readonly HttpClient _client;
  private readonly string _accessToken;

  protected IntegrationContext(IntegrationApplicationFactory<Testing> factory)
  {
    _factory = factory;

    _client = factory.CreateClient(new WebApplicationFactoryClientOptions
    {
      BaseAddress = new Uri("https://localhost:5001"),
      AllowAutoRedirect = false
    });

    _accessToken = _factory.AccessToken;
  }

  protected static T Deserialize<T>(string responseBody) 
    => JsonSerializer.Deserialize<T>(responseBody, JsonSerializerOptions);

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
    var payload = JsonSerializer.Serialize(content);
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
    var payload = JsonSerializer.Serialize(content);
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

  protected T GetRequiredService<T>() => _factory.Services.GetRequiredService<T>();

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
