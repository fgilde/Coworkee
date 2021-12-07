using System;
using System.Linq;
using System.Linq.Expressions;
using CleanArchitectureBase.Shared;
using StringToExpression.LanguageDefinitions;

namespace CleanArchitectureBase.Application.Common.Extensions;

public static class ODataQueryOptionsExtensions
{
    //public static Expression ToExpression<TElement>(this FilterQueryOption filter)
    //{
    //    IQueryable queryable = Enumerable.Empty<TElement>().AsQueryable();
    //    queryable = filter.ApplyTo(queryable, new ODataQuerySettings());
    //    return queryable.Expression;
    //}

    public static Expression<Func<T, bool>> ToExpression<T>(this TransferableExpression<T> odataFilter)
    {
        return ParseExpression<T>(odataFilter);
    }

    public static Expression<Func<T, bool>> ParseExpression<T>(string odataFilter)
    {
        return !string.IsNullOrEmpty(odataFilter) && odataFilter != "{}" ? new ODataFilterLanguage().Parse<T>(odataFilter) : null;
    }
}