using System;

namespace tomware.OpenIddict.UI.Core
{
  public class ClaimType
  {
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public static ClaimType Create(
      string name,
      string description = null
    )
    {
      return new ClaimType
      {
        Id = Guid.NewGuid(),
        Name = name,
        Description = description
      };
    }
  }
}