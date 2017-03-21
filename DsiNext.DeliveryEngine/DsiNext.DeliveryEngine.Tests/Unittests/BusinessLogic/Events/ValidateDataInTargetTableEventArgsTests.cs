using System;
using DsiNext.DeliveryEngine.BusinessLogic.Events;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.BusinessLogic.Events
{
    /// <summary>
    /// Tests arguments to the event raised before validating data in a target table.
    /// </summary>
    [TestFixture]
    public class ValidateDataInTargetTableEventArgsTests
    {
        /// <summary>
        /// Test that the constructor initialize arguments to the event raised before validating data in a target table.
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
            var rowsInDataBlock = fixture.CreateAnonymous<int>();
            var eventArgs = new ValidateDataInTargetTableEventArgs(dataSourceMock, targetTableMock, dataBlock, rowsInDataBlock);
            Assert.That(eventArgs, Is.Not.Null);
            Assert.That(eventArgs.DataSource, Is.Not.Null);
            Assert.That(eventArgs.DataSource, Is.EqualTo(dataSourceMock));
            Assert.That(eventArgs.TargetTable, Is.Not.Null);
            Assert.That(eventArgs.TargetTable, Is.EqualTo(targetTableMock));
            Assert.That(eventArgs.DataBlock, Is.EqualTo(dataBlock));
            Assert.That(eventArgs.RowsInDataBlock, Is.EqualTo(rowsInDataBlock));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the data source is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfDataSourceIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));

            Assert.Throws<ArgumentNullException>(() => new ValidateDataInTargetTableEventArgs(null, fixture.CreateAnonymous<ITable>(), fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<int>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the target table is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfTargetTableIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataSource>()));

            Assert.Throws<ArgumentNullException>(() => new ValidateDataInTargetTableEventArgs(fixture.CreateAnonymous<IDataSource>(), null, fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<int>()));
        }
    }
}
