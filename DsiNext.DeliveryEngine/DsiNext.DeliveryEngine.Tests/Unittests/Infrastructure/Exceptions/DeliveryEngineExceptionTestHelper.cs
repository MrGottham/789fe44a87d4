using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using NUnit.Framework;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Infrastructure.Exceptions
{
    /// <summary>
    /// Test helper class for testing exceptions for the delivery engine.
    /// </summary>
    public static class DeliveryEngineExceptionTestHelper
    {
        /// <summary>
        /// Test, that an exception for the delivery engine can be serialized and deserialized.
        /// </summary>
        /// <typeparam name="T">Type of the exception to be tested.</typeparam>
        /// <param name="exception">Exception to be tested.</param>
        public static void TestThatDeliveryEngineExceptionCanBeSerializedAndDeserialized<T>(T exception) where T : DeliveryEngineExceptionBase
        {
            Assert.That(exception, Is.Not.Null);

            var memoryStream = new MemoryStream();
            try
            {
                var serializer = new SoapFormatter();
                serializer.Serialize(memoryStream, exception);
                Assert.That(memoryStream.Length, Is.GreaterThan(0));

                memoryStream.Seek(0, SeekOrigin.Begin);
                Assert.That(memoryStream.Position, Is.EqualTo(0));

                var deserializedException = (T) serializer.Deserialize(memoryStream);
                Assert.That(deserializedException, Is.Not.Null);
                Assert.That(deserializedException.Message, Is.Not.Null);
                Assert.That(deserializedException.Message, Is.Not.Empty);
                Assert.That(deserializedException.Message, Is.EqualTo(exception.Message));
                if (exception.InnerException == null)
                {
                    Assert.That(deserializedException, Is.Null);
                    return;
                }
                Assert.That(deserializedException.InnerException, Is.Not.Null);
                Assert.That(deserializedException.InnerException.Message, Is.EqualTo(exception.InnerException.Message));
            }
            finally
            {
                memoryStream.Close();
            }
        }
    }
}
