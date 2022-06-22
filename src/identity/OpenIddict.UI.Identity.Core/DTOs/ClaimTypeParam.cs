using System;

namespace tomware.OpenIddict.UI.Identity.Core
{
  public class ClaimTypeParam
  {
    public Guid? Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ClaimValueType { get; set; }
  }
}