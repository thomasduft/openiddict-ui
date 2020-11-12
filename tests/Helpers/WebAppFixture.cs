using System;
using Alba;
using Microsoft.Extensions.Hosting;
using Mvc.Server;

namespace tomware.OpenIddict.UI.Tests.Helpers
{
  public class WebAppFixture : IDisposable
  {
    public SystemUnderTest System { get; }

    // see: https://jeremydmiller.com/2020/04/13/using-alba-for-integration-testing-asp-net-core-web-services/
    public WebAppFixture()
    {
      // Use the application configuration the way that it is in the real application
      // project
      var builder = Program.CreateHostBuilder(new string[0])

          // You may need to do this for any static
          // content or files in the main application including
          // appsettings.json files

          // DirectoryFinder is an Alba helper
          // .UseContentRoot(DirectoryFinder.FindParallelFolder("WebApplication"))

          // Override the hosting environment to "Testing"
          .UseEnvironment("Testing");

      // This is the Alba scenario wrapper around
      // TestServer and an active .Net Core IHost
      System = new SystemUnderTest(builder);

      // There's also a BeforeEachAsync() signature
      System.BeforeEach(httpContext =>
      {
        // Take any kind of setup action before
        // each simulated HTTP request

        // In this case, I'm setting a fake JWT token on each request
        // as a demonstration
        // httpContext.Request.Headers["Authorization"] = $"Bearer {generateToken()}";
      });

      System.AfterEach(httpContext =>
      {
        // Take any kind of teardown action after
        // each simulated HTTP request
      });
    }

    public void Dispose()
    {
      System?.Dispose();
    }
  }
}