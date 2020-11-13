using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Alba;
using tomware.OpenIddict.UI.Api;
using tomware.OpenIddict.UI.Tests.Helpers;
using Xunit;

namespace tomware.OpenIddict.UI.Tests.Integration
{
  public class RoleApiTest : IntegrationContext
  {
    public RoleApiTest(WebAppFixture fixture) : base(fixture)
    {
    }

    [Theory]
    [InlineData("/api/roles", HttpVerb.Get)]
    [InlineData("/api/roles/id", HttpVerb.Get)]
    [InlineData("/api/roles", HttpVerb.Post)]
    // [InlineData("/api/roles", HttpVerb.Put)]
    // [InlineData("/api/roles/01D7ACA3-575C-4E60-859F-DB95B70F8190", HttpVerb.Delete)]
    public async Task IsNotAuthenticated_ReturnsUnauthorized(
      string endpoint,
      HttpVerb verb
    )
    {
      // Arrange
      this.IssueAccessToken = false;

      // Act
      var response = await Scenario(_ =>
      {
        switch (verb)
        {
          case HttpVerb.Post:
            _.Post.Json(new RoleViewModel()).ToUrl(endpoint);
            break;
          case HttpVerb.Put:
            _.Put.Json(new RoleViewModel()).ToUrl(endpoint);
            break;
          case HttpVerb.Delete:
            _.Delete.Url(endpoint);
            break;
          default:
            _.Get.Url(endpoint);
            break;
        }

        // Assert
        _.StatusCodeShouldBe(HttpStatusCode.Unauthorized);
      });

      Assert.NotNull(response);
      Assert.Equal(401, response.Context.Response.StatusCode);
    }

    [Fact]
    public async Task GetRolesAsync_ReturnsAdministratorRole()
    {
      // Arrange
      var endpoint = "/api/roles";

      // Act
      var response = await System.GetAsJson<IEnumerable<RoleViewModel>>(endpoint);

      // Assert
      Assert.NotNull(response);
      Assert.Single(response);
    }
  }
}