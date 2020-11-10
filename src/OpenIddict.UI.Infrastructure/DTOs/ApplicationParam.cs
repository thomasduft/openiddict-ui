namespace tomware.OpenIddict.UI.Infrastructure
{
  public class ApplicationParam
  {
    public string Id { get; set; }
    public string ClientId { get; set; }
    public string DisplayName { get; set; }
    public string ClientSecret { get; set; }
    public string ConsentType { get; set; }
    public string Permissions { get; set; }
    public string Properties { get; set; }
    public string RedirectUris { get; set; }
    public string PostLogoutRedirectUris { get; set; }
    public string Requirements { get; set; }
    public string Type { get; set; }
  }
}