namespace RedCounterSoftware.Common.Mailing
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Service that handles mailing.
    /// </summary>
    public interface IMailingService
    {
        /// <summary>
        /// Sends an activation mail to the provided <see cref="User"/> containing the link to follow to complete the registration.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <param name="activationGuid">The activation guid.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a completed task.</returns>
        Task SendActivationEmail(string email, Guid activationGuid, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a password recovery mail to the provided <see cref="User"/> containing a link to follow to generate a new password.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <param name="passwordResetGuid">The password reset guid.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a completed task.</returns>
        Task SendPasswordRecoveryMail(string email, Guid passwordResetGuid, CancellationToken cancellationToken = default);
    }
}
