using System.Net.Http.Headers;
using NUnit.Framework;

namespace Barricade.Tests
{
    public class OAuthTest : BaseTest
    {
        [Test]
        public void GenerateAccessToken()
        {
            var token = SecurityContext.GenerateAccessToken();
            Assert.IsNotNull(token);
            Assert.AreEqual(token.Length, 32);
        }

        [Test]
        public void GenerateBearerToken()
        {
            var token = SecurityContext.GenerateBearerToken("Foo");
            Assert.IsNotNull(token);
        }

        [Test]
        public void GetBearerToken()
        {
            var token = SecurityContext.GetBearerToken(new AuthenticationHeaderValue("Basic"));
            Assert.IsNull(token);

            var header = new AuthenticationHeaderValue("Bearer", "Foo");
            token = SecurityContext.GetBearerToken(header);
            Assert.IsNotNull(token);
            Assert.AreEqual(token, "Foo");
        }

        [Test]
        public void GetAccessToken()
        {
            var accessToken = SecurityContext.GenerateAccessToken();
            var bearerToken = SecurityContext.GenerateBearerToken(accessToken);

            var badToken = SecurityContext.GetAccessToken(new AuthenticationHeaderValue("Bearer", accessToken));
            Assert.IsNull(badToken);

            var header = new AuthenticationHeaderValue("Bearer", bearerToken);
            var result = SecurityContext.GetAccessToken(header);
            Assert.IsNotNull(result);
            Assert.AreEqual(accessToken, result);
        }
    }
}
