using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace tomware.OpenIddict.UI.Tests;

public static class HttpExtensions
{
  private static readonly JsonSerializerOptions JsonSerializerOptions = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
  };

  public static async Task<T> ReadAsJsonAsync<T>(this HttpContent httpContent)
  {
    var obj = await httpContent.ReadAsStringAsync();

    return JsonSerializer.Deserialize<T>(obj, JsonSerializerOptions);
  }
}
