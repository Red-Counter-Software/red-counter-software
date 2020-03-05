namespace RedCounterSoftware.Security.Jwt
{
    using System.Collections.Generic;

    public class CreateUserModel
    {
        public CreateUserModel()
        {
            this.Email = string.Empty;
            this.PersonId = string.Empty;
            this.Roles = new List<string>();
        }

        public string Email { get; set; }

        public string PersonId { get; set; }

        public List<string> Roles { get; set; }
    }
}
