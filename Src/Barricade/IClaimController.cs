/*
 * Barricade
 * Copyright (C)2013 2Toad, LLC.
 * licensing@2toad.com
 * 
 * http://2toad.com/Project/Barricade/License
 */

using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace Barricade
{
    public interface IClaimController : IHttpController
    {
        Task<IClaimUser> GetUserByAccessToken(string accessToken);
    }
}
