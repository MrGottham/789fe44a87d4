using System;
using System.Collections.Specialized;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.Configuration;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Resources;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Repositories.Configuration
{
    /// <summary>
    /// Tests the configuration repository to be used by the delivery engine.
    /// </summary>
    [TestFixture]
    public class ConfigurationRepositoryTests
    {
        #region Private variables

        private Fixture _fixture;

        #endregion

        /// <summary>
        /// Setup each unit test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        /// <summary>
        /// Test that the constructor initialize the configuration repository.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeConfigurationRepository()
        {
            bool includeEmptyTablesValue = _fixture.CreateAnonymous<bool>();
            NameValueCollection appSettingCollection = BuildAppSettingCollection(includeEmptyTablesValue);

            IConfigurationRepository sut = CreateSut(appSettingCollection);
            Assert.That(sut, Is.Not.Null);
            Assert.That(sut.IncludeEmptyTables, Is.EqualTo(includeEmptyTablesValue));
        }

        /// <summary>
        /// Test that the constructor throws an <see cref="ArgumentNullException"/> when the collection of app settings is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionWhenAppSettingCollectionIsNull()
        {
            // ReSharper disable ObjectCreationAsStatement
            ArgumentNullException result = Assert.Throws<ArgumentNullException>(() => new ConfigurationRepository(null));
            // ReSharper restore ObjectCreationAsStatement

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ParamName, Is.Not.Null);
            Assert.That(result.ParamName, Is.Not.Empty);
            Assert.That(result.ParamName, Is.EqualTo("appSettingCollection"));
            Assert.That(result.InnerException, Is.Null);
        }

        /// <summary>
        /// Test that the constructor throws an <see cref="DeliveryEngineSystemException"/> when the collection of app settings does not contain element for include empty tables.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsDeliveryEngineSystemExceptionWhenAppSettingCollectionDoesNotContainIncludeEmptyTables()
        {
            NameValueCollection appSettingCollection = BuildAppSettingCollection();

            // ReSharper disable ObjectCreationAsStatement
            DeliveryEngineSystemException result = Assert.Throws<DeliveryEngineSystemException>(() => new ConfigurationRepository(appSettingCollection));
            // ReSharper restore ObjectCreationAsStatement

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Message, Is.Not.Null);
            Assert.That(result.Message, Is.Not.Empty);
            Assert.That(result.Message, Is.EqualTo(Resource.GetExceptionMessage(ExceptionMessage.ApplicationSettingMissing, "IncludeEmptyTables")));
            Assert.That(result.InnerException, Is.Null);
        }

        /// <summary>
        /// Creates an instance of the <see cref="ConfigurationRepository"/> which can be used for unit testing.
        /// </summary>
        /// <returns>Instance of the <see cref="ConfigurationRepository"/> which can be used for unit testing.</returns>
        private IConfigurationRepository CreateSut(NameValueCollection appSettingCollection = null)
        {
            if (appSettingCollection != null)
            {
                return new ConfigurationRepository(appSettingCollection);
            }

            appSettingCollection = BuildAppSettingCollection(_fixture.CreateAnonymous<bool>());
            return new ConfigurationRepository(appSettingCollection);
        }

        /// <summary>
        /// Builds a collection of app settings which can be used for unit testing.
        /// </summary>
        /// <returns>Collection of app settings which can be used for unit testing.</returns>
        private NameValueCollection BuildAppSettingCollection(bool? includeEmptyTablesValue = null)
        {
            NameValueCollection appSettingCollection = new NameValueCollection();
            if (includeEmptyTablesValue.HasValue)
            {
                appSettingCollection.Add("IncludeEmptyTables", Convert.ToString(includeEmptyTablesValue.Value));
            }

            return appSettingCollection;
        }
    }
}
