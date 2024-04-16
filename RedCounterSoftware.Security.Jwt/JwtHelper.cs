namespace RedCounterSoftware.Security.Jwt
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.IdentityModel.Tokens;
    using RedCounterSoftware.Common.Account;
    using RedCounterSoftware.Security.Jwt.Exceptions;

    public static class JwtHelper
    {
        public const string OriginalUserClaimType = "OriginalUser";

        public static JwtModel BuildToken(IUser user, IPerson person, string securityKey, string issuer, string audience, string[] permissions, int expirationInMinutes = 525600, string impersonatingUser = "", Claim[]? initializedPermissions = null)
        {
            ArgumentNullException.ThrowIfNull(user);
            ArgumentNullException.ThrowIfNull(person);

            if (string.IsNullOrEmpty(securityKey))
            {
                throw new ArgumentException("Cannot be empty", nameof(securityKey));
            }

            if (string.IsNullOrEmpty(issuer))
            {
                throw new ArgumentException("Cannot be empty", nameof(issuer));
            }

            if (string.IsNullOrEmpty(audience))
            {
                throw new ArgumentException("Cannot be empty", nameof(audience));
            }

            permissions ??= [];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = BuildClaims(user, person, permissions, impersonatingUser, initializedPermissions);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                expires: DateTime.Now.AddMinutes(expirationInMinutes),
                signingCredentials: creds,
                claims: claims);

            var jsonToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new JwtModel { ExpiresAt = token.ValidTo, Token = jsonToken };
        }

        public static RefreshTokenModel BuildRefreshToken(string associatedJwt, int? expirationInMinutes)
        {
            return expirationInMinutes != null ? new RefreshTokenModel(associatedJwt, (int)expirationInMinutes) : new RefreshTokenModel(associatedJwt);
        }

        public static Task<bool> AreTokensValid(string token, RefreshTokenModel refreshToken, TokenValidationParameters validationParameters)
        {
            ArgumentNullException.ThrowIfNull(token);
            ArgumentNullException.ThrowIfNull(refreshToken);
            ArgumentNullException.ThrowIfNull(validationParameters);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtExpiration = tokenHandler.ReadJwtToken(token)!.ValidTo;

            bool checksResult = jwtExpiration > DateTime.Now.ToUniversalTime()
                ? throw new InvalidTokenException("The Token is not expired yet")
                : DateTime.Now > refreshToken.ExpiryDate
                ? throw new InvalidTokenException("The RefreshToken for this Token has expired")
                : !refreshToken.IsValid
                ? throw new InvalidTokenException("The RefreshToken for this Token has been invalidated")
                : refreshToken.IsUsed
                ? throw new InvalidTokenException("The RefreshToken for this Token has already been used")
                : refreshToken.AssociatedJwt != token
                ? throw new InvalidTokenException("This is not this Token's RefreshToken")
                : true;

            return Task.FromResult(checksResult);
        }

        private static IEnumerable<Claim> BuildClaims(IUser user, IPerson person, string[] permissions, string impersonatingUser, Claim[]? initializedPermissions = null)
        {
            ArgumentNullException.ThrowIfNull(user);

            var userId = user.Id.ToString() ?? string.Empty;

            List<Claim> claims;

            // Lightweight token
            if (permissions.Length == 0 && initializedPermissions == null)
            {
                claims =
                [
                    new Claim(JwtRegisteredClaimNames.UniqueName, userId)
                ];
            }

            // Normal token
            else
            {
                claims =
                [
                    new Claim(JwtRegisteredClaimNames.Birthdate, person.BirthDate.ToString(CultureInfo.InvariantCulture)),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.UniqueName, userId),
                    new Claim(JwtRegisteredClaimNames.GivenName, person.FirstName),
                    new Claim(JwtRegisteredClaimNames.FamilyName, person.LastName),
                    .. permissions.Select(permission => new Claim(ClaimTypes.Role, permission)),
                ];

                if (initializedPermissions != null)
                {
                    claims.AddRange(initializedPermissions);
                    claims = claims.DistinctBy(x => new { x.ValueType, x.Value }).ToList();
                }
            }

            if (impersonatingUser == null)
            {
                return claims;
            }

            claims.Add(new Claim(OriginalUserClaimType, impersonatingUser));

            return claims;
        }
    }
}
