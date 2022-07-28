namespace RedCounterSoftware.Security.Jwt.Exceptions
{
    using System;
    using System.Globalization;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implementare costruttori di eccezioni standard", Justification = "Must not be initializated without message")]
    public class InvalidTokenException : Exception
    {
        public InvalidTokenException(string message)
            : base(string.Format(CultureInfo.InvariantCulture, "[InvalidToken] {0}", message))
        {
        }

        public InvalidTokenException(string message, Exception innerException)
            : base(string.Format(CultureInfo.InvariantCulture, "[InvalidToken] {0}", message), innerException)
        {
        }
    }
}
