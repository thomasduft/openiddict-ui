namespace Server.Helpers
{
  public static class Constants
  {
    public const string ADMIN_MAILADDRESS = "admin@openiddict.com";

    public const string TESTING_ENVIRONMENT = "Testing";

    public static bool IsTestingEnvironment(string environmentName)
    {
      return environmentName == Server.Helpers.Constants.TESTING_ENVIRONMENT;
    }
  }

  public static class Roles
  {
    public const string ADMINISTRATOR_ROLE = "Administrator";
  }
}