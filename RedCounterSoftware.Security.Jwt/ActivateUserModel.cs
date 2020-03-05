namespace RedCounterSoftware.Security.Jwt
{
    using System;

    public class ActivateUserModel
    {
        public ActivateUserModel()
        {
            this.Password = string.Empty;
        }

        public Guid Id { get; set; }

        public string Password { get; set; }
    }
}
