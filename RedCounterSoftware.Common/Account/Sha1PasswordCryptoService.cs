namespace RedCounterSoftware.Common.Account
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class Sha1PasswordCryptoService : IPasswordCryptoService
    {
        public byte[] EncryptPassword(string password, string salt = null)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Cannot be empty", nameof(password));
            }

            if (salt == string.Empty)
            {
                throw new ArgumentException("Salt cannot be an empty string", nameof(salt));
            }

            var toEncrypt = salt != null ? password + salt.ToLowerInvariant() : password;

            var data = Encoding.ASCII.GetBytes(toEncrypt);
            var sha1 = new SHA1CryptoServiceProvider();
            var sha1Data = sha1.ComputeHash(data);
            return sha1Data;
        }
    }
}
