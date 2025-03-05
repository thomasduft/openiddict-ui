using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace tomware.OpenIddict.UI.Identity.Api;

public interface IRoleApiService
{
  public Task<IEnumerable<RoleViewModel>> GetRolesAsync();

  public Task<RoleViewModel> GetAsync(string id);

  public Task<string> CreateAsync(RoleViewModel model);

  public Task UpdateAsync(RoleViewModel model);

  public Task DeleteAsync(string id);
}

public class RoleApiService<TIdentityRole, TKey> : IRoleApiService
  where TKey : IEquatable<TKey>
  where TIdentityRole : IdentityRole<TKey>, new()
{
  private readonly RoleManager<TIdentityRole> _manager;

  public RoleApiService(RoleManager<TIdentityRole> manager)
  {
    _manager = manager;
  }

  public async Task<IEnumerable<RoleViewModel>> GetRolesAsync()
  {
    var items = _manager.Roles
      .OrderBy(r => r.Name)
      .ToList();

    var models = items.Select(ToModel);

    return await Task.FromResult(models);
  }

  public async Task<RoleViewModel> GetAsync(string id)
  {
    ArgumentNullException.ThrowIfNull(id);

    var role = await _manager.FindByIdAsync(id);

    return role != null ? ToModel(role) : null;
  }

  public async Task<string> CreateAsync(RoleViewModel model)
  {
    ArgumentNullException.ThrowIfNull(model);

    var newRole = new TIdentityRole
    {
      Name = model.Name
    };
    if (!await _manager.RoleExistsAsync(model.Name))
    {
      await _manager.CreateAsync(newRole);
      return newRole.Id.ToString();
    }

    var role = await _manager.FindByNameAsync(model.Name);

    return role.Id.ToString();
  }

  public async Task UpdateAsync(RoleViewModel model)
  {
    ArgumentNullException.ThrowIfNull(model);

    if (string.IsNullOrWhiteSpace(model.Id))
    {
      throw new InvalidOperationException(nameof(model.Id));
    }

    var role = await _manager.FindByIdAsync(model.Id);
    role.Name = model.Name;

    await _manager.UpdateAsync(role);
  }

  public async Task DeleteAsync(string id)
  {
    ArgumentNullException.ThrowIfNull(id);

    var role = await _manager.FindByIdAsync(id);

    await _manager.DeleteAsync(role);
  }

  private static RoleViewModel ToModel(IdentityRole<TKey> entity)
  {
    return new RoleViewModel
    {
      Id = entity.Id.ToString(),
      Name = entity.Name
    };
  }
}
