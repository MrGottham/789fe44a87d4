using System;
using DsiNext.DeliveryEngine.Infrastructure.ExceptionHandling;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Infrastructure.ExceptionHandling
{
    /// <summary>
    /// Tests arguments to the eventhandler which handles exceptions.
    /// </summary>
    [TestFixture]
    public class HandleExceptionEventArgsTests
    {
        /// <summary>
        /// Test that the constructor initialize arguments for the eventhandler without an exception object and data for the exception object.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeHandleExceptionEventArgsWithoutExceptionObjectAndExceptionObjectData()
        {
            var fixture = new Fixture();
            var exception = fixture.CreateAnonymous<Exception>();
            var eventArgs = new HandleExceptionEventArgs(exception);
            Assert.That(eventArgs, Is.Not.Null);
            Assert.That(eventArgs.Message, Is.Not.Null);
            Assert.That(eventArgs.Message, Is.Not.Empty);
            Assert.That(eventArgs.Message, Is.EqualTo(exception.Message));
            Assert.That(eventArgs.Exception, Is.Not.Null);
            Assert.That(eventArgs.Exception, Is.EqualTo(exception));
            Assert.That(eventArgs.ExceptionObject, Is.Null);
            Assert.That(eventArgs.ExceptionObjectData, Is.Null);
            Assert.That(eventArgs.CanContinue, Is.False);
        }

        /// <summary>
        /// Test that the constructor initialize arguments for the eventhandler with an exception object and without data for the exception object.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeHandleExceptionEventArgsWithExceptionObjectAndWithoutExceptionObjectData()
        {
            var fixture = new Fixture();
            var exception = fixture.CreateAnonymous<Exception>();
            var exceptionObject = fixture.CreateAnonymous<object>();
            var eventArgs = new HandleExceptionEventArgs(exception, exceptionObject);
            Assert.That(eventArgs, Is.Not.Null);
            Assert.That(eventArgs.Message, Is.Not.Null);
            Assert.That(eventArgs.Message, Is.Not.Empty);
            Assert.That(eventArgs.Message, Is.EqualTo(exception.Message));
            Assert.That(eventArgs.Exception, Is.Not.Null);
            Assert.That(eventArgs.Exception, Is.EqualTo(exception));
            Assert.That(eventArgs.ExceptionObject, Is.Not.Null);
            Assert.That(eventArgs.ExceptionObject, Is.EqualTo(exceptionObject));
            Assert.That(eventArgs.ExceptionObjectData, Is.Null);
            Assert.That(eventArgs.CanContinue, Is.False);
        }

        /// <summary>
        /// Test that the constructor initialize arguments for the eventhandler with an exception object and data for the exception object.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeHandleExceptionEventArgsWithExceptionObjectAndExceptionObjectData()
        {
            var fixture = new Fixture();
            var exception = fixture.CreateAnonymous<Exception>();
            var exceptionObject = fixture.CreateAnonymous<object>();
            var exceptionObjectData = fixture.CreateAnonymous<object>();
            var eventArgs = new HandleExceptionEventArgs(exception, exceptionObject, exceptionObjectData);
            Assert.That(eventArgs, Is.Not.Null);
            Assert.That(eventArgs.Message, Is.Not.Null);
            Assert.That(eventArgs.Message, Is.Not.Empty);
            Assert.That(eventArgs.Message, Is.EqualTo(exception.Message));
            Assert.That(eventArgs.Exception, Is.Not.Null);
            Assert.That(eventArgs.Exception, Is.EqualTo(exception));
            Assert.That(eventArgs.ExceptionObject, Is.Not.Null);
            Assert.That(eventArgs.ExceptionObject, Is.EqualTo(exceptionObject));
            Assert.That(eventArgs.ExceptionObjectData, Is.Not.Null);
            Assert.That(eventArgs.ExceptionObjectData, Is.EqualTo(exceptionObjectData));
            Assert.That(eventArgs.CanContinue, Is.False);
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the exception is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfExceptionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new HandleExceptionEventArgs(null));
        }
    }
}
