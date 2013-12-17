using System;
using NUnit.Framework;

namespace Barricade.Tests
{
    public class PasswordTest : BaseTest
    {
        [Test]
        public void GeneratePasswordHash()
        {            
            var passwordHash = SecurityContext.GeneratePasswordHash("Foo", "Bar");
            Assert.IsNotNull(passwordHash);
            Assert.AreEqual(passwordHash.Length, 32);
        }

        [Test]
        public void ValidatePassword()
        {
            var password = Guid.NewGuid().ToString("N");
            var passwordSalt = Guid.NewGuid().ToString("N");
            var passwordHash = SecurityContext.GeneratePasswordHash(password, passwordSalt);

            var user = new ClaimUser { PasswordHash = passwordHash, PasswordSalt = passwordSalt };
            var credentials = new TokenRequestCredentials { Password = password, Username = "Foo" };

            var valid = SecurityContext.ValidatePassword(user, credentials);
            Assert.IsTrue(valid);
        }
    }
}
