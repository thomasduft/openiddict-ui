using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace tomware.OpenIddict.UI.Tests
{
  public static class HttpExtensions
  {
    public async static Task<T> ReadAsJson<T>(this HttpContent httpContent)
    {
      var obj = await httpContent.ReadAsStringAsync();

      return JsonSerializer.Deserialize<T>(obj, new JsonSerializerOptions
      {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
      });
    }
  }
}