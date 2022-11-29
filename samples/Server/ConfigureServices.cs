using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Quartz;
using Server.Models;
using Server.Services;
using tomware.OpenIddict.UI.Api;
using tomware.OpenIddict.UI.Identity.Api;
using tomware.OpenIddict.UI.Identity.Infrastructure;
using tomware.OpenIddict.UI.Infrastructure;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Server;

public static class ConfigureServices
{
  public static IServiceCollection AddServer(
    this IServiceCollection services,
    IConfiguration configuration,
    string environmentName
  )
  {
    services.AddRouting(options =>
    {
      options.LowercaseUrls = true;
    });

    services.AddDbContext<ApplicationDbContext>(options =>
    {
      // Configure the context to use Microsoft SQL Server.
      options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
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

    if (!Helpers.Constants.IsTestingEnvironment(environmentName))
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
      services.AddQuartzHostedService(options =>
      {
        options.WaitForJobsToComplete = true;
      });
    }

    services.AddOpenIddict()
      // Register the OpenIddict core components.
      .AddCore(options =>
      {
        options.UseEntityFrameworkCore();
        if (!Helpers.Constants.IsTestingEnvironment(environmentName))
        {
          options.UseQuartz();
        }
      })
      // Register the OpenIddict server components.
      .AddServer(options =>
      {
        options.SetIssuer(new Uri("https://localhost:5001/"));

        // Enable the authorization, device, logout, token, userinfo and verification endpoints.
        options.SetAuthorizationEndpointUris("/connect/authorize")
               .SetLogoutEndpointUris("/connect/logout")
               .SetTokenEndpointUris("/connect/token")
               .SetIntrospectionEndpointUris("/connect/introspect")
               .SetUserinfoEndpointUris("/connect/userinfo");

        // Note: this sample uses the code, device, password and refresh token flows, but you
        // can enable the other flows if you need to support implicit or client credentials.
        options.AllowAuthorizationCodeFlow()
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

        if (!Helpers.Constants.IsTestingEnvironment(environmentName))
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
               .EnableUserinfoEndpointPassthrough();
        // .DisableTransportSecurityRequirement(); // During development, you can disable the HTTPS requirement.

        if (configuration.GetValue("DisableAccessTokenEncryption", false))
        {
          options.DisableAccessTokenEncryption();
        }
      })
      // Register the OpenIddict validation components.
      .AddValidation(options =>
      {
        // Import the configuration from the local OpenIddict server instance.
        options.UseLocalServer();

        // Register the ASP.NET Core host.
        options.UseAspNetCore();
      })
      // Register the EF based UI Store for OpenIddict related entities.
      .AddUIStore(options =>
      {
        options.OpenIddictUIContext = builder =>
        {
          builder.UseSqlite(configuration.GetConnectionString("DefaultConnection"),
            sql =>
            {
              sql.MigrationsAssembly(typeof(Program)
                                    .GetTypeInfo()
                                    .Assembly
                                    .GetName()
                                    .Name);
            });
        };
      })
      // Register the APIs for the EF based UI Store based on OpenIddict.
      .AddUIApis(options =>
      {
        // Tell the system about the allowed Permissions it is built/configured for.
        options.Permissions = new List<string>
        {
          Permissions.Endpoints.Authorization,
          Permissions.Endpoints.Logout,
          Permissions.Endpoints.Token,
          Permissions.Endpoints.Introspection,
          Permissions.GrantTypes.AuthorizationCode,
          Permissions.GrantTypes.DeviceCode,
          Permissions.GrantTypes.RefreshToken,
          Permissions.ResponseTypes.Code,
          Permissions.Scopes.Email,
          Permissions.Scopes.Profile,
          Permissions.Scopes.Roles,
          Permissions.Prefixes.Scope + "server_scope",
          Permissions.Prefixes.Scope + "api_scope"
        };
      })
      // Register the EF based UI Store for the ASP.NET Identity related entities.
      .AddUIIdentityStore<ApplicationUser>(options =>
      {
        options.OpenIddictUIIdentityContext = builder =>
        {
          builder.UseSqlite(configuration.GetConnectionString("DefaultConnection"),
            sql =>
            {
              sql.MigrationsAssembly(typeof(Program)
                                    .GetTypeInfo()
                                    .Assembly
                                    .GetName()
                                    .Name);
            });
        };
      })
      // Register the APIs for the EF based UI Store based on ASP.NET Identity.
      .AddUIIdentityApis<ApplicationUser>();

    if (!Helpers.Constants.IsTestingEnvironment(environmentName))
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
        c.DocInclusionPredicate((name, api) =>
        {
          return true;
        });
        c.TagActionsBy(api =>
        {
          if (api.GroupName != null)
          {
            return new[] { api.GroupName };
          }

          if (api.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
          {
            return new[] { controllerActionDescriptor.ControllerName };
          }

          throw new InvalidOperationException("Unable to determine tag for endpoint.");
        });
        c.ResolveConflictingActions(apiDescriptions =>
        {
          return apiDescriptions.First();
        });
      });
    }

    services.AddScoped<IMigrationService, MigrationService>();

    services.AddMvc();

    return services;
  }
}
