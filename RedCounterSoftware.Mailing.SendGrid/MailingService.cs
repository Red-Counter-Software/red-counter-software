#pragma warning disable SA1200 // Using directives should be placed correctly
using SendGrid;
using SendGrid.Helpers.Mail;
#pragma warning restore SA1200 // Using directives should be placed correctly

namespace RedCounterSoftware.Mailing.SendGrid
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using Common.Mailing;

    public class MailingService : IMailingService
    {
        private readonly string sendGridApiKey;

        private readonly Uri baseUrl;

        public MailingService(string sendGridApiKey, Uri baseUrl)
        {
            if (string.IsNullOrEmpty(sendGridApiKey))
            {
                throw new ArgumentException("Cannot be empty", nameof(sendGridApiKey));
            }

            this.sendGridApiKey = sendGridApiKey;

            this.baseUrl = baseUrl ?? throw new ArgumentException("Cannot be empty", nameof(baseUrl));
        }

        public async Task SendActivationEmail(string email, Guid activationGuid, string subject, string textBody, string htmlBody, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (activationGuid == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(activationGuid));
            }

            var client = new SendGridClient(this.sendGridApiKey);
            var from = new EmailAddress("noreply@steelchihuahua.com", "NoReply");
            var to = new EmailAddress(email);
            var plainTextContent = string.Format(CultureInfo.InvariantCulture, textBody, this.baseUrl, activationGuid);
            var url = string.Format(CultureInfo.InvariantCulture, "<a href=\"{0}/public/user/activate/{1}\">link</a>", this.baseUrl, activationGuid);
            var htmlContent = string.Format(CultureInfo.InvariantCulture, htmlBody, url);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg, cancellationToken).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                var body = await response.DeserializeResponseBodyAsync(response.Body).ConfigureAwait(false);
                var error =
                    $"Error while using Sendgrid service. Status code: {response.StatusCode} - Error details: {body}";
                throw new InvalidOperationException(error);
            }
        }

        public async Task SendPasswordRecoveryMail(string email, Guid passwordResetGuid, string subject, string textBody, string htmlBody, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email));
            }

            if (passwordResetGuid == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(passwordResetGuid));
            }

            var client = new SendGridClient(this.sendGridApiKey);
            var from = new EmailAddress("noreply@steelchihuahua.com", "NoReply");
            var to = new EmailAddress(email);
            var plainTextContent = string.Format(CultureInfo.InvariantCulture, textBody, this.baseUrl, passwordResetGuid);
            var url = string.Format(CultureInfo.InvariantCulture, "<a href=\"{0}/public/user/password-reset/{1}\">link</a>", this.baseUrl, passwordResetGuid);
            var htmlContent = string.Format(CultureInfo.InvariantCulture, htmlBody, url);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg, cancellationToken).ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                var body = await response.DeserializeResponseBodyAsync(response.Body).ConfigureAwait(false);
                var error =
                    $"Error while using Sendgrid service. Status code: {response.StatusCode} - Error details: {body}";
                throw new InvalidOperationException(error);
            }
        }
    }
}
