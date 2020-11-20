using System;

namespace tomware.OpenIddict.UI.Core
{
  public class ClaimTypeInfo
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ClaimValueType { get; set; }
  }
}