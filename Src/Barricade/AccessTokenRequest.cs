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
    /// The client makes a request to the token endpoint by adding the following 
    /// parameters to the HTTP request.
    /// <seealso cref="http://tools.ietf.org/html/rfc6749#section-4.3.2"/>
    /// </summary>
    public class AccessTokenRequest
    {
        /// <summary>
        /// The resource owner username.
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// The resource owner password.
        /// </summary>
        public string Password { get; set; }
    }
}
