using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace tomware.Microip.Web
{
  public static class ServicesExtension
  {
    public static IServiceCollection AddSTSServices(
      this IServiceCollection services
    )
    {
      services.AddScoped<IMigrationService, MigrationService>();
      services.AddTransient<IEmailSender, LogEmailSender>();
      services.AddSingleton<ITitleService, TitleService>();

      return services;
    }
  }
}
