using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation.AspNetCore;

namespace Api
{
  public class Startup
  {
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddRouting(o => o.LowercaseUrls = true);

      services.AddAuthentication(options =>
      {
        options.DefaultScheme
          = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
      });

      // Register the OpenIddict validation components.
      services.AddOpenIddict()
        .AddValidation(options =>
        {
          // Note: the validation handler uses OpenID Connect discovery
          // to retrieve the address of the introspection endpoint.
          options.SetIssuer("https://localhost:5001/");
          options.AddAudiences("api_service");

          // Configure the validation handler to use introspection and register the client
          // credentials used when communicating with the remote introspection endpoint.
          options.UseIntrospection()
                 .SetClientId("api_service")
                 .SetClientSecret("my-api-secret");

          // Register the System.Net.Http integration.
          options.UseSystemNetHttp();

          // Register the ASP.NET Core host.
          options.UseAspNetCore();
        });

      services.AddControllers();
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });
      });
    }

    public void Configure(
      IApplicationBuilder app,
      IWebHostEnvironment env
    )
    {
      if (env.IsDevelopment())
      {
        app.UseCors(builder =>
        {
          builder.WithOrigins("http://localhost:4200", "https://localhost:5001");
          builder.WithMethods("GET");
          builder.WithHeaders("Authorization");
        });

        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1"));
      }

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
