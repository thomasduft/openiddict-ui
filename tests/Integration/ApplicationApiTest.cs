using Mvc.Server;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using tomware.OpenIddict.UI.Api;
using tomware.OpenIddict.UI.Tests.Helpers;
using Xunit;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace tomware.OpenIddict.UI.Tests.Integration
{
  public class ApplicationApiTest : IntegrationContext
  {
    private const string TEST_APPLICATION = "test_application";

    public ApplicationApiTest(IntegrationApplicationFactory<Startup> fixture)
      : base(fixture)
    { }

    [Theory]
    [InlineData("/api/application", HttpVerb.Get)]
    [InlineData("/api/application/01D7ACA3-575C-4E60-859F-DB95B70F8190", HttpVerb.Get)]
    [InlineData("/api/application", HttpVerb.Post)]
    // [InlineData("/api/application", HttpVerb.Put)]
    // [InlineData("/api/application/01D7ACA3-575C-4E60-859F-DB95B70F8190", HttpVerb.Delete)]
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
        case HttpVerb.Get:
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
      var endpoint = "/api/application";

      // Act
      var response = await GetAsync(endpoint);

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);

      var model = await response.Content.ReadAsJson<List<ApplicationViewModel>>();
      Assert.NotNull(model);
      Assert.True(model.Count >= 0);
    }

    [Fact]
    public async Task CreateAsync_ApplicationCreated()
    {
      // Arrange
      var endpoint = "/api/application";

      // Act
      var response = await PostAsync(endpoint, GetViewModel());

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.Created, response.StatusCode);

      var id = await response.Content.ReadAsJson<string>();
      Assert.NotNull(id);
    }

    [Fact]
    public async Task GetAsync_ApplicationReceived()
    {
      // Arrange
      var endpoint = "/api/application";
      var createResponse = await PostAsync(endpoint, GetViewModel());
      var id = await createResponse.Content.ReadAsJson<string>();

      // Act
      var response = await GetAsync($"{endpoint}/{id}");

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);

      var model = await response.Content.ReadAsJson<ApplicationViewModel>();

      Assert.NotNull(model);
      Assert.Equal(id, model.Id);
      Assert.Equal(TEST_APPLICATION, model.ClientId);
      Assert.Equal("displayname", model.DisplayName);
      Assert.Null(model.ClientSecret);
      Assert.False(model.RequirePkce);
      Assert.False(model.RequireConsent);
      Assert.Empty(model.Permissions);
      Assert.Empty(model.RedirectUris);
      Assert.Empty(model.PostLogoutRedirectUris);
      Assert.Equal(ClientTypes.Public, model.Type);
    }

    [Fact]
    public async Task UpdateAsync_ApplicationUpdated()
    {
      // Arrange
      var endpoint = "/api/application";
      var createResponse = await PostAsync(endpoint, GetViewModel());
      var id = await createResponse.Content.ReadAsJson<string>();

      // Act
      var response = await PutAsync(endpoint, GetViewModel(id));

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAsync_ApplicationDeleted()
    {
      // Arrange
      var endpoint = "/api/application";
      var createResponse = await PostAsync(endpoint, GetViewModel());
      var id = await createResponse.Content.ReadAsJson<string>();

      // Act
      var response = await DeleteAsync($"{endpoint}/{id}");

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetOptionsAsync_ReturnsAListOfApplicationOptions()
    {
      // Arrange
      var endpoint = "/api/application/options";

      // Act
      var response = await GetAsync(endpoint);

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);

      var model = await response.Content.ReadAsJson<ApplicationOptionsViewModel>();
      Assert.NotNull(model);
      Assert.True(model.Types.Count == 2);
    }

    private static ApplicationViewModel GetViewModel(string id = null)
    {
      return new ApplicationViewModel
      {
        Id = id,
        ClientId = TEST_APPLICATION,
        DisplayName = "displayname",
        // ClientSecret = "clientsecret", // only when Type is confidential
        RequirePkce = false,
        RequireConsent = false,
        Permissions = new List<string>(),
        RedirectUris = new List<string>(),
        PostLogoutRedirectUris = new List<string>(),
        Type = ClientTypes.Public
      };
    }
  }
}