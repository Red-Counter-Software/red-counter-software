namespace RedCounterSoftware.Common.Account
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IReadAuthenticationService
    {
        /// <summary>
        /// Returns a single entity that matches the provided email address, null if no match is found, or throws an exception if more than one match is found.
        /// </summary>
        /// <param name="email">The user email.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a single instance of <see cref="IUser"/> if found, null if not found, throws an exception if more than one is found.</returns>
        Task<IUser> GetByEmail(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a single entity that matches the provided password reset guid, null if no match is found, or throws an exception if more than one match is found.
        /// </summary>
        /// <param name="passwordResetGuid">The password reset guid.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a single instance of <see cref="IUser"/> if found, null if not found, throws an exception if more than one is found.</returns>
        Task<IUser> GetByPasswordResetGuid(Guid passwordResetGuid, CancellationToken cancellationToken = default);

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
    }
}
