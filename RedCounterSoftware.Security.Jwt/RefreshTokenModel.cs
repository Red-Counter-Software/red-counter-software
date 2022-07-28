namespace RedCounterSoftware.Security.Jwt
{
    using System;

    public class RefreshTokenModel
    {
        public RefreshTokenModel(string associatedJwt, int lifespanInMinutes = 0)
        {
            this.Token = Guid.NewGuid();
            this.IssuedAt = DateTime.Now;
            this.IsValid = true;
            this.ExpiryDate = (lifespanInMinutes > 0.0) ? DateTime.Now.AddMinutes(lifespanInMinutes) : null;
            this.AssociatedJwt = associatedJwt;
        }

        public Guid Token { get; set; }

        public DateTime IssuedAt { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public bool IsUsed { get; set; }

        public bool IsValid { get; set; }

        public string AssociatedJwt { get; set; }
    }
}
