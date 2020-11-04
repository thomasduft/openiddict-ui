using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace tomware.OpenIddict.UI.Infrastructure
{
  public class MigrationService : IMigrationService
  {
    private readonly OpenIddictUIContext context;

    public MigrationService(OpenIddictUIContext context)
    {
      this.context = context;
    }

    public async Task EnsureMigrationAsync()
    {
      await this.context.Database.MigrateAsync();
    }
  }
}