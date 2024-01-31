namespace RedCounterSoftware.Common.Account
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class Sha1PasswordCryptoService : IPasswordCryptoService
    {
        public byte[] EncryptPassword(string password, string salt = "")
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));
            ArgumentException.ThrowIfNullOrWhiteSpace(salt, nameof(salt));

            var toEncrypt = password + salt.ToUpperInvariant();

            var data = Encoding.ASCII.GetBytes(toEncrypt);
            using var sha1 = SHA256.Create();
            var sha1Data = sha1.ComputeHash(data);
            return sha1Data;
        }
    }
}
