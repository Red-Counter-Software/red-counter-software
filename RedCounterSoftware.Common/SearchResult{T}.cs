namespace RedCounterSoftware.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a standard search result for dataobjects.
    /// </summary>
    /// <typeparam name="T">The type of the dataobjects returned.</typeparam>
    public class SearchResult<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResult{T}"/> class.
        /// </summary>
        /// <param name="totalCount">The total count of the found elements, regardless of paging.</param>
        /// <param name="items">The returned items.</param>
        public SearchResult(int totalCount, List<T> items)
        {
            this.TotalCount = totalCount >= 0 ? totalCount : throw new ArgumentException("Cannot be less than zero", nameof(totalCount));
            this.Items = items ?? throw new ArgumentNullException(nameof(items));
        }

        /// <summary>
        /// Gets the total count of the found elements, regardless of paging.
        /// </summary>
        /// <value>
        /// The total count of the found elements, regardless of paging.
        /// </value>
        public int TotalCount { get; }

        /// <summary>
        /// Gets the returned items.
        /// </summary>
        /// <value>
        /// The returned items.
        /// </value>
        public List<T> Items { get; }
    }
}
