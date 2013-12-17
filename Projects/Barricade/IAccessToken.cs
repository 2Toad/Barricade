/*
 * Barricade
 * Copyright (C)2013 2Toad, LLC.
 * licensing@2toad.com
 * 
 * http://2toad.com/Project/Barricade/License
 */

using System;

namespace Barricade
{
    public interface IAccessToken
    {
        string AccessToken { get; set; }
        DateTime? AccessTokenExpiration { get; set; }
    }
}