using System.Collections.Generic;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace tomware.OpenIddict.UI.Api
{
  public class ApplicationOptionsViewModel
  {
    public List<string> Permissions { get; set; } = new List<string>();
    
    public List<string> Requirements { get; set; } = new List<string>();
    
    public List<string> Types { get; set; } = new List<string>{
      ClientTypes.Public,
      ClientTypes.Confidential
    };
  }
}
