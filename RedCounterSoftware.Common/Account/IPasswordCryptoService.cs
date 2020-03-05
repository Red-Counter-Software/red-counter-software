namespace RedCounterSoftware.Common.Account
{
    /// <summary>
    /// Service that handles password encryption.
    /// </summary>
    public interface IPasswordCryptoService
    {
        /// <summary>
        /// Encrypts the provided <see cref="password"/> with optional <see cref="salt"/> and returns the corresponding byte array.
        /// </summary>
        /// <param name="password">The password to encrypt.</param>
        /// <param name="salt">The optional salt.</param>
        /// <returns>Returns the encrypted password in the form of <see cref="byte"/> array.</returns>
        byte[] EncryptPassword(string password, string salt = "");
    }
}
