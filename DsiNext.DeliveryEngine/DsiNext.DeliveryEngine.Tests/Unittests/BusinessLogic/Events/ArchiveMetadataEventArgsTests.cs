using System;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;
using DsiNext.DeliveryEngine.BusinessLogic.Events;

namespace DsiNext.DeliveryEngine.Tests.Unittests.BusinessLogic.Events
{
    /// <summary>
    /// Tests arguments to the event raised before archiving the metadata.
    /// </summary>
    [TestFixture]
    public class ArchiveMetadataEventArgsTests
    {
        /// <summary>
        /// Test that the constructor initalize arguments to the event raised before archiving the metadata.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeEventArgs()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataSource>()));

            var dataSourceMock = fixture.CreateAnonymous<IDataSource>();
            var eventArgs = new ArchiveMetadataEventArgs(dataSourceMock);
            Assert.That(eventArgs, Is.Not.Null);
            Assert.That(eventArgs.DataSource, Is.Not.Null);
            Assert.That(eventArgs.DataSource, Is.EqualTo(dataSourceMock));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the data source is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfDataSourceIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ArchiveMetadataEventArgs(null));
        }
    }
}
