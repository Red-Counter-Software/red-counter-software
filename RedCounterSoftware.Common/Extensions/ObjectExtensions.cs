namespace RedCounterSoftware.Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.Extensions.Caching.Memory;

    public static class ObjectExtensions
    {
        private static readonly IMemoryCache Cache = new MemoryCache(new MemoryCacheOptions());

        public static T1 With<T1, T2>(this T1 item, Expression<Func<T1, T2>> selector, T2 value)
        {
            return With(item, selector, (arg1, arg2) => value);
        }

        public static T1 With<T1, T2>(this T1 item, Expression<Func<T1, T2>> selector, Func<T1, T2, T2> mutator)
        {
            var (constructor, parameters, properties, propertyToBeMutated) = GetInfo(selector);

            return Apply(item, constructor, parameters, properties, propertyToBeMutated, mutator);
        }

        public static IEnumerable<T1> With<T1, T2>(this IEnumerable<T1> source, Expression<Func<T1, T2>> selector, Func<T1, T2, T2> mutator)
        {
            var (constructor, parameters, properties, propertyToBeMutated) = GetInfo(selector);

            foreach (var item in source)
            {
                yield return Apply(item, constructor, parameters, properties, propertyToBeMutated, mutator);
            }
        }

        private static (ConstructorInfo constructor, IList<ParameterInfo> parameters, IList<PropertyInfo> properties, string propertyToBeMutated) GetInfo<T1, T2>(Expression<Func<T1, T2>> selector)
        {
            var type = typeof(T1);

            PropertyInfo[] properties;
            ConstructorInfo constructor;
            ParameterInfo[] parameters;

            lock (Cache)
            {
                (properties, constructor, parameters) = Cache.GetOrCreate(type.AssemblyQualifiedName, entry =>
                {
                    var a = type.GetProperties();
                    var b = type.GetConstructors().Single();
                    var c = b.GetParameters();

                    return (a, b, c);
                });
            }

            var propertyToBeMutated = selector.GetPropertyName();

            return (constructor, parameters, properties, propertyToBeMutated);
        }

        private static T1 Apply<T1, T2>(T1 item, ConstructorInfo constructor, IList<ParameterInfo> parameters, IList<PropertyInfo> properties, string propertyToBeMutated, Func<T1, T2, T2> mutator)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (constructor == null)
            {
                throw new ArgumentNullException(nameof(constructor));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            if (string.IsNullOrEmpty(propertyToBeMutated))
            {
                throw new ArgumentException("Cannot be empty", nameof(propertyToBeMutated));
            }

            if (mutator == null)
            {
                throw new ArgumentNullException(nameof(mutator));
            }

            var constructorParametersCount = parameters.Count;
            var values = new object[constructorParametersCount];

            for (var i = 0; i < constructorParametersCount; i++)
            {
                var property = properties.Single(a => a.Name.Equals(parameters[i].Name, StringComparison.OrdinalIgnoreCase));
                var value = property.GetValue(item);

                if (value == null)
                {
                    throw new InvalidOperationException("The property returned a null value");
                }

                if (property.Name.Equals(propertyToBeMutated, StringComparison.OrdinalIgnoreCase))
                {
                    value = mutator.Invoke(item, (T2)value);
                }

                values[i] = value ?? throw new InvalidOperationException("The mutated property is null");
            }

            return (T1)constructor.Invoke(values);
        }
    }
}
