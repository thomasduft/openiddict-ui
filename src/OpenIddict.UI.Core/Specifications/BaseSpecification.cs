using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace tomware.OpenIddict.UI.Core
{
  public abstract class BaseSpecification<T> : ISpecification<T>
  {
    public List<Expression<Func<T, bool>>> Criterias { get; }
      = new List<Expression<Func<T, bool>>>();
    public List<Expression<Func<T, object>>> Includes { get; }
      = new List<Expression<Func<T, object>>>();
    public List<string> IncludeStrings { get; } = new List<string>();
    public Expression<Func<T, object>> OrderBy { get; private set; }
    public Expression<Func<T, object>> OrderByDescending { get; private set; }
    public Expression<Func<T, object>> GroupBy { get; private set; }

    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; } = false;
    public bool AsNoTracking { get; private set; } = false;

    /// <summary>
    /// Constructor for BaseSpecification with no Criteria.
    /// Note: Use AddCriteria(criteria) when criterias are required!
    /// </summary>
    protected BaseSpecification()
    {
    }

    /// <summary>
    /// Constructor for BaseSpecification with default Criteria.
    /// </summary>
    protected BaseSpecification(Expression<Func<T, bool>> criteria)
    {
      this.Criterias.Add(criteria);
    }

    protected virtual void AddCriteria(Expression<Func<T, bool>> criteria)
    {
      this.Criterias.Add(criteria);
    }

    protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
    {
      this.Includes.Add(includeExpression);
    }

    protected virtual void AddInclude(string includeString)
    {
      this.IncludeStrings.Add(includeString);
    }

    protected virtual void ApplyPaging(int skip, int take)
    {
      this.Skip = skip;
      this.Take = take;
      this.IsPagingEnabled = true;
    }

    protected virtual void ApplyNoTracking()
    {
      this.AsNoTracking = true;
    }

    protected virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
    {
      this.OrderBy = orderByExpression;
    }

    protected virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
      this.OrderByDescending = orderByDescendingExpression;
    }

    protected virtual void ApplyGroupBy(Expression<Func<T, object>> groupByExpression)
    {
      this.GroupBy = groupByExpression;
    }
  }
}
