using NUnit.Framework;

namespace Barricade.Tests
{
    /// <summary>
    /// Base class for tests.
    /// </summary>
    public abstract class BaseTest
    {
        /// <summary>
        /// Allocates and configures resources needed by all tests in the test class.
        /// </summary>
        [SetUp]
        public virtual void Initialize()
        {
            LoadSettings();
        }

        /// <summary>
        /// Loads settings from the configuration file.
        /// </summary>
        public virtual void LoadSettings()
        {
            // Mock settings
            // Do not use these settings in your application. These should
            // come from a config file.
            const int passwordIterations = 5000;
            const string pepper = "5-6QGo7¼FlUQ/;cAÒqyjû]Ef0_m2K881";
            const string bearerTokenKey = "ø3sDÝvK¦yg9Oy`OZÿ9I1¿Ù6nB}06N7dd";
            const string accessTokenHeader = "EDA2AC9242B7463C8E4D28CDDF256FD9";
            const int accessTokenCacheDuration = 5;

            SecurityContext.Configure(passwordIterations, pepper, bearerTokenKey, accessTokenHeader, accessTokenCacheDuration);
        }
    }
}
