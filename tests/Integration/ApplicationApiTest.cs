using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Server;
using tomware.OpenIddict.UI.Api;
using tomware.OpenIddict.UI.Tests.Helpers;
using Xunit;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace tomware.OpenIddict.UI.Tests.Integration;

public class ApplicationApiTest : IntegrationContext
{
  private const string TEST_APPLICATION = "test_application";

  public ApplicationApiTest(IntegrationApplicationFactory<Testing> fixture)
    : base(fixture)
  { }

  [Theory]
  [InlineData("/api/application", HttpVerb.Get)]
  [InlineData("/api/application/01D7ACA3-575C-4E60-859F-DB95B70F8190", HttpVerb.Get)]
  [InlineData("/api/application", HttpVerb.Post)]
  [InlineData("/api/application", HttpVerb.Put)]
  [InlineData("/api/application/01D7ACA3-575C-4E60-859F-DB95B70F8190", HttpVerb.Delete)]
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
        response = await PostAsync(endpoint, new ApplicationViewModel(), authorized);
        break;
      case HttpVerb.Put:
        response = await PutAsync(endpoint, new ApplicationViewModel(), authorized);
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
  public async Task GetClaimTypesAsyncReturnsAList()
  {
    // Arrange
    var endpoint = "/api/application";

    // Act
    var response = await GetAsync(endpoint);

    // Assert
    Assert.NotNull(response);
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var model = await response.Content.ReadAsJsonAsync<List<ApplicationViewModel>>();
    Assert.NotNull(model);
    Assert.True(model.Count >= 0);
  }

  [Fact]
  public async Task CreateAsyncApplicationCreated()
  {
    // Arrange
    var endpoint = "/api/application";

    // Act
    var response = await PostAsync(endpoint, GetPublicApplicationViewModel());

    // Assert
    Assert.NotNull(response);
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);

    var id = await response.Content.ReadAsJsonAsync<string>();
    Assert.NotNull(id);
  }

  [Fact]
  public async Task GetAsyncApplicationReceived()
  {
    // Arrange
    var endpoint = "/api/application";
    var createResponse = await PostAsync(endpoint, GetPublicApplicationViewModel());
    var id = await createResponse.Content.ReadAsJsonAsync<string>();

    // Act
    var response = await GetAsync($"{endpoint}/{id}");

    // Assert
    Assert.NotNull(response);
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var model = await response.Content.ReadAsJsonAsync<ApplicationViewModel>();

    Assert.NotNull(model);
    Assert.Equal(id, model.Id);
    Assert.Equal(TEST_APPLICATION, model.ClientId);
    Assert.Equal("displayname", model.DisplayName);
    Assert.Null(model.ClientSecret);
    Assert.False(model.RequirePkce);
    Assert.False(model.RequireConsent);
    Assert.Single(model.Permissions);
    Assert.Single(model.RedirectUris);
    Assert.Single(model.PostLogoutRedirectUris);
    Assert.Equal(ClientTypes.Public, model.Type);
  }

  [Fact]
  public async Task UpdatePublicApplicationApplicationUpdated()
  {
    // Arrange
    var endpoint = "/api/application";
    var createResponse = await PostAsync(endpoint, GetPublicApplicationViewModel());
    var id = await createResponse.Content.ReadAsJsonAsync<string>();

    // Act
    var response = await PutAsync(endpoint, GetPublicApplicationViewModel(id));

    // Assert
    Assert.NotNull(response);
    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
  }

  [Fact]
  public async Task UpdateConfidentialApplicationApplicationUpdated()
  {
    // Arrange
    var endpoint = "/api/application";
    var viewModel = GetConfidentialApplicationViewModel();
    var createResponse = await PostAsync(endpoint, viewModel);
    var id = await createResponse.Content.ReadAsJsonAsync<string>();

    viewModel.Id = id;
    viewModel.DisplayName = "updatedDisplayName";
    viewModel.ClientSecret = "updatedclientsecret";

    // Act
    var response = await PutAsync(endpoint, viewModel);

    // Assert
    Assert.NotNull(response);
    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
  }

  [Fact]
  public async Task UpdateConfidentialApplicationApplicationIsNotUpdated()
  {
    // Arrange
    var endpoint = "/api/application";
    var viewModel = GetConfidentialApplicationViewModel();
    var createResponse = await PostAsync(endpoint, viewModel);
    var id = await createResponse.Content.ReadAsJsonAsync<string>();

    viewModel.Id = id;
    viewModel.DisplayName = "updatedDisplayName";
    viewModel.ClientSecret = ""; // forgetting sending required ClientSecret

    // Act
    var response = await PutAsync(endpoint, viewModel);

    // Assert
    Assert.NotNull(response);
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }

  [Fact]
  public async Task DeleteAsyncApplicationDeleted()
  {
    // Arrange
    var endpoint = "/api/application";
    var createResponse = await PostAsync(endpoint, GetPublicApplicationViewModel());
    var id = await createResponse.Content.ReadAsJsonAsync<string>();

    // Act
    var response = await DeleteAsync($"{endpoint}/{id}");

    // Assert
    Assert.NotNull(response);
    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
  }

  [Fact]
  public async Task GetOptionsAsyncReturnsAListOfApplicationOptions()
  {
    // Arrange
    var endpoint = "/api/application/options";

    // Act
    var response = await GetAsync(endpoint);

    // Assert
    Assert.NotNull(response);
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var model = await response.Content.ReadAsJsonAsync<ApplicationOptionsViewModel>();
    Assert.NotNull(model);
    Assert.True(model.Types.Count == 2);
  }

  private static ApplicationViewModel GetPublicApplicationViewModel(string id = null)
  {
    return new ApplicationViewModel
    {
      Id = id,
      ClientId = TEST_APPLICATION,
      DisplayName = "displayname",
      // ClientSecret = "clientsecret", // only when Type is confidential
      RequirePkce = false,
      RequireConsent = false,
      Permissions = new List<string> { "somePermission " },
      RedirectUris = new List<string> { "https://tomware.ch/redirect" },
      PostLogoutRedirectUris = new List<string> { "https://tomware.ch/postLogout" },
      Type = ClientTypes.Public
    };
  }

  private static ApplicationViewModel GetConfidentialApplicationViewModel(string id = null)
  {
    return new ApplicationViewModel
    {
      Id = id,
      ClientId = TEST_APPLICATION,
      DisplayName = "displayname",
      ClientSecret = "clientsecret", // only when Type is confidential
      RequirePkce = false,
      RequireConsent = false,
      Permissions = new List<string> { "somePermission " },
      Type = ClientTypes.Confidential
    };
  }
}
