using System.Threading.Tasks;

namespace tomware.OpenIddict.UI.Infrastructure
{
  public interface IMigrationService
  {
    Task EnsureMigrationAsync();
  }
}