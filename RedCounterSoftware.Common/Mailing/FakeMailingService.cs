namespace RedCounterSoftware.Common.Mailing
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Common.Mailing;

    public class FakeMailingService : IMailingService
    {
        public void Dispose()
        {
        }

        public Task SendActivationEmail(string email, Guid activationGuid, string subject, string textBody, string htmlBody, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task SendPasswordRecoveryMail(string email, Guid passwordResetGuid, string subject, string textBody, string htmlBody, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
