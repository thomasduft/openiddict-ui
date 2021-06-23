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

  public sealed class ClaimTypeByName : BaseSpecification<ClaimType>
  {
    public ClaimTypeByName(string name) : base(x => x.Name == name)
    {
      ApplyNoTracking();
    }
  }
}