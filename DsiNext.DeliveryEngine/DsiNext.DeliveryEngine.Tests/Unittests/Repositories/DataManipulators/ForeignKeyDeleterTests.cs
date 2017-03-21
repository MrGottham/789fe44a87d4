using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Log;
using DsiNext.DeliveryEngine.Repositories.DataManipulators;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Events;
using DsiNext.DeliveryEngine.Resources;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Repositories.DataManipulators
{
    /// <summary>
    /// Tests the worker to delete rows with missing foreign keys.
    /// </summary>
    [TestFixture]
    public class ForeignKeyDeleterTests
    {
        /// <summary>
        /// Tests that the constructor initialize the worker to manipulate missing foreign key values.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeWorker()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var informationLoggerMock = MockRepository.GenerateMock<IInformationLogger>();
            fixture.Customize<IInformationLogger>(e => e.FromFactory(() => informationLoggerMock));

            var worker = new ForeignKeyDeleter(dataSourceMock.Tables.ElementAt(1).NameTarget, fixture.CreateMany<string>(5).ToList(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IInformationLogger>());
            Assert.That(worker, Is.Not.Null);
            Assert.That(worker.ForeignKeyTable, Is.Not.Null);
            Assert.That(worker.ForeignKeyTable, Is.EqualTo(dataSourceMock.Tables.ElementAt(1)));
            Assert.That(worker.InformationLogger, Is.Not.Null);
            Assert.That(worker.InformationLogger, Is.EqualTo(informationLoggerMock));

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentNullException if name of the target table is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfTargetTableNameIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IMetadataRepository>()));
            fixture.Customize<IInformationLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IInformationLogger>()));

            Assert.Throws<ArgumentNullException>(() => new ForeignKeyDeleter(null, fixture.CreateMany<string>(5).ToList(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IInformationLogger>()));
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentNullException if name of the target table is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfTargetTableNameIsEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IMetadataRepository>()));
            fixture.Customize<IInformationLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IInformationLogger>()));

            Assert.Throws<ArgumentNullException>(() => new ForeignKeyDeleter(string.Empty, fixture.CreateMany<string>(5).ToList(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IInformationLogger>()));
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentNullException if names of fields used for the foreign key is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfForeignKeyFieldsIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IInformationLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IInformationLogger>()));
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            Assert.Throws<ArgumentNullException>(() => new ForeignKeyDeleter(dataSourceMock.Tables.ElementAt(0).NameTarget, null, fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IInformationLogger>()));
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentNullException if metadata repository is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfMetadataRepositoryIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IInformationLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IInformationLogger>()));

            Assert.Throws<ArgumentNullException>(() => new ForeignKeyDeleter(fixture.CreateAnonymous<string>(), fixture.CreateMany<string>(5).ToList(), null, fixture.CreateAnonymous<IInformationLogger>()));
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentNullException if information logger is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfInformationLoggerIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            Assert.Throws<ArgumentNullException>(() => new ForeignKeyDeleter(dataSourceMock.Tables.ElementAt(0).NameTarget, fixture.CreateMany<string>(5).ToList(), fixture.CreateAnonymous<IMetadataRepository>(), null));
        }

        /// <summary>
        /// Tests that the constructor throws an DeliveryEngineSystemException if name of the target table does not exists in the data source.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsDeliveryEngineSystemExceptionIfTargetTableNameNotInDataSource()
        {
            var fixture = new Fixture();
            fixture.Customize<IInformationLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IInformationLogger>()));

            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));
            fixture.Customize<IDataSource>(e => e.FromFactory(() =>
                {
                    var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
                    dataSourceMock.Expect(m => m.Tables)
                                  .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                                  .Repeat.Any();
                    return dataSourceMock;
                }));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var exception = Assert.Throws<DeliveryEngineSystemException>(() => new ForeignKeyDeleter(fixture.CreateAnonymous<string>(), fixture.CreateMany<string>(5).ToList(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IInformationLogger>()));
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Empty);
            Assert.That(exception.Message, Is.StringStarting(Resource.GetExceptionMessage(ExceptionMessage.TableNotFound, string.Empty)));

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
        }

        /// <summary>
        /// Tests that Manipulate deletes missing foreign key values.
        /// </summary>
        [Test]
        public void TestThatManipulateDeletesMissingForeignKeyValues()
        {
            var r = new Random(DateTime.Now.Millisecond);
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    fieldMock.Expect(m => m.DatatypeOfSource)
                             .Return(typeof (string))
                             .Repeat.Any();
                    fieldMock.Expect(m => m.DatatypeOfTarget)
                             .Return(typeof (string))
                             .Repeat.Any();
                    return fieldMock;
                }));
            // ReSharper restore ImplicitlyCapturedClosure
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var fieldCollection = fixture.CreateMany<IField>(r.Next(3, 10)).ToList();
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    var candidateKeyMock = MockRepository.GenerateMock<ICandidateKey>();
                    candidateKeyMock.Expect(m => m.Table)
                                    .Return(tableMock)
                                    .Repeat.Any();
                    candidateKeyMock.Expect(m => m.Fields)
                                    .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(new List<KeyValuePair<IField, IMap>> {new KeyValuePair<IField, IMap>(fieldCollection.ElementAt(0), null), new KeyValuePair<IField, IMap>(fieldCollection.ElementAt(1), null), new KeyValuePair<IField, IMap>(fieldCollection.ElementAt(2), null)})))
                                    .Repeat.Any();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    tableMock.Expect(m => m.Fields)
                             .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(fieldCollection)))
                             .Repeat.Any();
                    tableMock.Expect(m => m.CandidateKeys)
                             .Return(new ReadOnlyObservableCollection<ICandidateKey>(new ObservableCollection<ICandidateKey>(new List<ICandidateKey> {candidateKeyMock})))
                             .Repeat.Any();
                    tableMock.Expect(m => m.PrimaryKey)
                             .Return(tableMock.CandidateKeys.ElementAt(0))
                             .Repeat.Any();
                    tableMock.Expect(m => m.CreateRow())
                             .Return(tableMock.Fields.Select(m => m.CreateDataObject(null)).ToList())
                             .Repeat.Any();
                    return tableMock;
                }));
            // ReSharper restore ImplicitlyCapturedClosure

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            dataRepositoryMock.Expect(m => m.DataGetFromTable(Arg<ITable>.Is.NotNull))
                              .WhenCalled(e =>
                                  {
                                      var table = (ITable) e.Arguments[0];
                                      var numberOfRows = r.Next(10, 250);
                                      var dataForTable = new Collection<IEnumerable<IDataObjectBase>>();
                                      while (dataForTable.Count < numberOfRows)
                                      {
                                          var dataForRow = new Collection<IDataObjectBase>();
                                          for (var columnNo = 0; columnNo < table.Fields.Count; columnNo++)
                                          {
                                              var fieldDataMock = MockRepository.GenerateMock<IFieldData<string, string>>();
                                              fieldDataMock.Expect(m => m.Field)
                                                           .Return(table.Fields.ElementAt(columnNo))
                                                           .Repeat.Any();
                                              fieldDataMock.Expect(m => m.SourceValue)
                                                           .Return(fixture.CreateAnonymous<string>())
                                                           .Repeat.Any();
                                              fieldDataMock.Expect(m => m.GetSourceValue<string>())
                                                           .Return(fieldDataMock.SourceValue)
                                                           .Repeat.Any();
                                              fieldDataMock.Expect(m => m.GetTargetValue<string>(Arg<IMap>.Is.Anything))
                                                           .Return(fieldDataMock.SourceValue)
                                                           .Repeat.Any();
                                              dataForRow.Add(fieldDataMock);
                                          }
                                          dataForTable.Add(dataForRow);
                                      }
                                      var eventArgsMock = MockRepository.GenerateMock<IHandleDataEventArgs>();
                                      eventArgsMock.Expect(m => m.Table)
                                                   .Return(table)
                                                   .Repeat.Any();
                                      eventArgsMock.Expect(m => m.Data)
                                                   .Return(dataForTable)
                                                   .Repeat.Any();
                                      dataRepositoryMock.Raise(n => n.OnHandleData += null, this, eventArgsMock);
                                  })
                              .Repeat.Any();
            dataRepositoryMock.Expect(m => m.Clone())
                              .Return(dataRepositoryMock)
                              .Repeat.Any();
            dataRepositoryMock.Expect(m => m.GetDataQueryer())
                              .Throw(fixture.CreateAnonymous<NotSupportedException>())
                              .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var informationLoggerMock = MockRepository.GenerateMock<IInformationLogger>();
            fixture.Customize<IInformationLogger>(e => e.FromFactory(() => informationLoggerMock));

            var primaryTable = dataSourceMock.Tables.ElementAt(0);
            var secondaryTable = dataSourceMock.Tables.ElementAt(1);

            var worker = new ForeignKeyDeleter(secondaryTable.NameTarget, new List<string> {primaryTable.Fields.ElementAt(primaryTable.Fields.Count - 3).NameSource, primaryTable.Fields.ElementAt(primaryTable.Fields.Count - 2).NameSource, primaryTable.Fields.ElementAt(primaryTable.Fields.Count - 1).NameSource}, fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IInformationLogger>());
            Assert.That(worker, Is.Not.Null);

            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.OnHandleData += (s, e) =>
                {
                    Assert.That(s, Is.Not.Null);
                    Assert.That(e, Is.Not.Null);
                    Assert.That(e.Table, Is.Not.Null);
                    Assert.That(e.Data, Is.Not.Null);
                    Assert.That(e.Data, Is.Not.Empty);

                    if (e.Table.Equals(primaryTable) == false)
                    {
                        return;
                    }
                    var primaryTableData = e.Data.ToList();
                    var manipulatedData = worker.FinalizeDataManipulation(primaryTable, fixture.CreateAnonymous<IDataRepository>(), worker.ManipulateData(primaryTable, fixture.CreateAnonymous<IDataRepository>(), primaryTableData).ToList()).ToList();
                    Assert.That(manipulatedData, Is.Not.Null);
                    Assert.That(manipulatedData, Is.Empty);
                    Assert.That(manipulatedData.Count, Is.EqualTo(0));
                };
            // ReSharper restore ImplicitlyCapturedClosure
            dataRepositoryMock.DataGetFromTable(primaryTable);

            dataRepositoryMock.AssertWasCalled(m => m.GetDataQueryer());
            dataRepositoryMock.AssertWasCalled(m => m.Clone());
            dataRepositoryMock.AssertWasCalled(m => m.DataGetFromTable(Arg<ITable>.Is.Equal(secondaryTable)));

            informationLoggerMock.AssertWasCalled(m => m.LogInformation(Arg<string>.Is.NotNull));
        }
    }
}
