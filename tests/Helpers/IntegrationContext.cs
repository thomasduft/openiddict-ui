using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Alba;
using Newtonsoft.Json.Linq;
using Xunit;

namespace tomware.OpenIddict.UI.Tests.Helpers
{
  public abstract class IntegrationContext : IClassFixture<WebAppFixture>
  {
    // The Alba system
    protected SystemUnderTest System => Fixture.System;
    // Just a convenience because you use it pretty often in tests to get at application services
    protected IServiceProvider Services => Fixture.System.Services;

    public WebAppFixture Fixture { get; }

    protected IntegrationContext(WebAppFixture fixture)
    {
      Fixture = fixture;
    }

    /// <summary>
    /// Runs Alba HTTP scenarios through your ASP.Net Core system
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    protected Task<IScenarioResult> Scenario(Action<Scenario> configure)
    {
      return Fixture.System.Scenario(configure);
    }
  }
}