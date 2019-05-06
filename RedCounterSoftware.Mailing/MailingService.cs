using SendGrid;
using SendGrid.Helpers.Mail;

namespace RedCounterSoftware.Mailing.SendGrid
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using Common.Mailing;

    public class SendGridMailingService : IMailingService
    {
        private readonly string sendGridApiKey;

        private readonly string baseUrl;

        public SendGridMailingService(string sendGridApiKey, string baseUrl)
        {
            if (string.IsNullOrEmpty(sendGridApiKey))
            {
                throw new ArgumentException("Cannot be empty", nameof(sendGridApiKey));
            }

            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentException("Cannot be empty", nameof(baseUrl));
            }

            this.sendGridApiKey = sendGridApiKey;

            this.baseUrl = baseUrl;
        }

        public async Task SendActivationEmail(string email, Guid activationGuid, string subject, string textBody, string htmlBody, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException(nameof(email));
            }

            if (activationGuid == Guid.Empty)
            {
                throw new ArgumentException(nameof(activationGuid));
            }

            var client = new SendGridClient(this.sendGridApiKey);
            var from = new EmailAddress("noreply@steelchihuahua.com", "NoReply");
            var to = new EmailAddress(email);
            var plainTextContent = string.Format(textBody, this.baseUrl, activationGuid);
            var url = string.Format("<a href=\"{0}/public/user/activate/{1}\">link</a>", this.baseUrl, activationGuid);
            var htmlContent = string.Format(htmlBody, url);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg, cancellationToken);
            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                var body = await response.DeserializeResponseBodyAsync(response.Body);
                var error =
                    $"Error while using Sendgrid service. Status code: {response.StatusCode} - Error details: {body}";
                throw new Exception(error);
            }
        }

        public async Task SendPasswordRecoveryMail(string email, Guid passwordResetGuid, string subject, string textBody, string htmlBody, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException(nameof(email));
            }

            if (passwordResetGuid == Guid.Empty)
            {
                throw new ArgumentException(nameof(passwordResetGuid));
            }

            var client = new SendGridClient(this.sendGridApiKey);
            var from = new EmailAddress("noreply@steelchihuahua.com", "NoReply");
            var to = new EmailAddress(email);
            var plainTextContent = string.Format(textBody, this.baseUrl, passwordResetGuid);
            var url = string.Format("<a href=\"{0}/public/user/password-reset/{1}\">link</a>", this.baseUrl, passwordResetGuid);
            var htmlContent = string.Format(htmlBody, url);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg, cancellationToken);
            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                var body = await response.DeserializeResponseBodyAsync(response.Body);
                var error =
                    $"Error while using Sendgrid service. Status code: {response.StatusCode} - Error details: {body}";
                throw new Exception(error);
            }
        }

        public void Dispose()
        {
        }
    }
}
