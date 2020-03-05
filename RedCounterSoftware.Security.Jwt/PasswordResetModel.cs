namespace RedCounterSoftware.Security.Jwt
{
    using System;

    public class PasswordResetModel
    {
        public PasswordResetModel()
        {
            this.Password = string.Empty;
        }

        public Guid Id { get; set; }

        public string Password { get; set; }
    }
}
