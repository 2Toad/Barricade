/*
 * Barricade
 * Copyright (C)2013 2Toad, LLC.
 * licensing@2toad.com
 * 
 * http://2toad.com/Project/Barricade/License
 */

using System.Threading.Tasks;

namespace Barricade
{
    public interface IClaimController
    {
        Task<IClaimUser> GetUserByAccessToken(string accessToken);
    }
}
