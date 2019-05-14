namespace RedCounterSoftware.Common.Account
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using RedCounterSoftware.Common.Validation;

    /// <inheritdoc />
    /// <summary>
    /// Store service to interact with <see cref="T:RedCounterSoftware.Common.Account.IUser" /> dataobjects.
    /// </summary>
    public interface IUserStoreService : IStoreService<IUser>
    {
        /// <summary>
        /// Activates a <see cref="User"/> by setting its <see cref="User.IsActive"/> flag to true and nulling its <see cref="User.ActivationGuid"/>.
        /// </summary>
        /// <param name="activationGuid">The activation guid used to find the user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a <see cref="Result{User}"/> that indicates whether the operation succeeded, and if positive it contains the activated user.</returns>
        Task<Result<IUser>> Activate(Guid activationGuid, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new user with the provided <see cref="email"/>, <see cref="personId"/> and <see cref="roles"/>. The <see cref="email"/> must not be already present in the store. The linked <see cref="People.Person"/> must exist and not have a user already associated. The <see cref="roles"/> collection must contain at least one existing <see cref="Role"/>.
        /// </summary>
        /// <param name="email">The user unique email.</param>
        /// <param name="personId">The id of the person to link to the user.</param>
        /// <param name="roles">The collection of <see cref="Role"/> the user starts with. Must contain at least one existing <see cref="Role"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a <see cref="Result{User}"/> that indicates whether the operation succeeded, and if positive it contains the created user.</returns>
        Task<Result<IUser>> Add(string email, object personId, List<string> roles, CancellationToken cancellationToken = default);

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
