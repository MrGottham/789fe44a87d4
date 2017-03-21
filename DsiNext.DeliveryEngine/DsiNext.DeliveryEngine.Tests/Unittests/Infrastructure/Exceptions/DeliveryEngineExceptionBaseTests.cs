using System;
using System.Runtime.Serialization;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Infrastructure.Exceptions
{
    /// <summary>
    /// Tests basic exception for the delivery engine.
    /// </summary>
    [TestFixture]
    public class DeliveryEngineExceptionBaseTests
    {
        /// <summary>
        /// Own class for testing basic exception for the delivery engine.
        /// </summary>
        [Serializable]
        private class MyDeliveryEngineException : DeliveryEngineExceptionBase
        {
            #region Constructors

            /// <summary>
            /// Creates own class for testing basic exception for the delivery engine.
            /// </summary>
            /// <param name="message">Message.</param>
            public MyDeliveryEngineException(string message)
                : base(message)
            {
            }

            /// <summary>
            /// Creates own class for testing basic exception for the delivery engine.
            /// </summary>
            /// <param name="message">Message.</param>
            /// <param name="innerException">Inner exception.</param>
            public MyDeliveryEngineException(string message, Exception innerException)
                : base(message, innerException)
            {
            }

            /// <summary>
            /// Creates own class for testing basic exception for the delivery engine.
            /// </summary>
            /// <param name="info">Serialization information.</param>
            /// <param name="context">Streaming context.</param>
            protected MyDeliveryEngineException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }

            #endregion
        }

        /// <summary>
        /// Test that the constructor creates a DeliverEngineExceptionBase without an inner exception.
        /// </summary>
        [Test]
        public void TestThatConstructurCreatesDeliverEngineExceptionBaseWithoutInnerException()
        {
            var fixture = new Fixture();
            var message = fixture.CreateAnonymous<string>();

            var exception = new MyDeliveryEngineException(message);
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Empty);
            Assert.That(exception.Message, Is.EqualTo(message));
            Assert.That(exception.InnerException, Is.Null);
        }

        /// <summary>
        /// Test that the constructor creates a DeliverEngineExceptionBase with an inner exception.
        /// </summary>
        [Test]
        public void TestThatConstructurCreatesDeliverEngineExceptionBaseWithInnerException()
        {
            var fixture = new Fixture();
            var message = fixture.CreateAnonymous<string>();
            var innerException = fixture.CreateAnonymous<Exception>();

            var exception = new MyDeliveryEngineException(message, innerException);
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Empty);
            Assert.That(exception.Message, Is.EqualTo(message));
            Assert.That(exception.InnerException, Is.Not.Null);
            Assert.That(exception.InnerException, Is.EqualTo(innerException));
        }

        /// <summary>
        /// Test that DeliverEngineExceptionBase can be serialized and deserialized.
        /// </summary>
        [Test]
        public void TestThatDeliverEngineExceptionBaseCanBeSerializedAndDeserialized()
        {
            var fixture = new Fixture();
            DeliveryEngineExceptionTestHelper.TestThatDeliveryEngineExceptionCanBeSerializedAndDeserialized(new MyDeliveryEngineException(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<Exception>()));
        }
    }
}
