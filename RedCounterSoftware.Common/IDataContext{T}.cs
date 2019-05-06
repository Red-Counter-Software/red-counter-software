namespace RedCounterSoftware.Common
{
    using System;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <inheritdoc />
    /// <summary>
    /// Generic DataContext interface to interact with underlying storage of a specific type <see cref="!:T" /> of entity.
    /// </summary>
    /// <typeparam name="T">The type of the entity handled.</typeparam>
    public interface IDataContext<T> : IDisposable
        where T : DataObjectBase
    {
        /// <summary>
        /// Adds an instance of <see cref="T"/> to the underlying storage.
        /// </summary>
        /// <param name="toAdd">Entity to add.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns the added entity.</returns>
        Task<T> Add(T toAdd, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a total count of the elements of type <see cref="T"/> in the storage.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns the total count.</returns>
        Task<int> Count(CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the entity with matching id from the storage.
        /// </summary>
        /// <param name="id">The entity id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a completed task.</returns>
        Task Delete(string id, CancellationToken cancellationToken = default);

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
        /// Updates the property indicated by the <see cref="selector"/> of the entity with provided <see cref="id"/> using the provided <see cref="value"/>.
        /// </summary>
        /// <typeparam name="TK">The type of the property to update.</typeparam>
        /// <param name="id">The entity id.</param>
        /// <param name="selector">The lamdba expression representing the property to update.</param>
        /// <param name="value">The new value to set.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns the updated entity.</returns>
        Task<T> Patch<TK>(string id, Expression<Func<T, TK>> selector, TK value, CancellationToken cancellationToken = default);
    }
}
