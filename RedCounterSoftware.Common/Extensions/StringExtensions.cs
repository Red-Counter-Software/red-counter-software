namespace RedCounterSoftware.Common.Extensions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public static class StringExtensions
    {
        public static Expression<Func<TModel, object>> GetPropertyExpression<TModel>(this string propertyName)
        {
            var parameter = Expression.Parameter(typeof(TModel), "model");
            var property = Expression.Property(parameter, propertyName);
            var conversion = Expression.Convert(property, typeof(object));
            var expression = Expression.Lambda<Func<TModel, object>>(conversion, parameter);
            return expression;
        }

        public static string ToCamelCase(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                throw new ArgumentException("Cannot be empty", nameof(s));
            }

            if (s.Length == 1)
            {
                return s.ToLowerInvariant();
            }

            var words = s.Split('.', StringSplitOptions.RemoveEmptyEntries);
            return words
                .Select(w => $"{w.Substring(0, 1).ToLowerInvariant()}{w.Substring(1)}")
                .Aggregate((w1, w2) => $"{w1}.{w2}");
        }
    }
}
