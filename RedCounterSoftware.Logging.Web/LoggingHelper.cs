namespace RedCounterSoftware.Logging.Web
{
    using System;
    using System.Collections.Generic;
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

        public static IDisposable? ScopeRemoteIp(this ILogger logger, HttpContext httpContext)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var remoteIp = httpContext.Connection?.RemoteIpAddress?.ToString();
            remoteIp = remoteIp == "::1" ? "127.0.0.1" : remoteIp;
            return logger.BeginScope(KeyValuePair.Create(RemoteAddressKey, remoteIp));
        }

        public static IDisposable? ScopeImpersonator(this ILogger logger, ClaimsPrincipal claimsPrincipal)
        {
            return logger == null
                ? throw new ArgumentNullException(nameof(logger))
                : claimsPrincipal == null
                ? throw new ArgumentNullException(nameof(claimsPrincipal))
                : logger.BeginScope(KeyValuePair.Create(
                ImpersonatorKey,
                claimsPrincipal.HasClaim(c => c.Type == OriginalUserKey) ? claimsPrincipal.Claims.Single(c => c.Type == OriginalUserKey).Value : string.Empty));
        }

        public static IDisposable? ScopeCurrentUser(this ILogger logger, ClaimsPrincipal claimsPrincipal)
        {
            return logger == null
                ? throw new ArgumentNullException(nameof(logger))
                : claimsPrincipal == null
                ? throw new ArgumentNullException(nameof(claimsPrincipal))
                : logger.BeginScope(KeyValuePair.Create(
                CurrentUserKey,
                claimsPrincipal.HasClaim(c => c.Type == ClaimTypes.Name) ? claimsPrincipal.Claims.Single(c => c.Type == ClaimTypes.Name).Value : string.Empty));
        }

        public static IDisposable GetCommonScopes(this ILogger logger, HttpContext httpContext, ClaimsPrincipal claimsPrincipal)
        {
            return logger == null
                ? throw new ArgumentNullException(nameof(logger))
                : httpContext == null
                ? throw new ArgumentNullException(nameof(httpContext))
                : (claimsPrincipal == null
                ? throw new ArgumentNullException(nameof(claimsPrincipal))
                : new LoggingScopes(logger, httpContext, claimsPrincipal));
        }
    }
}
