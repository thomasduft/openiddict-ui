using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace tomware.OpenIddict.UI.Suite.Core
{
  public interface ISpecification<T>
  {
    List<Expression<Func<T, bool>>> Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    List<string> IncludeStrings { get; }
    Expression<Func<T, object>> OrderBy { get; }
    Expression<Func<T, object>> OrderByDescending { get; }
    Expression<Func<T, object>> GroupBy { get; }

    int Take { get; }
    int Skip { get; }
    bool IsPagingEnabled { get; }
    bool AsNoTracking { get; }
  }
}