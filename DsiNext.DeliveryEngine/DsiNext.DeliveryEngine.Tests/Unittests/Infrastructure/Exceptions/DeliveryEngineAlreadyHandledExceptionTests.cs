using System;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Infrastructure.Exceptions
{
    /// <summary>
    /// Tests exception that has been handled by the exception handler in the delivery engine.
    /// </summary>
    [TestFixture]
    public class DeliveryEngineAlreadyHandledExceptionTests
    {
        /// <summary>
        /// Test that the constructor creates a DeliveryEngineAlreadyHandledException without an inner exception.
        /// </summary>
        [Test]
        public void TestThatConstructurCreatesDeliveryEngineAlreadyHandledExceptionWithoutInnerException()
        {
            var fixture = new Fixture();
            var message = fixture.CreateAnonymous<string>();

            var exception = new DeliveryEngineAlreadyHandledException(message);
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Empty);
            Assert.That(exception.Message, Is.EqualTo(message));
            Assert.That(exception.InnerException, Is.Null);
        }

        /// <summary>
        /// Test that the constructor creates a DeliveryEngineAlreadyHandledException with an inner exception.
        /// </summary>
        [Test]
        public void TestThatConstructurCreatesDeliveryEngineAlreadyHandledExceptionWithInnerException()
        {
            var fixture = new Fixture();
            var message = fixture.CreateAnonymous<string>();
            var innerException = fixture.CreateAnonymous<Exception>();

            var exception = new DeliveryEngineAlreadyHandledException(message, innerException);
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Empty);
            Assert.That(exception.Message, Is.EqualTo(message));
            Assert.That(exception.InnerException, Is.Not.Null);
            Assert.That(exception.InnerException, Is.EqualTo(innerException));
        }

        /// <summary>
        /// Test that DeliveryEngineAlreadyHandledException can be serialized and deserialized.
        /// </summary>
        [Test]
        public void TestThatDeliveryEngineAlreadyHandledExceptionCanBeSerializedAndDeserialized()
        {
            var fixture = new Fixture();
            DeliveryEngineExceptionTestHelper.TestThatDeliveryEngineExceptionCanBeSerializedAndDeserialized(new DeliveryEngineAlreadyHandledException(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<Exception>()));
        }
    }
}
