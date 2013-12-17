/*
 * Barricade
 * Copyright (C)2013 2Toad, LLC.
 * licensing@2toad.com
 * 
 * http://2toad.com/Project/Barricade/License
 */

namespace Barricade
{
    public class TokenRequestResponse
    {
        public string access_token { get; set; }
        public string token_type = "Bearer";
        public long expires_in { get; set; }
    }
}
