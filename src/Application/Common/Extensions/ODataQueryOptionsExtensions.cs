using System;
using System.Linq;
using System.Linq.Expressions;
using Coworkee.Shared;
using StringToExpression.LanguageDefinitions;

namespace Coworkee.Application.Common.Extensions;

public static class ODataQueryOptionsExtensions
{
    public static Expression<Func<T, bool>> ToExpression<T>(this TransferableExpression<T> odataFilter)
    {
        return ParseExpression<T>(odataFilter);
    }

    public static Expression<Func<T, bool>> ParseExpression<T>(string odataFilter)
    {
        return !string.IsNullOrEmpty(odataFilter) && odataFilter != "{}" ? new ODataFilterLanguage().Parse<T>(odataFilter) : null;
    }
}