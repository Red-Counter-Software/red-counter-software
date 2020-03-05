namespace RedCounterSoftware.Security.Jwt
{
    public class PasswordResetRequestModel
    {
        public PasswordResetRequestModel()
        {
            this.Email = string.Empty;
        }

        public string Email { get; set; }
    }
}
