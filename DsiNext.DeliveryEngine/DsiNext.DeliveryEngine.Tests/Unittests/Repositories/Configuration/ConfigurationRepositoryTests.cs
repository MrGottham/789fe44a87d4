using DsiNext.DeliveryEngine.Repositories.Configuration;
using NUnit.Framework;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Repositories.Configuration
{
    /// <summary>
    /// Tests the configuration repository to be used by the delivery engine.
    /// </summary>
    [TestFixture]
    public class ConfigurationRepositoryTests
    {
        /// <summary>
        /// Test that the constructor initialize the configuration repository.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeConfigurationRepository()
        {
            var configurationRepository = new ConfigurationRepository();
            Assert.That(configurationRepository, Is.Not.Null);
        }
    }
}
