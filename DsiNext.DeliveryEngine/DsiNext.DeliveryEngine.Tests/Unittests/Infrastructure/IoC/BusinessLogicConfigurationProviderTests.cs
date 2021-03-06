﻿using System;
using Domstolene.JFS.CommonLibrary.IoC;
using Domstolene.JFS.CommonLibrary.IoC.Interfaces;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces;
using NUnit.Framework;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Infrastructure.IoC
{
    /// <summary>
    /// Tests the configuration provider for business logic in the delivery engine.
    /// </summary>
    [TestFixture]
    public class BusinessLogicConfigurationProviderTests
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
        public void TestConfiguration([Values(typeof(IContainer), typeof(IDeliveryEngine))] Type type)
        {
            var resolvedType = _container.Resolve(type);
            Assert.That(resolvedType, Is.Not.Null);
        }
    }
}
