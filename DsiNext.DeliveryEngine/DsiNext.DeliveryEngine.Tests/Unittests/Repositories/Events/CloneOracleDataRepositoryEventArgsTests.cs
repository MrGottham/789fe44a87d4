using System;
using DsiNext.DeliveryEngine.Repositories.Data.Oracle.Events;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Repositories.Events
{
    /// <summary>
    /// Tests arguments for the event raised when the oracle data repository is cloned.
    /// </summary>
    [TestFixture]
    public class CloneOracleDataRepositoryEventArgsTests
    {
        /// <summary>
        /// Test that the constructor initialize arguments for the event raised when the oracle data repository is cloned..
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeEventArgs()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));

            var dataRepositoryMock = fixture.CreateAnonymous<IDataRepository>();
            var eventArgs = new CloneOracleDataRepositoryEventArgs(dataRepositoryMock);
            Assert.That(eventArgs, Is.Not.Null);
            Assert.That(eventArgs.ClonedDataRepository, Is.Not.Null);
            Assert.That(eventArgs.ClonedDataRepository, Is.EqualTo(dataRepositoryMock));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the cloned data repository is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfClonedDataRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CloneOracleDataRepositoryEventArgs(null));
        }
    }
}
