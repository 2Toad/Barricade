/*
 * Barricade
 * Copyright (C)2013 2Toad, LLC.
 * licensing@2toad.com
 * 
 * http://2toad.com/Project/Barricade/License
 */

namespace Barricade
{
    public interface IClaimController
    {
        IClaimUser GetUserByAccessToken(string accessToken);
    }
}
