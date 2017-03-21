using System;
using DsiNext.DeliveryEngine.Repositories.Data.Oracle;
using DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators;
using NUnit.Framework;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Repositories.Data
{
    /// <summary>
    /// Tests the factory create Oracle clients used by the delivery engine.
    /// </summary>
    [TestFixture]
    public class OracleClientFactoryTests
    {
        /// <summary>
        /// Tests that the constructor initialize the factory to create Oracle client to be used by the delivery engine.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeOracleClientFactory()
        {
            var oracleClientFactory = new OracleClientFactory();
            Assert.That(oracleClientFactory, Is.Not.Null);
        }

        /// <summary>
        /// Test that Create creates an Oracle client for the delivery engine.
        /// </summary>
        [Test]
        public void TestThatCrateCreatesOracleClient()
        {
            var oracleClientFactory = new OracleClientFactory();
            Assert.That(oracleClientFactory, Is.Not.Null);

            using (var oracleClient = oracleClientFactory.Create())
            {
                Assert.That(oracleClient, Is.Not.Null);

                oracleClient.Dispose();
            }
        }

        /// <summary>
        /// Test that CreateDataQueryer throws an ArgumentNullException if the data manipulators is null.
        /// </summary>
        [Test]
        public void TestThatCreateDataQueryerThrowsArgumentNullExceptionIfDataManipulatorsIsNull()
        {
            var oracleClientFactory = new OracleClientFactory();
            Assert.That(oracleClientFactory, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => oracleClientFactory.CreateDataQueryer(null));
        }

        /// <summary>
        /// Test that CreateDataQueryer creates a data queryer for executing queries on oracle.
        /// </summary>
        [Test]
        public void TestThatCreateDataQueryerCreatesDataQueryerForOracle()
        {
            var oracleClientFactory = new OracleClientFactory();
            Assert.That(oracleClientFactory, Is.Not.Null);

            using (var dataQueryer = oracleClientFactory.CreateDataQueryer(MockRepository.GenerateMock<IDataManipulators>()))
            {
                Assert.That(dataQueryer, Is.Not.Null);

                dataQueryer.Dispose();
            }
        }
    }
}
