namespace RedCounterSoftware.Security.Jwt
{
    using System;

    public class ActivateUserModel
    {
        public Guid Id { get; set; }

        public string Password { get; set; }
    }
}
