namespace RedCounterSoftware.Common.Mailing
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class FakeMailingService : IMailingService
    {
        public Task SendActivationEmail(string email, Guid activationGuid, string subject, string textBody, string htmlBody, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task SendPasswordRecoveryMail(string email, Guid passwordResetGuid, string subject, string textBody, string htmlBody, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
