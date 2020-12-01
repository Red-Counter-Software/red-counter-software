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
#pragma warning disable CA1308 // Normalize strings to uppercase
                return s.ToLowerInvariant();
#pragma warning restore CA1308 // Normalize strings to uppercase
            }

            var words = s.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            return words
#pragma warning disable CA1308 // Normalize strings to uppercase
                .Select(w => $"{w.Substring(0, 1).ToLowerInvariant()}{w[1..]}")
#pragma warning restore CA1308 // Normalize strings to uppercase
                .Aggregate((w1, w2) => $"{w1}.{w2}");
        }
    }
}
