using Microsoft.Extensions.Configuration;

namespace tomware.Microip.Web
{
  public interface ITitleService
  {
    string Title { get; }
  }

  public class TitleService : ITitleService
  {
    private readonly IConfiguration configuration;

    public string Title => !string.IsNullOrWhiteSpace(this.configuration["AppTitle"])
      ? this.configuration["AppTitle"]
      : "Micro STS";

    public TitleService(IConfiguration configuration)
    {
      this.configuration = configuration;
    }
  }
}
