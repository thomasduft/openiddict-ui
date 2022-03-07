using Swashbuckle.AspNetCore.SwaggerUI;

namespace Server
{
  public static class ConfigureApplication
  {
    public static void UseServer(
      this IApplicationBuilder app,
      string environmentName
    )
    {
      if (Helpers.Constants.IsDevelopmentEnvironment(environmentName))
      {
        app.UseCors(builder =>
        {
          builder
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });

        if (!Helpers.Constants.IsTestingEnvironment(environmentName))
        {
          app.UseSwagger();
          app.UseSwaggerUI(c =>
          {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Server API V1");
            c.DocExpansion(DocExpansion.None);
          });
        }

        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Error");
      }

      app.UseDefaultFiles();
      app.UseStaticFiles();

      app.UseStatusCodePagesWithReExecute("/error");

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(builder =>
      {
        builder.MapControllers();
        builder.MapRazorPages();
        builder.MapDefaultControllerRoute();
      });
    }
  }
}