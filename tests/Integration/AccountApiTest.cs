using Microsoft.AspNetCore.Identity;
using Mvc.Server;
using Mvc.Server.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using tomware.OpenIddict.UI.Identity.Api;
using tomware.OpenIddict.UI.Tests.Helpers;
using Xunit;

namespace tomware.OpenIddict.UI.Tests.Integration
{
  public class AccountApiTest : IntegrationContext
  {
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
        case HttpVerb.Get:
          response = await GetAsync(endpoint, authorized);
          break;
      }

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/accounts/register")]
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
      Assert.True(model.Count > 0);
    }

    [Fact]
    public async Task Register_UserRegistered()
    {
      // Arrange
      var email = "mail@openiddict.com";
      await DeleteUser(email);

      // Act
      var response = await PostAsync("/api/accounts/register", new RegisterUserViewModel
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

    [Fact]
    public async Task GetAsync_UserReceived()
    {
      // Arrange
      var email = "mail@openiddict.com";
      var user = await FindUserByEmail(email);
      Assert.NotNull(user);

      // Act
      var response = await GetAsync($"/api/accounts/user/{user.Id}");

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);

      var model = await response.Content.ReadAsJson<UserViewModel>();
      Assert.NotNull(model);
      Assert.Equal(user.Id, model.Id);
      Assert.Equal(email, model.Email);
    }

    [Fact]
    public async Task UpdateAsync_UserUpdated()
    {
      // Arrange
      var email = "mail@openiddict.com";
      await PostAsync("/api/accounts/register", new RegisterUserViewModel
      {
        UserName = "username",
        Email = email,
        Password = "Pass123$",
        ConfirmPassword = "Pass123$"
      });

      var user = await FindUserByEmail(email);
      Assert.NotNull(user);

      // Act
      var response = await PutAsync("/api/accounts/user", new UserViewModel
      {
        Id = user.Id,
        UserName = user.UserName,
        Email = user.Email,
        LockoutEnabled = !user.LockoutEnabled
      });

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAsync_UserDeleted()
    {
      // Arrange
      var email = "userToDelete@openiddict.com";
      await PostAsync("/api/accounts/register", new RegisterUserViewModel
      {
        UserName = "username",
        Email = email,
        Password = "Pass123$",
        ConfirmPassword = "Pass123$"
      });

      var user = await FindUserByEmail(email);
      Assert.NotNull(user);

      // Act
      var response = await DeleteAsync($"/api/accounts/user/{user.Id}");

      // Assert
      Assert.NotNull(response);
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task DeleteUser(string email)
    {
      var user = await FindUserByEmail(email);
      if (user == null) return;

      var usermanager = GetRequiredService<UserManager<ApplicationUser>>();
      var deleteResult = await usermanager.DeleteAsync(user);
      Assert.True(deleteResult.Succeeded);
    }

    private async Task<ApplicationUser> FindUserByEmail(string email)
    {
      var usermanager = GetRequiredService<UserManager<ApplicationUser>>();
      return await usermanager.FindByEmailAsync(email);
    }
  }
}