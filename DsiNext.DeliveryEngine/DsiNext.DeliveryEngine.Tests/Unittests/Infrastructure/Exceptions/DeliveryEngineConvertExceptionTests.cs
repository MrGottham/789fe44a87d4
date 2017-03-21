using System;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Infrastructure.Exceptions
{
    /// <summary>
    /// Tests convert exception for the delivery engine.
    /// </summary>
    [TestFixture]
    public class DeliveryEngineConvertExceptionTests
    {
        /// <summary>
        /// Test that the constructor creates a DeliveryEngineConvertException without an inner exception.
        /// </summary>
        [Test]
        public void TestThatConstructurDeliveryEngineConvertExceptionWithoutInnerException()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineConvertExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineConvertExceptionInfo>()));
            var message = fixture.CreateAnonymous<string>();
            var information = fixture.CreateAnonymous<IDeliveryEngineConvertExceptionInfo>();

            var exception = new DeliveryEngineConvertException(message, information);
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Empty);
            Assert.That(exception.Message, Is.EqualTo(message));
            Assert.That(exception.Information, Is.Not.Null);
            Assert.That(exception.Information, Is.EqualTo(information));
            Assert.That(exception.InnerException, Is.Null);
        }

        /// <summary>
        /// Test that the constructor creates a DeliveryEngineConvertException with an inner exception.
        /// </summary>
        [Test]
        public void TestThatConstructurCreatesDeliveryEngineConvertExceptionWithInnerException()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineConvertExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineConvertExceptionInfo>()));
            var message = fixture.CreateAnonymous<string>();
            var information = fixture.CreateAnonymous<IDeliveryEngineConvertExceptionInfo>();
            var innerException = fixture.CreateAnonymous<Exception>();

            var exception = new DeliveryEngineConvertException(message, information, innerException);
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
        /// Test taht the constructor without an inner exception throws ArgumentNullException if information about the convert exception is null.
        /// </summary>
        [Test]
        public void TestThatConstructorWithoutInnerExceptionThrowsArgumentNullExceptionIfDeliveryEngineConvertExceptionInfoIsNull()
        {
            var fixture = new Fixture();

            Assert.Throws<ArgumentNullException>(() => new DeliveryEngineConvertException(fixture.CreateAnonymous<string>(), null));
        }

        /// <summary>
        /// Test taht the constructor with an inner exception throws ArgumentNullException if information about the convert exception is null.
        /// </summary>
        [Test]
        public void TestThatConstructorWithInnerExceptionThrowsArgumentNullExceptionIfDeliveryEngineConvertExceptionInfoIsNull()
        {
            var fixture = new Fixture();

            Assert.Throws<ArgumentNullException>(() => new DeliveryEngineConvertException(fixture.CreateAnonymous<string>(), null, fixture.CreateAnonymous<Exception>()));
        }

        /// <summary>
        /// Test that DeliveryEngineConvertException can be serialized and deserialized.
        /// </summary>
        [Test]
        public void TestThatDeliveryEngineConvertExceptionCanBeSerializedAndDeserialized()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineConvertExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineConvertExceptionInfo>()));
            DeliveryEngineExceptionTestHelper.TestThatDeliveryEngineExceptionCanBeSerializedAndDeserialized(new DeliveryEngineConvertException(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<IDeliveryEngineConvertExceptionInfo>(), fixture.CreateAnonymous<Exception>()));
        }
    }
}
