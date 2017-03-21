using System;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Infrastructure.Exceptions
{
    /// <summary>
    /// Tests metadata exception for the delivery engine.
    /// </summary>
    [TestFixture]
    public class DeliveryEngineMetadataExceptionTests
    {
        /// <summary>
        /// Test that the constructor creates a DeliveryEngineMetadataException without an inner exception.
        /// </summary>
        [Test]
        public void TestThatConstructurDeliveryEngineMetadataExceptionWithoutInnerException()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineMetadataExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineMetadataExceptionInfo>()));
            var message = fixture.CreateAnonymous<string>();
            var information = fixture.CreateAnonymous<IDeliveryEngineMetadataExceptionInfo>();

            var exception = new DeliveryEngineMetadataException(message, information);
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Empty);
            Assert.That(exception.Message, Is.EqualTo(message));
            Assert.That(exception.Information, Is.Not.Null);
            Assert.That(exception.Information, Is.EqualTo(information));
            Assert.That(exception.InnerException, Is.Null);
        }

        /// <summary>
        /// Test that the constructor creates a DeliveryEngineMetadataException with an inner exception.
        /// </summary>
        [Test]
        public void TestThatConstructurCreatesDeliveryEngineMetadataExceptionWithInnerException()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineMetadataExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineMetadataExceptionInfo>()));
            var message = fixture.CreateAnonymous<string>();
            var information = fixture.CreateAnonymous<IDeliveryEngineMetadataExceptionInfo>();
            var innerException = fixture.CreateAnonymous<Exception>();

            var exception = new DeliveryEngineMetadataException(message, information, innerException);
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Empty);
            Assert.That(exception.Message, Is.EqualTo(message));
            Assert.That(exception.Information, Is.Not.Null);
            Assert.That(exception.Information, Is.EqualTo(information));
            Assert.That(exception.InnerException, Is.Not.Null);
            Assert.That(exception.InnerException, Is.EqualTo(innerException));
        }

        /// <summary>
        /// Test taht the constructor without an inner exception throws ArgumentNullException if information about the metadata exception is null.
        /// </summary>
        [Test]
        public void TestThatConstructorWithoutInnerExceptionThrowsArgumentNullExceptionIfDeliveryEngineMetadataExceptionInfoIsNull()
        {
            var fixture = new Fixture();

            Assert.Throws<ArgumentNullException>(() => new DeliveryEngineMetadataException(fixture.CreateAnonymous<string>(), null));
        }

        /// <summary>
        /// Test taht the constructor with an inner exception throws ArgumentNullException if information about the metadata exception is null.
        /// </summary>
        [Test]
        public void TestThatConstructorWithInnerExceptionThrowsArgumentNullExceptionIfDeliveryEngineMetadataExceptionInfoIsNull()
        {
            var fixture = new Fixture();

            Assert.Throws<ArgumentNullException>(() => new DeliveryEngineMetadataException(fixture.CreateAnonymous<string>(), null, fixture.CreateAnonymous<Exception>()));
        }

        /// <summary>
        /// Test that DeliveryEngineMetadataException can be serialized and deserialized.
        /// </summary>
        [Test]
        public void TestThatDeliveryEngineMetadataExceptionCanBeSerializedAndDeserialized()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineMetadataExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineMetadataExceptionInfo>()));
            DeliveryEngineExceptionTestHelper.TestThatDeliveryEngineExceptionCanBeSerializedAndDeserialized(new DeliveryEngineMetadataException(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<IDeliveryEngineMetadataExceptionInfo>(), fixture.CreateAnonymous<Exception>()));
        }
    }
}
