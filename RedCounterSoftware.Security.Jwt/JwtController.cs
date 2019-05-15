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
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using RedCounterSoftware.Logging.Web;

    [AllowAnonymous]
    public abstract class JwtController : Controller
    {
        private readonly IAuthenticationService authenticationService;

        private readonly IRoleStoreService roleStoreService;

        private readonly IPersonStoreService personStoreService;

        private readonly IMailingService mailingService;

        private readonly IConfiguration configuration;

        private readonly ILogger<JwtController> logger;

        private readonly string passwordResetSubject;

        private readonly string passwordResetTextBody;

        private readonly string passwordResetHtmlBody;

        protected JwtController(
            IAuthenticationService authenticationService,
            IRoleStoreService roleStoreService,
            IPersonStoreService personStoreService,
            IMailingService mailingService,
            IConfiguration configuration,
            ILogger<JwtController> logger,
            string passwordResetSubject,
            string passwordResetTextBody,
            string passwordResetHtmlBody)
        {
            this.authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));

            this.roleStoreService = roleStoreService ?? throw new ArgumentNullException(nameof(roleStoreService));

            this.personStoreService = personStoreService ?? throw new ArgumentNullException(nameof(personStoreService));

            this.mailingService = mailingService ?? throw new ArgumentNullException(nameof(mailingService));

            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this.passwordResetSubject = passwordResetSubject ?? throw new ArgumentNullException(nameof(passwordResetSubject));

            this.passwordResetTextBody = passwordResetTextBody ?? throw new ArgumentNullException(nameof(passwordResetTextBody));

            this.passwordResetHtmlBody = passwordResetHtmlBody ?? throw new ArgumentNullException(nameof(passwordResetHtmlBody));
        }

        [HttpPost]
        public async Task<IActionResult> ActivateUser([FromBody]ActivateUserModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            using (this.logger.BeginScope(LoggingEvents.Activation))
            using (this.logger.ScopeRemoteIp(this.HttpContext))
            {
                var result = await this.authenticationService.Activate(model.Id);

                if (!result.IsValid)
                {
                    this.logger.LogInformation(LoggingEvents.ActivationFail, "An activation attempt with an invalid code [{code}] has been made", model.Id);
                    return this.NotFound();
                }

                await this.authenticationService.SetPassword(result.Item.Id, model.Password);
                this.logger.LogInformation(LoggingEvents.ActivationOk, "User {user} with activation code [{code}] created a password and activated succesfully", result.Item.Email, model.Id);
                return this.Ok();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody]LoginModel loginModel)
        {
            if (loginModel == null)
            {
                throw new ArgumentNullException(nameof(loginModel));
            }

            using (this.logger.BeginScope(LoggingEvents.Authentication))
            using (this.logger.ScopeRemoteIp(this.HttpContext))
            {
                this.logger.LogInformation(LoggingEvents.Authentication, "JWT Token requested for user {user}", loginModel.Username);

                var isAuthorized = await this.authenticationService.IsPasswordValid(loginModel.Username, loginModel.Password);

                if (!isAuthorized)
                {
                    this.logger.LogInformation(LoggingEvents.AuthenticationFail, "Invalid credentials for user {user}", loginModel.Username);
                    return this.Unauthorized();
                }

                var user = await this.authenticationService.GetBy(u => u.Email, loginModel.Username, CancellationToken.None);

                var isValidStatus = await this.authenticationService.IsUserActive(user);

                if (!isValidStatus)
                {
                    this.logger.LogInformation(LoggingEvents.AuthenticationFail, "User {user} is inactive or locked out", loginModel.Username);
                    return this.Forbid();
                }

                var token = await this.CreateToken(loginModel, user, true);
                var lightweightToken = await this.CreateToken(loginModel, user, false);

                return this.Ok(new JwtModel { ExpiresAt = token.ExpiresAt, Token = token.Token, LightweightToken = lightweightToken.Token });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendPasswordResetMail([FromBody] PasswordResetRequestModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            using (this.logger.BeginScope(LoggingEvents.ResetPassword))
            using (this.logger.ScopeRemoteIp(this.HttpContext))
            {
                var user = await this.authenticationService.GetBy(u => u.Email, model.Email);
                if (user == null)
                {
                    this.logger.LogInformation(LoggingEvents.ResetPassword, "Attempted to reset password for non existing user {user}", model.Email);

                    // Intenzionale: non vogliamo far sapere a un malintenzionato se un utenza esiste o meno a sistema.
                    return this.Ok();
                }

                var guid = Guid.NewGuid();
                var result = await this.authenticationService.Patch(user.Id, c => c.PasswordResetGuid, guid, CancellationToken.None);
                this.logger.LogInformation(LoggingEvents.ResetPassword, "Set password reset guid for user {user} to [{id}]", model.Email, guid);

                user = result.Item;

                await this.mailingService.SendPasswordRecoveryMail(user.Email, guid, this.passwordResetSubject, this.passwordResetTextBody, this.passwordResetHtmlBody, CancellationToken.None);
                this.logger.LogInformation(LoggingEvents.ResetPassword, "Reset password email sent for user {user} with reset guid [{id}]", model.Email, guid);

                return this.Ok();
            }
        }

        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            using (this.logger.BeginScope(LoggingEvents.ResetPassword))
            using (this.logger.ScopeRemoteIp(this.HttpContext))
            {
                var user = await this.authenticationService.GetBy(u => u.PasswordResetGuid, model.Id, CancellationToken.None);
                if (user == null)
                {
                    this.logger.LogInformation("No user found with matching password reset guid [{id}]", model.Id);
                    return this.NotFound();
                }

                var result = await this.authenticationService.SetPassword(user.Id, model.Password, CancellationToken.None);
                if (result.IsValid)
                {
                    return this.Ok();
                }

                this.logger.LogInformation("Invalid user input: {input}", result.Failures.Select(c => c.ErrorMessage).Aggregate((s1, s2) => s1 + Environment.NewLine + s2));
                return new StatusCodeResult(422);
            }
        }

        private async Task<JwtModel> CreateToken(LoginModel loginModel, IUser user, bool includeRoles)
        {
            if (loginModel == null)
            {
                throw new ArgumentNullException(nameof(loginModel));
            }

            var roles = includeRoles ? (await this.roleStoreService.GetByUserId(user.Id)).SelectMany(c => c.Claims).ToArray() : null;
            var person = await this.personStoreService.GetBy(c => c.Id, user.PersonId);
            var token = JwtHelper.BuildToken(user, person, this.configuration["Jwt:Key"], this.configuration["Jwt:Issuer"], this.configuration["Jwt:Audience"], roles);

            this.logger.LogInformation(LoggingEvents.AuthenticationOk, "Jwt Token created for user {user}", loginModel.Username);
            return token;
        }
    }
}
