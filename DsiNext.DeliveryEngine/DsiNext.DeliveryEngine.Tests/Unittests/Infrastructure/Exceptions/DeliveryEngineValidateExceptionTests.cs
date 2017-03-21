using System;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Infrastructure.Exceptions
{
    /// <summary>
    /// Tests validate exception for the delivery engine.
    /// </summary>
    [TestFixture]
    public class DeliveryEngineValidateExceptionTests
    {
        /// <summary>
        /// Test that the constructor creates a DeliveryEngineValidateException without an inner exception.
        /// </summary>
        [Test]
        public void TestThatConstructurDeliveryEngineValidateExceptionWithoutInnerException()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineValidateExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineValidateExceptionInfo>()));
            var message = fixture.CreateAnonymous<string>();
            var information = fixture.CreateAnonymous<IDeliveryEngineValidateExceptionInfo>();

            var exception = new DeliveryEngineValidateException(message, information);
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Empty);
            Assert.That(exception.Message, Is.EqualTo(message));
            Assert.That(exception.Information, Is.Not.Null);
            Assert.That(exception.Information, Is.EqualTo(information));
            Assert.That(exception.InnerException, Is.Null);
        }

        /// <summary>
        /// Test that the constructor creates a DeliveryEngineValidateException with an inner exception.
        /// </summary>
        [Test]
        public void TestThatConstructurCreatesDeliveryEngineValidateExceptionWithInnerException()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineValidateExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineValidateExceptionInfo>()));
            var message = fixture.CreateAnonymous<string>();
            var information = fixture.CreateAnonymous<IDeliveryEngineValidateExceptionInfo>();
            var innerException = fixture.CreateAnonymous<Exception>();

            var exception = new DeliveryEngineValidateException(message, information, innerException);
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
        /// Test taht the constructor without an inner exception throws ArgumentNullException if information about the validate exception is null.
        /// </summary>
        [Test]
        public void TestThatConstructorWithoutInnerExceptionThrowsArgumentNullExceptionIfDeliveryEngineValidateExceptionInfoIsNull()
        {
            var fixture = new Fixture();

            Assert.Throws<ArgumentNullException>(() => new DeliveryEngineValidateException(fixture.CreateAnonymous<string>(), null));
        }

        /// <summary>
        /// Test taht the constructor with an inner exception throws ArgumentNullException if information about the validate exception is null.
        /// </summary>
        [Test]
        public void TestThatConstructorWithInnerExceptionThrowsArgumentNullExceptionIfDeliveryEngineValidateExceptionInfoIsNull()
        {
            var fixture = new Fixture();

            Assert.Throws<ArgumentNullException>(() => new DeliveryEngineValidateException(fixture.CreateAnonymous<string>(), null, fixture.CreateAnonymous<Exception>()));
        }

        /// <summary>
        /// Test that DeliveryEngineValidateException can be serialized and deserialized.
        /// </summary>
        [Test]
        public void TestThatDeliveryEngineValidateExceptionCanBeSerializedAndDeserialized()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineValidateExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineValidateExceptionInfo>()));
            DeliveryEngineExceptionTestHelper.TestThatDeliveryEngineExceptionCanBeSerializedAndDeserialized(new DeliveryEngineValidateException(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<IDeliveryEngineValidateExceptionInfo>(), fixture.CreateAnonymous<Exception>()));
        }
    }
}
