using System;
using System.ComponentModel.DataAnnotations;

namespace tomware.OpenIddict.UI.Api
{
  public class ClaimTypeViewModel
  {
    public Guid? Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string Description { get; set; }
  }
}
