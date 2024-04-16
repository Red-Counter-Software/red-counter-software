namespace RedCounterSoftware.Security.Jwt
{
    using System;

    public class RefreshTokenModel(string associatedJwt, int lifespanInMinutes = 0)
    {
        public Guid Token { get; set; } = Guid.NewGuid();

        public DateTime IssuedAt { get; set; } = DateTime.Now;

        public DateTime? ExpiryDate { get; set; } = (lifespanInMinutes > 0.0) ? DateTime.Now.AddMinutes(lifespanInMinutes) : null;

        public bool IsUsed { get; set; }

        public bool IsValid { get; set; } = true;

        public string AssociatedJwt { get; set; } = associatedJwt;
    }
}
