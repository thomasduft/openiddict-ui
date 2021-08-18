using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tomware.OpenIddict.UI.Identity.Api
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
    private readonly RoleManager<IdentityRole> _manager;

    public RoleService(RoleManager<IdentityRole> manager)
    {
      _manager = manager;
    }

    public async Task<IEnumerable<RoleViewModel>> GetRolesAsync()
    {
      var items = _manager.Roles
        .OrderBy(r => r.Name)
        .ToList();

      var models = items.Select(c => ToModel(c));

      return await Task.FromResult(models);
    }

    public async Task<RoleViewModel> GetAsync(string id)
    {
      if (id == null) throw new ArgumentNullException(nameof(id));

      var role = await _manager.FindByIdAsync(id);

      return role != null ? ToModel(role) : null;
    }

    public async Task<string> CreateAsync(RoleViewModel model)
    {
      if (model == null) throw new ArgumentNullException(nameof(model));

      var newRole = new IdentityRole(model.Name);
      if (!await _manager.RoleExistsAsync(model.Name))
      {
        await _manager.CreateAsync(newRole);

        return newRole.Id;
      }

      var role = await _manager.FindByNameAsync(model.Name);

      return role.Id;
    }

    public async Task UpdateAsync(RoleViewModel model)
    {
      if (model == null) throw new ArgumentNullException(nameof(model));
      if (string.IsNullOrWhiteSpace(model.Id)) throw new InvalidOperationException(nameof(model.Id));

      var role = await _manager.FindByIdAsync(model.Id);
      role.Name = model.Name;

      await _manager.UpdateAsync(role);
    }

    public async Task DeleteAsync(string id)
    {
      if (id == null) throw new ArgumentNullException(nameof(id));

      var role = await _manager.FindByIdAsync(id);

      await _manager.DeleteAsync(role);
    }

    private static RoleViewModel ToModel(IdentityRole entity)
    {
      return new RoleViewModel
      {
        Id = entity.Id,
        Name = entity.Name
      };
    }
  }
}