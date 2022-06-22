using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using tomware.OpenIddict.UI.Identity.Api;
using Xunit;

namespace tomware.OpenIddict.UI.Tests.Unit;

public class AccountControllerTest
{
  [Fact]
  public async Task RegisterWithNullModelReturnsBadRequest()
  {
    // Arrange
    var controller = GetController();

    // Act
    var result = await controller.Register(null);

    // Assert
    Assert.NotNull(result);
    Assert.IsType<BadRequestResult>(result);
  }


  [Fact]
  public async Task ChangePasswordWithNullModelReturnsBadRequest()
  {
    // Arrange
    var controller = GetController();

    // Act
    var result = await controller.ChangePassword(null);

    // Assert
    Assert.NotNull(result);
    Assert.IsType<BadRequestResult>(result);
  }

  [Fact]
  public async Task GetUserWithNullIdReturnsBadRequest()
  {
    // Arrange
    var controller = GetController();

    // Act
    var result = await controller.GetUser(null);

    // Assert
    Assert.NotNull(result);
    Assert.IsType<BadRequestResult>(result);
  }

  [Fact]
  public async Task UpdateAsyncWithNullModelReturnsBadRequest()
  {
    // Arrange
    var controller = GetController();

    // Act
    var result = await controller.UpdateAsync(null);

    // Assert
    Assert.NotNull(result);
    Assert.IsType<BadRequestResult>(result);
  }

  [Fact]
  public async Task UpdateAsyncWithInvalidModelReturnsBadRequest()
  {
    // Arrange
    var controller = GetController();
    var model = new UserViewModel
    {
      Id = Guid.NewGuid().ToString(), // required
      UserName = "UserName" // required
      // Email // required
    };

    // ModelState must be manually adjusted
    controller.ModelState.AddModelError(string.Empty, "Email required!");

    // Act
    var result = await controller.UpdateAsync(model);

    // Assert
    Assert.NotNull(result);
    Assert.IsType<BadRequestObjectResult>(result);
  }

  [Fact]
  public async Task DeleteAsyncWithNullModelReturnsBadRequest()
  {
    // Arrange
    var controller = GetController();

    // Act
    var result = await controller.DeleteAsync(null);

    // Assert
    Assert.NotNull(result);
    Assert.IsType<BadRequestResult>(result);
  }

  private static AccountController GetController()
  {
    var service = new Mock<IAccountApiService>();

    return new AccountController(service.Object);
  }
}
