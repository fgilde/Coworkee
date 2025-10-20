using Microsoft.Rest.Azure.OData;
using Nextended.Core.OData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Coworkee.Shared;

public class TransferableExpression<T>
{
    private readonly string _odataFilter;

    public TransferableExpression()
    {}

    public TransferableExpression(string odataFilter) // This constructor is required and also used by activator create instance
    {
        _odataFilter = odataFilter;
    }

    public TransferableExpression(Expression<Func<T, bool>> expression)
    {
        //_odataFilter = expression.ToFilterString();
        _odataFilter = FilterString.Generate(expression, true);
    }

    public static implicit operator string(TransferableExpression<T> t) => t?._odataFilter ?? string.Empty;
    public static implicit operator TransferableExpression<T>(string s) => new(s);


    public override string ToString()
    {
        return _odataFilter;
    }
}