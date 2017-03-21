using System;
using DsiNext.DeliveryEngine.BusinessLogic.Events;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace DsiNext.DeliveryEngine.Tests.Unittests.BusinessLogic.Events
{
    /// <summary>
    /// Tests arguments to events raised by data validators.
    /// </summary>
    [TestFixture]
    public class DataValidatorEventArgsTests
    {
        /// <summary>
        /// Test that the constructor initialize arguments to the events raised by data validators.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeEventArgs()
        {
            var fixture = new Fixture();

            var dataObjectMock = fixture.CreateAnonymous<object>();
            var eventArgs = new DataValidatorEventArgs(dataObjectMock);
            Assert.That(eventArgs, Is.Not.Null);
            Assert.That(eventArgs.Data, Is.Not.Null);
            Assert.That(eventArgs.Data, Is.EqualTo(dataObjectMock));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the data object is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfDataIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new DataValidatorEventArgs(null));
        }
    }
}
