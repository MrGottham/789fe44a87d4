using System;
using DsiNext.DeliveryEngine.BusinessLogic.Events;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.BusinessLogic.Events
{
    /// <summary>
    /// Test arguments to the event raised before getting data for a target table.
    /// </summary>
    [TestFixture]
    public class GetDataForTargetTableEventArgsTests
    {
        /// <summary>
        /// Test that the constructor initialize arguments to the event raised before getting data for a target table.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeEventArgs()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataSource>()));
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));

            var dataSourceMock = fixture.CreateAnonymous<IDataSource>();
            var targetTableMock = fixture.CreateAnonymous<ITable>();
            var dataBlock = fixture.CreateAnonymous<int>();
            var eventArgs = new GetDataForTargetTableEventArgs(dataSourceMock, targetTableMock, dataBlock);
            Assert.That(eventArgs, Is.Not.Null);
            Assert.That(eventArgs.DataSource, Is.Not.Null);
            Assert.That(eventArgs.DataSource, Is.EqualTo(dataSourceMock));
            Assert.That(eventArgs.TargetTable, Is.Not.Null);
            Assert.That(eventArgs.TargetTable, Is.EqualTo(targetTableMock));
            Assert.That(eventArgs.DataBlock, Is.EqualTo(dataBlock));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the data source is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfDataSourceIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));

            Assert.Throws<ArgumentNullException>(() => new GetDataForTargetTableEventArgs(null, fixture.CreateAnonymous<ITable>(), fixture.CreateAnonymous<int>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the target table is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfTargetTableIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataSource>()));

            Assert.Throws<ArgumentNullException>(() => new GetDataForTargetTableEventArgs(fixture.CreateAnonymous<IDataSource>(), null, fixture.CreateAnonymous<int>()));
        }
    }
}
