namespace RedCounterSoftware.Common.Extensions
{
    using System;
    using System.Collections.Generic;

    public static class ICollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> toAdd)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(toAdd);

            foreach (var item in toAdd)
            {
                source.Add(item);
            }
        }
    }
}
