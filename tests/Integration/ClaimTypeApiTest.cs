using System.Net;
using System.Threading.Tasks;
using Alba;
using tomware.OpenIddict.UI.Api;
using tomware.OpenIddict.UI.Tests.Helpers;
using Xunit;

namespace tomware.OpenIddict.UI.Tests.Integration
{
  public class ClaimTypeApiTest : IntegrationContext
  {
    public ClaimTypeApiTest(WebAppFixture fixture) : base(fixture)
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
      this.IssueAccessToken = false;

      // Act
      var response = await Scenario(s =>
      {
        switch (verb)
        {
          case HttpVerb.Post:
            s.Post.Json(new ClaimTypeViewModel()).ToUrl(endpoint);
            break;
          case HttpVerb.Put:
            s.Put.Json(new ClaimTypeViewModel()).ToUrl(endpoint);
            break;
          case HttpVerb.Delete:
            s.Delete.Url(endpoint);
            break;
          default:
            s.Get.Url(endpoint);
            break;
        }

        // Assert
        s.StatusCodeShouldBe(HttpStatusCode.Unauthorized);
      });

      Assert.NotNull(response);
      Assert.Equal(401, response.Context.Response.StatusCode);
    }
  }
}