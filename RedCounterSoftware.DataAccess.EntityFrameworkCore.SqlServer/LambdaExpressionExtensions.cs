namespace RedCounterSoftware.DataAccess.EntityFrameworkCore.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using RedCounterSoftware.Common.Extensions;

    public static class LambdaExpressionExtensions
    {
        public static Expression<Func<T, bool>> InExpression<T, TK>(
            this Expression<Func<T, TK>> selector, IEnumerable<TK> array)
        {
            ArgumentNullException.ThrowIfNull(selector);

            var p = selector.GetParameterExpression();
            var contains = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).Where(c => c.Name == "Contains" && c.GetParameters().Length == 2).First().MakeGenericMethod(typeof(TK));
            var body = Expression.Call(contains, Expression.Constant(array), selector.Body);
            return Expression.Lambda<Func<T, bool>>(body, p);
        }
    }
}
