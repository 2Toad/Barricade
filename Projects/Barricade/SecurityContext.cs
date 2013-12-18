/*
 * Barricade
 * Copyright (C)2013 2Toad, LLC.
 * licensing@2toad.com
 * 
 * http://2toad.com/Project/Barricade/License
 */

using System;
using System.Net.Http.Headers;
using System.Threading;
using Utopia.Security.Cryptography;

namespace Barricade
{
    public static class SecurityContext
    {
        #region Properties

        /// <summary>
        /// The pepper used to generate password hashes.
        /// </summary>
        public static string PasswordPepper { get; private set; }

        /// <summary>
        /// The encryption key used to secure bearer tokens.
        /// </summary>
        public static string BearerTokenKey { get; private set; }

        /// <summary>
        /// The header used to verify the authenticity of access tokens.
        /// </summary>
        public static string AccessTokenHeader { get; private set; }

        /// <summary>
        /// The number of minutes authenticated access tokens should remain cached. This is a sliding 
        /// expiration that is reset each time the cached access token is accessed.
        /// </summary>
        public static int AccessTokenCacheDuration { get; private set; }

        #endregion

        /// <summary>
        /// Configures the security context with application specific settings.
        /// </summary>
        /// <param name="passwordPepper">The pepper used to generate password hashes. This should be a minimum of 128-bits.</param>
        /// <param name="bearerTokenKey">The encryption key used to secure bearer tokens. This should be a minimum of 128-bits.</param>
        /// <param name="accessTokenHeader">The header used to verify the authenticity of access tokens.</param>
        /// <param name="accessTokenCacheDuration">The number of minutes authenticated access tokens should remain cached.</param>
        public static void Configure(string passwordPepper, string bearerTokenKey, string accessTokenHeader, int accessTokenCacheDuration)
        {
            PasswordPepper = passwordPepper;
            BearerTokenKey = bearerTokenKey;
            AccessTokenHeader = accessTokenHeader;
            AccessTokenCacheDuration = accessTokenCacheDuration;
        }

        /// <summary>
        /// Generates a bearer token for the specified user, and caches the
        /// associated access token for future access.
        /// </summary>
        /// <param name="user">The user to login.</param>
        /// <returns>A unique bearer token.</returns>
        public static TokenRequestResponse Login(IClaimUser user)
        {
            if (user == null) return null;

            Logout(user.AccessToken);
            Cache.Add(user.AccessToken, user, AccessTokenCacheDuration, true);

            return new TokenRequestResponse {
                access_token = GenerateBearerToken(user.AccessToken),
                expires_in = (long)(user.AccessTokenExpiration - DateTime.UtcNow).Value.TotalSeconds
            };
        }

        /// <summary>
        /// Removes the user, associated with the specified access token,
        /// from the cache. This should be coupled with a method that
        /// invalidates the token on the application side (e.g., remove
        /// it from the database).
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        public static void Logout(string accessToken)
        {
            Cache.Remove(accessToken);
        }

        /// <summary>
        /// Determines whether the specified access token is valid.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="getUser">The delegate that will be called if the user associated with the access token is not cached.</param>
        /// <returns><c>true</c> if a user is associated with the access token; otherwise <c>false</c>.</returns>
        public static bool ValidAccessToken(string accessToken, Func<string, IClaimUser> getUser)
        {
            if (String.IsNullOrWhiteSpace(accessToken)) return false;

            var user = Cache.Get<IClaimUser>(accessToken);
            if (user != null) return true;

            return Login(getUser(accessToken)) != null;
        }

        /// <summary>
        /// Generates a 128-bit MD5 HMAC using the specified password, salt, and application pepper.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <param name="passwordSalt">The salt for the hash.</param>
        /// <returns>A 128-bit hash.</returns>
        public static string GeneratePasswordHash(string password, string passwordSalt)
        {
            return Md5.Hmac(password, Md5.Hmac(passwordSalt, PasswordPepper));
        }

        /// <summary>
        /// Validates the specified credentials against the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="credentials">The credentials to validate.</param>
        /// <returns><c>true</c> if the credentials match the user; otherwise <c>false</c>.</returns>
        public static bool ValidatePassword(IClaimUser user, TokenRequestCredentials credentials)
        {
            return user.PasswordHash == GeneratePasswordHash(credentials.Password, user.PasswordSalt);
        }

        /// <summary>
        /// Determines whether the specified header contains an authenticated Accesss Token,
        /// and whether the user has the necessary <see cref="Claim"/> (if defined).
        /// </summary>
        /// <param name="authorization">The authentication header.</param>
        /// <param name="claim">The required claim.</param>
        /// <param name="getUser">The delegate that will be called if the user associated with the access token is not cached.</param>
        /// <returns><c>true</c> when authenticatd and authorized; otherwise <c>false</c>.</returns>
        public static bool IsAuthorized(AuthenticationHeaderValue authorization, IClaim claim, Func<string, IClaimUser> getUser)
        {
            var accessToken = GetAccessToken(authorization);
            return ValidAccessToken(accessToken, getUser)
                && (claim == null || HasClaim(accessToken, claim));
        }

        /// <summary>
        /// Extracts the access token from the header, and verifies its authenticity.
        /// </summary>
        /// <param name="authorization">The authorization header.</param>
        /// <returns>The access token.</returns>
        public static string GetAccessToken(AuthenticationHeaderValue authorization)
        {
            var bearerToken = GetBearerToken(authorization);
            if (String.IsNullOrWhiteSpace(bearerToken)) return null;

            try
            {
                var token = Rijndael.Decrypt(bearerToken, BearerTokenKey);
                return token.StartsWith(AccessTokenHeader)
                    ? token.Remove(0, AccessTokenHeader.Length)
                    : null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Extracts the Bearer token from the Authorization header.
        /// </summary>
        /// <param name="authorization">The Authorization header.</param>
        /// <returns>The Bearer token.</returns>
        public static string GetBearerToken(AuthenticationHeaderValue authorization)
        {
            return authorization == null || authorization.Scheme != "Bearer"
                ? null
                : authorization.Parameter;
        }

        /// <summary>
        /// Determines whether the specified access token is authorized to access
        /// the specified claim.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <param name="claim">The claim.</param>
        /// <returns><c>true</c> if the access token is authorized; otherwise <c>false</c>.</returns>
        public static bool HasClaim(string accessToken, IClaim claim)
        {
            var user = Cache.Get<IClaimUser>(accessToken);
            return user.Claims.Contains(claim);
        }

        /// <summary>
        /// Generates a unique access token.
        /// </summary>
        /// <returns>The 128-bit access token.</returns>
        public static string GenerateAccessToken()
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Generates a Bearer token from the provided Access Token.
        /// </summary>
        /// <param name="accessToken">The Access Token.</param>
        /// <returns>A Bearer token.</returns>
        public static string GenerateBearerToken(string accessToken)
        {
            return Rijndael.Encrypt(AccessTokenHeader + accessToken, BearerTokenKey);
        }

        /// <summary>
        /// Security measures to mitigate brute force attacks.
        /// </summary>
        public static void MitigateBruteForceAttacks()
        {
            // Add some throttling
            Thread.Sleep(new Random().Next(5000));
        }
    }
}
