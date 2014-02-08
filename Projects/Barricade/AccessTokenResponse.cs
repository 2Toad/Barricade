/*
 * Barricade
 * Copyright (C)2013 2Toad, LLC.
 * licensing@2toad.com
 * 
 * http://2toad.com/Project/Barricade/License
 */

namespace Barricade
{
    /// <summary>
    /// The authorization server issues an access token, and constructs the response 
    /// by adding the following parameters to the HTTP response.
    /// <seealso cref="http://tools.ietf.org/html/rfc6749#section-5.1"/>
    /// </summary>
    public class AccessTokenResponse
    {
        /// <summary>
        /// The access token issued by the authorization server.
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// The type of the token issued.
        /// </summary>
        public string token_type = "Bearer";
        /// <summary>
        /// The lifetime in seconds of the access token.
        /// </summary>
        public long expires_in { get; set; }
    }
}
