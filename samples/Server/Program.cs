using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace tomware.Microip.Web
{
  public class Program
  {
    public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile(
        "appsettings.json",
        optional: false,
        reloadOnChange: true
      )
      .AddEnvironmentVariables()
      .Build();

    public static async Task Main(string[] args)
    {
      Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(Configuration)
        .CreateLogger();

      try
      {
        Log.Information("Starting up");
        await Start(args);
      }
      catch (Exception ex)
      {
        Log.Fatal(ex, "Application start failed!");
      }
      finally
      {
        Log.CloseAndFlush();
      }
    }

    public static async Task Start(string[] args)
    {
      var host = CreateHostBuilder(args).Build();

      // ensure database will be migrated
      using (var scope = host.Services.CreateScope())
      {
        var services = scope.ServiceProvider;
        try
        {
          var migrator = services.GetRequiredService<IMigrationService>();
          await migrator.EnsureMigrationAsync();
        }
        catch (Exception ex)
        {
          var logger = services.GetRequiredService<ILogger<Program>>();
          logger.LogError(ex, "An error occurred while migrating the database.");
        }
      }

      await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureWebHostDefaults(webBuilder =>
        {
          webBuilder.UseKestrel(o => o.AddServerHeader = false);
          webBuilder.UseConfiguration(Configuration);
          webBuilder.UseUrls(GetUrls(Configuration));
          webBuilder.UseStartup<Startup>();
        });

    //TODO: refactor/move to better place
    public static string GetUrls(IConfiguration config)
    {
      var domainSettings = config.GetSection("DomainSettings");
      var schema = domainSettings.GetValue<string>("Schema");
      var host = domainSettings.GetValue<string>("Host");
      var port = domainSettings.GetValue<int>("Port");

      return $"{schema}://{host}:{port}";
    }
  }
}