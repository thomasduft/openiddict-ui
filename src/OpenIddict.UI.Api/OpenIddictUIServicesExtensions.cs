using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace tomware.OpenIddict.UI.Api
{
  public static class OpenIddictUIServicesExtensions
  {
    /// <summary>
    /// Register the Api for the EF based UI Store.
    /// </summary>
    public static OpenIddictBuilder AddUIApis<TApplicationUser>(
      this OpenIddictBuilder builder,
      OpenIddictUIApiOptions uiApiOptions
    ) where TApplicationUser : IdentityUser, new()
    {
      builder.Services
        .AddOpenIddictUIApiServices<TApplicationUser>(uiApiOptions);

      return builder;
    }

    /// <summary>
    /// Registers the UserName to UserName UserCreationStrategy.
    /// </summary>
    public static OpenIddictBuilder AddUserNameUserCreationStrategy<TApplicationUser>(
      this OpenIddictBuilder builder
    ) where TApplicationUser : IdentityUser, new()
    {
      builder.Services
        .AddTransient<IUserCreationStrategy<TApplicationUser>, UserNameUserCreationStrategy<TApplicationUser>>();

      return builder;
    }

    private static IServiceCollection AddOpenIddictUIApiServices<TApplicationUser>(
      this IServiceCollection services,
      OpenIddictUIApiOptions uiApiOptions
    ) where TApplicationUser : IdentityUser, new()
    {
      services.AddAuthorizationServices();

      services.AddApiServices<TApplicationUser>();

      services.Configure<OpenIddictUIApiOptions>(options => {
        options.Permissions = uiApiOptions.Permissions;
      });

      return services;
    }

    private static IServiceCollection AddAuthorizationServices(
      this IServiceCollection services
    )
    {
      services.AddAuthorization(options =>
      {
        options.AddPolicy(
          Policies.ADMIN_POLICY,
          policy => policy
            .RequireAuthenticatedUser()
            .RequireRole(Roles.ADMINISTRATOR_ROLE)
        );
      });

      return services;
    }

    private static IServiceCollection AddApiServices<TApplicationUser>(
      this IServiceCollection services
    ) where TApplicationUser : IdentityUser, new()
    {
      // Identity services
      services.AddTransient<IUserCreationStrategy<TApplicationUser>, EmailUserCreationStrategy<TApplicationUser>>();
      services.AddTransient<IAccountApiService, AccountApiService<TApplicationUser>>();
      services.AddTransient<IRoleService, RoleService>();
      services.AddTransient<IClaimTypeApiService, ClaimTypeApiService>();

      // OpenIddict services
      services.AddTransient<IScopeApiService, ScopeApiService>();
      services.AddTransient<IApplicationApiService, ApplicationApiService>();

      return services;
    }
  }
}