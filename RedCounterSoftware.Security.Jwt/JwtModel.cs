namespace RedCounterSoftware.Security.Jwt
{
    using System;

    public class JwtModel
    {
        public JwtModel()
        {
            this.Token = string.Empty;
            this.LightweightToken = string.Empty;
        }

        public DateTime ExpiresAt { get; set; }

        public string Token { get; set; }

        public string LightweightToken { get; set; }
    }
}
