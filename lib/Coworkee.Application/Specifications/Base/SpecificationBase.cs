using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using lib.Coworkee.Domain.Contracts;

namespace lib.Coworkee.Application.Specifications.Base
{
    public abstract class SpecificationBase<T> : ISpecification<T> where T : class, IEntity
    {
        public Expression<Func<T, bool>> Criteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; } = new();
        public List<string> IncludeStrings { get; } = new();

        protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        protected virtual void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }
    }
}