using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Server;
using tomware.OpenIddict.UI.Identity.Api;
using tomware.OpenIddict.UI.Tests.Helpers;
using Xunit;

namespace tomware.OpenIddict.UI.Tests.Integration;

public class RoleApiTest : IntegrationContext
{
  private const string TEST_ROLE = "test_role";
  private const string NEW_ROLE = "new_role";
  private const string UPDATE_ROLE = "update_role";

  public RoleApiTest(IntegrationApplicationFactory<Testing> fixture)
    : base(fixture)
  { }

  [Theory]
  [InlineData("/api/roles", HttpVerb.Get)]
  [InlineData("/api/roles/01D7ACA3-575C-4E60-859F-DB95B70F8190", HttpVerb.Get)]
  [InlineData("/api/roles", HttpVerb.Post)]
  // [InlineData("/api/roles", HttpVerb.Put)]
  // [InlineData("/api/roles/01D7ACA3-575C-4E60-859F-DB95B70F8190", HttpVerb.Delete)]
  public async Task IsNotAuthenticatedReturnsUnauthorized(
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
      case HttpVerb.Get:
        response = await GetAsync(endpoint, authorized);
        break;
      default:
        break;
    }

    // Assert
    Assert.NotNull(response);
    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
  }

  [Fact]
  public async Task GetRolesAsyncReturnsAdministratorRole()
  {
    // Arrange
    var endpoint = "/api/roles";

    // Act
    var response = await GetAsync(endpoint);

    // Assert
    Assert.NotNull(response);
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var model = await response.Content.ReadAsJson<List<RoleViewModel>>();
    Assert.NotNull(model);
    Assert.True(model.Count > 0);
  }

  [Fact]
  public async Task CreateAsyncRoleCreated()
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
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

    var id = await response.Content.ReadAsJson<string>();
    Assert.NotNull(id);
  }

  [Fact]
  public async Task GetAsyncRoleReceived()
  {
    // Arrange
    var endpoint = "/api/roles";
    var createResponse = await PostAsync(endpoint, new RoleViewModel
    {
      Name = NEW_ROLE
    });
    var id = await createResponse.Content.ReadAsJson<string>();

    // Act
    var response = await GetAsync($"{endpoint}/{id}");

    // Assert
    Assert.NotNull(response);
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var model = await response.Content.ReadAsJson<RoleViewModel>();

    Assert.NotNull(model);
    Assert.Equal(id, model.Id);
    Assert.Equal(NEW_ROLE, model.Name);
  }

  [Fact]
  public async Task UpdateAsyncRoleUpdated()
  {
    // Arrange
    var endpoint = "/api/roles";
    var createResponse = await PostAsync(endpoint, new RoleViewModel
    {
      Name = NEW_ROLE
    });
    var id = await createResponse.Content.ReadAsJson<string>();

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
  public async Task DeleteAsyncRoleDeleted()
  {
    // Arrange
    var endpoint = "/api/roles";
    var createResponse = await PostAsync(endpoint, new RoleViewModel
    {
      Name = NEW_ROLE
    });
    var id = await createResponse.Content.ReadAsJson<string>();

    // Act
    var response = await DeleteAsync($"{endpoint}/{id}");

    // Assert
    Assert.NotNull(response);
    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
  }
}
