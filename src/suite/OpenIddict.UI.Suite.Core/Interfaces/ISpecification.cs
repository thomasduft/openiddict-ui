using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace tomware.OpenIddict.UI.Suite.Core;

public interface ISpecification<T>
{
  public List<Expression<Func<T, bool>>> Criteria { get; }
  public List<Expression<Func<T, object>>> Includes { get; }
  public List<string> IncludeStrings { get; }
  public Expression<Func<T, object>> OrderBy { get; }
  public Expression<Func<T, object>> OrderByDescending { get; }
  public Expression<Func<T, object>> GroupBy { get; }

  public int Take { get; }
  public int Skip { get; }
  public bool IsPagingEnabled { get; }
  public bool AsNoTracking { get; }
}
