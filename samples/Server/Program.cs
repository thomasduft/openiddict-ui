using Serilog;
using Server;
using Server.Services;


var loggerConfiguration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

Log.Logger = new LoggerConfiguration()
              .ReadFrom.Configuration(loggerConfiguration)
              .CreateLogger();

try
{
  Log.Information("Starting web host");

  var builder = WebApplication.CreateBuilder(args);
  var configuration = builder.Configuration;
  var environmentName = builder.Environment.EnvironmentName;

  // Configure services
  builder.Services.AddServer(configuration, environmentName);

  // Configure application pipeline
  var app = builder.Build();
  app.UseServer(environmentName);

  // Ensure DB migrations
  using (var scope = app.Services.CreateScope())
  {
    var services = scope.ServiceProvider;
    try
    {
      var migrator = services.GetRequiredService<IMigrationService>();
      migrator.EnsureMigrationAsync().GetAwaiter().GetResult();
    }
    catch (Exception ex)
    {
      app.Logger.LogError(ex, "An error occurred while migrating the database.");
    }
  }

  app.Run();
}
catch (Exception ex)
{
  Log.Fatal(ex, "Host terminated unexpectedly");
  return 1;
}
finally
{
  Log.CloseAndFlush();
}

return 0;