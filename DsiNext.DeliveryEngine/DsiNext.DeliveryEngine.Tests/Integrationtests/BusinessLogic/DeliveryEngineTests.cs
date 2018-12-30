using System.Diagnostics;
using Domstolene.JFS.CommonLibrary.IoC;
using Domstolene.JFS.CommonLibrary.IoC.Interfaces;
using DsiNext.DeliveryEngine.BusinessLogic.Commands;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Commands;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using NUnit.Framework;

namespace DsiNext.DeliveryEngine.Tests.Integrationtests.BusinessLogic
{
    /// <summary>
    /// Integration tests of business logic in the delivery engine.
    /// </summary>
    [TestFixture]
    [Category("IntegrationTests")]
    public class DeliveryEngineTests
    {
        #region Private variables

        private IContainer _container;
        private IConfigurationRepository _configurationRepository;

        #endregion

        /// <summary>
        /// Setup each test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _container = ContainerFactory.Create();
            _configurationRepository = _container.Resolve<IConfigurationRepository>();
        }

        /// <summary>
        /// Test that the delivery engine can be initialized.
        /// </summary>
        [Test]
        public void TestThatDeliveryEngineCanBeInitialized()
        {
            IDeliveryEngine deliveryEngine = _container.Resolve<IDeliveryEngine>();
            Assert.That(deliveryEngine, Is.Not.Null);
        }

        /// <summary>
        /// Test that the delivery engine can execute and create a delivery.
        /// </summary>
        [Test]
        public void TestThatDeliveryEngineCanExecute()
        {
            IDeliveryEngine deliveryEngine = _container.Resolve<IDeliveryEngine>();
            Assert.That(deliveryEngine, Is.Not.Null);
            Assert.That(deliveryEngine.ExceptionHandler, Is.Not.Null);

            deliveryEngine.ExceptionHandler.OnException += (s, e) =>
            {
                Assert.That(s, Is.Not.Null);
                Assert.That(e, Is.Not.Null);
                Assert.That(e.Message, Is.Not.Null);
                Assert.That(e.Message, Is.Not.Empty);
                Assert.That(e.Exception, Is.Not.Null);
                Debug.WriteLine(e.Message);
                e.CanContinue = false;

                Assert.Fail(e.Message);
            };

            IDeliveryEngineExecuteCommand command = new DeliveryEngineExecuteCommand
            {
                OverrideArchiveInformationPackageId = "AVID.SA.40330",
                ValidationOnly = false,
                RemoveMissingRelationshipsOnForeignKeys = false,
                NumberOfForeignTablesToCache = 10,
                IncludeEmptyTables = _configurationRepository.IncludeEmptyTables
            };
            deliveryEngine.Execute(command);
        }
    }
}
