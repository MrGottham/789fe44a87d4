using System;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Infrastructure.Exceptions
{
    /// <summary>
    /// Tests repository exception for the delivery engine.
    /// </summary>
    [TestFixture]
    public class DeliveryEngineRepositoryExceptionTests
    {
        /// <summary>
        /// Test that the constructor creates a DeliveryEngineRepositoryException without an inner exception.
        /// </summary>
        [Test]
        public void TestThatConstructurDeliveryEngineRepositoryExceptionWithoutInnerException()
        {
            var fixture = new Fixture();
            var message = fixture.CreateAnonymous<string>();

            var exception = new DeliveryEngineRepositoryException(message);
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Empty);
            Assert.That(exception.Message, Is.EqualTo(message));
            Assert.That(exception.InnerException, Is.Null);
        }

        /// <summary>
        /// Test that the constructor creates a DeliveryEngineRepositoryException with an inner exception.
        /// </summary>
        [Test]
        public void TestThatConstructurCreatesDeliveryEngineRepositoryExceptionWithInnerException()
        {
            var fixture = new Fixture();
            var message = fixture.CreateAnonymous<string>();
            var innerException = fixture.CreateAnonymous<Exception>();

            var exception = new DeliveryEngineRepositoryException(message, innerException);
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Empty);
            Assert.That(exception.Message, Is.EqualTo(message));
            Assert.That(exception.InnerException, Is.Not.Null);
            Assert.That(exception.InnerException, Is.EqualTo(innerException));
        }

        /// <summary>
        /// Test that DeliveryEngineRepositoryException can be serialized and deserialized.
        /// </summary>
        [Test]
        public void TestThatDeliveryEngineRepositoryExceptionCanBeSerializedAndDeserialized()
        {
            var fixture = new Fixture();
            DeliveryEngineExceptionTestHelper.TestThatDeliveryEngineExceptionCanBeSerializedAndDeserialized(new DeliveryEngineRepositoryException(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<Exception>()));
        }
    }
}
