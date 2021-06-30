using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using tomware.OpenIddict.UI.Identity.Api;
using Xunit;

namespace tomware.OpenIddict.UI.Tests.Unit
{
  public class RoleControllerTest
  {
    [Fact]
    public async Task GetAsync_WithNullId_ReturnsBadRequest()
    {
      // Arrange
      var controller = GetController();

      // Act
      var result = await controller.GetAsync(null);

      // Assert
      Assert.NotNull(result);
      Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task GetAsync_WithNotExistingId_ReturnsNotFound()
    {
      // Arrange
      var serviceMock = new Mock<IRoleService>();
      var controller = GetController(serviceMock);
      var id = "id";

      serviceMock
        .Setup(r => r.GetAsync(id))
        .Returns(Task.FromResult<RoleViewModel>(null));

      // Act
      var result = await controller.GetAsync(id);

      // Assert
      Assert.NotNull(result);
      Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateAsync_WithNullModel_ReturnsBadRequest()
    {
      // Arrange
      var controller = GetController();

      // Act
      var result = await controller.CreateAsync(null);

      // Assert
      Assert.NotNull(result);
      Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidModel_ReturnsBadRequest()
    {
      // Arrange
      var controller = GetController();
      var model = new RoleViewModel
      {
        // Name // required
      };

      // ModelState must be manually adjusted
      controller.ModelState.AddModelError(string.Empty, "Name required!");

      // Act
      var result = await controller.CreateAsync(model);

      // Assert
      Assert.NotNull(result);
      Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateAsync_WithNullModel_ReturnsBadRequest()
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
    public async Task UpdateAsync_WithInvalidModel_ReturnsBadRequest()
    {
      // Arrange
      var controller = GetController();
      var model = new RoleViewModel
      {
        // Name // required
      };

      // ModelState must be manually adjusted
      controller.ModelState.AddModelError(string.Empty, "Name required!");

      // Act
      var result = await controller.UpdateAsync(model);

      // Assert
      Assert.NotNull(result);
      Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteAsync_WithNullModel_ReturnsBadRequest()
    {
      // Arrange
      var controller = GetController();

      // Act
      var result = await controller.DeleteAsync(null);

      // Assert
      Assert.NotNull(result);
      Assert.IsType<BadRequestResult>(result);
    }

    private static RoleController GetController(Mock<IRoleService> service = null)
    {
      service ??= new Mock<IRoleService>();

      return new RoleController(service.Object);
    }
  }
}