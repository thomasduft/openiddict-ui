using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace tomware.OpenIddict.UI.Api;

public class RequiredIfClientTypeIsAttribute : ValidationAttribute
{
  private readonly string _clientType;

  public RequiredIfClientTypeIsAttribute(string clientType)
  {
    _clientType = clientType;
  }

  protected override ValidationResult IsValid(object value, ValidationContext validationContext)
  {
    var clientTypePropertyName = nameof(ApplicationViewModel.Type);
    var dependentProperty = validationContext.ObjectType
      .GetProperty(clientTypePropertyName);

    var dependentPropertyValue = dependentProperty
      .GetValue(validationContext.ObjectInstance, null) as string;
    var stringValue = (string)value;
    if (dependentPropertyValue != _clientType)
    {
      return ValidationResult.Success;
    }

    return !string.IsNullOrWhiteSpace(stringValue)
      ? ValidationResult.Success
      : new ValidationResult(string.Format(
          CultureInfo.CurrentCulture,
          "{0} required for confidential applications",
          validationContext.DisplayName
        ));
  }
}
