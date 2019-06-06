namespace RedCounterSoftware.Common
{
    using System;
    using System.Linq.Expressions;
    using Extensions;

    /// <summary>
    /// Defines standard search parameters to query an object store.
    /// </summary>
    /// <typeparam name="T">The type of entity to search.</typeparam>
    public class SearchParameters<T> : SearchParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchParameters{T}"/> class.
        /// </summary>
        /// <param name="searchTerm">The term to search for.</param>
        /// <param name="sortTerm">The name of the property to sort by.</param>
        /// <param name="isDescending">True for a descending order, false for ascending.</param>
        /// <param name="pageSize">The amount of items to return.</param>
        /// <param name="currentPage">The 0 based index of the subset of items to return.</param>
        public SearchParameters(string searchTerm, string sortTerm, bool isDescending = false, int pageSize = 10, int currentPage = 0)
            : base(searchTerm, sortTerm, isDescending, pageSize, currentPage)
        {
            this.SortExpression = sortTerm.GetPropertyExpression<T>();
        }


        /// <summary>
        /// Gets the lambda expression indicating the property to sort by.
        /// </summary>
        /// <value>
        /// The lambda expression indicating the property to sort by.
        /// </value>
        public Expression<Func<T, object>> SortExpression { get; }
    }
}
