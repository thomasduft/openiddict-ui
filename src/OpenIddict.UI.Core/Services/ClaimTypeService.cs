using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tomware.OpenIddict.UI.Core
{
  public class ClaimTypeService : IClaimTypeService
  {
    private readonly IClaimTypeRepository _repository;

    public ClaimTypeService(IClaimTypeRepository repository)
    {
      _repository = repository
        ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IEnumerable<ClaimTypeInfo>> GetClaimTypesAsync()
    {
      var items = await _repository.ListAsync(new AllClaimTypes());

      return items.Select(x => ToInfo(x));
    }

    public async Task<ClaimTypeInfo> GetAsync(Guid id)
    {
      if (id == Guid.Empty) throw new InvalidOperationException(nameof(id));

      var entity = await _repository.GetByIdAsync(id);

      return entity != null ? ToInfo(entity) : null;
    }

    public async Task<Guid> CreateAsync(ClaimTypeParam model)
    {
      if (model == null) throw new ArgumentNullException(nameof(model));

      var items = await _repository.ListAsync(new ClaimTypeByName(model.Name));
      var entity = items.Count > 0 ? items[0] : null;
      if (entity == null)
      {
        // create new entity
        var newEntity = ClaimType.Create(
          model.Name,
          model.ClaimValueType,
          model.Description
        );
        await _repository.AddAsync(newEntity);

        return newEntity.Id;
      }

      // update existing entity
      model.Id = entity.Id;
      SimpleMapper.Map(model, entity);
      await _repository.UpdateAsync(entity);

      return entity.Id;
    }

    public async Task UpdateAsync(ClaimTypeParam model)
    {
      if (!model.Id.HasValue) throw new InvalidOperationException(nameof(model.Id));

      var entity = await _repository.GetByIdAsync(model.Id.Value);

      SimpleMapper.Map(model, entity);

      await _repository.UpdateAsync(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
      // TODO: check correct removals of claimtypes!
      // Consider an interaction service

      if (id == Guid.Empty) throw new InvalidOperationException(nameof(id));

      var entity = await _repository.GetByIdAsync(id);

      await _repository.DeleteAsync(entity);
    }

    private static ClaimTypeInfo ToInfo(ClaimType entity)
    {
      return SimpleMapper.From<ClaimType, ClaimTypeInfo>(entity);
    }
  }
}