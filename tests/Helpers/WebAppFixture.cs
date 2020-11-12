using System;
using Alba;

namespace tomware.OpenIddict.UI.Tests.Helpers
{
  public class WebAppFixture : IDisposable
  {
    public readonly SystemUnderTest SystemUnderTest = SystemUnderTest
      .ForStartup<Mvc.Server.Startup>();

    public void Dispose()
    {
      SystemUnderTest?.Dispose();
    }
  }
}