namespace RedCounterSoftware.Security.Jwt
{
    using System.Collections.ObjectModel;

    public class CreateUserModel
    {
        public CreateUserModel()
        {
            this.Email = string.Empty;
            this.PersonId = string.Empty;
            this.Roles = new Collection<string>();
        }

        public string Email { get; set; }

        public string PersonId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Client binding")]
        public Collection<string> Roles { get; set; }
    }
}
