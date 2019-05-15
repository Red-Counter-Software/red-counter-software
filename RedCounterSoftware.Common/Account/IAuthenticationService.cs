namespace RedCounterSoftware.Common.Account
{
    using System;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using RedCounterSoftware.Common.Validation;

    /// <summary>
    /// Store service to interact with <see cref="T:RedCounterSoftware.Common.Account.IUser" /> dataobjects.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Activates a <see cref="User"/> by setting its <see cref="User.IsActive"/> flag to true and nulling its <see cref="User.ActivationGuid"/>.
        /// </summary>
        /// <param name="activationGuid">The activation guid used to find the user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a <see cref="Result{IUser}"/> that indicates whether the operation succeeded, and if positive it contains the activated user.</returns>
        Task<Result<IUser>> Activate(Guid activationGuid, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a single entity that matches the provided selector and value, null if no match is found, or throws an exception if more than one match is found.
        /// </summary>
        /// <typeparam name="TK">The type of the property to search on.</typeparam>
        /// <param name="selector">The selector pointing to the search property.</param>
        /// <param name="value">The value to search for.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a single instance of <see cref="T"/> if found, null if not found, throws an exception if more than one is found.</returns>
        Task<IUser> GetBy<TK>(Expression<Func<IUser, TK>> selector, TK value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns false if the provided password does not match the one saved in the document store, otherwise returns true.
        /// </summary>
        /// <param name="email">The unique email address of the user.</param>
        /// <param name="password">The password to test.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns whether the provided password matches the one stored.</returns>
        Task<bool> IsPasswordValid(string email, string password, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns false if the <see cref="User"/> with provided id does not exist, if the <see cref="User.IsActive"/> flag is false, or if the user is <see cref="User.LockedOutUntil"/> a date in the future, otherwise returns true.
        /// </summary>
        /// <param name="user">The user to test.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns whether the user is active or not.</returns>
        Task<bool> IsUserActive(IUser user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Applies validation and business rules before updating the property indicated by the <see cref="selector"/> of the entity with provided <see cref="id"/> using the provided <see cref="value"/>.
        /// </summary>
        /// <param name="id">The entity id.</param>
        /// <param name="selector">The lamdba expression representing the property to update.</param>
        /// <param name="value">The new value to set.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="TK">The type of the property being updated.</typeparam>
        /// <returns>Returns a <see cref="Result{T}"/> indicating wether the operation was successful and the updated entity.</returns>
        Task<Result<IUser>> Patch<TK>(object id, Expression<Func<IUser, TK>> selector, TK value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Computes a new password hash using the provided <see cref="password"/>, optional salt defined in the implementation and sets it as the new <see cref="User.Password"/>.
        /// </summary>
        /// <param name="userId">The id of the user to update.</param>
        /// <param name="password">The new password to set.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a <see cref="Result"/> that informs whether the operation succeeded or not.</returns>
        Task<Result> SetPassword(object userId, string password, CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs a search using the provided <see cref="SearchParameters{User}"/>.
        /// </summary>
        /// <param name="searchParameters">The search parameters.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a <see cref="SearchResult{User}"/> containing the entities matching the search criteria.</returns>
        Task<SearchResult<IUser>> Search(SearchParameters<IUser> searchParameters, CancellationToken cancellationToken = default);
    }
}
