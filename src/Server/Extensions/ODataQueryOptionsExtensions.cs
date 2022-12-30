using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.OData.Query;
using System.Linq;
using System.Linq.Expressions;

namespace Coworkee.Server.Extensions;

public static class ODataQueryOptionsExtensions
{
    public static Expression ToExpression<TElement>(this FilterQueryOption filter)
    {
        IQueryable queryable = Enumerable.Empty<TElement>().AsQueryable();
        queryable = filter.ApplyTo(queryable, new ODataQuerySettings());
        return queryable.Expression;
    }

    public static Expression ToExpression<TPublicEntity>(this OrderByQueryOption filter)
    {
        IQueryable queryable = Enumerable.Empty<TPublicEntity>().AsQueryable();
        queryable = filter.ApplyTo(queryable, new ODataQuerySettings());
        return queryable.Expression;
    }
}