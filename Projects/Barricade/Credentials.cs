/*
 * Barricade
 * Copyright (C)2013 2Toad, LLC.
 * licensing@2toad.com
 * 
 * http://2toad.com/Project/Barricade/License
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Barricade
{
    public class Credentials
    {
        public DateTime? AccessTokenExpiration { get; set; }
        public List<IClaim> Claims { get; set; }

        public static Credentials From(IClaimUser user)
        {
            return new Credentials {
                AccessTokenExpiration = user.AccessTokenExpiration,
                Claims = user.Claims.ToList()
            };
        }
    }
}