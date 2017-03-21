using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Events;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.Data.Oracle;
using DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Events;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Repositories.Data
{
    /// <summary>
    /// Tests Oracle client used by the delivery engine.
    /// </summary>
    [TestFixture]
    public class OracleClientTests
    {
        /// <summary>
        /// Test that the constructor initialize an Oracle client to be used by the delivery engine.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeOracleClient()
        {
            using (var oracleClient = new OracleClient())
            {
                Assert.That(oracleClient, Is.Not.Null);

                oracleClient.Dispose();
            }
        }

        /// <summary>
        /// Test that Dispose can be called.
        /// </summary>
        [Test]
        public void TestThatDisposeCanBeCalled()
        {
            using (var oracleClient = new OracleClient())
            {
                Assert.That(oracleClient, Is.Not.Null);

                oracleClient.Dispose();
            }
        }

        /// <summary>
        /// Test that Dispose can be called more than one time.
        /// </summary>
        [Test]
        public void TestThatDisposeCanBeCalledMoreThanOnce()
        {
            using (var oracleClient = new OracleClient())
            {
                Assert.That(oracleClient, Is.Not.Null);

                oracleClient.Dispose();
                oracleClient.Dispose();
            }
        }

        /// <summary>
        /// Test that ValidateTable throws an ArgumentNullException if the table is null.
        /// </summary>
        [Test]
        public void TestThatValidateTableThrowsArgumentNullExceptionIfTableIsNull()
        {
            using (var oracleClient = new OracleClient())
            {
                Assert.Throws<ArgumentNullException>(() => oracleClient.ValidateTable(null));

                oracleClient.Dispose();
            }
        }

        /// <summary>
        /// Test that ValidateTable validates a table.
        /// </summary>
        [Test]
        public void TestThatValidateTableValidatesTable()
        {
            var fieldCollectionMock = new List<IField>(7)
                {
                    MockRepository.GenerateMock<IField>(),
                    MockRepository.GenerateMock<IField>(),
                    MockRepository.GenerateMock<IField>(),
                    MockRepository.GenerateMock<IField>(),
                    MockRepository.GenerateMock<IField>(),
                    MockRepository.GenerateMock<IField>(),
                    MockRepository.GenerateMock<IField>()
                };
            fieldCollectionMock.ElementAt(0).Expect(m => m.NameSource)
                .Return("SYSTEM_NR")
                .Repeat.Any();
            fieldCollectionMock.ElementAt(0).Expect(m => m.DatatypeOfSource)
                .Return(typeof (int?))
                .Repeat.Any();
            fieldCollectionMock.ElementAt(1).Expect(m => m.NameSource)
                .Return("TABEL")
                .Repeat.Any();
            fieldCollectionMock.ElementAt(1).Expect(m => m.DatatypeOfSource)
                .Return(typeof (int?))
                .Repeat.Any();
            fieldCollectionMock.ElementAt(2).Expect(m => m.NameSource)
                .Return("NR")
                .Repeat.Any();
            fieldCollectionMock.ElementAt(2).Expect(m => m.DatatypeOfSource)
                .Return(typeof (string))
                .Repeat.Any();
            fieldCollectionMock.ElementAt(3).Expect(m => m.NameSource)
                .Return("TEKST")
                .Repeat.Any();
            fieldCollectionMock.ElementAt(3).Expect(m => m.DatatypeOfSource)
                .Return(typeof (string))
                .Repeat.Any();
            fieldCollectionMock.ElementAt(4).Expect(m => m.NameSource)
                .Return("FORDELPCT")
                .Repeat.Any();
            fieldCollectionMock.ElementAt(4).Expect(m => m.DatatypeOfSource)
                .Return(typeof (decimal?))
                .Repeat.Any();
            fieldCollectionMock.ElementAt(5).Expect(m => m.NameSource)
                .Return("O_DATO")
                .Repeat.Any();
            fieldCollectionMock.ElementAt(5).Expect(m => m.DatatypeOfSource)
                .Return(typeof (DateTime?))
                .Repeat.Any();
            fieldCollectionMock.ElementAt(6).Expect(m => m.NameSource)
                .Return("O_TID")
                .Repeat.Any();
            fieldCollectionMock.ElementAt(6).Expect(m => m.DatatypeOfSource)
                .Return(typeof (TimeSpan?))
                .Repeat.Any();
            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameSource)
                .Return("TABEL")
                .Repeat.Any();
            tableMock.Expect(m => m.Fields)
                .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(fieldCollectionMock)))
                .Repeat.Any();

            using (var oracleClient = new OracleClient())
            {
                oracleClient.ValidateTable(tableMock);

                oracleClient.Dispose();
            }

            tableMock.AssertWasCalled(m => m.NameSource);
            tableMock.AssertWasCalled(m => m.Fields);
            fieldCollectionMock.ElementAt(0).AssertWasCalled(m => m.NameSource);
            fieldCollectionMock.ElementAt(0).AssertWasCalled(m => m.DatatypeOfSource);
            fieldCollectionMock.ElementAt(1).AssertWasCalled(m => m.NameSource);
            fieldCollectionMock.ElementAt(1).AssertWasCalled(m => m.DatatypeOfSource);
            fieldCollectionMock.ElementAt(2).AssertWasCalled(m => m.NameSource);
            fieldCollectionMock.ElementAt(2).AssertWasCalled(m => m.DatatypeOfSource);
            fieldCollectionMock.ElementAt(3).AssertWasCalled(m => m.NameSource);
            fieldCollectionMock.ElementAt(3).AssertWasCalled(m => m.DatatypeOfSource);
        }

        /// <summary>
        /// Test that ValidateTable throws an DeliveryEngineMetadataException if the table does not exist.
        /// </summary>
        [Test]
        public void TestThatValidateTableThrowsDeliveryEngineMetadataExceptionIfTableDoesNotExist()
        {
            var fixture = new Fixture();

            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameSource)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();

            using (var oracleClient = new OracleClient())
            {
                Assert.Throws<DeliveryEngineMetadataException>(() => oracleClient.ValidateTable(tableMock));

                oracleClient.Dispose();
            }

            tableMock.AssertWasCalled(m => m.NameSource);
        }

        /// <summary>
        /// Test that ValidateTable throws an DeliveryEngineMetadataException if the a column in the table does not exist.
        /// </summary>
        [Test]
        public void TestThatValidateTableThrowsDeliveryEngineMetadataExceptionIfColumnDoesNotExist()
        {
            var fixture = new Fixture();

            var fieldCollectionMock = new List<IField>(1) {MockRepository.GenerateMock<IField>()};
            fieldCollectionMock.ElementAt(0).Expect(m => m.NameSource)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();

            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameSource)
                .Return("TABEL")
                .Repeat.Any();
            tableMock.Expect(m => m.Fields)
                .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(fieldCollectionMock)))
                .Repeat.Any();

            using (var oracleClient = new OracleClient())
            {
                Assert.Throws<DeliveryEngineMetadataException>(() => oracleClient.ValidateTable(tableMock));

                oracleClient.Dispose();
            }

            tableMock.AssertWasCalled(m => m.Fields);
            fieldCollectionMock.ElementAt(0).AssertWasCalled(m => m.NameSource);
        }

        /// <summary>
        /// Test that ValidateTable throws an DeliveryEngineMetadataException if the datatype from the database can't be mapped.
        /// </summary>
        [Test]
        public void TestThatValidateTableThrowsDeliveryEngineMetadataExceptionIfDatatypeFromDatabaseCanNotBeMapped()
        {
            var fieldCollectionMock = new List<IField>(1) { MockRepository.GenerateMock<IField>() };
            fieldCollectionMock.ElementAt(0).Expect(m => m.NameSource)
                .Return("SYSTEM_NR")
                .Repeat.Any();
            fieldCollectionMock.ElementAt(0).Expect(m => m.DatatypeOfSource)
                .Return(typeof (string))
                .Repeat.Any();

            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameSource)
                .Return("TABEL")
                .Repeat.Any();
            tableMock.Expect(m => m.Fields)
                .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(fieldCollectionMock)))
                .Repeat.Any();

            using (var oracleClient = new OracleClient())
            {
                Assert.Throws<DeliveryEngineMetadataException>(() => oracleClient.ValidateTable(tableMock));

                oracleClient.Dispose();
            }

            tableMock.AssertWasCalled(m => m.Fields);
            fieldCollectionMock.ElementAt(0).AssertWasCalled(m => m.DatatypeOfSource);
        }

        /// <summary>
        /// Test that GetData throws an ArgumentNullException if the table is null.
        /// </summary>
        [Test]
        public void TestThatGetDataThrowsArgumentNullExceptionIfTableIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IHandleDataEventArgs>(e => e.FromFactory(() => MockRepository.GenerateMock<IHandleDataEventArgs>()));
            DeliveryEngineEventHandler<IHandleDataEventArgs> onHandleOracleData = (s, e) =>
                {
                    Assert.That(s, Is.Not.Null);
                    Assert.That(e, Is.Not.Null);
                };
            onHandleOracleData.Invoke(fixture.CreateAnonymous<object>(), fixture.CreateAnonymous<IHandleDataEventArgs>());

            using (var oracleClient = new OracleClient())
            {
                Assert.Throws<ArgumentNullException>(() => oracleClient.GetData(null, onHandleOracleData));

                oracleClient.Dispose();
            }
        }

        /// <summary>
        /// Test that GetData throws an ArgumentNullException if the event handler to handle data from the oracle data repository is null.
        /// </summary>
        [Test]
        public void TestThatGetDataThrowsArgumentNullExceptionIfOnHandleOracleDataIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));

            var tableMock = fixture.CreateAnonymous<ITable>();

            using (var oracleClient = new OracleClient())
            {
                Assert.Throws<ArgumentNullException>(() => oracleClient.GetData(tableMock, null));

                oracleClient.Dispose();
            }

            tableMock.AssertWasNotCalled(m => m.Fields);
            tableMock.AssertWasNotCalled(m => m.RecordFilters);
            tableMock.AssertWasNotCalled(m => m.PrimaryKey);
            tableMock.AssertWasNotCalled(m => m.NameSource);
        }

        /// <summary>
        /// Test that GetData gets data without record filters on the table.
        /// </summary>
        [Test]
        public void TestThatGetDataGetDataWithoutRecordFilters()
        {
            var fieldCollectionMock = new List<IField>(7)
                {
                    MockRepository.GenerateMock<IField>(),
                    MockRepository.GenerateMock<IField>(),
                    MockRepository.GenerateMock<IField>(),
                    MockRepository.GenerateMock<IField>(),
                    MockRepository.GenerateMock<IField>(),
                    MockRepository.GenerateMock<IField>(),
                    MockRepository.GenerateMock<IField>()
                };
            fieldCollectionMock.ElementAt(0).Expect(m => m.NameSource)
                               .Return("SYSTEM_NR")
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(0).Expect(m => m.DatatypeOfSource)
                               .Return(typeof (int?))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(0).Expect(m => m.DatatypeOfTarget)
                               .Return(typeof (int?))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(0).Expect(m => m.Nullable)
                               .Return(false)
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(1).Expect(m => m.NameSource)
                               .Return("TABEL")
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(1).Expect(m => m.DatatypeOfSource)
                               .Return(typeof (int?))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(1).Expect(m => m.DatatypeOfTarget)
                               .Return(typeof (int?))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(1).Expect(m => m.Nullable)
                               .Return(false)
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(2).Expect(m => m.NameSource)
                               .Return("NR")
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(2).Expect(m => m.DatatypeOfSource)
                               .Return(typeof (string))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(2).Expect(m => m.DatatypeOfTarget)
                               .Return(typeof (string))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(2).Expect(m => m.Nullable)
                               .Return(false)
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(3).Expect(m => m.NameSource)
                               .Return("TEKST")
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(3).Expect(m => m.DatatypeOfSource)
                               .Return(typeof (string))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(3).Expect(m => m.DatatypeOfTarget)
                               .Return(typeof (string))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(3).Expect(m => m.Nullable)
                               .Return(true)
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(4).Expect(m => m.NameSource)
                               .Return("FORDELPCT")
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(4).Expect(m => m.DatatypeOfSource)
                               .Return(typeof (decimal?))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(4).Expect(m => m.DatatypeOfTarget)
                               .Return(typeof (decimal?))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(4).Expect(m => m.Nullable)
                               .Return(true)
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(5).Expect(m => m.NameSource)
                               .Return("O_DATO")
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(5).Expect(m => m.DatatypeOfSource)
                               .Return(typeof (DateTime?))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(5).Expect(m => m.DatatypeOfTarget)
                               .Return(typeof (DateTime?))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(5).Expect(m => m.Nullable)
                               .Return(true)
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(6).Expect(m => m.NameSource)
                               .Return("O_TID")
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(6).Expect(m => m.DatatypeOfSource)
                               .Return(typeof (TimeSpan?))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(6).Expect(m => m.DatatypeOfTarget)
                               .Return(typeof (TimeSpan?))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(6).Expect(m => m.Nullable)
                               .Return(true)
                               .Repeat.Any();
            var primaryKeyMock = MockRepository.GenerateMock<ICandidateKey>();
            primaryKeyMock.Expect(m => m.Fields)
                          .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(fieldCollectionMock.GetRange(0, 3).Select(field => new KeyValuePair<IField, IMap>(field, null)))))
                          .Repeat.Any();
            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameSource)
                     .Return("TABEL")
                     .Repeat.Any();
            tableMock.Expect(m => m.PrimaryKey)
                     .Return(primaryKeyMock)
                     .Repeat.Any();
            tableMock.Expect(m => m.RecordFilters)
                     .Return(new ReadOnlyObservableCollection<IFilter>(new ObservableCollection<IFilter>(new List<IFilter>(0))))
                     .Repeat.Any();
            tableMock.Expect(m => m.Fields)
                     .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(fieldCollectionMock)))
                     .Repeat.Any();

            using (var oracleClient = new OracleClient())
            {
                var data = new List<IEnumerable<IDataObjectBase>>();
                oracleClient.GetData(tableMock, (s, e) => data.AddRange(e.Data));
                Assert.That(data, Is.Not.Null);
                Assert.That(data.Count(), Is.GreaterThan(0));

                oracleClient.Dispose();
            }

            tableMock.AssertWasCalled(m => m.Fields);
            tableMock.AssertWasCalled(m => m.RecordFilters);
            tableMock.AssertWasCalled(m => m.PrimaryKey);
            tableMock.AssertWasCalled(m => m.NameSource);
            primaryKeyMock.AssertWasCalled(m => m.Fields);
        }

        /// <summary>
        /// Test that GetData gets data with record filters on the table.
        /// </summary>
        [Test]
        public void TestThatGetDataGetDataWithRecordFilters()
        {
            var fieldCollectionMock = new List<IField>(7)
                {
                    MockRepository.GenerateMock<IField>(),
                    MockRepository.GenerateMock<IField>(),
                    MockRepository.GenerateMock<IField>(),
                    MockRepository.GenerateMock<IField>(),
                    MockRepository.GenerateMock<IField>(),
                    MockRepository.GenerateMock<IField>(),
                    MockRepository.GenerateMock<IField>()
                };
            fieldCollectionMock.ElementAt(0).Expect(m => m.NameSource)
                               .Return("SYSTEM_NR")
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(0).Expect(m => m.DatatypeOfSource)
                               .Return(typeof (int?))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(0).Expect(m => m.DatatypeOfTarget)
                               .Return(typeof (int?))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(0).Expect(m => m.Nullable)
                               .Return(false)
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(1).Expect(m => m.NameSource)
                               .Return("TABEL")
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(1).Expect(m => m.DatatypeOfSource)
                               .Return(typeof (int?))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(1).Expect(m => m.DatatypeOfTarget)
                               .Return(typeof (int?))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(1).Expect(m => m.Nullable)
                               .Return(false)
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(2).Expect(m => m.NameSource)
                               .Return("NR")
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(2).Expect(m => m.DatatypeOfSource)
                               .Return(typeof (string))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(2).Expect(m => m.DatatypeOfTarget)
                               .Return(typeof (string))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(2).Expect(m => m.Nullable)
                               .Return(false)
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(3).Expect(m => m.NameSource)
                               .Return("TEKST")
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(3).Expect(m => m.DatatypeOfSource)
                               .Return(typeof (string))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(3).Expect(m => m.DatatypeOfTarget)
                               .Return(typeof (string))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(3).Expect(m => m.Nullable)
                               .Return(true)
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(4).Expect(m => m.NameSource)
                               .Return("FORDELPCT")
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(4).Expect(m => m.DatatypeOfSource)
                               .Return(typeof (decimal?))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(4).Expect(m => m.DatatypeOfTarget)
                               .Return(typeof (decimal?))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(4).Expect(m => m.Nullable)
                               .Return(true)
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(5).Expect(m => m.NameSource)
                               .Return("O_DATO")
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(5).Expect(m => m.DatatypeOfSource)
                               .Return(typeof (DateTime?))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(5).Expect(m => m.DatatypeOfTarget)
                               .Return(typeof (DateTime?))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(5).Expect(m => m.Nullable)
                               .Return(true)
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(6).Expect(m => m.NameSource)
                               .Return("O_TID")
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(6).Expect(m => m.DatatypeOfSource)
                               .Return(typeof (TimeSpan?))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(6).Expect(m => m.DatatypeOfTarget)
                               .Return(typeof (TimeSpan?))
                               .Repeat.Any();
            fieldCollectionMock.ElementAt(6).Expect(m => m.Nullable)
                               .Return(true)
                               .Repeat.Any();
            var primaryKeyMock = MockRepository.GenerateMock<ICandidateKey>();
            primaryKeyMock.Expect(m => m.Fields)
                          .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(fieldCollectionMock.GetRange(0, 3).Select(field => new KeyValuePair<IField, IMap>(field, null)))))
                          .Repeat.Any();
            var recordFilterCollectionMock = new List<IFilter>(2)
                {
                    MockRepository.GenerateMock<IFilter>(),
                    MockRepository.GenerateMock<IFilter>()
                };
            recordFilterCollectionMock.ElementAt(0).Expect(m => m.AsSql())
                                      .Return("TABEL=0")
                                      .Repeat.Any();
            recordFilterCollectionMock.ElementAt(1).Expect(m => m.AsSql())
                                      .Return("TABEL=1")
                                      .Repeat.Any();
            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameSource)
                     .Return("TABEL")
                     .Repeat.Any();
            tableMock.Expect(m => m.PrimaryKey)
                     .Return(primaryKeyMock)
                     .Repeat.Any();
            tableMock.Expect(m => m.RecordFilters)
                     .Return(new ReadOnlyObservableCollection<IFilter>(new ObservableCollection<IFilter>(recordFilterCollectionMock)))
                     .Repeat.Any();
            tableMock.Expect(m => m.Fields)
                     .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(fieldCollectionMock)))
                     .Repeat.Any();

            using (var oracleClient = new OracleClient())
            {
                var data = new List<IEnumerable<IDataObjectBase>>();
                oracleClient.GetData(tableMock, (s, e) =>
                    {
                        data.AddRange(e.Data);
                        Assert.That(data.Any(row =>
                            {
                                var dataRow = row.ToList();
                                return dataRow.ElementAt(1).GetSourceValue<int?>() == 0 &&
                                       dataRow.ElementAt(1).GetSourceValue<int?>() == 1;
                            }), Is.False);
                    });
                Assert.That(data, Is.Not.Null);

                var dataAsList = data.ToList();
                Assert.That(dataAsList, Is.Not.Null);
                Assert.That(dataAsList.Count(), Is.GreaterThan(0));

                oracleClient.Dispose();
            }

            tableMock.AssertWasCalled(m => m.Fields);
            tableMock.AssertWasCalled(m => m.RecordFilters);
            tableMock.AssertWasCalled(m => m.PrimaryKey);
            tableMock.AssertWasCalled(m => m.NameSource);
            recordFilterCollectionMock.ElementAt(0).AssertWasCalled(m => m.AsSql());
            recordFilterCollectionMock.ElementAt(1).AssertWasCalled(m => m.AsSql());
            primaryKeyMock.AssertWasCalled(m => m.Fields);
        }

        /// <summary>
        /// Test that SelectCountForTable throws an ArgumentNullException if the table is null.
        /// </summary>
        [Test]
        public void TestThatSelectCountForTableThrowsArgumentNullExceptionIfTableIsNull()
        {
            using (var oracleClient = new OracleClient())
            {
                Assert.Throws<ArgumentNullException>(() => oracleClient.SelectCountForTable(null));

                oracleClient.Dispose();
            }
        }

        /// <summary>
        /// Test that SelectCountForTable gets the number of records in a table without record filters.
        /// </summary>
        [Test]
        public void TestThatSelectCountForTableGetNumberOfRecordsWithoutRecordFilters()
        {
            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameSource)
                     .Return("TABEL")
                     .Repeat.Any();
            tableMock.Expect(m => m.RecordFilters)
                     .Return(new ReadOnlyObservableCollection<IFilter>(new ObservableCollection<IFilter>(new List<IFilter>(0))))
                     .Repeat.Any();

            using (var oracleClient = new OracleClient())
            {
                var numberOfRecords = oracleClient.SelectCountForTable(tableMock);
                Assert.That(numberOfRecords, Is.GreaterThan(0));

                oracleClient.Dispose();
            }

            tableMock.AssertWasCalled(m => m.NameSource);
            tableMock.AssertWasCalled(m => m.RecordFilters);
        }

        /// <summary>
        /// Test that SelectCountForTable gets the number of records in a table with record filters.
        /// </summary>
        [Test]
        public void TestThatSelectCountForTableGetNumberOfRecordsWithRecordFilters()
        {
            var firstFilter = MockRepository.GenerateMock<IFilter>();
            firstFilter.Expect(m => m.AsSql())
                       .Return("SYSTEM_NR=0 AND TABEL=0 AND NR='00'")
                       .Repeat.Any();

            var secordFilter = MockRepository.GenerateMock<IFilter>();
            secordFilter.Expect(m => m.AsSql())
                        .Return("SYSTEM_NR=0 AND TABEL=0 AND NR='01'")
                        .Repeat.Any();

            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameSource)
                     .Return("TABEL")
                     .Repeat.Any();
            tableMock.Expect(m => m.RecordFilters)
                     .Return(new ReadOnlyObservableCollection<IFilter>(new ObservableCollection<IFilter>(new List<IFilter> {firstFilter, secordFilter})))
                     .Repeat.Any();

            using (var oracleClient = new OracleClient())
            {
                var numberOfRecords = oracleClient.SelectCountForTable(tableMock);
                Assert.That(numberOfRecords, Is.EqualTo(2));

                oracleClient.Dispose();
            }

            tableMock.AssertWasCalled(m => m.NameSource);
            tableMock.AssertWasCalled(m => m.RecordFilters);
            firstFilter.AssertWasCalled(m => m.AsSql());
            secordFilter.AssertWasCalled(m => m.AsSql());
        }

        /// <summary>
        /// Test that GetNumberOfEqualKeyValues throws an ArgumentNullException if the key is null.
        /// </summary>
        [Test]
        public void TestThatGetNumberOfEqualKeyValuesThrowsArgumentNullExceptionIfKeyIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<KeyValuePair<string, object>>(e => e.FromFactory(() => new KeyValuePair<string, object>(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>())));

            using (var oracleClient = new OracleClient())
            {
                Assert.Throws<ArgumentNullException>(() => oracleClient.GetNumberOfEqualKeyValues(null, fixture.CreateMany<KeyValuePair<string, object>>(5).ToList(), fixture.CreateAnonymous<string>()));

                oracleClient.Dispose();
            }
        }

        /// <summary>
        /// Test that GetNumberOfEqualKeyValues throws an ArgumentNullException if extra criterias for the records filters is null.
        /// </summary>
        [Test]
        public void TestThatGetNumberOfEqualKeyValuesThrowsArgumentNullExceptionIfExtraCriteriasIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IKey>(e => e.FromFactory(() => MockRepository.GenerateMock<IKey>()));

            using (var oracleClient = new OracleClient())
            {
                Assert.Throws<ArgumentNullException>(() => oracleClient.GetNumberOfEqualKeyValues(fixture.CreateAnonymous<IKey>(), null, fixture.CreateAnonymous<string>()));

                oracleClient.Dispose();
            }
        }

        /// <summary>
        /// Test that GetNumberOfEqualKeyValues throws an ArgumentNullException if matching key value is null.
        /// </summary>
        [Test]
        public void TestThatGetNumberOfEqualKeyValuesThrowsArgumentNullExceptionIfMatchingKeyValueIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IKey>(e => e.FromFactory(() => MockRepository.GenerateMock<IKey>()));
            fixture.Customize<KeyValuePair<string, object>>(e => e.FromFactory(() => new KeyValuePair<string, object>(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>())));

            using (var oracleClient = new OracleClient())
            {
                Assert.Throws<ArgumentNullException>(() => oracleClient.GetNumberOfEqualKeyValues(fixture.CreateAnonymous<IKey>(), fixture.CreateMany<KeyValuePair<string, object>>(5).ToList(), null));

                oracleClient.Dispose();
            }
        }

        /// <summary>
        /// Test that GetNumberOfEqualKeyValues throws an ArgumentNullException if the matching key value is empty.
        /// </summary>
        [Test]
        public void TestThatGetNumberOfEqualKeyValuesThrowsArgumentNullExceptionIfMatchingKeyValueIsEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize<IKey>(e => e.FromFactory(() => MockRepository.GenerateMock<IKey>()));
            fixture.Customize<KeyValuePair<string, object>>(e => e.FromFactory(() => new KeyValuePair<string, object>(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>())));

            using (var oracleClient = new OracleClient())
            {
                Assert.Throws<ArgumentNullException>(() => oracleClient.GetNumberOfEqualKeyValues(fixture.CreateAnonymous<IKey>(), fixture.CreateMany<KeyValuePair<string, object>>(5).ToList(), string.Empty));

                oracleClient.Dispose();
            }
        }

        /// <summary>
        /// Test that GetNumberOfEqualKeyValues throws an DeliveryEngineMetadataException if table is not defined on the key.
        /// </summary>
        [Test]
        public void TestThatGetNumberOfEqualKeyValuesThrowsDeliveryEngineMetadataExceptionIfTableIsNullOnKey()
        {
            var fixture = new Fixture();
            fixture.Customize<KeyValuePair<string, object>>(e => e.FromFactory(() => new KeyValuePair<string, object>(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>())));

            var keyMock = MockRepository.GenerateMock<IKey>();
            keyMock.Expect(m => m.Table)
                   .Return(null)
                   .Repeat.Any();

            using (var oracleClient = new OracleClient())
            {
                Assert.Throws<DeliveryEngineMetadataException>(() => oracleClient.GetNumberOfEqualKeyValues(keyMock, fixture.CreateMany<KeyValuePair<string, object>>(5).ToList(), fixture.CreateAnonymous<string>()));

                oracleClient.Dispose();
            }

            keyMock.AssertWasCalled(m => m.Table);
        }

        /// <summary>
        /// Test that GetNumberOfEqualKeyValues gets number of equal key values for a given key where there only are one record.
        /// </summary>
        [Test]
        public void TestThatGetNumberOfEqualKeyValuesGetsNumberOfEqualKeyValuesForKeyWhereThereOnlyAreOnRecord()
        {
            var fixture = new Fixture();

            var fieldCollection = new List<IField>
                {
                    MockRepository.GenerateMock<IField>(),
                    MockRepository.GenerateMock<IField>(),
                    MockRepository.GenerateMock<IField>(),
                    MockRepository.GenerateMock<IField>()
                };
            fieldCollection.ElementAt(0).Expect(m => m.NameSource)
                           .Return("SYSTEM_NR")
                           .Repeat.Any();
            fieldCollection.ElementAt(0).Expect(m => m.DatatypeOfSource)
                           .Return(typeof (int?))
                           .Repeat.Any();
            fieldCollection.ElementAt(1).Expect(m => m.NameSource)
                           .Return("TABEL")
                           .Repeat.Any();
            fieldCollection.ElementAt(1).Expect(m => m.DatatypeOfSource)
                           .Return(typeof (int?))
                           .Repeat.Any();
            fieldCollection.ElementAt(2).Expect(m => m.NameSource)
                           .Return("NR")
                           .Repeat.Any();
            fieldCollection.ElementAt(2).Expect(m => m.DatatypeOfSource)
                           .Return(typeof (string))
                           .Repeat.Any();
            fieldCollection.ElementAt(3).Expect(m => m.NameSource)
                           .Return("TEKST")
                           .Repeat.Any();
            fieldCollection.ElementAt(3).Expect(m => m.DatatypeOfSource)
                           .Return(typeof (string))
                           .Repeat.Any();

            var filterCollection = new List<IFilter>();
            var tableMock = MockRepository.GenerateMock<ITable>();
            var keyMock = MockRepository.GenerateMock<ICandidateKey>();
            keyMock.Expect(m => m.Table)
                   .Return(tableMock)
                   .Repeat.Any();
            keyMock.Expect(m => m.Fields)
                   .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(fieldCollection.GetRange(0, 3).Select(field => new KeyValuePair<IField, IMap>(field, null)))))
                   .Repeat.Any();
            tableMock.Expect(m => m.NameSource)
                     .Return("TABEL")
                     .Repeat.Any();
            tableMock.Expect(m => m.Fields)
                     .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(fieldCollection)))
                     .Repeat.Any();
            tableMock.Expect(m => m.PrimaryKey)
                     .Return(keyMock)
                     .Repeat.Any();
            tableMock.Expect(m => m.RecordFilters)
                     .WhenCalled(e => e.ReturnValue = new ReadOnlyObservableCollection<IFilter>(new ObservableCollection<IFilter>(filterCollection)))
                     .Return(new ReadOnlyObservableCollection<IFilter>(new ObservableCollection<IFilter>(filterCollection)))
                     .Repeat.Any();
            tableMock.Expect(m => m.AddRecordFilter(Arg<IFilter>.Is.NotNull))
                     .WhenCalled(e => filterCollection.Add((IFilter) e.Arguments.ElementAt(0)))
                     .Repeat.Any();
            tableMock.Expect(m => m.Clone())
                     .Return(tableMock)
                     .Repeat.Any();

            using (var oracleClient = new OracleClient())
            {
                var extraCriterias = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>("SYSTEM_NR", 0),
                        new KeyValuePair<string, object>("TABEL", 0),
                        new KeyValuePair<string, object>("NR", "00")
                    };
                var numberOfEqualKeys = oracleClient.GetNumberOfEqualKeyValues(keyMock, extraCriterias, fixture.CreateAnonymous<string>());
                Assert.That(numberOfEqualKeys, Is.EqualTo(1));

                oracleClient.Dispose();
            }

            keyMock.AssertWasCalled(m => m.Table);
            tableMock.AssertWasCalled(m => m.Clone());
            tableMock.AssertWasCalled(m => m.RecordFilters, opt => opt.Repeat.Times(7));
            tableMock.AssertWasCalled(m => m.AddRecordFilter(Arg<IFilter>.Is.NotNull));
        }

        /// <summary>
        /// Test that GetNumberOfEqualKeyValues gets number of equal key values for a given key where there are multiple records.
        /// </summary>
        [Test]
        public void TestThatGetNumberOfEqualKeyValuesGetsNumberOfEqualKeyValuesForKeyWhereThereMultipleRecords()
        {
            var fieldCollection = new List<IField>
                {
                    MockRepository.GenerateMock<IField>(),
                    MockRepository.GenerateMock<IField>(),
                    MockRepository.GenerateMock<IField>(),
                    MockRepository.GenerateMock<IField>()
                };
            fieldCollection.ElementAt(0).Expect(m => m.NameSource)
                           .Return("SYSTEM_NR")
                           .Repeat.Any();
            fieldCollection.ElementAt(0).Expect(m => m.DatatypeOfSource)
                           .Return(typeof (int?))
                           .Repeat.Any();
            fieldCollection.ElementAt(0).Expect(m => m.DatatypeOfTarget)
                           .Return(typeof (int?))
                           .Repeat.Any();
            fieldCollection.ElementAt(1).Expect(m => m.NameSource)
                           .Return("TABEL")
                           .Repeat.Any();
            fieldCollection.ElementAt(1).Expect(m => m.DatatypeOfSource)
                           .Return(typeof (int?))
                           .Repeat.Any();
            fieldCollection.ElementAt(1).Expect(m => m.DatatypeOfTarget)
                           .Return(typeof (int?))
                           .Repeat.Any();
            fieldCollection.ElementAt(2).Expect(m => m.NameSource)
                           .Return("NR")
                           .Repeat.Any();
            fieldCollection.ElementAt(2).Expect(m => m.DatatypeOfSource)
                           .Return(typeof (string))
                           .Repeat.Any();
            fieldCollection.ElementAt(2).Expect(m => m.DatatypeOfTarget)
                           .Return(typeof (string))
                           .Repeat.Any();
            fieldCollection.ElementAt(3).Expect(m => m.NameSource)
                           .Return("TEKST")
                           .Repeat.Any();
            fieldCollection.ElementAt(3).Expect(m => m.DatatypeOfSource)
                           .Return(typeof (string))
                           .Repeat.Any();
            fieldCollection.ElementAt(3).Expect(m => m.DatatypeOfTarget)
                           .Return(typeof (string))
                           .Repeat.Any();

            var filterCollection = new List<IFilter>();
            var tableMock = MockRepository.GenerateMock<ITable>();
            var keyMock = MockRepository.GenerateMock<ICandidateKey>();
            keyMock.Expect(m => m.Table)
                   .Return(tableMock)
                   .Repeat.Any();
            keyMock.Expect(m => m.Fields)
                   .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(new List<KeyValuePair<IField, IMap>>(fieldCollection.GetRange(0, 3).Select(field => new KeyValuePair<IField, IMap>(field, null))))))
                   .Repeat.Any();
            tableMock.Expect(m => m.NameSource)
                     .Return("TABEL")
                     .Repeat.Any();
            tableMock.Expect(m => m.Fields)
                     .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(fieldCollection)))
                     .Repeat.Any();
            tableMock.Expect(m => m.PrimaryKey)
                     .Return(keyMock)
                     .Repeat.Any();
            tableMock.Expect(m => m.RecordFilters)
                     .WhenCalled(e => e.ReturnValue = new ReadOnlyObservableCollection<IFilter>(new ObservableCollection<IFilter>(filterCollection)))
                     .Return(new ReadOnlyObservableCollection<IFilter>(new ObservableCollection<IFilter>(filterCollection)))
                     .Repeat.Any();
            tableMock.Expect(m => m.AddRecordFilter(Arg<IFilter>.Is.NotNull))
                     .WhenCalled(e => filterCollection.Add((IFilter)e.Arguments.ElementAt(0)))
                     .Repeat.Any();
            tableMock.Expect(m => m.Clone())
                     .Return(tableMock)
                     .Repeat.Any();

            var mocker = new MockRepository();
            var dataManipulatorsMock = mocker.DynamicMultiMock<IDataManipulators>(new[] {typeof (IEnumerable<IDataManipulator>)});
            Expect.Call(dataManipulatorsMock.GetEnumerator())
                  .Return(new List<IDataManipulator>(0).GetEnumerator())
                  .Repeat.Any();
            mocker.ReplayAll();

            using (var oracleClient = new OracleClient(dataManipulatorsMock))
            {
                var extraCriterias = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>("SYSTEM_NR", 0),
                        new KeyValuePair<string, object>("TABEL", 0)
                    };
                var numberOfEqualKeys = oracleClient.GetNumberOfEqualKeyValues(keyMock, extraCriterias, "0|0|00");
                Assert.That(numberOfEqualKeys, Is.EqualTo(1));

                oracleClient.Dispose();
            }

            keyMock.AssertWasCalled(m => m.Table);
            tableMock.AssertWasCalled(m => m.Clone());
            tableMock.AssertWasCalled(m => m.RecordFilters, opt => opt.Repeat.Times(6));
            tableMock.AssertWasCalled(m => m.AddRecordFilter(Arg<IFilter>.Is.NotNull));
            dataManipulatorsMock.AssertWasCalled(m => m.GetEnumerator());
        }
    }
}
