namespace RedCounterSoftware.Common.Account
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class Sha256PasswordCryptoService : IPasswordCryptoService
    {
        public byte[] EncryptPassword(string password, string salt = "")
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));
            ArgumentException.ThrowIfNullOrWhiteSpace(salt, nameof(salt));

            var toEncrypt = password + salt.ToUpperInvariant();

            var data = Encoding.ASCII.GetBytes(toEncrypt);
            var sha256Data = SHA256.HashData(data);
            return sha256Data;
        }
    }
}
