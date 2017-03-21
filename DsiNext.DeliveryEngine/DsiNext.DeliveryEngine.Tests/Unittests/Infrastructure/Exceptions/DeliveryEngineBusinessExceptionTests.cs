using System;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Infrastructure.Exceptions
{
    /// <summary>
    /// Tests business exception for the delivery engine.
    /// </summary>
    [TestFixture]
    public class DeliveryEngineBusinessExceptionTests
    {
        /// <summary>
        /// Test that the constructor creates a DeliveryEngineBusinessException without an inner exception.
        /// </summary>
        [Test]
        public void TestThatConstructurDeliveryEngineBusinessExceptionWithoutInnerException()
        {
            var fixture = new Fixture();
            var message = fixture.CreateAnonymous<string>();

            var exception = new DeliveryEngineBusinessException(message);
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Empty);
            Assert.That(exception.Message, Is.EqualTo(message));
            Assert.That(exception.InnerException, Is.Null);
        }

        /// <summary>
        /// Test that the constructor creates a DeliveryEngineBusinessException with an inner exception.
        /// </summary>
        [Test]
        public void TestThatConstructurCreatesDeliveryEngineBusinessExceptionWithInnerException()
        {
            var fixture = new Fixture();
            var message = fixture.CreateAnonymous<string>();
            var innerException = fixture.CreateAnonymous<Exception>();

            var exception = new DeliveryEngineBusinessException(message, innerException);
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Empty);
            Assert.That(exception.Message, Is.EqualTo(message));
            Assert.That(exception.InnerException, Is.Not.Null);
            Assert.That(exception.InnerException, Is.EqualTo(innerException));
        }

        /// <summary>
        /// Test that DeliveryEngineBusinessException can be serialized and deserialized.
        /// </summary>
        [Test]
        public void TestThatDeliveryEngineBusinessExceptionCanBeSerializedAndDeserialized()
        {
            var fixture = new Fixture();
            DeliveryEngineExceptionTestHelper.TestThatDeliveryEngineExceptionCanBeSerializedAndDeserialized(new DeliveryEngineBusinessException(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<Exception>()));
        }
    }
}
