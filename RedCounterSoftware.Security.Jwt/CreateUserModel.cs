namespace RedCounterSoftware.Security.Jwt
{
    using System.Collections.Generic;

    public class CreateUserModel
    {
        public string Email { get; set; }

        public string PersonId { get; set; }

        public List<string> Roles { get; set; }
    }
}
