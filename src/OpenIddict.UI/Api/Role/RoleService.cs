using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace tomware.OpenIddict.UI
{
  public interface IRoleService
  {
    Task<IEnumerable<RoleViewModel>> GetRolesAsync();

    Task<RoleViewModel> GetAsync(string id);

    Task<string> CreateAsync(RoleViewModel model);

    Task UpdateAsync(RoleViewModel model);

    Task DeleteAsync(string id);
  }

  public class RoleService : IRoleService
  {
    private readonly RoleManager<IdentityRole> manager;

    public RoleService(RoleManager<IdentityRole> manager)
    {
      this.manager = manager;
    }

    public async Task<IEnumerable<RoleViewModel>> GetRolesAsync()
    {
      var items = this.manager.Roles
        .OrderBy(r => r.Name)
        .ToList();

      var models = items.Select(c => ToModel(c));

      return await Task.FromResult(models);
    }

    public async Task<RoleViewModel> GetAsync(string id)
    {
      if (id == null) throw new ArgumentNullException(nameof(id));

      var role = await this.manager.FindByIdAsync(id);

      return role != null ? ToModel(role) : null;
    }

    public async Task<string> CreateAsync(RoleViewModel model)
    {
      if (model == null) throw new ArgumentNullException(nameof(model));

      var newRole = new IdentityRole(model.Name);
      if (!await this.manager.RoleExistsAsync(model.Name))
      {
        await this.manager.CreateAsync(newRole);
      }

      return newRole.Id;
    }

    public async Task UpdateAsync(RoleViewModel model)
    {
      if (model == null) throw new ArgumentNullException(nameof(model));
      if (string.IsNullOrWhiteSpace(model.Id)) throw new ArgumentNullException(nameof(model.Id));

      var role = await this.manager.FindByIdAsync(model.Id);
      role.Name = model.Name;

      await this.manager.UpdateAsync(role);
    }

    public async Task DeleteAsync(string id)
    {
      if (id == null) throw new ArgumentNullException(nameof(id));

      var role = await this.manager.FindByIdAsync(id);

      await this.manager.DeleteAsync(role);
    }

    private RoleViewModel ToModel(IdentityRole entity)
    {
      return new RoleViewModel
      {
        Id = entity.Id,
        Name = entity.Name
      };
    }
  }
}