namespace RedCounterSoftware.Logging.Web
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public static class LoggingHelper
    {
        public const string CurrentUserKey = "CurrentUser";

        public const string ImpersonatorKey = "Impersonator";

        public const string OriginalUserKey = "OriginalUser";

        public const string RemoteAddressKey = "RemoteAddress";

        public static IDisposable ScopeRemoteIp(this ILogger logger, HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var remoteIp = httpContext.Connection.RemoteIpAddress.ToString();
            remoteIp = remoteIp == "::1" ? "127.0.0.1" : remoteIp;
            return logger.BeginScope(KeyValuePair.Create(RemoteAddressKey, remoteIp));
        }

        public static IDisposable ScopeImpersonator(this ILogger logger, ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null)
            {
                throw new ArgumentNullException(nameof(claimsPrincipal));
            }

            return logger.BeginScope(KeyValuePair.Create(
                ImpersonatorKey,
                claimsPrincipal.HasClaim(c => c.Type == OriginalUserKey) ? claimsPrincipal.Claims.Single(c => c.Type == OriginalUserKey).Value : string.Empty));
        }

        public static IDisposable ScopeCurrentUser(this ILogger logger, ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null)
            {
                throw new ArgumentNullException(nameof(claimsPrincipal));
            }

            return logger.BeginScope(KeyValuePair.Create(CurrentUserKey, claimsPrincipal.Claims.Single(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value));
        }

        public static IDisposable GetCommonScopes(this ILogger logger, HttpContext httpContext, ClaimsPrincipal claimsPrincipal)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (claimsPrincipal == null)
            {
                throw new ArgumentNullException(nameof(claimsPrincipal));
            }

            return new LoggingScopes(logger, httpContext, claimsPrincipal);
        }

        public class LoggingScopes : IDisposable
        {
            private readonly IDisposable remoteIp;

            private readonly IDisposable impersonator;

            private readonly IDisposable currentUser;

            public LoggingScopes(ILogger logger, HttpContext httpContext, ClaimsPrincipal claimsPrincipal)
            {
                if (httpContext == null)
                {
                    throw new ArgumentNullException(nameof(httpContext));
                }

                if (claimsPrincipal == null)
                {
                    throw new ArgumentNullException(nameof(claimsPrincipal));
                }

                this.remoteIp = logger.ScopeRemoteIp(httpContext);
                this.impersonator = logger.ScopeImpersonator(claimsPrincipal);
                this.currentUser = logger.ScopeCurrentUser(claimsPrincipal);
            }

            public void Dispose()
            {
                this.remoteIp?.Dispose();
                this.impersonator?.Dispose();
                this.currentUser?.Dispose();
            }
        }
    }
}
