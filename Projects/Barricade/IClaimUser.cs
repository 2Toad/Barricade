/*
 * Barricade
 * Copyright (C)2013 2Toad, LLC.
 * licensing@2toad.com
 * 
 * http://2toad.com/Project/Barricade/License
 */

using System.Collections.Generic;

namespace Barricade
{
    public interface IClaimUser : IAccessToken
    {
        string Username { get; }
        string PasswordHash { get; }
        string PasswordSalt { get; }

        IEnumerable<IClaim> Claims { get; }
    }
}