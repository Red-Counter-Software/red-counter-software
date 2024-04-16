namespace RedCounterSoftware.Security.Jwt
{
    using System;

    public class JwtModel
    {
        public DateTime ExpiresAt { get; set; }

        public string Token { get; set; } = string.Empty;

        public string LightweightToken { get; set; } = string.Empty;
    }
}
