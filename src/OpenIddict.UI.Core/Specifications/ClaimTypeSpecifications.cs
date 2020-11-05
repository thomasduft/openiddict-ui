namespace tomware.OpenIddict.UI.Core
{
  public sealed class AllClaimTypes : BaseSpecification<ClaimType>
  {
    public AllClaimTypes()
    {
      ApplyOrderBy(x => x.Name);
      ApplyNoTracking();
    }
  }
}