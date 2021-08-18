using Microsoft.EntityFrameworkCore;
using System.Linq;
using tomware.OpenIddict.UI.Suite.Core;

namespace tomware.OpenIddict.UI.Infrastructure
{
  internal class SpecificationEvaluator<T> where T : class
  {
    public static IQueryable<T> GetQuery(
      IQueryable<T> inputQuery,
      ISpecification<T> specification
    )
    {
      var query = inputQuery;

      // modify the IQueryable using the specification's criteria expression
      if (specification.Criteria.Count > 0)
      {
        foreach (var criteria in specification.Criteria)
        {
          query = query.Where(criteria);
        }
      }

      // Includes all expression-based includes
      query = specification
        .Includes.Aggregate(
          query,
          (current, include) => current.Include(include)
        );

      // Include any string-based include statements
      query = specification
        .IncludeStrings.Aggregate(
          query,
          (current, include) => current.Include(include)
        );

      // Apply ordering if expressions are set
      if (specification.OrderBy != null)
      {
        query = query.OrderBy(specification.OrderBy);
      }
      else if (specification.OrderByDescending != null)
      {
        query = query.OrderByDescending(specification.OrderByDescending);
      }

      if (specification.GroupBy != null)
      {
        query = query.GroupBy(specification.GroupBy).SelectMany(x => x);
      }

      // Apply paging if enabled
      if (specification.IsPagingEnabled)
      {
        query = query
          .Skip(specification.Skip)
          .Take(specification.Take);
      }

      if (specification.AsNoTracking)
      {
        query.AsNoTracking();
      }

      return query;
    }
  }
}
