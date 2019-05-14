namespace RedCounterSoftware.Security.Jwt
{
    using System;

    public class JwtModel
    {
        public DateTime ExpiresAt { get; set; }

        public string Token { get; set; }

        public string LightweightToken { get; set; }
    }
}
