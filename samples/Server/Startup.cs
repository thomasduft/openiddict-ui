using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using tomware.Microip.Web.Resources;
using tomware.OpenIddict.UI.Api;
using tomware.OpenIddict.UI.Infrastructure;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace tomware.Microip.Web
{
  public class Startup
  {
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
      this.Configuration = configuration;
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
            .AllowCredentials()
            .WithExposedHeaders("X-Pagination");
        });
      });

      services.AddRouting(o => o.LowercaseUrls = true);

      // services.Configure<CookiePolicyOptions>(options =>
      // {
      //   options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
      //   options.OnAppendCookie = cookieContext =>
      //     CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
      //   options.OnDeleteCookie = cookieContext =>
      //     CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
      // });

      // var cookieSecurePolicy = CookieSecurePolicy.SameAsRequest; // CookieSecurePolicy.Always;
      // services.AddAntiforgery(options =>
      // {
      //   options.SuppressXFrameOptionsHeader = true;
      //   options.Cookie.SameSite = SameSiteMode.Strict;
      //   options.Cookie.SecurePolicy = cookieSecurePolicy;
      // });

      // services.AddSession(options =>
      // {
      //   options.IdleTimeout = TimeSpan.FromMinutes(2);
      //   options.Cookie.HttpOnly = true;
      //   options.Cookie.SameSite = SameSiteMode.None;
      //   options.Cookie.SecurePolicy = cookieSecurePolicy;
      // });

      services.AddDbContext<STSContext>(options =>
        {
          options.UseSqlite(this.Configuration["ConnectionStrings:Application"]);
        });

      services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
          options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<STSContext>()
        .AddDefaultTokenProviders();

      services.Configure<IdentityOptions>(options =>
        {
          // Password settings.I
          options.Password.RequireDigit = false;
          options.Password.RequiredLength = 5;
          options.Password.RequireNonAlphanumeric = false;
          options.Password.RequireUppercase = false;
          options.Password.RequireLowercase = false;

          options.ClaimsIdentity.UserNameClaimType = Claims.Name;
          options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
          options.ClaimsIdentity.RoleClaimType = Claims.Role;

          options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
          options.Lockout.MaxFailedAccessAttempts = 5;
          options.Lockout.AllowedForNewUsers = true;
        });

      // STS Services
      services.AddSTSServices(); // TODO: combine with above

      // ---------------------------------------------------------------------------------------- //

      var authority = !string.IsNullOrWhiteSpace(Configuration["AuthorityForDocker"])
        ? Configuration["AuthorityForDocker"]
        : Program.GetUrls(Configuration);

      services.AddOpenIddict()
        // Register the OpenIddict core components.
        .AddCore(options =>
        {
          // Configure OpenIddict to use the Entity Framework Core stores and models.
          // Note: call ReplaceDefaultEntities() to replace the default OpenIddict entities.
          options.UseEntityFrameworkCore();
        })
        // Register the OpenIddict server components.
        .AddServer(options =>
        {
          options.SetIssuer(new Uri(authority));

          // Enable the authorization, logout, token and userinfo endpoints.
          options.SetAuthorizationEndpointUris("/connect/authorize")
                 .SetLogoutEndpointUris("/connect/logout")
                 .SetTokenEndpointUris("/connect/token")
                 .SetUserinfoEndpointUris("/connect/userinfo");

          // Mark the "email", "profile" and "roles" scopes as supported scopes.
          options.RegisterScopes(
            Scopes.Email,
            Scopes.Profile,
            Scopes.OpenId,
            Scopes.Roles,
            Constants.STS_API);

          // Note: this sample only uses the authorization code flow but you can enable
          // the other flows if you need to support implicit, password or client credentials.
          options.AllowAuthorizationCodeFlow()
                 .AllowRefreshTokenFlow();

          // Register the signing and encryption credentials.
          options.AddDevelopmentEncryptionCertificate()
                 .AddDevelopmentSigningCertificate();

          options.RequireProofKeyForCodeExchange();

          // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
          options.UseAspNetCore()
                 .EnableAuthorizationEndpointPassthrough()
                 .EnableLogoutEndpointPassthrough()
                 .EnableTokenEndpointPassthrough()
                 .EnableUserinfoEndpointPassthrough()
                 .EnableStatusCodePagesIntegration();

          // options.DisableHttpsRequirement(); ID2083
          // options.EnableDegradedMode();
        })
        // Register the OpenIddict validation components.
        .AddValidation(options =>
        {
          // Configure the audience accepted by this resource server.
          // The value MUST match the audience associated with the
          // "demo_api" scope, which is used by ResourceController
          // options.AddAudiences(Constants.STS_API);

          // Import the configuration from the local OpenIddict server instance.
          options.UseLocalServer();

          // Register the ASP.NET Core host.
          options.UseAspNetCore();
        })
        // Register the EF based UI Store
        .AddUIStore(options =>
        {
          options.OpenIddictUIContext = builder =>
              builder.UseSqlite(this.Configuration["ConnectionStrings:Application"],
                  sql => sql.MigrationsAssembly(typeof(Startup)
                    .GetTypeInfo()
                    .Assembly
                    .GetName()
                    .Name));
        })
        // Register the Api for the EF based UI Store
        .AddUIApis<ApplicationUser>();

      // ---------------------------------------------------------------------------------------- //

      // localization
      services.AddSingleton<IdentityLocalizationService>();
      services.AddSingleton<SharedLocalizationService>();

      services.AddLocalization(options => options.ResourcesPath = "Resources");
      services.Configure<RequestLocalizationOptions>(options =>
      {
        var supportedCultures = new List<CultureInfo>
          {
          new CultureInfo("en-US"),
          new CultureInfo("de-CH")
        };
        options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
        options.SupportedCultures = supportedCultures;
        options.SupportedUICultures = supportedCultures;
        options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
      });

      // Swagger
      services.AddSwaggerDocumentation();

      // Allow razor pages
      services.AddControllers()
        .AddNewtonsoftJson()
        .SetCompatibilityVersion(CompatibilityVersion.Latest);

      var builder = services.AddRazorPages()
        .SetCompatibilityVersion(CompatibilityVersion.Latest)
        .AddViewLocalization()
        .AddDataAnnotationsLocalization(options =>
        {
          options.DataAnnotationLocalizerProvider = (type, factory) =>
          {
            var name = new AssemblyName(typeof(IdentityResource).GetTypeInfo().Assembly.FullName);
            return factory.Create(nameof(IdentityResource), name.Name);
          };
        });

      var allowSelfRegister = this.Configuration.GetValue<bool>("AllowSelfRegister");
      if (!allowSelfRegister)
      {
        builder.AddRazorPagesOptions(options =>
        {
          options.Conventions.AuthorizeAreaPage(
            "identity",
            "/account/register",
            "AdministratorPolicy"
          );
        });
      }
    }

    public void Configure(
      IApplicationBuilder app,
      IWebHostEnvironment env
    )
    {
      if (env.IsDevelopment())
      {
        app.UseCors("AllowAllOrigins");
        app.UseSwaggerDocumentation();

        IdentityModelEventSource.ShowPII = true;

        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }

      // app.UseMiddleware<SecurityHeadersMiddleware>();

      // ConsiderSpaRoutes(app);

      var locOptions = app.ApplicationServices
        .GetService<IOptions<RequestLocalizationOptions>>();
      app.UseRequestLocalization(locOptions.Value);

      app.UseDefaultFiles();
      app.UseStaticFiles();

      app.UseSerilogRequestLogging();

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapRazorPages();
        endpoints.MapDefaultControllerRoute();
      });
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

    private static void CheckSameSite(HttpContext httpContext, CookieOptions options)
    {
      if (options.SameSite == SameSiteMode.None)
      {
        var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
        if (DisallowsSameSiteNone(userAgent))
        {
          // For .NET Core < 3.1 set SameSite = (SameSiteMode)(-1)
          options.SameSite = SameSiteMode.Unspecified;
        }
      }
    }

    private static bool DisallowsSameSiteNone(string userAgent)
    {
      // Cover all iOS based browsers here. This includes:
      // - Safari on iOS 12 for iPhone, iPod Touch, iPad
      // - WkWebview on iOS 12 for iPhone, iPod Touch, iPad
      // - Chrome on iOS 12 for iPhone, iPod Touch, iPad
      // All of which are broken by SameSite=None, because they use the iOS networking stack
      if (userAgent.Contains("CPU iPhone OS 12") || userAgent.Contains("iPad; CPU OS 12"))
      {
        return true;
      }

      // Cover Mac OS X based browsers that use the Mac OS networking stack. This includes:
      // - Safari on Mac OS X.
      // This does not include:
      // - Chrome on Mac OS X
      // Because they do not use the Mac OS networking stack.
      if (userAgent.Contains("Macintosh; Intel Mac OS X 10_14") &&
          userAgent.Contains("Version/") && userAgent.Contains("Safari"))
      {
        return true;
      }

      // Cover Chrome 50-69, because some versions are broken by SameSite=None,
      // and none in this range require it.
      // Note: this covers some pre-Chromium Edge versions,
      // but pre-Chromium Edge does not require SameSite=None.
      if (userAgent.Contains("Chrome/5") || userAgent.Contains("Chrome/6"))
      {
        return true;
      }

      return false;
    }

    private static void ConsiderSpaRoutes(IApplicationBuilder app)
    {
      var angularRoutes = new[]
      {
        "/home",
        "/forbidden",
        "/claimtypes",
        "/roles",
        "/users",
        "/users/register",
        "/scopes",
        "/clients"
      };

      app.Use(async (context, next) =>
      {
        if (context.Request.Path.HasValue
          && null != angularRoutes.FirstOrDefault(
            (ar) => context.Request.Path.Value.StartsWith(ar, StringComparison.OrdinalIgnoreCase)))
        {
          context.Request.Path = new PathString("/");
        }

        await next();
      });
    }
  }
}
