using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Mvc.Server;
using tomware.OpenIddict.UI.Api;
using tomware.OpenIddict.UI.Tests.Helpers;
using Xunit;

namespace tomware.OpenIddict.UI.Tests.Integration
{
  public class ClaimeTypeApiTest : IntegrationContext
  {
    public ClaimeTypeApiTest(IntegrationApplicationFactory<Startup> fixture)
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
        default:
          response = await GetAsync(endpoint, authorized);
          break;
      }

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
  }
}