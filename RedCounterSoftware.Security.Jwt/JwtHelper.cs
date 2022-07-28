﻿namespace RedCounterSoftware.Security.Jwt
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

        public static JwtModel BuildToken(IUser user, IPerson person, string securityKey, string issuer, string audience, string[] permissions, int expirationInMinutes = 525600, string impersonatingUser = "")
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (person == null)
            {
                throw new ArgumentNullException(nameof(person));
            }

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

            if (permissions == null)
            {
                permissions = Array.Empty<string>();
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = BuildClaims(user, person, permissions, impersonatingUser);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                expires: DateTime.Now.AddMinutes(expirationInMinutes),
                signingCredentials: creds,
                claims: claims);

            var jsonToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new JwtModel { ExpiresAt = token.ValidTo, Token = jsonToken };
        }

        public static RefreshTokenModel BuildRefreshToken(Guid jwtId, int? expirationInMinutes)
        {
            return expirationInMinutes != null ? new RefreshTokenModel(jwtId, (int)expirationInMinutes) : new RefreshTokenModel(jwtId);
        }

        public static Task<bool> AreTokensValid(JwtModel token, RefreshTokenModel refreshToken, TokenValidationParameters validationParameters)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (refreshToken == null)
            {
                throw new ArgumentNullException(nameof(refreshToken));
            }

            if (validationParameters == null)
            {
                throw new ArgumentNullException(nameof(validationParameters));
            }

            var validatedToken = GetPrincipalFromToken(token, validationParameters);
            var expirationDate = Convert.ToDateTime(validatedToken!.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Exp).Value, CultureInfo.InvariantCulture);

            if (expirationDate > DateTime.Now)
            {
                throw new InvalidTokenException("The Token is not expired yet");
            }

            if (DateTime.Now > refreshToken.ExpiryDate)
            {
                throw new InvalidTokenException("The RefreshToken for this Token has expired");
            }

            if (!refreshToken.IsValid)
            {
                throw new InvalidTokenException("The RefreshToken for this Token has been invalidated");
            }

            if (!refreshToken.IsUsed)
            {
                throw new InvalidTokenException("The RefreshToken for this Token has already been used");
            }

            var jti = validatedToken!.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
            return refreshToken.JwtId.ToString() != jti
                ? throw new InvalidTokenException("This RefreshToken does not exist")
                : Task.FromResult(true);
        }

        private static IEnumerable<Claim> BuildClaims(IUser user, IPerson person, string[] permissions, string impersonatingUser)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var userId = user.Id.ToString() ?? string.Empty;

            List<Claim> claims;

            // Lightweight token
            if (!permissions.Any())
            {
                claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.UniqueName, userId)
                };
            }

            // Normal token
            else
            {
                claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Birthdate, person.BirthDate.ToString(CultureInfo.InvariantCulture)),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.UniqueName, userId),
                    new Claim(JwtRegisteredClaimNames.GivenName, person.FirstName),
                    new Claim(JwtRegisteredClaimNames.FamilyName, person.LastName)
                };

                claims.AddRange(permissions.Select(permission => new Claim(ClaimTypes.Role, permission)));
            }

            if (impersonatingUser == null)
            {
                return claims;
            }

            claims.Add(new Claim(OriginalUserClaimType, impersonatingUser));

            return claims;
        }

        private static ClaimsPrincipal GetPrincipalFromToken(JwtModel token, TokenValidationParameters validationParameters)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token.Token, validationParameters, out var validatedToken);
            return HasValidSecurityAlgorithm(validatedToken) ? principal : null!;
        }

        private static bool HasValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase);
        }
    }
}
