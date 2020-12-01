namespace RedCounterSoftware.Logging.Web
{
    using System;
    using System.Security.Claims;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public class LoggingScopes : IDisposable
    {
        private readonly IDisposable remoteIp;

        private readonly IDisposable impersonator;

        private readonly IDisposable currentUser;

        private bool disposedValue;

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
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.remoteIp?.Dispose();
                    this.impersonator?.Dispose();
                    this.currentUser?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                this.disposedValue = true;
            }
        }
    }
}
