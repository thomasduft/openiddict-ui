using System;

namespace tomware.OpenIddict.UI.Identity.Core;

public class ClaimType
{
  public Guid Id { get; set; }

  public string Name { get; set; }

  public string Description { get; set; }

  public string ClaimValueType { get; set; }

  public static ClaimType Create(
    string name,
    string claimValueType,
    string description = null
  )
  {
    return new ClaimType
    {
      Id = Guid.NewGuid(),
      Name = name,
      ClaimValueType = claimValueType,
      Description = description,
    };
  }
}
