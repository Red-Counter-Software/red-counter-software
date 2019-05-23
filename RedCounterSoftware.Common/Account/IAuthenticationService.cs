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

        /// <summary>
        /// Computes a new password hash using the provided <see cref="password"/>, optional salt defined in the implementation and sets it as the new <see cref="User.Password"/>.
        /// </summary>
        /// <param name="userId">The id of the user to update.</param>
        /// <param name="password">The new password to set.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a <see cref="Result"/> that informs whether the operation succeeded or not.</returns>
        Task<Result> SetPassword(object userId, string password, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets a new single use password reset guid to be sent to the user.
        /// </summary>
        /// <param name="userId">The Id of the user.</param>
        /// <param name="passwordResetGuid">The new password reset guid.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a <see cref="Result"/> that informs whether the operation succeeded or not.</returns>
        Task<Result<IUser>> SetPasswordResetGuid(object userId, Guid passwordResetGuid, CancellationToken cancellationToken = default);
    }
}
