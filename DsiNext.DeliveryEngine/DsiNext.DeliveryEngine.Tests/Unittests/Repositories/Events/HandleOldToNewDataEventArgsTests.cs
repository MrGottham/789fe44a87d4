using System;
using System.Collections.Generic;
using System.Linq;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Repositories.Data.OldToNew.Events;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Repositories.Events
{
    /// <summary>
    /// Tests arguments to the event raised when to handle data from a data repository which converts old delivery format to the new delivery format.
    /// </summary>
    [TestFixture]
    public class HandleOldToNewDataEventArgsTests
    {
        /// <summary>
        /// Test that the constructor initialize arguments where end of data is true to the event raised when to handle data from a data repository which converts old delivery format to the new delivery format.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeEventArgsWhereEndOfDataIsTrue()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));
            fixture.Customize<IDataObjectBase>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataObjectBase>()));
            fixture.Customize<IEnumerable<IDataObjectBase>>(e => e.FromFactory(() => fixture.CreateMany<IDataObjectBase>(5).ToList()));

            var table = fixture.CreateAnonymous<ITable>();
            var data = fixture.CreateMany<IEnumerable<IDataObjectBase>>(1024).ToList();
            var eventArgs = new HandleOldToNewDataEventArgs(table, data, true);
            Assert.That(eventArgs, Is.Not.Null);
            Assert.That(eventArgs.Table, Is.Not.Null);
            Assert.That(eventArgs.Table, Is.EqualTo(table));
            Assert.That(eventArgs.Data, Is.Not.Null);
            Assert.That(eventArgs.Data, Is.Not.Empty);
            Assert.That(eventArgs.Data, Is.EqualTo(data));
            Assert.That(eventArgs.EndOfData, Is.True);
        }

        /// <summary>
        /// Test that the constructor initialize arguments where end of data is false to the event raised when to handle data from a data repository which converts old delivery format to the new delivery format.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeEventArgsWhereEndOfDataIsFalse()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));
            fixture.Customize<IDataObjectBase>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataObjectBase>()));
            fixture.Customize<IEnumerable<IDataObjectBase>>(e => e.FromFactory(() => fixture.CreateMany<IDataObjectBase>(5).ToList()));

            var table = fixture.CreateAnonymous<ITable>();
            var data = fixture.CreateMany<IEnumerable<IDataObjectBase>>(1024).ToList();
            var eventArgs = new HandleOldToNewDataEventArgs(table, data, false);
            Assert.That(eventArgs, Is.Not.Null);
            Assert.That(eventArgs.Table, Is.Not.Null);
            Assert.That(eventArgs.Table, Is.EqualTo(table));
            Assert.That(eventArgs.Data, Is.Not.Null);
            Assert.That(eventArgs.Data, Is.Not.Empty);
            Assert.That(eventArgs.Data, Is.EqualTo(data));
            Assert.That(eventArgs.EndOfData, Is.False);
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the table for which to handle data is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfTableIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataObjectBase>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataObjectBase>()));
            fixture.Customize<IEnumerable<IDataObjectBase>>(e => e.FromFactory(() => fixture.CreateMany<IDataObjectBase>(5).ToList()));

            Assert.Throws<ArgumentNullException>(() => new HandleOldToNewDataEventArgs(null, fixture.CreateMany<IEnumerable<IDataObjectBase>>(1024), true));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the data to be handled is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfDataIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));

            Assert.Throws<ArgumentNullException>(() => new HandleOldToNewDataEventArgs(fixture.CreateAnonymous<ITable>(), null, true));
        }
    }
}
