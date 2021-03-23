namespace RedCounterSoftware.DataAccess.RavenDb.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Raven.Client.Documents.Linq;
    using RedCounterSoftware.Common.Extensions;

    public static class LambdaExpressionExtensions
    {
        public static Expression<Func<T, bool>> InExpression<T, TK>(this Expression<Func<T, TK>> selector, IEnumerable<TK> array)
        {
            var p = selector.GetParameterExpression();
            var property = Expression.PropertyOrField(p, selector.GetPropertyName());
            var contains = typeof(RavenQueryableExtensions).GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(x => x.Name == "In" && x.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TK));
            var body = Expression.Call(contains, property, Expression.Constant(array));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }
    }
}
