namespace RedCounterSoftware.Common.Account
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using RedCounterSoftware.Common.Validation;

    /// <summary>
    /// Store service to interact with <see cref="IUser" /> dataobjects.
    /// </summary>
    public interface IAuthenticationService : IReadAuthenticationService
    {
        /// <summary>
        /// Activates a <see cref="User"/> by setting its <see cref="User.IsActive"/> flag to true and nulling its <see cref="User.ActivationGuid"/>.
        /// </summary>
        /// <param name="activationGuid">The activation guid used to find the user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a <see cref="Result{IUser}"/> that indicates whether the operation succeeded, and if positive it contains the activated user.</returns>
        Task<Result<IUser>> Activate(Guid activationGuid, CancellationToken cancellationToken = default);

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
