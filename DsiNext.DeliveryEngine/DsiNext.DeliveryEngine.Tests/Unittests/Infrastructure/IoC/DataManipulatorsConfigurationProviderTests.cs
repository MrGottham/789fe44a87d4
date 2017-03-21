using System;
using Domstolene.JFS.CommonLibrary.IoC;
using Domstolene.JFS.CommonLibrary.IoC.Interfaces;
using DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators;
using NUnit.Framework;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Infrastructure.IoC
{
    /// <summary>
    /// Tests configuration provider for a collection of data manipulators.
    /// </summary>
    [TestFixture]
    public class DataManipulatorsConfigurationProviderTests
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
        public void TestConfiguration([Values(typeof(IContainer), typeof(IDataManipulators))] Type type)
        {
            var resolvedType = _container.Resolve(type);
            Assert.That(resolvedType, Is.Not.Null);
        }

        /// <summary>
        /// Tests the configuration for the collection of data manipulators.
        /// </summary>
        [Test]
        public void TestConfigurationForDataManipulators()
        {
            var dataManipulatorCollection = _container.Resolve<IDataManipulators>();
            Assert.That(dataManipulatorCollection, Is.Not.Null);

            var dataManipulators = _container.ResolveAll(typeof (IDataManipulator));
            Assert.That(dataManipulators, Is.Not.Null);

            var dataSetters = _container.ResolveAll(typeof (IDataSetter));
            Assert.That(dataSetters, Is.Not.Null);

            var regularExpressionReplacers = _container.ResolveAll(typeof (IRegularExpressionReplacer));
            Assert.That(regularExpressionReplacers, Is.Not.Null);

            var rowDuplicators = _container.ResolveAll(typeof (IRowDuplicator));
            Assert.That(rowDuplicators, Is.Not.Null);

            var missignForeignKeyHandlers = _container.ResolveAll(typeof (IMissingForeignKeyHandler));
            Assert.That(missignForeignKeyHandlers, Is.Not.Null);
        }
    }
}
