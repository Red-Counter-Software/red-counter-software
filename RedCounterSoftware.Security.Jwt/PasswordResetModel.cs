namespace RedCounterSoftware.Security.Jwt
{
    using System;

    public class PasswordResetModel
    {
        public Guid Id { get; set; }

        public string Password { get; set; }
    }
}
