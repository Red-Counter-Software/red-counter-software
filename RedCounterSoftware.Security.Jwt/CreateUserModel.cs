namespace RedCounterSoftware.Security.Jwt
{
    using System.Collections.ObjectModel;

    public class CreateUserModel
    {
        public string Email { get; set; } = string.Empty;

        public string PersonId { get; set; } = string.Empty;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Client binding")]
        public Collection<string> Roles { get; set; } = [];
    }
}
