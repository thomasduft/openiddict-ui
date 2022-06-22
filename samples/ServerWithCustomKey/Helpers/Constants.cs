namespace ServerWithCustomKey.Helpers;

public static class Constants
{
  public const string AdminMailAddress = "admin@openiddict.com";
  public const string DevelopmentEnvironment = "Development";
  public const string TestingEnvironment = "Testing";

  public static bool IsDevelopmentEnvironment(string environmentName) => environmentName == DevelopmentEnvironment;

  public static bool IsTestingEnvironment(string environmentName) => environmentName == TestingEnvironment;
}
