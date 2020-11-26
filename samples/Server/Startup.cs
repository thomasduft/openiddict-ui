using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Mvc.Server.Models;
using Mvc.Server.Services;
using Quartz;
using Swashbuckle.AspNetCore.SwaggerUI;
using tomware.OpenIddict.UI.Api;
using tomware.OpenIddict.UI.Infrastructure;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Mvc.Server
{
  public class Startup
  {
    public IConfiguration Configuration { get; }
    public IWebHostEnvironment Environment { get; }

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
      Configuration = configuration;
      Environment = environment;
    }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddCors(o =>
      {
        o.AddPolicy("AllowAllOrigins", builder =>
        {
          builder
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
      });

      services.AddControllersWithViews();

      services.AddDbContext<ApplicationDbContext>(options =>
      {
        // Configure the context to use Microsoft SQL Server.
        options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));

        // // Register the entity sets needed by OpenIddict.
        // // Note: use the generic overload if you need
        // // to replace the default OpenIddict entities.
        // options.UseOpenIddict(); // TODO: Explain!!
      });

      // Register the Identity services.
      services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
          options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

      // Configure Identity to use the same JWT claims as OpenIddict instead
      // of the legacy WS-Federation claims it uses by default (ClaimTypes),
      // which saves you from doing the mapping in your authorization controller.
      services.Configure<IdentityOptions>(options =>
      {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 5;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;

        options.ClaimsIdentity.UserNameClaimType = Claims.Name;
        options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
        options.ClaimsIdentity.RoleClaimType = Claims.Role;
      });

      if (!Helpers.Constants.IsTestingEnvironment(Environment.EnvironmentName))
      {
        // OpenIddict offers native integration with Quartz.NET to perform scheduled tasks
        // (like pruning orphaned authorizations/tokens from the database) at regular intervals.
        services.AddQuartz(options =>
        {
          options.UseMicrosoftDependencyInjectionJobFactory();
          options.UseSimpleTypeLoader();
          options.UseInMemoryStore();
        });

        // Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
      }

      services.AddOpenIddict()
        // Register the OpenIddict core components.
        .AddCore(options =>
        {
          // Configure OpenIddict to use the Entity Framework Core stores and models.
          // Note: call ReplaceDefaultEntities() to replace the default OpenIddict entities.
          options.UseEntityFrameworkCore();
          //  .UseDbContext<ApplicationDbContext>(); // TODO: Explain!!
          // Developers who prefer using MongoDB can remove the previous lines
          // and configure OpenIddict to use the specified MongoDB database:
          // options.UseMongoDb()
          //        .UseDatabase(new MongoClient().GetDatabase("openiddict"));
          // Enable Quartz.NET integration.
          if (!Helpers.Constants.IsTestingEnvironment(Environment.EnvironmentName))
          {
            options.UseQuartz();
          }
        })
        // Register the OpenIddict server components.
        .AddServer(options =>
        {
          options.SetIssuer(new Uri("https://localhost:5000/"));

          // Enable the authorization, device, logout, token, userinfo and verification endpoints.
          options.SetAuthorizationEndpointUris("/connect/authorize")
                 .SetDeviceEndpointUris("/connect/device")
                 .SetLogoutEndpointUris("/connect/logout")
                 .SetTokenEndpointUris("/connect/token")
                 .SetIntrospectionEndpointUris("/connect/introspect")
                 .SetUserinfoEndpointUris("/connect/userinfo")
                 .SetVerificationEndpointUris("/connect/verify");

          // Note: this sample uses the code, device, password and refresh token flows, but you
          // can enable the other flows if you need to support implicit or client credentials.
          options.AllowAuthorizationCodeFlow()
                 .AllowDeviceCodeFlow()
                 .AllowPasswordFlow()
                 .AllowRefreshTokenFlow();

          // Mark the "email", "profile", "roles" and "demo_api" scopes as supported scopes.
          options.RegisterScopes(
            Scopes.OpenId,
            Scopes.Email,
            Scopes.Profile,
            Scopes.Roles,
            "server_scope",
            "api_scope"
          );

          if (!Helpers.Constants.IsTestingEnvironment(Environment.EnvironmentName))
          {
            // Register the signing and encryption credentials.
            options.AddDevelopmentEncryptionCertificate()
                   .AddDevelopmentSigningCertificate();
          }
          else
          {
            options.AddEphemeralEncryptionKey()
                   .AddEphemeralSigningKey();
          }

          // Force client applications to use Proof Key for Code Exchange (PKCE).
          options.RequireProofKeyForCodeExchange();

          // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
          options.UseAspNetCore()
                 .EnableStatusCodePagesIntegration()
                 .EnableAuthorizationEndpointPassthrough()
                 .EnableLogoutEndpointPassthrough()
                 .EnableTokenEndpointPassthrough()
                 .EnableUserinfoEndpointPassthrough()
                 .EnableVerificationEndpointPassthrough()
                 .DisableTransportSecurityRequirement(); // During development, you can disable the HTTPS requirement.

          // Note: if you don't want to specify a client_id when sending
          // a token or revocation request, uncomment the following line:
          //
          // options.AcceptAnonymousClients();
          // Note: if you want to process authorization and token requests
          // that specify non-registered scopes, uncomment the following line:
          //
          // options.DisableScopeValidation();
          // Note: if you don't want to use permissions, you can disable
          // permission enforcement by uncommenting the following lines:
          //
          // options.IgnoreEndpointPermissions()
          //        .IgnoreGrantTypePermissions()
          //        .IgnoreResponseTypePermissions()
          //        .IgnoreScopePermissions();
          // Note: when issuing access tokens used by third-party APIs
          // you don't own, you can disable access token encryption:
          //
          // options.DisableAccessTokenEncryption();
        })
        // Register the OpenIddict validation components.
        .AddValidation(options =>
        {
          // Configure the audience accepted by this resource server.
          // The value MUST match the audience associated with the
          // "demo_api" scope, which is used by ResourceController.
          // options.AddAudiences("resource_server");
          // Import the configuration from the local OpenIddict server instance.
          options.UseLocalServer();

          // Register the ASP.NET Core host.
          options.UseAspNetCore();
          // For applications that need immediate access token or authorization
          // revocation, the database entry of the received tokens and their
          // associated authorizations can be validated for each API call.
          // Enabling these options may have a negative impact on performance.
          //
          // options.EnableAuthorizationEntryValidation();
          // options.EnableTokenEntryValidation();
        })
        // Register the EF based UI Store
        .AddUIStore(options =>
        {
          options.OpenIddictUIContext = builder =>
           builder.UseSqlite(Configuration.GetConnectionString("DefaultConnection"),
             sql => sql.MigrationsAssembly(typeof(Startup)
                       .GetTypeInfo()
                       .Assembly
                       .GetName()
                       .Name));
        })
        // Register the Api for the EF based UI Store
        .AddUIApis<ApplicationUser>(new OpenIddictUIApiOptions
        {
          // Tell the system about the allowed Permissions it is built/configured for.
          Permissions =
          {
            Permissions.Endpoints.Authorization,
            Permissions.Endpoints.Logout,
            Permissions.Endpoints.Token,
            Permissions.Endpoints.Introspection,
            Permissions.GrantTypes.AuthorizationCode,
            Permissions.GrantTypes.DeviceCode,
            Permissions.GrantTypes.Password,
            Permissions.GrantTypes.RefreshToken,
            Permissions.ResponseTypes.Code,
            Permissions.Scopes.Email,
            Permissions.Scopes.Profile,
            Permissions.Scopes.Roles,
            Permissions.Prefixes.Scope + "server_scope",
            Permissions.Prefixes.Scope + "api_scope"
          }
        });

      if (!Helpers.Constants.IsTestingEnvironment(Environment.EnvironmentName))
      {
        services.AddSwaggerGen(c =>
        {
          c.SwaggerDoc("v1", new OpenApiInfo
          {
            Version = "v1",
            Title = "API Server Documentation",
            Description = "API Server Documentation"
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
      }

      services.AddTransient<IEmailSender, AuthMessageSender>();
      services.AddTransient<ISmsSender, AuthMessageSender>();

      services.AddScoped<IMigrationService, MigrationService>();
    }

    public void Configure(IApplicationBuilder app)
    {
      app.UseCors("AllowAllOrigins");

      if (!Helpers.Constants.IsTestingEnvironment(Environment.EnvironmentName))
      {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
          c.SwaggerEndpoint("/swagger/v1/swagger.json", "Server API V1");
          c.DocExpansion(DocExpansion.None);
        });
      }

      app.UseDeveloperExceptionPage();

      app.UseRequestLocalization(options =>
      {
        options.AddSupportedCultures("en-US", "fr-FR");
        options.AddSupportedUICultures("en-US", "fr-FR");
        options.SetDefaultCulture("en-US");
      });

      app.UseDefaultFiles();
      app.UseStaticFiles();

      app.UseStatusCodePagesWithReExecute("/error");

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(options =>
      {
        options.MapControllers();
        options.MapDefaultControllerRoute();
      });
    }
  }
}
