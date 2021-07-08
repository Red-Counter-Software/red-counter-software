namespace RedCounterSoftware.Common
{
    using System;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IWriteDataContext<T> : IDisposable
        where T : RecordBase
    {
        /// <summary>
        /// Adds an instance of <see cref="T"/> to the underlying storage.
        /// </summary>
        /// <typeparam name="TId">The type of the Id property.</typeparam>
        /// <param name="filter">The expression to filter by Id.</param>
        /// <param name="id">The id.</param>
        /// <param name="toAdd">Entity to add.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns the added entity.</returns>
        Task<T> Add<TId>(Expression<Func<T, TId>> filter, TId id, T toAdd, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds multiple instances of <see cref="T"/> to the underlying storage in a single transaction.
        /// </summary>
        /// <typeparam name="TId">The type of the Id property.</typeparam>
        /// <param name="filter">The expression to filter by Id.</param>
        /// <param name="toAdd">Entities to add.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns the added entities.</returns>
        Task<T[]> AddBulk<TId>(Expression<Func<T, TId>> filter, T[] toAdd, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the entity with matching id from the storage.
        /// </summary>
        /// <typeparam name="TId">The type of the Id property.</typeparam>
        /// <param name="filter">The expression to filter by Id.</param>
        /// <param name="id">The id.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a completed task.</returns>
        Task Delete<TId>(Expression<Func<T, TId>> filter, TId id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the property indicated by the <see cref="selector"/> of the entity with provided <see cref="id"/> using the provided <see cref="value"/>.
        /// </summary>
        /// <typeparam name="TId">The type of the Id property.</typeparam>
        /// <typeparam name="TK">The type of the property to update.</typeparam>
        /// <param name="filter">The expression to filter by Id.</param>
        /// <param name="id">The id.</param>
        /// <param name="selector">The lamdba expression representing the property to update.</param>
        /// <param name="value">The new value to set.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns the updated entity.</returns>
        Task<T> Patch<TId, TK>(Expression<Func<T, TId>> filter, TId id, Expression<Func<T, TK>> selector, TK value, CancellationToken cancellationToken = default);
    }
}
