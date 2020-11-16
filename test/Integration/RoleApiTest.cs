using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Mvc.Server;
using tomware.OpenIddict.UI.Api;
using tomware.OpenIddict.UI.Test.Helpers;
using Xunit;

namespace tomware.OpenIddict.UI.Test.Integration
{
  public class RoleApiTest : IntegrationContext
  {
    private const string TEST_ROLE = "test_role";
    private const string NEW_ROLE = "new_role";
    private const string UPDATE_ROLE = "update_role";

    public RoleApiTest(IntegrationApplicationFactory<Startup> fixture)
      : base(fixture)
    { }

    [Theory]
    [InlineData("/api/roles", HttpVerb.Get)]
    [InlineData("/api/roles/01D7ACA3-575C-4E60-859F-DB95B70F8190", HttpVerb.Get)]
    [InlineData("/api/roles", HttpVerb.Post)]
    // [InlineData("/api/roles", HttpVerb.Put)]
    // [InlineData("/api/roles/01D7ACA3-575C-4E60-859F-DB95B70F8190", HttpVerb.Delete)]
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
    public async Task GetRolesAsync_ReturnsAdministratorRole()
    {
      // Arrange
      var endpoint = "/api/roles";

      // Act
      var response = await GetAsync(endpoint);

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);

      string responseContent = await response.Content.ReadAsStringAsync();
      Assert.NotNull(response);

      var model = Deserialize<List<RoleViewModel>>(responseContent);
      Assert.NotNull(model);
      Assert.True(model.Count() > 0);
    }

    [Fact]
    public async Task CreateAsync_RoleCreated()
    {
      // Arrange
      var endpoint = "/api/roles";

      // Act
      var response = await PostAsync(endpoint, new RoleViewModel
      {
        Name = TEST_ROLE
      });

      // Assert
      Assert.NotNull(response);

      var id = await response.Content.ReadAsStringAsync();
      Assert.NotNull(id);
    }

    [Fact]
    public async Task GetAsync_RoleReceived()
    {
      // Arrange
      var endpoint = "/api/roles";
      var createResponse = await PostAsync(endpoint, new RoleViewModel
      {
        Name = NEW_ROLE
      });
      var id = await createResponse.Content.ReadAsStringAsync();

      // Act
      var response = await GetAsync($"{endpoint}/{id}");

      // Assert
      Assert.NotNull(response);

      var responseContent = await response.Content.ReadAsStringAsync();
      var model = Deserialize<RoleViewModel>(responseContent);

      Assert.NotNull(model);
      Assert.Equal(id, model.Id);
      Assert.Equal(NEW_ROLE, model.Name);
    }

    [Fact]
    public async Task UpdateAsync_RoleUpdated()
    {
      // Arrange
      var endpoint = "/api/roles";
      var createResponse = await PostAsync(endpoint, new RoleViewModel
      {
        Name = NEW_ROLE
      });
      var id = await createResponse.Content.ReadAsStringAsync();

      // Act
      var response = await PutAsync(endpoint, new RoleViewModel
      {
        Id = id,
        Name = UPDATE_ROLE
      });

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAsync_RoleDeleted()
    {
      // Arrange
      var endpoint = "/api/roles";
      var createResponse = await PostAsync(endpoint, new RoleViewModel
      {
        Name = NEW_ROLE
      });
      var id = await createResponse.Content.ReadAsStringAsync();

      // Act
      var response = await DeleteAsync($"{endpoint}/{id}");

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
  }
}