using System;
using Domstolene.JFS.CommonLibrary.IoC;
using Domstolene.JFS.CommonLibrary.IoC.Interfaces;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.DataValidators;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.ExceptionHandling;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Log;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators;
using NUnit.Framework;

namespace DsiNext.DeliveryEngine.Tests.Integrationtests.Infrastructure.IoC
{
    /// <summary>
    /// Tests the IoC configuration.
    /// </summary>
    [TestFixture]
    [Category("IntegrationTests")]
    public class IoCConfigurationTests
    {
        #region Private variables

        private IContainer _container;

        #endregion

        /// <summary>
        /// Setup the tests.
        /// </summary>
        [TestFixtureSetUp]
        public void TestSetUp()
        {
            _container = ContainerFactory.Create();
        }

        /// <summary>
        /// Test the configuration in the container for Inversion Of Control.
        /// </summary>
        [Test]
        public void TestConfiguration([Values(typeof(IContainer), typeof(IInformationLogger), typeof(IExceptionLogger), typeof(IExceptionHandler), typeof(IConfigurationRepository), typeof(IMetadataRepository), typeof(IDataManipulators), typeof(IDataRepository), typeof(IDocumentRepository), typeof(IArchiveVersionRepository), typeof(IPrimaryKeyDataValidator), typeof(IForeignKeysDataValidator), typeof(IMappingDataValidator), typeof(IDataValidators), typeof(IDeliveryEngine))] Type type)
        {
            var resolvedType = _container.Resolve(type);
            Assert.That(resolvedType, Is.Not.Null);
        }
    }
}
