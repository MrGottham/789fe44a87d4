using System.Diagnostics;
using Domstolene.JFS.CommonLibrary.IoC;
using DsiNext.DeliveryEngine.BusinessLogic.Commands;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces;
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
        /// <summary>
        /// Test that the delivery engine can be initialized.
        /// </summary>
        [Test]
        public void TestThatDeliveryEngineCanBeInitialized()
        {
            var container = ContainerFactory.Create();
            var deliveryEngine = container.Resolve<IDeliveryEngine>();
            Assert.That(deliveryEngine, Is.Not.Null);
        }

        /// <summary>
        /// Test that the delivery engine can execture and create a delivery.
        /// </summary>
        [Test]
        public void TestThatDeliveryEngineCanExecute()
        {
            var container = ContainerFactory.Create();
            var deliveryEngine = container.Resolve<IDeliveryEngine>();
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

            var command = new DeliveryEngineExecuteCommand
                {
                    OverrideArchiveInformationPackageId = "AVID.SA.40330",
                    ValidationOnly = false,
                    RemoveMissingRelationshipsOnForeignKeys = false,
                    NumberOfForeignTablesToCache = 10
                };
            deliveryEngine.Execute(command);
        }
    }
}
