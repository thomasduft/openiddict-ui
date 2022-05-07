namespace ServerWithCustomKey.Helpers
{
  public static class Constants
  {
    public const string ADMIN_MAILADDRESS = "admin@openiddict.com";
    public const string DEVELOPMENT_ENVIRONMENT = "Development";
    public const string TESTING_ENVIRONMENT = "Testing";

    public static bool IsDevelopmentEnvironment(string environmentName)
    {
      return environmentName == DEVELOPMENT_ENVIRONMENT;
    }

    public static bool IsTestingEnvironment(string environmentName)
    {
      return environmentName == TESTING_ENVIRONMENT;
    }
  }

  public static class Roles
  {
    public const string ADMINISTRATOR_ROLE = "Administrator";
  }
}