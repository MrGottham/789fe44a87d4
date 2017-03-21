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
    /// Tests worker to add missing primary keys.
    /// </summary>
    [TestFixture]
    public class PrimaryKeyAdderTests
    {
        /// <summary>
        /// Tests that the constructor initialize the worker to to add missing primary keys.
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

            var setFieldValuesMock = new Dictionary<string, object>
                {
                    {fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>()},
                    {fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>()},
                    {fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>()}
                };

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var informationLoggerMock = MockRepository.GenerateMock<IInformationLogger>();
            fixture.Customize<IInformationLogger>(e => e.FromFactory(() => informationLoggerMock));

            var worker = new PrimaryKeyAdder(dataSourceMock.Tables.ElementAt(0).NameTarget, fixture.CreateMany<string>(5).ToList(), setFieldValuesMock, fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IInformationLogger>());
            Assert.That(worker, Is.Not.Null);
            Assert.That(worker.ForeignKeyTable, Is.Not.Null);
            Assert.That(worker.ForeignKeyTable, Is.EqualTo(dataSourceMock.Tables.ElementAt(0)));
            Assert.That(worker.SetFieldValues, Is.Not.Null);
            Assert.That(worker.SetFieldValues, Is.Not.Empty);
            Assert.That(worker.SetFieldValues, Is.EqualTo(setFieldValuesMock));
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

            Assert.Throws<ArgumentNullException>(() => new PrimaryKeyAdder(null, fixture.CreateMany<string>(5).ToList(), new Dictionary<string, object>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IInformationLogger>()));
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

            Assert.Throws<ArgumentNullException>(() => new PrimaryKeyAdder(string.Empty, fixture.CreateMany<string>(5).ToList(), new Dictionary<string, object>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IInformationLogger>()));
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

            Assert.Throws<ArgumentNullException>(() => new PrimaryKeyAdder(dataSourceMock.Tables.ElementAt(0).NameTarget, null, new Dictionary<string, object>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IInformationLogger>()));
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentNullException if field values to be updated on missing primary keys is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfSetFieldValuesIsNull()
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

            Assert.Throws<ArgumentNullException>(() => new PrimaryKeyAdder(dataSourceMock.Tables.ElementAt(0).NameTarget, fixture.CreateMany<string>().ToList(), null, fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IInformationLogger>()));
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentNullException if metadata repository is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfMetadataRepositoryIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IInformationLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IInformationLogger>()));

            Assert.Throws<ArgumentNullException>(() => new PrimaryKeyAdder(fixture.CreateAnonymous<string>(), fixture.CreateMany<string>(5).ToList(), new Dictionary<string, object>(), null, fixture.CreateAnonymous<IInformationLogger>()));
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

            Assert.Throws<ArgumentNullException>(() => new PrimaryKeyAdder(dataSourceMock.Tables.ElementAt(0).NameTarget, fixture.CreateMany<string>().ToList(), new Dictionary<string, object>(), fixture.CreateAnonymous<IMetadataRepository>(), null));
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

            var exception = Assert.Throws<DeliveryEngineSystemException>(() => new PrimaryKeyAdder(fixture.CreateAnonymous<string>(), fixture.CreateMany<string>(5).ToList(), new Dictionary<string, object>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IInformationLogger>()));
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Empty);
            Assert.That(exception.Message, Is.StringStarting(Resource.GetExceptionMessage(ExceptionMessage.TableNotFound, string.Empty)));

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
        }

        /// <summary>
        /// Tests that Manipulate adds the missing primary key values.
        /// </summary>
        [Test]
        public void TestThatManipulateAddsMissingPrimaryKeyValues()
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
                    fieldMock.Expect(m => m.CreateDataObject(Arg<object>.Is.Anything))
                             .WhenCalled(f =>
                                 {
                                     var dataObjectMock = MockRepository.GenerateMock<IFieldData<string, string>>();
                                     dataObjectMock.Expect(m => m.Field)
                                                   .Return(fieldMock)
                                                   .Repeat.Any();
                                     f.ReturnValue = dataObjectMock;
                                 })
                             .Return(null)
                             .Repeat.Any();
                    return fieldMock;
                }));
            // ReSharper restore ImplicitlyCapturedClosure
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var fieldCollection = fixture.CreateMany<IField>(r.Next(5, 10)).ToList();
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
                    tableMock.Expect(m => m.RecordFilters)
                             .Return(new ReadOnlyObservableCollection<IFilter>(new ObservableCollection<IFilter>(new List<IFilter>(0))))
                             .Repeat.Any();
                    tableMock.Expect(m => m.CreateRow())
                             .WhenCalled(f => f.ReturnValue = tableMock.Fields.Select(m => m.CreateDataObject(null)).ToList())
                             .Return(null)
                             .Repeat.Any();
                    tableMock.Expect(m => m.Clone())
                             .Return(tableMock)
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
                                              fieldDataMock.Expect(m => m.GetTargetValue<string>())
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
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var informationLoggerMock = MockRepository.GenerateMock<IInformationLogger>();
            fixture.Customize<IInformationLogger>(e => e.FromFactory(() => informationLoggerMock));

            var primaryTable = dataSourceMock.Tables.ElementAt(0);
            var secondaryTable = dataSourceMock.Tables.ElementAt(1);

            var worker = new PrimaryKeyAdder(secondaryTable.NameTarget, new[] {secondaryTable.Fields.ElementAt(secondaryTable.Fields.Count - 3).NameSource, secondaryTable.Fields.ElementAt(secondaryTable.Fields.Count - 2).NameSource, secondaryTable.Fields.ElementAt(secondaryTable.Fields.Count - 1).NameSource}, new Dictionary<string, object> {{primaryTable.Fields.Last().NameSource, fixture.CreateAnonymous<string>()}}, fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IInformationLogger>());
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
                    var numberOfRowsBefore = primaryTableData.Count;
                    var manipulatedData = worker.FinalizeDataManipulation(primaryTable, fixture.CreateAnonymous<IDataRepository>(), worker.ManipulateData(primaryTable, fixture.CreateAnonymous<IDataRepository>(), primaryTableData).ToList()).ToList();
                    Assert.That(manipulatedData, Is.Not.Null);
                    Assert.That(manipulatedData, Is.Not.Empty);
                    Assert.That(manipulatedData.Count, Is.GreaterThan(numberOfRowsBefore));
                    for (var manipulatedDataRowNo = numberOfRowsBefore; manipulatedDataRowNo < manipulatedData.Count; manipulatedDataRowNo++)
                    {
                        manipulatedData.ElementAt(manipulatedDataRowNo)
                                       .ElementAt(0)
                                       .AssertWasCalled(m => m.UpdateSourceValue(Arg<string>.Is.NotNull));
                        manipulatedData.ElementAt(manipulatedDataRowNo)
                                       .ElementAt(1)
                                       .AssertWasCalled(m => m.UpdateSourceValue(Arg<string>.Is.NotNull));
                        manipulatedData.ElementAt(manipulatedDataRowNo)
                                       .ElementAt(2)
                                       .AssertWasCalled(m => m.UpdateSourceValue(Arg<string>.Is.NotNull));
                        manipulatedData.ElementAt(manipulatedDataRowNo)
                                       .Last()
                                       .AssertWasCalled(m => m.UpdateSourceValue(Arg<string>.Is.NotNull));
                    }
                };
            // ReSharper restore ImplicitlyCapturedClosure
            dataRepositoryMock.DataGetFromTable(primaryTable);

            primaryTable.AssertWasCalled(m => m.PrimaryKey);
            secondaryTable.AssertWasCalled(m => m.Clone());
            secondaryTable.AssertWasCalled(m => m.RecordFilters);
            dataRepositoryMock.AssertWasCalled(m => m.Clone());
            dataRepositoryMock.AssertWasCalled(m => m.DataGetFromTable(Arg<ITable>.Is.Equal(secondaryTable)));

            primaryTable.AssertWasCalled(m => m.CreateRow());

            informationLoggerMock.AssertWasCalled(m => m.LogInformation(Arg<string>.Is.NotNull));
        }

        /// <summary>
        /// Tests that IsManipulatingField returns true if name of the field is used in the tables primary key.
        /// </summary>
        [Test]
        public void TestThatIsManipulatingFieldReturnsTrueIfFieldNameIsUsedInPrimaryKey()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<IInformationLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IInformationLogger>()));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return fieldMock;
                }));
            fixture.Customize<KeyValuePair<IField, IMap>>(e => e.FromFactory(() => new KeyValuePair<IField, IMap>(fixture.CreateAnonymous<IField>(), null)));
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var candidateKey = MockRepository.GenerateMock<ICandidateKey>();
                    candidateKey.Expect(m => m.Fields)
                                .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(fixture.CreateMany<KeyValuePair<IField, IMap>>(5).ToList())))
                                .Repeat.Any();
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    tableMock.Expect(m => m.PrimaryKey)
                             .Return(candidateKey)
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

            var primaryKeyAdder = new PrimaryKeyAdder(dataSourceMock.Tables.ElementAt(0).NameTarget, fixture.CreateMany<string>().ToList(), new Dictionary<string, object>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IInformationLogger>());
            Assert.That(primaryKeyAdder, Is.Not.Null);

            Assert.That(primaryKeyAdder.IsManipulatingField(dataSourceMock.Tables.ElementAt(0).PrimaryKey.Fields.ElementAt(0).Key.NameSource, dataSourceMock.Tables.ElementAt(0)), Is.True);
        }

        /// <summary>
        /// Tests that IsManipulatingField returns false if name of the field is not used in the tables primary key.
        /// </summary>
        [Test]
        public void TestThatIsManipulatingFieldReturnsFalseIfFieldNameIsNotUsedInPrimaryKey()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<IInformationLogger>(e => e.FromFactory(() => MockRepository.GenerateMock<IInformationLogger>()));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return fieldMock;
                }));
            fixture.Customize<KeyValuePair<IField, IMap>>(e => e.FromFactory(() => new KeyValuePair<IField, IMap>(fixture.CreateAnonymous<IField>(), null)));
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var candidateKey = MockRepository.GenerateMock<ICandidateKey>();
                    candidateKey.Expect(m => m.Fields)
                                .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(fixture.CreateMany<KeyValuePair<IField, IMap>>(5).ToList())))
                                .Repeat.Any();
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    tableMock.Expect(m => m.PrimaryKey)
                             .Return(candidateKey)
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

            var primaryKeyAdder = new PrimaryKeyAdder(dataSourceMock.Tables.ElementAt(0).NameTarget, fixture.CreateMany<string>().ToList(), new Dictionary<string, object>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IInformationLogger>());
            Assert.That(primaryKeyAdder, Is.Not.Null);

            Assert.That(primaryKeyAdder.IsManipulatingField(fixture.CreateAnonymous<string>(), dataSourceMock.Tables.ElementAt(0)), Is.False);
        }
    }
}
