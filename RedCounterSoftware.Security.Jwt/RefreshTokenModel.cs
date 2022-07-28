namespace RedCounterSoftware.Security.Jwt
{
    using System;

    public class RefreshTokenModel
    {
        public RefreshTokenModel(Guid jwtId, int lifespanInMinutes = 0)
        {
            this.Token = Guid.NewGuid();
            this.IssuedAt = DateTime.Now;
            this.IsValid = true;
            this.ExpiryDate = (lifespanInMinutes > 0.0) ? DateTime.Now.AddMinutes(lifespanInMinutes) : null;
            this.JwtId = jwtId;
        }

        public Guid Token { get; set; }

        public DateTime IssuedAt { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public bool IsUsed { get; set; }

        public bool IsValid { get; set; }

        public Guid JwtId { get; set; }
    }
}
