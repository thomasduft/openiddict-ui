using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace tomware.Microip.Web
{
  public static class ServicesExtension
  {
    public static IServiceCollection AddSTSServices(
      this IServiceCollection services
    )
    {
      var configuration = services
        .BuildServiceProvider()
        .GetRequiredService<IConfiguration>();
     
      var authority = GetAuthority(configuration);
      services
        .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
        {
          opt.Authority = authority;
          opt.Audience = Constants.STS_API;
          opt.RequireHttpsMetadata = false;
          opt.IncludeErrorDetails = true;
          opt.SaveToken = true;
          opt.TokenValidationParameters = new TokenValidationParameters()
          {
            ValidateIssuer = true,
            ValidateAudience = false,
            NameClaimType = "name",
            RoleClaimType = "role"
          };
        });

      // own services
      services.AddScoped<IMigrationService, MigrationService>();
      services.AddTransient<IEmailSender, LogEmailSender>();
      services.AddSingleton<ITitleService, TitleService>();

      return services;
    }

    private static string GetAuthority(IConfiguration configuration)
    {
      return Program.GetUrls(configuration);
    }

    private static X509Certificate2 GetCertificate(IConfiguration config)
    {
      var settings = config.GetSection("CertificateSettings");
      if (settings == null) return null;

      string filename = settings.GetValue<string>("filename");
      string password = settings.GetValue<string>("password");
      if (!string.IsNullOrEmpty(filename) && !string.IsNullOrEmpty(password))
      {
        return new X509Certificate2(filename, password);
      }

      return null;
    }
  }
}
