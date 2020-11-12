using System.Net;
using System.Threading.Tasks;
using Alba;
using tomware.OpenIddict.UI.Tests.Helpers;
using Xunit;

namespace tomware.OpenIddict.UI.Tests.Integration
{
  public class RoleApiTest : IClassFixture<WebAppFixture>
  {
    private readonly SystemUnderTest _system;

    public RoleApiTest(WebAppFixture app)
    {
      _system = app.SystemUnderTest;
    }

    [Fact]
    public async Task GetRolesAsync_IsNotAuthenticated_ReturnsUnauthorized()
    {
      // Arrange
      var endpoint = "/api/roles";

      // Act
      var response = await _system.Scenario(s =>
      {
        s.Get.Url(endpoint);

        // Assert
        s.StatusCodeShouldBe(HttpStatusCode.Unauthorized);
      });

      Assert.NotNull(response);
      Assert.Equal(401, response.Context.Response.StatusCode);
    }
  }
}