using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace tomware.Microip.Web
{
  public class ProfileService : IProfileService
  {
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory;

    public ProfileService(
      UserManager<ApplicationUser> userManager,
      IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory
    )
    {
      this.userManager = userManager;
      this.claimsFactory = claimsFactory;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
      var sub = context.Subject?.GetSubjectId();
      if (sub == null) throw new Exception("No sub claim present");

      var user = await this.userManager.FindByIdAsync(sub);
      if (user != null)
      {
        var principal = await this.claimsFactory.CreateAsync(user);
        var claims = principal.Claims.ToList();
        claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();
        claims.Add(new Claim(JwtClaimTypes.GivenName, user.UserName));

        var userRoles = await this.userManager.GetRolesAsync(user);
        if (userRoles.Count > 0)
        {
          claims.AddRange(userRoles.Select(r => new Claim(JwtClaimTypes.Role, r)));
        }
        var userClaims = await this.userManager.GetClaimsAsync(user);
        if (userClaims.Count > 0)
        {
          claims.AddRange(userClaims);
        }

        claims.Add(new Claim(IdentityServerConstants.StandardScopes.Email, user.Email));

        context.IssuedClaims = claims;
      }
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
      var sub = context.Subject?.GetSubjectId();
      if (sub == null) throw new Exception("No subject Id claim present");

      var user = await this.userManager.FindByIdAsync(sub);

      context.IsActive = user != null;
    }
  }
}