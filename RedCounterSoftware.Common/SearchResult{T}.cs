namespace RedCounterSoftware.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a standard search result for dataobjects.
    /// </summary>
    /// <typeparam name="T">The type of the dataobjects returned.</typeparam>
    /// <remarks>
    /// Initializes a new instance of the <see cref="SearchResult{T}"/> class.
    /// </remarks>
    /// <param name="totalCount">The total count of the found elements, regardless of paging.</param>
    /// <param name="items">The returned items.</param>
    public class SearchResult<T>(int totalCount, ICollection<T> items)
    {
        /// <summary>
        /// Gets the total count of the found elements, regardless of paging.
        /// </summary>
        /// <value>
        /// The total count of the found elements, regardless of paging.
        /// </value>
        public int TotalCount { get; } = totalCount >= 0 ? totalCount : throw new ArgumentException("Cannot be less than zero", nameof(totalCount));

        /// <summary>
        /// Gets the returned items.
        /// </summary>
        /// <value>
        /// The returned items.
        /// </value>
        public ICollection<T> Items { get; } = items ?? throw new ArgumentNullException(nameof(items));
    }
}
