using System;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Infrastructure.Exceptions
{
    /// <summary>
    /// Tests system exception for the delivery engine.
    /// </summary>
    [TestFixture]
    public class DeliveryEngineSystemExceptionTests
    {
        /// <summary>
        /// Test that the constructor creates a DeliveryEngineSystemException without an inner exception.
        /// </summary>
        [Test]
        public void TestThatConstructurDeliveryEngineSystemExceptionWithoutInnerException()
        {
            var fixture = new Fixture();
            var message = fixture.CreateAnonymous<string>();

            var exception = new DeliveryEngineSystemException(message);
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Empty);
            Assert.That(exception.Message, Is.EqualTo(message));
            Assert.That(exception.InnerException, Is.Null);
        }

        /// <summary>
        /// Test that the constructor creates a DeliveryEngineSystemException with an inner exception.
        /// </summary>
        [Test]
        public void TestThatConstructurCreatesDeliveryEngineSystemExceptionWithInnerException()
        {
            var fixture = new Fixture();
            var message = fixture.CreateAnonymous<string>();
            var innerException = fixture.CreateAnonymous<Exception>();

            var exception = new DeliveryEngineSystemException(message, innerException);
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Empty);
            Assert.That(exception.Message, Is.EqualTo(message));
            Assert.That(exception.InnerException, Is.Not.Null);
            Assert.That(exception.InnerException, Is.EqualTo(innerException));
        }

        /// <summary>
        /// Test that DeliveryEngineSystemException can be serialized and deserialized.
        /// </summary>
        [Test]
        public void TestThatDeliveryEngineSystemExceptionCanBeSerializedAndDeserialized()
        {
            var fixture = new Fixture();
            DeliveryEngineExceptionTestHelper.TestThatDeliveryEngineExceptionCanBeSerializedAndDeserialized(new DeliveryEngineSystemException(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<Exception>()));
        }
    }
}
