namespace RedCounterSoftware.Common
{
    using System;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IReadDataContext<T> : IDisposable
        where T : RecordBase
    {
        /// <summary>
        /// Returns a total count of the elements of type <see cref="T"/> in the storage.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns the total count.</returns>
        Task<int> Count(CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns true if the entity with specified property exists.
        /// </summary>
        /// <typeparam name="TK">The type of the property.</typeparam>
        /// <param name="selector">The property selector.</param>
        /// <param name="value">The value to search for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns whether the entity with specified property value exists.</returns>
        Task<bool> ExistsBy<TK>(Expression<Func<T, TK>> selector, TK value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a single entity that matches the provided selector and value, null if no match is found, or throws an exception if more than one match is found.
        /// </summary>
        /// <typeparam name="TK">The type of the property to search on.</typeparam>
        /// <param name="selector">The selector pointing to the search property.</param>
        /// <param name="value">The value to search for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a single instance of <see cref="T"/> if found, null if not found, throws an exception if more than one is found.</returns>
        Task<T> GetBy<TK>(Expression<Func<T, TK>> selector, TK value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a <see cref="SearchResult{T}"/> containing multiple entities that match the array provided values.
        /// </summary>
        /// <typeparam name="TK">The type of the property to search on.</typeparam>
        /// <param name="selector">The selector pointing to the search property.</param>
        /// <param name="values">The values to look for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a <see cref="SearchResult{T}"/> containing the matching entities.</returns>
        Task<SearchResult<T>> GetByMultipleValues<TK>(Expression<Func<T, TK>> selector, TK[] values, CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs a search based on the provided parameters.
        /// </summary>
        /// <param name="searchParameters">The search parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns the result of the search.</returns>
        Task<SearchResult<T>> Search(SearchParameters<T> searchParameters, CancellationToken cancellationToken = default);
    }
}
