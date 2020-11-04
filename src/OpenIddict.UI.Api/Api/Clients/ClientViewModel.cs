using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace tomware.OpenIddict.UI.Api
{
  public class ClientViewModel
  {
    public int? Id { get; set; }

    [Required]
    public bool Enabled { get; set; }

    [Required]
    public string ClientId { get; set; }

    [Required]
    public string ClientName { get; set; }

    public bool RequireClientSecret { get; set; }

    public string ClientSecret { get; set; }

    public bool RequirePkce { get; set; }

    public bool RequireConsent { get; set; }

    public bool AllowAccessTokensViaBrowser { get; set; }

    [RequireGrantTypeAttribute]
    public List<string> AllowedGrantTypes { get; set; } = new List<string>();

    public List<string> RedirectUris { get; set; } = new List<string>();

    public List<string> PostLogoutRedirectUris { get; set; } = new List<string>();

    public List<string> AllowedCorsOrigins { get; set; } = new List<string>();

    public List<string> AllowedScopes { get; set; } = new List<string>();
  }

  public class RequireGrantTypeAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      var model = (ClientViewModel)validationContext.ObjectInstance;

      if (model.AllowedGrantTypes.Count == 0)
      {
        return new ValidationResult(GetErrorMessage());
      }

      return ValidationResult.Success;
    }

    public string GetErrorMessage()
    {
      return $"Requires at least one grant type!";
    }
  }
}
