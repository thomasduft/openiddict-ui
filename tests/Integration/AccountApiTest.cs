using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Mvc.Server;
using Mvc.Server.Models;
using tomware.OpenIddict.UI.Api;
using tomware.OpenIddict.UI.Tests.Helpers;
using Xunit;

namespace tomware.OpenIddict.UI.Tests.Integration
{
  public class AccountApiTest : IntegrationContext
  {
    private const string TEST_ROLE = "test_role";
    private const string NEW_ROLE = "new_role";
    private const string UPDATE_ROLE = "update_role";

    public AccountApiTest(IntegrationApplicationFactory<Startup> fixture)
      : base(fixture)
    { }

    [Theory]
    [InlineData("/api/accounts/users", HttpVerb.Get)]
    [InlineData("/api/accounts/user/01D7ACA3-575C-4E60-859F-DB95B70F8190", HttpVerb.Get)]
    // [InlineData("/api/accounts/user", HttpVerb.Put)]
    // [InlineData("/api/accounts/user/01D7ACA3-575C-4E60-859F-DB95B70F8190", HttpVerb.Delete)]
    public async Task IsNotAuthenticatedForUsers_ReturnsUnauthorized(
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
          response = await PostAsync(endpoint, new UserViewModel(), authorized);
          break;
        case HttpVerb.Put:
          response = await PutAsync(endpoint, new UserViewModel(), authorized);
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

    [Theory]
    [InlineData("/api/accounts/register")]
    [InlineData("/api/accounts/changepassword")]
    public async Task IsNotAuthenticated_ReturnsUnauthorized(string endpoint)
    {
      // Arrange
      HttpResponseMessage response = null;
      var authorized = false;

      // Act
      if (endpoint.EndsWith("register"))
      {
        response = await PostAsync(endpoint, new RegisterUserViewModel(), authorized);
      }

      if (endpoint.EndsWith("changepassword"))
      {
        response = await PostAsync(endpoint, new ChangePasswordViewModel(), authorized);
      }

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Users_ReturnsListOfUsers()
    {
      // Arrange
      var endpoint = "/api/accounts/users";

      // Act
      var response = await GetAsync(endpoint);

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);

      var model = await response.Content.ReadAsJson<List<UserViewModel>>();
      Assert.NotNull(model);
      Assert.True(model.Count() > 0);
    }

    [Fact]
    public async Task Register_UserRegistered()
    {
      // Arrange
      var email = "mail@openiddict.com";
      await DeleteUser(email);

      var endpoint = "/api/accounts/register";

      // Act
      var response = await PostAsync(endpoint, new RegisterUserViewModel
      {
        UserName = "username",
        Email = email,
        Password = "Pass123$",
        ConfirmPassword = "Pass123$"
      });

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // [Fact]
    // public async Task GetAsync_RoleReceived()
    // {
    //   // Arrange
    //   var endpoint = "/api/roles";
    //   var createResponse = await PostAsync(endpoint, new RoleViewModel
    //   {
    //     Name = NEW_ROLE
    //   });
    //   var id = await createResponse.Content.ReadAsJson<string>();

    //   // Act
    //   var response = await GetAsync($"{endpoint}/{id}");

    //   // Assert
    //   Assert.NotNull(response);
    //   Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    //   var model = await response.Content.ReadAsJson<RoleViewModel>();

    //   Assert.NotNull(model);
    //   Assert.Equal(id, model.Id);
    //   Assert.Equal(NEW_ROLE, model.Name);
    // }

    // [Fact]
    // public async Task UpdateAsync_RoleUpdated()
    // {
    //   // Arrange
    //   var endpoint = "/api/roles";
    //   var createResponse = await PostAsync(endpoint, new RoleViewModel
    //   {
    //     Name = NEW_ROLE
    //   });
    //   var id = await createResponse.Content.ReadAsJson<string>();

    //   // Act
    //   var response = await PutAsync(endpoint, new RoleViewModel
    //   {
    //     Id = id,
    //     Name = UPDATE_ROLE
    //   });

    //   // Assert
    //   Assert.NotNull(response);
    //   Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    // }

    // [Fact]
    // public async Task DeleteAsync_RoleDeleted()
    // {
    //   // Arrange
    //   var endpoint = "/api/roles";
    //   var createResponse = await PostAsync(endpoint, new RoleViewModel
    //   {
    //     Name = NEW_ROLE
    //   });
    //   var id = await createResponse.Content.ReadAsJson<string>();

    //   // Act
    //   var response = await DeleteAsync($"{endpoint}/{id}");

    //   // Assert
    //   Assert.NotNull(response);
    //   Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    // }

    private async Task DeleteUser(string email)
    {
      UserManager<ApplicationUser> usermanager
       = (UserManager<ApplicationUser>)Services.GetService(typeof(UserManager<ApplicationUser>));
      var user = await usermanager.FindByEmailAsync(email);
      if (user == null) return;

      var deleteResult = await usermanager.DeleteAsync(user);
      Assert.True(deleteResult.Succeeded);
    }
  }
}