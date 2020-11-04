using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tomware.OpenIddict.UI.Core
{
  public class ClaimTypeService : IClaimTypeService
  {
    private readonly IClaimTypeRepository repository;

    public ClaimTypeService(IClaimTypeRepository repository)
    {
      this.repository = repository
        ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IEnumerable<ClaimeTypeInfo>> GetClaimTypesAsync()
    {
      var items = await this.repository.ListAsync(new AllClaimTypes());

      return items.Select(x => ToInfo(x));
    }

    public async Task<ClaimeTypeInfo> GetAsync(Guid id)
    {
      if (id == null) throw new ArgumentNullException(nameof(id));

      var entity = await this.repository.GetByIdAsync(id);

      return entity != null ? ToInfo(entity) : null;
    }

    public async Task<Guid> CreateAsync(ClaimeTypeParam model)
    {
      if (model == null) throw new ArgumentNullException(nameof(model));

      var entity = ClaimType.Create(model.Name, model.Description);

      await this.repository.AddAsync(entity);

      return entity.Id;
    }

    public async Task UpdateAsync(ClaimeTypeParam model)
    {
      if (!model.Id.HasValue) throw new ArgumentNullException(nameof(model.Id));

      var entity = await this.repository.GetByIdAsync(model.Id.Value);

      SimpleMapper.Map<ClaimeTypeParam, ClaimType>(model, entity);

      await this.repository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
      // TODO: check correct removals of claimtypes!

      if (id == null) throw new ArgumentNullException(nameof(id));

      var entity = await this.repository.GetByIdAsync(id);

      await this.repository.DeleteAsync(entity);
    }

    private ClaimeTypeInfo ToInfo(ClaimType entity)
    {
      return SimpleMapper.From<ClaimType, ClaimeTypeInfo>(entity);
    }
  }
}