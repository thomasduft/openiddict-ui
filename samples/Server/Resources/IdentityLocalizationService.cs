using Microsoft.Extensions.Localization;
using System.Reflection;

namespace tomware.Microip.Web.Resources
{
  public class IdentityLocalizationService
  {
    private readonly IStringLocalizer localizer;

    public IdentityLocalizationService(IStringLocalizerFactory factory)
    {
      var type = typeof(IdentityResource);
      var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);

      localizer = factory.Create(nameof(IdentityResource), assemblyName.Name);
    }

    public LocalizedString GetLocalizedHtmlString(string key)
    {
      return localizer[key];
    }

    public LocalizedString GetLocalizedHtmlString(string key, string parameter)
    {
      return localizer[key, parameter];
    }
  }
}