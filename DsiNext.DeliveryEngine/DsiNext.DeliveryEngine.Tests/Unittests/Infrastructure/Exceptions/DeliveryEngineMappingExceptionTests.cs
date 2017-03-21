using System;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Infrastructure.Exceptions
{
    /// <summary>
    /// Tests mapping exception for the delivery engine.
    /// </summary>
    [TestFixture]
    public class DeliveryEngineMappingExceptionTests
    {
        /// <summary>
        /// Test that the constructor creates a DeliveryEngineMappingException without an inner exception.
        /// </summary>
        [Test]
        public void TestThatConstructurDeliveryEngineMappingExceptionWithoutInnerException()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineMappingExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineMappingExceptionInfo>()));
            var message = fixture.CreateAnonymous<string>();
            var information = fixture.CreateAnonymous<IDeliveryEngineMappingExceptionInfo>();

            var exception = new DeliveryEngineMappingException(message, information);
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Empty);
            Assert.That(exception.Message, Is.EqualTo(message));
            Assert.That(exception.Information, Is.Not.Null);
            Assert.That(exception.Information, Is.EqualTo(information));
            Assert.That(exception.InnerException, Is.Null);
        }

        /// <summary>
        /// Test that the constructor creates a DeliveryEngineMappingException with an inner exception.
        /// </summary>
        [Test]
        public void TestThatConstructurCreatesDeliveryEngineMappingExceptionWithInnerException()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineMappingExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineMappingExceptionInfo>()));
            var message = fixture.CreateAnonymous<string>();
            var information = fixture.CreateAnonymous<IDeliveryEngineMappingExceptionInfo>();
            var innerException = fixture.CreateAnonymous<Exception>();

            var exception = new DeliveryEngineMappingException(message, information, innerException);
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
        /// Test taht the constructor without an inner exception throws ArgumentNullException if information about the mapping exception is null.
        /// </summary>
        [Test]
        public void TestThatConstructorWithoutInnerExceptionThrowsArgumentNullExceptionIfDeliveryEngineMappingExceptionInfoIsNull()
        {
            var fixture = new Fixture();

            Assert.Throws<ArgumentNullException>(() => new DeliveryEngineMappingException(fixture.CreateAnonymous<string>(), null));
        }

        /// <summary>
        /// Test taht the constructor with an inner exception throws ArgumentNullException if information about the mapping exception is null.
        /// </summary>
        [Test]
        public void TestThatConstructorWithInnerExceptionThrowsArgumentNullExceptionIfDeliveryEngineMappingExceptionInfoIsNull()
        {
            var fixture = new Fixture();

            Assert.Throws<ArgumentNullException>(() => new DeliveryEngineMappingException(fixture.CreateAnonymous<string>(), null, fixture.CreateAnonymous<Exception>()));
        }

        /// <summary>
        /// Test that DeliveryEngineMappingException can be serialized and deserialized.
        /// </summary>
        [Test]
        public void TestThatDeliveryEngineMappingExceptionCanBeSerializedAndDeserialized()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineMappingExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineMappingExceptionInfo>()));
            DeliveryEngineExceptionTestHelper.TestThatDeliveryEngineExceptionCanBeSerializedAndDeserialized(new DeliveryEngineMappingException(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<IDeliveryEngineMappingExceptionInfo>(), fixture.CreateAnonymous<Exception>()));
        }
    }
}
