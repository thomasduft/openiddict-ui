using System.Collections.Generic;

namespace tomware.OpenIddict.UI.Infrastructure;

public class ScopeParam
{
  public string Id { get; set; }
  public string Name { get; set; }
  public string DisplayName { get; set; }
  public string Description { get; set; }
  public List<string> Resources { get; set; } = new List<string>();
}
