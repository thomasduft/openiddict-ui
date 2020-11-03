using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(tomware.Microip.Web.Areas.Identity.IdentityHostingStartup))]
namespace tomware.Microip.Web.Areas.Identity
{
  public class IdentityHostingStartup : IHostingStartup
  {
    public void Configure(IWebHostBuilder builder)
    {
      builder.ConfigureServices((context, services) =>
      {
      });
    }
  }
}