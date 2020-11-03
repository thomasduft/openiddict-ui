using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace tomware.Microip.Web
{
  public static class SwaggerServicesExtension
  {
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
          Version = "v1",
          Title = "STS API Documentation",
          Description = "STS API Documentation"
        });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
          In = ParameterLocation.Header,
          Name = "Authorization",
          Description = "Example: \"Bearer {token}\"",
          Type = SecuritySchemeType.ApiKey
        });

        c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
      });

      return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MicroWF API V1");
        c.DocExpansion(DocExpansion.None);
      });

      return app;
    }
  }
}
