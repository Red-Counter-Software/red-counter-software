namespace RedCounterSoftware.Security.Jwt
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RedCounterSoftware.Common.Account;
    using RedCounterSoftware.Common.Logging;
    using RedCounterSoftware.Logging.Web;

    public abstract class ReadOnlyJwtController : Controller
    {
        private readonly string jwtKey;

        private readonly string jwtIssuer;

        private readonly string jwtAudience;

        private readonly IReadAuthenticationService authenticationService;

        private readonly ILogger<JwtController> logger;

        private readonly IRoleService roleService;

        private readonly IPersonService personService;

        protected ReadOnlyJwtController(
            IReadAuthenticationService authenticationService,
            IRoleService roleService,
            IPersonService personService,
            ILogger<JwtController> logger,
            string jwtKey,
            string jwtIssuer,
            string jwtAudience)
        {
            this.authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));

            this.roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));

            this.personService = personService ?? throw new ArgumentNullException(nameof(personService));

            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this.jwtKey = jwtKey ?? throw new ArgumentNullException(nameof(jwtKey));

            this.jwtIssuer = jwtIssuer ?? throw new ArgumentNullException(nameof(jwtIssuer));

            this.jwtAudience = jwtAudience ?? throw new ArgumentNullException(nameof(jwtAudience));
        }

        [HttpPost("createtoken")]
        public async Task<IActionResult> CreateToken([FromBody] LoginModel loginModel)
        {
            if (loginModel == null)
            {
                throw new ArgumentNullException(nameof(loginModel));
            }

            using (this.logger.BeginScope(LoggingEvents.Authentication))
            using (this.logger.ScopeRemoteIp(this.HttpContext))
            {
                this.logger.LogInformation(LoggingEvents.Authentication, "JWT Token requested for user {user}", loginModel.Username);

                var isAuthorized = await this.authenticationService.IsPasswordValid(loginModel.Username, loginModel.Password).ConfigureAwait(false);

                if (!isAuthorized)
                {
                    this.logger.LogInformation(LoggingEvents.AuthenticationFail, "Invalid credentials for user {user}", loginModel.Username);
                    return this.Unauthorized();
                }

                var user = await this.authenticationService.GetByEmail(loginModel.Username, CancellationToken.None).ConfigureAwait(false);

                var isValidStatus = await this.authenticationService.IsUserActive(user).ConfigureAwait(false);

                if (!isValidStatus)
                {
                    this.logger.LogInformation(LoggingEvents.AuthenticationFail, "User {user} is inactive or locked out", loginModel.Username);
                    return this.Forbid();
                }

                var token = await this.CreateToken(loginModel, user, true).ConfigureAwait(false);
                var lightweightToken = await this.CreateToken(loginModel, user, false).ConfigureAwait(false);

                return this.Ok(new JwtModel { ExpiresAt = token.ExpiresAt, Token = token.Token, LightweightToken = lightweightToken.Token });
            }
        }

        private async Task<JwtModel> CreateToken(LoginModel loginModel, IUser user, bool includeRoles)
        {
            if (loginModel == null)
            {
                throw new ArgumentNullException(nameof(loginModel));
            }

            var roles = includeRoles ? (await this.roleService.GetByUserId(user.Id).ConfigureAwait(false)).SelectMany(c => c.Claims).ToArray() : Array.Empty<string>();
            var person = await this.personService.GetById(user.PersonId, CancellationToken.None).ConfigureAwait(false);
            var token = JwtHelper.BuildToken(user, person, this.jwtKey, this.jwtIssuer, this.jwtAudience, roles);

            this.logger.LogInformation(LoggingEvents.AuthenticationOk, "Jwt Token created for user {user}", loginModel.Username);
            return token;
        }
    }
}
