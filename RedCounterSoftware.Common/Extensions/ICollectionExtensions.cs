namespace RedCounterSoftware.Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class ICollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> toAdd)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (toAdd == null)
            {
                throw new ArgumentNullException(nameof(toAdd));
            }

            toAdd.ToList().ForEach(c => source.Add(c));
        }
    }
}
