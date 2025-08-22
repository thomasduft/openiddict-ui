using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Server.Helpers;
using Server.Models;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Server;

[ApiExplorerSettings(GroupName = "sample-server")]
public class AuthorizationController : Controller
{
  private readonly IOpenIddictApplicationManager _applicationManager;
  private readonly IOpenIddictAuthorizationManager _authorizationManager;
  private readonly IOpenIddictScopeManager _scopeManager;
  private readonly SignInManager<ApplicationUser> _signInManager;
  private readonly UserManager<ApplicationUser> _userManager;

  public AuthorizationController(
      IOpenIddictApplicationManager applicationManager,
      IOpenIddictAuthorizationManager authorizationManager,
      IOpenIddictScopeManager scopeManager,
      SignInManager<ApplicationUser> signInManager,
      UserManager<ApplicationUser> userManager)
  {
    _applicationManager = applicationManager;
    _authorizationManager = authorizationManager;
    _scopeManager = scopeManager;
    _signInManager = signInManager;
    _userManager = userManager;
  }

  [IgnoreAntiforgeryToken]
  [HttpGet("~/connect/authorize")]
  [HttpPost("~/connect/authorize")]
  public async Task<IActionResult> Authorize()
  {
    var request = HttpContext.GetOpenIddictServerRequest() ??
        throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

    // Retrieve the user principal stored in the authentication cookie.
    // If it can't be extracted, redirect the user to the login page.
    var result = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
    if (result is null || !result.Succeeded)
    {
      // If the client application requested promptless authentication,
      // return an error indicating that the user is not logged in.
      if (request.HasPromptValue(PromptValues.None))
      {
        return Forbid(
            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            properties: new AuthenticationProperties(new Dictionary<string, string>
            {
              [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.LoginRequired,
              [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is not logged in."
            }));
      }

      return Challenge(
          authenticationSchemes: IdentityConstants.ApplicationScheme,
          properties: new AuthenticationProperties
          {
            RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                  Request.HasFormContentType ? Request.Form.ToList() : [.. Request.Query])
          });
    }

    // If prompt=login was specified by the client application,
    // immediately return the user agent to the login page.
    if (request.HasPromptValue(PromptValues.Login))
    {
      // To avoid endless login -> authorization redirects, the prompt=login flag
      // is removed from the authorization request payload before redirecting the user.
      var prompt = string.Join(" ", request.GetPromptValues().Remove(PromptValues.Login));

      var parameters = Request.HasFormContentType ?
          Request.Form.Where(parameter =>
          {
            return parameter.Key != Parameters.Prompt;
          }).ToList() :
          Request.Query.Where(parameter =>
          {
            return parameter.Key != Parameters.Prompt;
          }).ToList();

      parameters.Add(KeyValuePair.Create(Parameters.Prompt, new StringValues(prompt)));

      return Challenge(
          authenticationSchemes: IdentityConstants.ApplicationScheme,
          properties: new AuthenticationProperties
          {
            RedirectUri = Request.PathBase + Request.Path + QueryString.Create(parameters)
          });
    }

    // If a max_age parameter was provided, ensure that the cookie is not too old.
    // If it's too old, automatically redirect the user agent to the login page.
    if (request.MaxAge is not null && result.Properties?.IssuedUtc is not null &&
        DateTimeOffset.UtcNow - result.Properties.IssuedUtc > TimeSpan.FromSeconds(request.MaxAge.Value))
    {
      if (request.HasPromptValue(PromptValues.None))
      {
        return Forbid(
            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            properties: new AuthenticationProperties(new Dictionary<string, string>
            {
              [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.LoginRequired,
              [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is not logged in."
            }));
      }

      return Challenge(
          authenticationSchemes: IdentityConstants.ApplicationScheme,
          properties: new AuthenticationProperties
          {
            RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                  Request.HasFormContentType ? Request.Form.ToList() : [.. Request.Query])
          });
    }

    // Retrieve the profile of the logged in user.
    var user = await _userManager.GetUserAsync(result.Principal) ??
        throw new InvalidOperationException("The user details cannot be retrieved.");

    // Retrieve the application details from the database.
    var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
        throw new InvalidOperationException("Details concerning the calling client application cannot be found.");

    // Retrieve the permanent authorizations associated with the user and the calling client application.
    var authorizations = await _authorizationManager.FindAsync(
        subject: await _userManager.GetUserIdAsync(user),
        client: await _applicationManager.GetIdAsync(application),
        status: Statuses.Valid,
        type: AuthorizationTypes.Permanent,
        scopes: request.GetScopes()).ToListAsync();

    switch (await _applicationManager.GetConsentTypeAsync(application))
    {
      // If the consent is external (e.g when authorizations are granted by a sysadmin),
      // immediately return an error if no authorization can be found in the database.
      case ConsentTypes.External when authorizations.Count == 0:
        return Forbid(
            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            properties: new AuthenticationProperties(new Dictionary<string, string>
            {
              [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
              [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                    "The logged in user is not allowed to access this client application."
            }));

      // If the consent is implicit or if an authorization was found,
      // return an authorization response without displaying the consent form.
      case ConsentTypes.Implicit:
      case ConsentTypes.External when authorizations.Count != 0:
      case ConsentTypes.Explicit when authorizations.Count != 0 && !request.HasPromptValue(PromptValues.Consent):
        var principal = await _signInManager.CreateUserPrincipalAsync(user);

        // Note: in this sample, the granted scopes match the requested scope
        // but you may want to allow the user to uncheck specific scopes.
        // For that, simply restrict the list of scopes before calling SetScopes.
        principal.SetScopes(request.GetScopes());
        principal.SetResources(await _scopeManager.ListResourcesAsync(principal.GetScopes()).ToListAsync());

        // Automatically create a permanent authorization to avoid requiring explicit consent
        // for future authorization or token requests containing the same scopes.
        var authorization = authorizations.LastOrDefault();
        authorization ??= await _authorizationManager.CreateAsync(
              principal: principal,
              subject: await _userManager.GetUserIdAsync(user),
              client: await _applicationManager.GetIdAsync(application),
              type: AuthorizationTypes.Permanent,
              scopes: principal.GetScopes());

        principal.SetAuthorizationId(await _authorizationManager.GetIdAsync(authorization));

        foreach (var claim in principal.Claims)
        {
          claim.SetDestinations(GetDestinations(claim, principal));
        }

        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

      // At this point, no authorization was found in the database and an error must be returned
      // if the client application specified prompt=none in the authorization request.
      case ConsentTypes.Explicit when request.HasPromptValue(PromptValues.None):
      case ConsentTypes.Systematic when request.HasPromptValue(PromptValues.None):
        return Forbid(
            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            properties: new AuthenticationProperties(new Dictionary<string, string>
            {
              [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
              [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                    "Interactive user consent is required."
            }));

      // In every other case, render the consent form.
      default:

        // TODO: sample at the moment comes without any consent page...
        throw new NotImplementedException("Consent screen not yet implemented!");

        // return View(new AuthorizeViewModel
        // {
        //   ApplicationName = await _applicationManager.GetLocalizedDisplayNameAsync(application),
        //   Scope = request.Scope
        // });
    }
  }

  [Authorize]
  [ValidateAntiForgeryToken]
  [HttpPost("~/connect/authorize")]
  [FormValueRequired("submit.Accept")]
  public async Task<IActionResult> Accept()
  {
    var request = HttpContext.GetOpenIddictServerRequest() ??
        throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

    // Retrieve the profile of the logged in user.
    var user = await _userManager.GetUserAsync(User) ??
        throw new InvalidOperationException("The user details cannot be retrieved.");

    // Retrieve the application details from the database.
    var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
        throw new InvalidOperationException("Details concerning the calling client application cannot be found.");

    // Retrieve the permanent authorizations associated with the user and the calling client application.
    var authorizations = await _authorizationManager.FindAsync(
        subject: await _userManager.GetUserIdAsync(user),
        client: await _applicationManager.GetIdAsync(application),
        status: Statuses.Valid,
        type: AuthorizationTypes.Permanent,
        scopes: request.GetScopes()).ToListAsync();

    // Note: the same check is already made in the other action but is repeated
    // here to ensure a malicious user can't abuse this POST-only endpoint and
    // force it to return a valid response without the external authorization.
    if (authorizations.Count == 0 && await _applicationManager.HasConsentTypeAsync(application, ConsentTypes.External))
    {
      return Forbid(
          authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
          properties: new AuthenticationProperties(new Dictionary<string, string>
          {
            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                  "The logged in user is not allowed to access this client application."
          }));
    }

    var principal = await _signInManager.CreateUserPrincipalAsync(user);

    // Note: in this sample, the granted scopes match the requested scope
    // but you may want to allow the user to uncheck specific scopes.
    // For that, simply restrict the list of scopes before calling SetScopes.
    principal.SetScopes(request.GetScopes());
    principal.SetResources(await _scopeManager.ListResourcesAsync(principal.GetScopes()).ToListAsync());

    // Automatically create a permanent authorization to avoid requiring explicit consent
    // for future authorization or token requests containing the same scopes.
    var authorization = authorizations.LastOrDefault();
    authorization ??= await _authorizationManager.CreateAsync(
          principal: principal,
          subject: await _userManager.GetUserIdAsync(user),
          client: await _applicationManager.GetIdAsync(application),
          type: AuthorizationTypes.Permanent,
          scopes: principal.GetScopes());

    principal.SetAuthorizationId(await _authorizationManager.GetIdAsync(authorization));

    foreach (var claim in principal.Claims)
    {
      claim.SetDestinations(GetDestinations(claim, principal));
    }

    // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
    return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
  }

  [Authorize]
  [ValidateAntiForgeryToken]
  [HttpPost("~/connect/authorize")]
  [FormValueRequired("submit.Deny")]
  public IActionResult Deny() => Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

  [HttpGet("~/connect/logout")]
  public IActionResult Logout()
  {
    return RedirectToPage("/Account/Logout", new
    {
      logoutId = Request.Query["id_token_hint"],
      redirectUri = Request.Query["post_logout_redirect_uri"]
    });
  }

  [ValidateAntiForgeryToken]
  [ActionName(nameof(Logout))]
  [HttpPost("~/connect/logout")]
  public async Task<IActionResult> LogoutPost()
  {
    // Ask ASP.NET Core Identity to delete the local and external cookies created
    // when the user agent is redirected from the external identity provider
    // after a successful authentication flow (e.g Google or Facebook).
    await _signInManager.SignOutAsync();

    // Returning a SignOutResult will ask OpenIddict to redirect the user agent
    // to the post_logout_redirect_uri specified by the client application or to
    // the RedirectUri specified in the authentication properties if none was set.
    return SignOut(
        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
        properties: new AuthenticationProperties
        {
          RedirectUri = "/"
        });
  }

  [Produces("application/json")]
  [HttpPost("~/connect/token")]
  public async Task<IActionResult> Exchange()
  {
    var request = HttpContext.GetOpenIddictServerRequest() ??
        throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

    if (request.IsPasswordGrantType())
    {
      var user = await _userManager.FindByNameAsync(request.Username);
      if (user is null)
      {
        return Forbid(
            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            properties: new AuthenticationProperties(new Dictionary<string, string>
            {
              [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
              [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The username/password couple is invalid."
            }));
      }

      // Validate the username/password parameters and ensure the account is not locked out.
      var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
      if (!result.Succeeded)
      {
        return Forbid(
            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            properties: new AuthenticationProperties(new Dictionary<string, string>
            {
              [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
              [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The username/password couple is invalid."
            }));
      }

      var principal = await _signInManager.CreateUserPrincipalAsync(user);

      // Note: in this sample, the granted scopes match the requested scope
      // but you may want to allow the user to uncheck specific scopes.
      // For that, simply restrict the list of scopes before calling SetScopes.
      principal.SetScopes(request.GetScopes());
      principal.SetResources(await _scopeManager.ListResourcesAsync(principal.GetScopes()).ToListAsync());

      foreach (var claim in principal.Claims)
      {
        claim.SetDestinations(GetDestinations(claim, principal));
      }

      // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
      return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    else if (request.IsClientCredentialsGrantType())
    {
      // Note: the client credentials are automatically validated by OpenIddict:
      // if client_id or client_secret are invalid, this action won't be invoked.

      var application = await _applicationManager.FindByClientIdAsync(request.ClientId!)
        ?? throw new InvalidOperationException("The application details cannot be found in the database.");

      // Create the claims-based identity that will be used by OpenIddict to generate tokens.
      var identity = new ClaimsIdentity(
          authenticationType: TokenValidationParameters.DefaultAuthenticationType,
          nameType: Claims.Name,
          roleType: Claims.Role
      );

      // Add the claims that will be persisted in the tokens (use the client_id as the subject identifier).
      identity.SetClaim(Claims.Subject, await _applicationManager.GetClientIdAsync(application));
      identity.SetClaim(Claims.Name, await _applicationManager.GetDisplayNameAsync(application));

      // Note: In the original OAuth 2.0 specification, the client credentials grant
      // doesn't return an identity token, which is an OpenID Connect concept.
      //
      // As a non-standardized extension, OpenIddict allows returning an id_token
      // to convey information about the client application when the "openid" scope
      // is granted (i.e specified when calling principal.SetScopes()). When the "openid"
      // scope is not explicitly set, no identity token is returned to the client application.

      // Set the list of scopes granted to the client application in access_token.
      identity.SetScopes(request.GetScopes());
      identity.SetResources(await _scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());
      identity.SetDestinations(GetDestinations);

      return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    else if (request.IsAuthorizationCodeGrantType()
      || request.IsDeviceCodeGrantType()
      || request.IsRefreshTokenGrantType())
    {
      // Retrieve the claims principal stored in the authorization code/device code/refresh token.
      var principal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;

      // Retrieve the user profile corresponding to the authorization code/refresh token.
      // Note: if you want to automatically invalidate the authorization code/refresh token
      // when the user password/roles change, use the following line instead:
      // var user = _signInManager.ValidateSecurityStampAsync(info.Principal);
      var user = await _userManager.GetUserAsync(principal);
      if (user is null)
      {
        return Forbid(
            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            properties: new AuthenticationProperties(new Dictionary<string, string>
            {
              [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
              [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The token is no longer valid."
            }));
      }

      // Ensure the user is still allowed to sign in.
      if (!await _signInManager.CanSignInAsync(user))
      {
        return Forbid(
            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            properties: new AuthenticationProperties(new Dictionary<string, string>
            {
              [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
              [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is no longer allowed to sign in."
            }));
      }

      foreach (var claim in principal.Claims)
      {
        claim.SetDestinations(GetDestinations(claim, principal));
      }

      // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
      return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    throw new InvalidOperationException("The specified grant type is not supported.");
  }

  private static IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
  {
    // Note: by default, claims are NOT automatically included in the access and identity tokens.
    // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
    // whether they should be included in access tokens, in identity tokens or in both.

    switch (claim.Type)
    {
      case Claims.Name:
        yield return Destinations.AccessToken;

        if (principal.HasScope(Scopes.Profile))
        {
          yield return Destinations.IdentityToken;
        }

        yield break;

      case Claims.Email:
        yield return Destinations.AccessToken;

        if (principal.HasScope(Scopes.Email))
        {
          yield return Destinations.IdentityToken;
        }

        yield break;

      case Claims.Role:
        yield return Destinations.AccessToken;

        if (principal.HasScope(Scopes.Roles))
        {
          yield return Destinations.IdentityToken;
        }

        yield break;

      // Never include the security stamp in the access and identity tokens, as it's a secret value.
      case "AspNet.Identity.SecurityStamp":
        yield break;

      default:
        yield return Destinations.AccessToken;
        yield break;
    }
  }

  private static IEnumerable<string> GetDestinations(Claim claim)
  {
    // Note: by default, claims are NOT automatically included in the access and identity tokens.
    // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
    // whether they should be included in access tokens, in identity tokens or in both.

    return claim.Type switch
    {
      Claims.Name or Claims.Subject => [Destinations.AccessToken, Destinations.IdentityToken],

      _ => [Destinations.AccessToken],
    };
  }
}
