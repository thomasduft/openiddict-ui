using tomware.OpenIddict.UI.Suite.Core;

namespace tomware.OpenIddict.UI.Identity.Core
{
  public sealed class AllClaimTypes : BaseSpecification<ClaimType>
  {
    public AllClaimTypes()
    {
      ApplyOrderBy(x => x.Name);
      ApplyNoTracking();
    }
  }

  public sealed class ClaimTypeByName : BaseSpecification<ClaimType>
  {
    public ClaimTypeByName(string name) : base(x => x.Name == name)
    {
      ApplyNoTracking();
    }
  }
}