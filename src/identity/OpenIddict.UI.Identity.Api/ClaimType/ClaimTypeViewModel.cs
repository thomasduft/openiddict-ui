using System;
using System.ComponentModel.DataAnnotations;

namespace tomware.OpenIddict.UI.Identity.Api;

public class ClaimTypeViewModel
{
  public Guid? Id { get; set; }

  [Required(AllowEmptyStrings = false)]
  public string Name { get; set; }

  public string Description { get; set; }

  [Required(AllowEmptyStrings = false)]
  public string ClaimValueType { get; set; }
}
