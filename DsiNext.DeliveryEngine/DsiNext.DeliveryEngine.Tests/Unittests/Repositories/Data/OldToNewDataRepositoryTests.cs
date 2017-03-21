using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.Data.OldToNew;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Repositories.Data
{
    /// <summary>
    /// Tests the data repository for converting old delivery format to the new delivery format.
    /// </summary>
    [TestFixture]
    public class OldToNewDataRepositoryTests
    {
        /// <summary>
        /// Test that the constructor initialize the data repository.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeOldToNewDataRepository()
        {
            var oldToNewDataRepository = new OldToNewDataRepository(RepositoryTestHelper.GetSourcePathForTest());
            Assert.That(oldToNewDataRepository, Is.Not.Null);
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the path containing the old delivery format is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfPathIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new OldToNewDataRepository(null));
        }

        /// <summary>
        /// Test that the constructor throws an DeliveryEngineRepositoryException if a FILMAP.TAB file is not fout.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsDeliveryEngineRepositoryExceptionIfFilmapTabIsNotFound()
        {
            var exception = Assert.Throws<DeliveryEngineRepositoryException>(() => new OldToNewDataRepository(new DirectoryInfo(Environment.ExpandEnvironmentVariables("%Temp%"))));
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Empty);
            Assert.That(exception.Message, Is.StringContaining("'FILMAP.TAB'"));
        }

        /// <summary>
        /// Test that DataGetForTargetTable gets data from one or more source tables.
        /// </summary>
        [Test]
        public void TestThatDataGetForTargetTableGetDataFromOneOrMoreSourceTables()
        {
            var fixture = new Fixture();

            var fieldCollectionMock = new ObservableCollection<IField>();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                     .Return("SagsbehandlerID")
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (int?))
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfTarget)
                     .Return(typeof (int?))
                     .Repeat.Any();
            fieldMock.Expect(m => m.LengthOfSource)
                     .Return(4)
                     .Repeat.Any();
            fieldMock.Expect(m => m.Nullable)
                     .Return(false)
                     .Repeat.Any();
            fieldCollectionMock.Add(fieldMock);

            fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                     .Return("Navn")
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfTarget)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.LengthOfSource)
                     .Return(25)
                     .Repeat.Any();
            fieldMock.Expect(m => m.Nullable)
                     .Return(true)
                     .Repeat.Any();
            fieldCollectionMock.Add(fieldMock);

            fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                     .Return("Initialer")
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfTarget)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.LengthOfSource)
                     .Return(4)
                     .Repeat.Any();
            fieldMock.Expect(m => m.Nullable)
                     .Return(true)
                     .Repeat.Any();
            fieldCollectionMock.Add(fieldMock);

            fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                     .Return("Kontor")
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (int?))
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfTarget)
                     .Return(typeof (int?))
                     .Repeat.Any();
            fieldMock.Expect(m => m.LengthOfSource)
                     .Return(2)
                     .Repeat.Any();
            fieldMock.Expect(m => m.Nullable)
                     .Return(true)
                     .Repeat.Any();
            fieldCollectionMock.Add(fieldMock);

            fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                     .Return("Tlf")
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfTarget)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.LengthOfSource)
                     .Return(10)
                     .Repeat.Any();
            fieldMock.Expect(m => m.Nullable)
                     .Return(true)
                     .Repeat.Any();
            fieldCollectionMock.Add(fieldMock);

            fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                     .Return("Epost")
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfTarget)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.LengthOfSource)
                     .Return(40)
                     .Repeat.Any();
            fieldMock.Expect(m => m.Nullable)
                     .Return(true)
                     .Repeat.Any();
            fieldCollectionMock.Add(fieldMock);

            var mergedTableName = fixture.CreateAnonymous<string>();

            var firstTableMock = MockRepository.GenerateMock<ITable>();
            firstTableMock.Expect(m => m.NameTarget)
                .Return(mergedTableName)
                .Repeat.Any();
            firstTableMock.Expect(m => m.NameSource)
                .Return("SAGSBEH")
                .Repeat.Any();
            firstTableMock.Expect(m => m.Fields)
                .Return(new ReadOnlyObservableCollection<IField>(fieldCollectionMock))
                .Repeat.Any();

            var secondTableMock = MockRepository.GenerateMock<ITable>();
            secondTableMock.Expect(m => m.NameTarget)
                .Return(mergedTableName)
                .Repeat.Any();
            secondTableMock.Expect(m => m.NameSource)
                .Return("SAGSBEH")
                .Repeat.Any();
            secondTableMock.Expect(m => m.Fields)
                .Return(new ReadOnlyObservableCollection<IField>(fieldCollectionMock))
                .Repeat.Any();

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(new List<ITable> {firstTableMock, secondTableMock})))
                .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var oldToNewDataRepository = new OldToNewDataRepository(RepositoryTestHelper.GetSourcePathForTest());
            Assert.That(oldToNewDataRepository, Is.Not.Null);

            var eventCalled = 0;
            oldToNewDataRepository.OnHandleData += (s, e) =>
                {
                    Assert.That(s, Is.Not.Null);
                    Assert.That(e, Is.Not.Null);
                    Assert.That(e.Data, Is.Not.Null);
                    Assert.That(e.Data, Is.Not.Empty);
                    Assert.That(e.Data.Count(), Is.EqualTo(7));
                    Assert.That(e.EndOfData, Is.True);
                    eventCalled++;
                };
            oldToNewDataRepository.DataGetForTargetTable(mergedTableName, fixture.CreateAnonymous<IDataSource>());

            Assert.That(eventCalled, Is.EqualTo(2));
        }

        /// <summary>
        /// Test that DataGetForTargetTable throws an ArgumentNullException if name of the target table is null.
        /// </summary>
        [Test]
        public void TestThatDataGetForTargetTableThrowsArgumentNullExceptionIfTargetTableNameIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataSource>()));

            var oldToNewDataRepository = new OldToNewDataRepository(RepositoryTestHelper.GetSourcePathForTest());
            Assert.That(oldToNewDataRepository, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => oldToNewDataRepository.DataGetForTargetTable(null, fixture.CreateAnonymous<IDataSource>()));
        }

        /// <summary>
        /// Test that DataGetForTargetTable throws an ArgumentNullException if name of the target table is empty.
        /// </summary>
        [Test]
        public void TestThatDataGetForTargetTableThrowsArgumentNullExceptionIfTargetTableNameIsEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataSource>()));

            var oldToNewDataRepository = new OldToNewDataRepository(RepositoryTestHelper.GetSourcePathForTest());
            Assert.That(oldToNewDataRepository, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => oldToNewDataRepository.DataGetForTargetTable(string.Empty, fixture.CreateAnonymous<IDataSource>()));
        }

        /// <summary>
        /// Test that DataGetForTargetTable throws an ArgumentNullException if the data source is null
        /// </summary>
        [Test]
        public void TestThatDataGetForTargetTableThrowsArgumentNullExceptionIfDataSourceIsNull()
        {
            var fixture = new Fixture();

            var oldToNewDataRepository = new OldToNewDataRepository(RepositoryTestHelper.GetSourcePathForTest());
            Assert.That(oldToNewDataRepository, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => oldToNewDataRepository.DataGetForTargetTable(fixture.CreateAnonymous<string>(), null));
        }

        /// <summary>
        /// Tests that DataGetFromTable gets data from the table.
        /// </summary>
        [Test]
        public void TestThatDataGetFromTableGetsDataFromTable()
        {
            var fixture = new Fixture();

            var oldToNewDataRepository = new OldToNewDataRepository(RepositoryTestHelper.GetSourcePathForTest());
            Assert.That(oldToNewDataRepository, Is.Not.Null);

            var tableData = new List<IEnumerable<IDataObjectBase>>();
            oldToNewDataRepository.OnHandleData += (s, e) =>
                {
                    Assert.That(s, Is.Not.Null);
                    Assert.That(e, Is.Not.Null);
                    foreach (var tableRecord in e.Data)
                    {
                        Assert.That(tableRecord, Is.Not.Null);
                        Assert.That(tableRecord.Count(), Is.EqualTo(6));
                    }
                    tableData.AddRange(e.Data);
                };

            var fieldCollectionMock = new ObservableCollection<IField>();
            
            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                     .Return("SagsbehandlerID")
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (int?))
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfTarget)
                     .Return(typeof (int?))
                     .Repeat.Any();
            fieldMock.Expect(m => m.LengthOfSource)
                     .Return(4)
                     .Repeat.Any();
            fieldMock.Expect(m => m.Nullable)
                     .Return(false)
                     .Repeat.Any();
            fieldCollectionMock.Add(fieldMock);

            fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                     .Return("Navn")
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfTarget)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.LengthOfSource)
                     .Return(25)
                     .Repeat.Any();
            fieldMock.Expect(m => m.Nullable)
                     .Return(true)
                     .Repeat.Any();
            fieldCollectionMock.Add(fieldMock);

            fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                     .Return("Initialer")
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfTarget)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.LengthOfSource)
                     .Return(4)
                     .Repeat.Any();
            fieldMock.Expect(m => m.Nullable)
                     .Return(true)
                     .Repeat.Any();
            fieldCollectionMock.Add(fieldMock);

            fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                     .Return("Kontor")
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (int?))
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfTarget)
                     .Return(typeof (int?))
                     .Repeat.Any();
            fieldMock.Expect(m => m.LengthOfSource)
                     .Return(2)
                     .Repeat.Any();
            fieldMock.Expect(m => m.Nullable)
                     .Return(true)
                     .Repeat.Any();
            fieldCollectionMock.Add(fieldMock);

            fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                     .Return("Tlf")
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfTarget)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.LengthOfSource)
                     .Return(10)
                     .Repeat.Any();
            fieldMock.Expect(m => m.Nullable)
                     .Return(true)
                     .Repeat.Any();
            fieldCollectionMock.Add(fieldMock);

            fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                     .Return("Epost")
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfTarget)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.LengthOfSource)
                     .Return(40)
                     .Repeat.Any();
            fieldMock.Expect(m => m.Nullable)
                     .Return(true)
                     .Repeat.Any();
            fieldCollectionMock.Add(fieldMock);

            var tabelMock = MockRepository.GenerateMock<ITable>();
            tabelMock.Expect(m => m.NameSource)
                .Return("SAGSBEH")
                .Repeat.Any();
            tabelMock.Expect(m => m.Fields)
                .Return(new ReadOnlyObservableCollection<IField>(fieldCollectionMock))
                .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => tabelMock));

            oldToNewDataRepository.DataGetFromTable(fixture.CreateAnonymous<ITable>());
            Assert.That(tableData, Is.Not.Null);
            Assert.That(tableData.Count, Is.EqualTo(7));
        }

        /// <summary>
        /// Tests that DataGetFromTable throws an ArgumentNullException if the table is null.
        /// </summary>
        [Test]
        public void TestThatDataGetFromTableThrowsArgumentNullExceptionIfTableIsNull()
        {
            var oldToNewDataRepository = new OldToNewDataRepository(RepositoryTestHelper.GetSourcePathForTest());
            Assert.That(oldToNewDataRepository, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => oldToNewDataRepository.DataGetFromTable(null));
        }

        /// <summary>
        /// Tests that DataGetFromTable throws an DeliveryEngineRepositoryException if the table is not found.
        /// </summary>
        [Test]
        public void TestThatDataGetFromTableThrowsDeliveryEngineRepositoryExceptionIfTableIsNotFound()
        {
            var fixture = new Fixture();

            var oldToNewDataRepository = new OldToNewDataRepository(RepositoryTestHelper.GetSourcePathForTest());
            Assert.That(oldToNewDataRepository, Is.Not.Null);

            var tabelMock = MockRepository.GenerateMock<ITable>();
            tabelMock.Expect(m => m.NameSource)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => tabelMock));

            Assert.Throws<DeliveryEngineRepositoryException>(() => oldToNewDataRepository.DataGetFromTable(fixture.CreateAnonymous<ITable>()));
        }

        /// <summary>
        /// Tests that DataGetFromTable throws an DeliveryEngineSystemException when this exception occurs.
        /// </summary>
        [Test]
        public void TestThatDataGetFromTableThrowsDeliveryEngineSystemExceptionWhenDeliveryEngineSystemExceptionOccurs()
        {
            var fixture = new Fixture();

            var oldToNewDataRepository = new OldToNewDataRepository(RepositoryTestHelper.GetSourcePathForTest());
            Assert.That(oldToNewDataRepository, Is.Not.Null);

            // ReSharper disable ImplicitlyCapturedClosure
            oldToNewDataRepository.OnHandleData += (s, e) =>
                {
                    throw fixture.CreateAnonymous<DeliveryEngineSystemException>();
                };
            // ReSharper restore ImplicitlyCapturedClosure

            var fieldCollectionMock = new ObservableCollection<IField>();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                     .Return("SagsbehandlerID")
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (int?))
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfTarget)
                     .Return(typeof (int?))
                     .Repeat.Any();
            fieldMock.Expect(m => m.LengthOfSource)
                     .Return(4)
                     .Repeat.Any();
            fieldMock.Expect(m => m.Nullable)
                     .Return(false)
                     .Repeat.Any();
            fieldCollectionMock.Add(fieldMock);

            fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                     .Return("Navn")
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfTarget)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.LengthOfSource)
                     .Return(25)
                     .Repeat.Any();
            fieldMock.Expect(m => m.Nullable)
                     .Return(true)
                     .Repeat.Any();
            fieldCollectionMock.Add(fieldMock);

            fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                     .Return("Initialer")
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfTarget)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.LengthOfSource)
                     .Return(4)
                     .Repeat.Any();
            fieldMock.Expect(m => m.Nullable)
                     .Return(true)
                     .Repeat.Any();
            fieldCollectionMock.Add(fieldMock);

            fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                     .Return("Kontor")
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (int?))
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfTarget)
                     .Return(typeof (int?))
                     .Repeat.Any();
            fieldMock.Expect(m => m.LengthOfSource)
                     .Return(2)
                     .Repeat.Any();
            fieldMock.Expect(m => m.Nullable)
                     .Return(true)
                     .Repeat.Any();
            fieldCollectionMock.Add(fieldMock);

            fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                     .Return("Tlf")
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfTarget)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.LengthOfSource)
                     .Return(10)
                     .Repeat.Any();
            fieldMock.Expect(m => m.Nullable)
                     .Return(true)
                     .Repeat.Any();
            fieldCollectionMock.Add(fieldMock);

            fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                     .Return("Epost")
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfTarget)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.LengthOfSource)
                     .Return(40)
                     .Repeat.Any();
            fieldMock.Expect(m => m.Nullable)
                     .Return(true)
                     .Repeat.Any();
            fieldCollectionMock.Add(fieldMock);

            var tabelMock = MockRepository.GenerateMock<ITable>();
            tabelMock.Expect(m => m.NameSource)
                .Return("SAGSBEH")
                .Repeat.Any();
            tabelMock.Expect(m => m.Fields)
                .Return(new ReadOnlyObservableCollection<IField>(fieldCollectionMock))
                .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => tabelMock));

            Assert.Throws<DeliveryEngineSystemException>(() => oldToNewDataRepository.DataGetFromTable(fixture.CreateAnonymous<ITable>()));
        }

        /// <summary>
        /// Tests that DataGetFromTable throws an DeliveryEngineRepositoryException when an exception occurs.
        /// </summary>
        [Test]
        public void TestThatDataGetFromTableThrowsDeliveryEngineRepositoryExceptionWhenExceptionOccurs()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineMetadataExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineMetadataExceptionInfo>()));

            var oldToNewDataRepository = new OldToNewDataRepository(RepositoryTestHelper.GetSourcePathForTest());
            Assert.That(oldToNewDataRepository, Is.Not.Null);

            var tabelMock = MockRepository.GenerateMock<ITable>();
            tabelMock.Expect(m => m.NameSource)
                .Throw(fixture.CreateAnonymous<DeliveryEngineMetadataException>());
            fixture.Customize<ITable>(e => e.FromFactory(() => tabelMock));

            Assert.Throws<DeliveryEngineRepositoryException>(() => oldToNewDataRepository.DataGetFromTable(fixture.CreateAnonymous<ITable>()));
        }

        /// <summary>
        /// Test that GetDataQueryer throws an NotSupportedException.
        /// </summary>
        [Test]
        public void TestThatGetDataQueryerThrowsNotSupportedException()
        {
            var oldToNewDataRepository = new OldToNewDataRepository(RepositoryTestHelper.GetSourcePathForTest());
            Assert.That(oldToNewDataRepository, Is.Not.Null);

            Assert.Throws<NotSupportedException>(() => oldToNewDataRepository.GetDataQueryer());
        }

        /// <summary>
        /// Test that Clone clones data repository.
        /// </summary>
        [Test]
        public void TestThatCloneClonesDataRepository()
        {
            var oldToNewDataRepository = new OldToNewDataRepository(RepositoryTestHelper.GetSourcePathForTest());
            Assert.That(oldToNewDataRepository, Is.Not.Null);

            var clonedRepository = oldToNewDataRepository.Clone() as IDataRepository;
            Assert.That(clonedRepository, Is.Not.Null);
            Assert.That(clonedRepository, Is.Not.EqualTo(oldToNewDataRepository));
            Assert.That(clonedRepository, Is.TypeOf<OldToNewDataRepository>());
        }

        /// <summary>
        /// Test that Clone raise the OnClone event.
        /// </summary>
        [Test]
        public void TestThatCloneRaiseOnCloneEvent()
        {
            var oldToNewDataRepository = new OldToNewDataRepository(RepositoryTestHelper.GetSourcePathForTest());
            Assert.That(oldToNewDataRepository, Is.Not.Null);

            var eventCalled = false;
            oldToNewDataRepository.OnClone += (sender, eventArgs) =>
                {
                    Assert.That(sender, Is.Not.Null);
                    Assert.That(eventArgs, Is.Not.Null);
                    Assert.That(eventArgs.ClonedDataRepository, Is.Not.Null);
                    eventCalled = true;
                };

            Assert.That(eventCalled, Is.False);
            var clonedRepository = oldToNewDataRepository.Clone() as IDataRepository;
            Assert.That(clonedRepository, Is.Not.Null);
            Assert.That(eventCalled, Is.True);
        }
    }
}
