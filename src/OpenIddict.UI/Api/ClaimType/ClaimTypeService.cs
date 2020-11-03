using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace tomware.OpenIddict.UI
{
  public interface IClaimTypeService
  {
    Task<IEnumerable<ClaimTypeViewModel>> GetClaimTypesAsync();

    Task<ClaimTypeViewModel> GetAsync(Guid id);

    Task<Guid> CreateAsync(ClaimTypeViewModel model);

    Task UpdateAsync(ClaimTypeViewModel model);

    Task DeleteAsync(Guid id);
  }

  // public class ClaimTypeService : IClaimTypeService
  // {
  //   private readonly STSContext context;

  //   public ClaimTypeService(STSContext context)
  //   {
  //     this.context = context;
  //   }

  //   public async Task<IEnumerable<ClaimTypeViewModel>> GetClaimTypesAsync()
  //   {
  //     var items = await this.context.ClaimTypes
  //       .OrderBy(c => c.Name)
  //       .AsNoTracking()
  //       .ToListAsync();

  //     return items.Select(c => ToModel(c));
  //   }

  //   public async Task<ClaimTypeViewModel> GetAsync(Guid id)
  //   {
  //     if (id == null) throw new ArgumentNullException(nameof(id));

  //     var claimType = await this.context.ClaimTypes.FindAsync(id);

  //     return claimType != null ? ToModel(claimType) : null;
  //   }

  //   public async Task<Guid> CreateAsync(ClaimTypeViewModel model)
  //   {
  //     if (model == null) throw new ArgumentNullException(nameof(model));

  //     // TODO: Check whether name already exists
  //     var claimType = ClaimType.Create(model.Name, model.Description);

  //     this.context.ClaimTypes.Add(claimType);

  //     await this.context.SaveChangesAsync();

  //     return claimType.Id;
  //   }

  //   public async Task UpdateAsync(ClaimTypeViewModel model)
  //   {
  //     if (model == null) throw new ArgumentNullException(nameof(model));
  //     if (!model.Id.HasValue) throw new ArgumentNullException(nameof(model.Id));

  //     var claimType = await this.context.ClaimTypes.FindAsync(model.Id.Value);
  //     if (claimType == null) throw new ArgumentNullException(nameof(claimType));

  //     claimType.Name = model.Name;
  //     claimType.Description = model.Description;

  //     this.context.ClaimTypes.Update(claimType);

  //     await this.context.SaveChangesAsync();
  //   }

  //   public async Task DeleteAsync(Guid id)
  //   {
  //     if (id == null) throw new ArgumentNullException(nameof(id));

  //     var claimType = await this.context.ClaimTypes.FindAsync(id);

  //     var userClaims = await this.context.UserClaims
  //       .Where(uc => uc.ClaimType == claimType.Name)
  //       .ToListAsync();
  //     this.context.UserClaims.RemoveRange(userClaims);

  //     this.context.ClaimTypes.Remove(claimType);

  //     await this.context.SaveChangesAsync();
  //   }

  //   private ClaimTypeViewModel ToModel(ClaimType entity)
  //   {
  //     return new ClaimTypeViewModel
  //     {
  //       Id = entity.Id,
  //       Name = entity.Name,
  //       Description = entity.Description
  //     };
  //   }
  // }
}