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
        /// <param name="subject">The subject of the mail.</param>
        /// <param name="textBody">The body of the mail in plain text. Must contain two placeholders, the first for the base url, the second for the activation guid.</param>
        /// <param name="htmlBody">The body of the mail in html. Must contain a placeholder for the activation url.</param>
        /// /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a completed task.</returns>
        Task SendActivationEmail(string email, Guid activationGuid, string subject, string textBody, string htmlBody, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a password recovery mail to the provided <see cref="User"/> containing a link to follow to generate a new password.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <param name="passwordResetGuid">The password reset guid.</param>
        /// /// <param name="subject">The subject of the mail.</param>
        /// <param name="textBody">The body of the mail in plain text. Must contain two placeholders, the first for the base url, the second for the password reset guid.</param>
        /// <param name="htmlBody">The body of the mail in html. Must contain a placeholder for the password reset url.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a completed task.</returns>
        Task SendPasswordRecoveryMail(string email, Guid passwordResetGuid, string subject, string textBody, string htmlBody, CancellationToken cancellationToken = default);
    }
}
