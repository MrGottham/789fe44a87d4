using System.Linq;
using Domstolene.JFS.CommonLibrary.IoC;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.DataValidators;
using NUnit.Framework;

namespace DsiNext.DeliveryEngine.Tests.Integrationtests.BusinessLogic
{
    /// <summary>
    /// Integration tests of the collection containing data validators to the delivery engine.
    /// </summary>
    [TestFixture]
    [Category("IntegrationTests")]
    public class DataValidatorsTests
    {
        /// <summary>
        /// Test that the collection of data validators for the delivery engine can be initialized
        /// </summary>
        [Test]
        public void TestThatDataValidatesCanBeInitialized()
        {
            var container = ContainerFactory.Create();
            var dataValidators = container.Resolve<IDataValidators>();
            Assert.That(dataValidators, Is.Not.Null);
        }

        /// <summary>
        /// Test that the collection of data validators contains the data validators.
        /// </summary>
        [Test]
        public void TestThatDataValidatesContainsDataValidators()
        {
            var container = ContainerFactory.Create();
            var dataValidators = container.Resolve<IDataValidators>();
            Assert.That(dataValidators, Is.Not.Null);
            Assert.That(dataValidators.Count(), Is.EqualTo(3));
        }
    }
}
