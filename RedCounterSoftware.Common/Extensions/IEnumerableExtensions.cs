namespace RedCounterSoftware.Common.Extensions
{
    using System;
    using System.Collections.Generic;

    public static class IEnumerableExtensions
    {
        public static IEnumerable<TResult> SelectWithPrevious<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TSource, TResult> projection)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (projection == null)
            {
                throw new ArgumentNullException(nameof(projection));
            }

            using var iterator = source.GetEnumerator();

            if (!iterator.MoveNext())
            {
                yield break;
            }

            TSource previous = iterator.Current;
            while (iterator.MoveNext())
            {
                yield return projection(previous, iterator.Current);
                previous = iterator.Current;
            }
        }
    }
}
