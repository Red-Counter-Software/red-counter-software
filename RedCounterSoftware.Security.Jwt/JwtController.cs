namespace RedCounterSoftware.Security.Jwt
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Common.Account;
    using Common.Logging;
    using Common.Mailing;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using RedCounterSoftware.Logging.Web;

    [AllowAnonymous]
    public abstract class JwtController : ReadOnlyJwtController
    {
        private readonly IAuthenticationService authenticationService;

        private readonly IMailingService mailingService;

        private readonly ILogger<JwtController> logger;

        private readonly string passwordResetSubject;

        private readonly string passwordResetTextBody;

        private readonly string passwordResetHtmlBody;

        protected JwtController(
            IAuthenticationService authenticationService,
            IRoleService roleService,
            IPersonService personService,
            IMailingService mailingService,
            ILogger<JwtController> logger,
            string jwtKey,
            string jwtIssuer,
            string jwtAudience,
            string passwordResetSubject,
            string passwordResetTextBody,
            string passwordResetHtmlBody)
            : base(authenticationService, roleService, personService, logger, jwtKey, jwtIssuer, jwtAudience)
        {
            this.authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));

            this.mailingService = mailingService ?? throw new ArgumentNullException(nameof(mailingService));

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this.passwordResetSubject = passwordResetSubject ?? throw new ArgumentNullException(nameof(passwordResetSubject));

            this.passwordResetTextBody = passwordResetTextBody ?? throw new ArgumentNullException(nameof(passwordResetTextBody));

            this.passwordResetHtmlBody = passwordResetHtmlBody ?? throw new ArgumentNullException(nameof(passwordResetHtmlBody));
        }

        [HttpPost("activate")]
        public async Task<IActionResult> ActivateUser([FromBody] ActivateUserModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            using (this.logger.BeginScope(LoggingEvents.Activation))
            using (this.logger.ScopeRemoteIp(this.HttpContext))
            {
                var result = await this.authenticationService.Activate(model.Id).ConfigureAwait(false);

                if (!result.IsValid)
                {
                    this.logger.LogInformation(LoggingEvents.ActivationFail, "An activation attempt with an invalid code [{code}] has been made", model.Id);
                    return this.NotFound();
                }

                _ = await this.authenticationService.SetPassword(result.Item.Id, model.Password).ConfigureAwait(false);
                this.logger.LogInformation(LoggingEvents.ActivationOk, "User {user} with activation code [{code}] created a password and activated succesfully", result.Item.Email, model.Id);
                return this.Ok();
            }
        }

        [HttpPost("requestresetpassword")]
        public async Task<IActionResult> SendPasswordResetMail([FromBody] PasswordResetRequestModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            using (this.logger.BeginScope(LoggingEvents.ResetPassword))
            using (this.logger.ScopeRemoteIp(this.HttpContext))
            {
                var user = await this.authenticationService.GetByEmail(model.Email, CancellationToken.None).ConfigureAwait(false);
                if (user == null)
                {
                    this.logger.LogInformation(LoggingEvents.ResetPassword, "Attempted to reset password for non existing user {user}", model.Email);

                    // Intenzionale: non vogliamo far sapere a un malintenzionato se un utenza esiste o meno a sistema.
                    return this.Ok();
                }

                var guid = Guid.NewGuid();
                var result = await this.authenticationService.SetPasswordResetGuid(user.Id, guid, CancellationToken.None).ConfigureAwait(false);
                this.logger.LogInformation(LoggingEvents.ResetPassword, "Set password reset guid for user {user} to [{id}]", model.Email, guid);

                user = result.Item;

                await this.mailingService.SendPasswordRecoveryMail(user.Email, guid, this.passwordResetSubject, this.passwordResetTextBody, this.passwordResetHtmlBody, CancellationToken.None).ConfigureAwait(false);
                this.logger.LogInformation(LoggingEvents.ResetPassword, "Reset password email sent for user {user} with reset guid [{id}]", model.Email, guid);

                return this.Ok();
            }
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            using (this.logger.BeginScope(LoggingEvents.ResetPassword))
            using (this.logger.ScopeRemoteIp(this.HttpContext))
            {
                var user = await this.authenticationService.GetByPasswordResetGuid(model.Id, CancellationToken.None).ConfigureAwait(false);
                if (user == null)
                {
                    this.logger.LogInformation("No user found with matching password reset guid [{id}]", model.Id);
                    return this.NotFound();
                }

                var result = await this.authenticationService.SetPassword(user.Id, model.Password, CancellationToken.None).ConfigureAwait(false);
                if (result.IsValid)
                {
                    return this.Ok();
                }

                this.logger.LogInformation("Invalid user input: {input}", result.Failures.Select(c => c.ErrorMessage).Aggregate((s1, s2) => s1 + Environment.NewLine + s2));
                return new StatusCodeResult(422);
            }
        }
    }
}
