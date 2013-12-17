using System;
using System.Collections.Generic;

namespace Barricade.Tests
{
    public class ClaimUser : IClaimUser
    {
        public string AccessToken { get; set; }
        public DateTime? AccessTokenExpiration { get; set; }

        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public List<IClaim> Claims { get; set; }
    }
}
