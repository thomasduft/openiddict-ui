namespace tomware.OpenIddict.UI.Identity.Core
{
  public class ChangePasswordParam
  {
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string UserName { get; set; }
  }
}