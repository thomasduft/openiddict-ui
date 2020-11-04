namespace tomware.OpenIddict.UI.Core
{
  public sealed class AllClaimTypes : BaseSpecification<ClaimType>
  {
    public AllClaimTypes() : base()
    {
      this.ApplyOrderBy(x => x.Name);
      this.ApplyNoTracking();
    }
  }
}