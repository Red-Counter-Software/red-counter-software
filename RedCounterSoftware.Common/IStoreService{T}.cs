namespace RedCounterSoftware.Common
{
    using System;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Validation;

    /// <inheritdoc />
    /// <summary>
    /// Generic interface to handle dataobjects of type <see cref="T"/> with related business logic.
    /// </summary>
    /// <typeparam name="T">The type of dataobject handled.</typeparam>
    public interface IStoreService<T> : IDisposable
        where T : DataObjectBase
    {
        /// <summary>
        /// Applies validation and business rules before adding an instance of <see cref="T"/> to the underlying context.
        /// </summary>
        /// <param name="toAdd">Entity to add.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a <see cref="Result{T}"/> indicating whether the operation was successful and the added entity.</returns>
        Task<Result<T>> Add(T toAdd, CancellationToken cancellationToken = default);

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
        Task<Result> Delete(string id, CancellationToken cancellationToken = default);

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
        /// Applies validation and business rules before updating the property indicated by the <see cref="selector"/> of the entity with provided <see cref="id"/> using the provided <see cref="value"/>.
        /// </summary>
        /// <param name="id">The entity id.</param>
        /// <param name="selector">The lamdba expression representing the property to update.</param>
        /// <param name="value">The new value to set.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="TK">The type of the property being updated.</typeparam>
        /// <returns>Returns a <see cref="Result{T}"/> indicating wether the operation was successful and the updated entity.</returns>
        Task<Result<T>> Patch<TK>(string id, Expression<Func<T, TK>> selector, TK value, CancellationToken cancellationToken = default);
    }
}
