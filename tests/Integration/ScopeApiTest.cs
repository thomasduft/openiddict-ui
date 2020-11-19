using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Mvc.Server;
using tomware.OpenIddict.UI.Api;
using tomware.OpenIddict.UI.Tests.Helpers;
using Xunit;

namespace tomware.OpenIddict.UI.Tests.Integration
{
  public class ScopeApiTest : IntegrationContext
  {
    private const string TEST_SCOPE = "test_scope";

    public ScopeApiTest(IntegrationApplicationFactory<Startup> fixture)
      : base(fixture)
    { }

    [Theory]
    [InlineData("/api/scopes", HttpVerb.Get)]
    [InlineData("/api/scopes/01D7ACA3-575C-4E60-859F-DB95B70F8190", HttpVerb.Get)]
    [InlineData("/api/scopes", HttpVerb.Post)]
    // [InlineData("/api/scopes", HttpVerb.Put)]
    // [InlineData("/api/scopes/01D7ACA3-575C-4E60-859F-DB95B70F8190", HttpVerb.Delete)]
    public async Task IsNotAuthenticated_ReturnsUnauthorized(
      string endpoint,
      HttpVerb verb
    )
    {
      // Arrange
      HttpResponseMessage response = null;
      var authorized = false;

      // Act
      switch (verb)
      {
        case HttpVerb.Post:
          response = await PostAsync(endpoint, new RoleViewModel(), authorized);
          break;
        case HttpVerb.Put:
          response = await PutAsync(endpoint, new RoleViewModel(), authorized);
          break;
        case HttpVerb.Delete:
          response = await DeleteAsync(endpoint, authorized);
          break;
        default:
          response = await GetAsync(endpoint, authorized);
          break;
      }

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetClaimTypesAsync_ReturnsAList()
    {
      // Arrange
      var endpoint = "/api/scopes";

      // Act
      var response = await GetAsync(endpoint);

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);

      var model = await response.Content.ReadAsJson<List<ScopeViewModel>>();
      Assert.NotNull(model);
      Assert.True(model.Count() >= 0);
    }

    [Fact]
    public async Task CreateAsync_ScopeCreated()
    {
      // Arrange
      var endpoint = "/api/scopes";

      // Act
      var response = await PostAsync(endpoint, new ScopeViewModel
      {
        Name = TEST_SCOPE,
        DisplayName = "displayname",
        Description = "description"
      });

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.Created, response.StatusCode);

      var id = await response.Content.ReadAsJson<string>();
      Assert.NotNull(id);
    }

    [Fact]
    public async Task GetAsync_RoleReceived()
    {
      // Arrange
      var endpoint = "/api/scopes";
      var createResponse = await PostAsync(endpoint, new ScopeViewModel
      {
        Name = TEST_SCOPE,
        DisplayName = "displayname",
        Description = "description",
        Resources = new List<string>
        {
          "resource1",
          "resource2"
        }
      });
      var id = await createResponse.Content.ReadAsJson<string>();

      // Act
      var response = await GetAsync($"{endpoint}/{id}");

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);

      var model = await response.Content.ReadAsJson<ScopeViewModel>();

      Assert.NotNull(model);
      Assert.Equal(id, model.Id);
      Assert.Equal(TEST_SCOPE, model.Name);
      Assert.Equal("displayname", model.DisplayName);
      Assert.Equal("description", model.Description);
      Assert.Equal(2, model.Resources.Count);
      Assert.Equal("resource1", model.Resources[0]);
      Assert.Equal("resource2", model.Resources[1]);
    }

    [Fact]
    public async Task UpdateAsync_ScopeUpdated()
    {
      // Arrange
      var endpoint = "/api/scopes";
      var createResponse = await PostAsync(endpoint, new ScopeViewModel
      {
        Name = TEST_SCOPE,
        DisplayName = "displayname",
        Description = "description"
      });
      var id = await createResponse.Content.ReadAsJson<string>();

      // Act
      var response = await PutAsync(endpoint, new ScopeViewModel
      {
        Id = id,
        Name = TEST_SCOPE,
        DisplayName = "displayname updated",
        Description = "description updated"
      });

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAsync_ScopeDeleted()
    {
      // Arrange
      var endpoint = "/api/scopes";
      var createResponse = await PostAsync(endpoint, new ScopeViewModel
      {
        Name = TEST_SCOPE,
        DisplayName = "displayname",
        Description = "description"
      });
      var id = await createResponse.Content.ReadAsJson<string>();

      // Act
      var response = await DeleteAsync($"{endpoint}/{id}");

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
  }
}