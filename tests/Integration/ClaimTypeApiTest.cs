using Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using tomware.OpenIddict.UI.Identity.Api;
using tomware.OpenIddict.UI.Tests.Helpers;
using Xunit;

namespace tomware.OpenIddict.UI.Tests.Integration
{
  public class ClaimTypeApiTest : IntegrationContext
  {
    private const string TEST_CLAIMTYPE = "test_claimtype";

    public ClaimTypeApiTest(IntegrationApplicationFactory<Startup> fixture)
      : base(fixture)
    { }

    [Theory]
    [InlineData("/api/claimtypes", HttpVerb.Get)]
    [InlineData("/api/claimtypes/id", HttpVerb.Get)]
    [InlineData("/api/claimtypes", HttpVerb.Post)]
    // [InlineData("/api/claimtypes", HttpVerb.Put)]
    // [InlineData("/api/claimtypes/01D7ACA3-575C-4E60-859F-DB95B70F8190", HttpVerb.Delete)]
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
          response = await PostAsync(endpoint, new ClaimTypeViewModel(), authorized);
          break;
        case HttpVerb.Put:
          response = await PutAsync(endpoint, new ClaimTypeViewModel(), authorized);
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
      var endpoint = "/api/claimtypes";

      // Act
      var response = await GetAsync(endpoint);

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);

      var model = await response.Content.ReadAsJson<List<ClaimTypeViewModel>>();
      Assert.NotNull(model);
      Assert.True(model.Count >= 0);
    }

    [Fact]
    public async Task CreateAsync_ClaimTypeCreated()
    {
      // Arrange
      var endpoint = "/api/claimtypes";

      // Act
      var response = await PostAsync(endpoint, new ClaimTypeViewModel
      {
        Name = TEST_CLAIMTYPE,
        Description = "description",
        ClaimValueType = ClaimValueTypes.Boolean
      });

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.Created, response.StatusCode);

      var id = await response.Content.ReadAsJson<Guid>();
      Assert.True(id != Guid.Empty);
    }

    [Fact]
    public async Task GetAsync_ClaimTypeReceived()
    {
      // Arrange
      var endpoint = "/api/claimtypes";
      var createResponse = await PostAsync(endpoint, new ClaimTypeViewModel
      {
        Name = TEST_CLAIMTYPE,
        Description = "description",
        ClaimValueType = ClaimValueTypes.Boolean
      });
      var id = await createResponse.Content.ReadAsJson<Guid>();

      // Act
      var response = await GetAsync($"{endpoint}/{id}");

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);

      var model = await response.Content.ReadAsJson<ClaimTypeViewModel>();

      Assert.NotNull(model);
      Assert.Equal(id, model.Id.Value);
      Assert.Equal(TEST_CLAIMTYPE, model.Name);
      Assert.Equal("description", model.Description);
      Assert.Equal(ClaimValueTypes.Boolean, model.ClaimValueType);
    }

    [Fact]
    public async Task UpdateAsync_ClaimTypeUpdated()
    {
      // Arrange
      var endpoint = "/api/claimtypes";
      var createResponse = await PostAsync(endpoint, new ClaimTypeViewModel
      {
        Name = TEST_CLAIMTYPE,
        Description = "description",
        ClaimValueType = ClaimValueTypes.Boolean
      });
      var id = await createResponse.Content.ReadAsJson<Guid>();

      // Act
      var response = await PutAsync(endpoint, new ClaimTypeViewModel
      {
        Id = id,
        Name = TEST_CLAIMTYPE,
        Description = "updated description",
        ClaimValueType = ClaimValueTypes.Boolean
      });

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAsync_ClaimTypeDeleted()
    {
      // Arrange
      var endpoint = "/api/claimtypes";
      var createResponse = await PostAsync(endpoint, new ClaimTypeViewModel
      {
        Name = TEST_CLAIMTYPE,
        Description = "description",
        ClaimValueType = ClaimValueTypes.Boolean
      });
      var id = await createResponse.Content.ReadAsJson<Guid>();

      // Act
      var response = await DeleteAsync($"{endpoint}/{id}");

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
  }
}