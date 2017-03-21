using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Domain.Metadata;
using DsiNext.DeliveryEngine.Repositories.DataManipulators;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Repositories.DataManipulators
{
    /// <summary>
    /// Tests the row duplicator which duplicates rows and updates some values on the duplicated rows.
    /// </summary>
    [TestFixture]
    public class RowDuplicatorTests
    {
        /// <summary>
        /// Test that the constructor initialize a row duplicator without configuration of the criterias used to duplicator rows.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeRowDuplicatorWithoutCriteriaConfigurations()
        {
            var fixture = new Fixture();
            fixture.Customize<Tuple<string, object>>(e =>e.FromFactory(() => new Tuple<string, object>(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<int>())));

            var tableName = fixture.CreateAnonymous<string>();
            var fieldUpdates = fixture.CreateMany<Tuple<string, object>>(5).ToList();
            var rowDuplicator = new RowDuplicator(tableName, fieldUpdates);
            Assert.That(rowDuplicator, Is.Not.Null);
            Assert.That(rowDuplicator.TableName, Is.Not.Null);
            Assert.That(rowDuplicator.TableName, Is.Not.Empty);
            Assert.That(rowDuplicator.TableName, Is.EqualTo(tableName));
            Assert.That(rowDuplicator.FieldUpdates, Is.Not.Null);
            Assert.That(rowDuplicator.FieldUpdates, Is.Not.Empty);
            Assert.That(rowDuplicator.FieldUpdates, Is.EqualTo(fieldUpdates));
            Assert.That(rowDuplicator.CriteriaConfigurations, Is.Not.Null);
            Assert.That(rowDuplicator.CriteriaConfigurations, Is.Empty);
        }

        /// <summary>
        /// Test that the constructor initialize a row duplicator with configuration of the criterias used to duplicator rows.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeRowDuplicatorWithCriteriaConfigurations()
        {
            var fixture = new Fixture();
            fixture.Customize<Tuple<string, object>>(e => e.FromFactory(() => new Tuple<string, object>(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<int>())));
            fixture.Customize<Tuple<Type, string, object>>(e => e.FromFactory(() => new Tuple<Type, string, object>(typeof (EqualCriteria<>), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<int>())));

            var tableName = fixture.CreateAnonymous<string>();
            var fieldUpdates = fixture.CreateMany<Tuple<string, object>>(5).ToList();
            var criteriaConfigurations = fixture.CreateMany<Tuple<Type, string, object>>(5).ToList();
            var rowDuplicator = new RowDuplicator(tableName, fieldUpdates, criteriaConfigurations);
            Assert.That(rowDuplicator, Is.Not.Null);
            Assert.That(rowDuplicator.TableName, Is.Not.Null);
            Assert.That(rowDuplicator.TableName, Is.Not.Empty);
            Assert.That(rowDuplicator.TableName, Is.EqualTo(tableName));
            Assert.That(rowDuplicator.FieldUpdates, Is.Not.Null);
            Assert.That(rowDuplicator.FieldUpdates, Is.Not.Empty);
            Assert.That(rowDuplicator.FieldUpdates, Is.EqualTo(fieldUpdates));
            Assert.That(rowDuplicator.CriteriaConfigurations, Is.Not.Null);
            Assert.That(rowDuplicator.CriteriaConfigurations, Is.Not.Empty);
            Assert.That(rowDuplicator.CriteriaConfigurations, Is.EqualTo(criteriaConfigurations));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if name of the table is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfTableNameIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<Tuple<string, object>>(e => e.FromFactory(() => new Tuple<string, object>(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<int>())));
            fixture.Customize<Tuple<Type, string, object>>(e => e.FromFactory(() => new Tuple<Type, string, object>(typeof(EqualCriteria<>), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<int>())));

            Assert.Throws<ArgumentNullException>(() => new RowDuplicator(null, fixture.CreateMany<Tuple<string, object>>(5).ToList()));
            Assert.Throws<ArgumentNullException>(() => new RowDuplicator(null, fixture.CreateMany<Tuple<string, object>>(5).ToList(), fixture.CreateMany<Tuple<Type, string, object>>(5).ToList()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if name of the table is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfTableNameIsEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize<Tuple<string, object>>(e => e.FromFactory(() => new Tuple<string, object>(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<int>())));
            fixture.Customize<Tuple<Type, string, object>>(e => e.FromFactory(() => new Tuple<Type, string, object>(typeof(EqualCriteria<>), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<int>())));

            Assert.Throws<ArgumentNullException>(() => new RowDuplicator(string.Empty, fixture.CreateMany<Tuple<string, object>>(5).ToList()));
            Assert.Throws<ArgumentNullException>(() => new RowDuplicator(string.Empty, fixture.CreateMany<Tuple<string, object>>(5).ToList(), fixture.CreateMany<Tuple<Type, string, object>>(5).ToList()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if field names and the new value to set on the duplicated rows is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfFieldUpdatesIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<Tuple<Type, string, object>>(e => e.FromFactory(() => new Tuple<Type, string, object>(typeof(EqualCriteria<>), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<int>())));

            Assert.Throws<ArgumentNullException>(() => new RowDuplicator(fixture.CreateAnonymous<string>(), null));
            Assert.Throws<ArgumentNullException>(() => new RowDuplicator(fixture.CreateAnonymous<string>(), null, fixture.CreateMany<Tuple<Type, string, object>>(5).ToList()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if configuration for the criterias used to duplicate rows is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfCriteriaConfigurationsIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<Tuple<string, object>>(e => e.FromFactory(() => new Tuple<string, object>(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<int>())));

            Assert.Throws<ArgumentNullException>(() => new RowDuplicator(fixture.CreateAnonymous<string>(), fixture.CreateMany<Tuple<string, object>>(5).ToList(), null));
        }

        /// <summary>
        /// Tests that ManipulateData returns without manipulating data if source name of the table does not match table name in the constructor.
        /// </summary>
        [Test]
        public void TestThatManipulateDataReturnsWithoutManipulatingDataIfTableSourceNameDoesNotMatchTableNameInTheConstructor()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (int)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return fieldMock;
                }));

            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            tableMock.Expect(m => m.Fields)
                     .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(fixture.CreateMany<IField>(5).ToList())))
                     .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => tableMock));

            var fieldUpdatesMock = new Collection<Tuple<string, object>>
                {
                    new Tuple<string, object>(tableMock.Fields.ElementAt(0).NameSource, fixture.CreateAnonymous<int>()),
                    new Tuple<string, object>(tableMock.Fields.ElementAt(1).NameSource, fixture.CreateAnonymous<int>())
                };

            var dataCollectionMock = new Collection<IEnumerable<IDataObjectBase>>();
            while (dataCollectionMock.Count < 250)
            {
                var dataObjects = new Collection<IDataObjectBase>();
                for (var i = 0; i < tableMock.Fields.Count; i++)
                {
                    dataObjects.Add(MockRepository.GenerateMock<IFieldData<int, int>>());
                }
                dataCollectionMock.Add(dataObjects);
            }
            
            var rowDuplicator = new RowDuplicator(fixture.CreateAnonymous<string>(), fieldUpdatesMock);
            Assert.That(rowDuplicator, Is.Not.Null);

            var manipulatedData = rowDuplicator.ManipulateData(fixture.CreateAnonymous<ITable>(), dataCollectionMock);
            Assert.That(manipulatedData, Is.Not.Null);
            Assert.That(manipulatedData, Is.Not.Empty);
            Assert.That(manipulatedData.Count(), Is.EqualTo(dataCollectionMock.Count));
        }

        /// <summary>
        /// Tests that ManipulateData duplicates rows without using criterias.
        /// </summary>
        [Test]
        public void TestThatManipulateDataDuplicatesRowsWithoutUsingCriterias()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (int)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return fieldMock;
                }));

            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            tableMock.Expect(m => m.Fields)
                     .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(fixture.CreateMany<IField>(5).ToList())))
                     .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => tableMock));

            var fieldUpdatesMock = new Collection<Tuple<string, object>>
                {
                    new Tuple<string, object>(tableMock.Fields.ElementAt(0).NameSource, fixture.CreateAnonymous<int>()),
                    new Tuple<string, object>(tableMock.Fields.ElementAt(1).NameSource, fixture.CreateAnonymous<int>())
                };

            var dataCollectionMock = new Collection<IEnumerable<IDataObjectBase>>();
            while (dataCollectionMock.Count < 250)
            {
                var dataObjects = new Collection<IDataObjectBase>();
                for (var i = 0; i < tableMock.Fields.Count; i++)
                {
                    var clonedDataObjectMock = MockRepository.GenerateMock<IFieldData<int, int>>();
                    clonedDataObjectMock.Expect(m => m.Field)
                                        .Return(tableMock.Fields.ElementAt(i))
                                        .Repeat.Any();
                    var dataObjectMock = MockRepository.GenerateMock<IFieldData<int, int>>();
                    dataObjectMock.Expect(m => m.Field)
                                  .Return(tableMock.Fields.ElementAt(i))
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.Clone())
                                  .Return(clonedDataObjectMock)
                                  .Repeat.Any();
                    dataObjects.Add(dataObjectMock);
                }
                dataCollectionMock.Add(dataObjects);
            }

            var rowDuplicator = new RowDuplicator(tableMock.NameSource, fieldUpdatesMock);
            Assert.That(rowDuplicator, Is.Not.Null);

            var rowsToDuplicate = dataCollectionMock.Count;
            var manipulatedData = rowDuplicator.ManipulateData(fixture.CreateAnonymous<ITable>(), dataCollectionMock).ToList();
            Assert.That(manipulatedData, Is.Not.Null);
            Assert.That(manipulatedData, Is.Not.Empty);
            Assert.That(manipulatedData.Count, Is.EqualTo(rowsToDuplicate*2));
            for (var manipulatedDataRowNo = 0; manipulatedDataRowNo < manipulatedData.Count; manipulatedDataRowNo++)
            {
                if (manipulatedDataRowNo < 250)
                {
                    foreach (var dataObject in manipulatedData.ElementAt(manipulatedDataRowNo))
                    {
                        dataObject.AssertWasCalled(m => m.Clone());
                    }
                    continue;
                }
                manipulatedData.ElementAt(manipulatedDataRowNo).ElementAt(0).AssertWasCalled(m => m.UpdateSourceValue(Arg<int>.Is.Equal(fieldUpdatesMock.ElementAt(0).Item2)));
                manipulatedData.ElementAt(manipulatedDataRowNo).ElementAt(1).AssertWasCalled(m => m.UpdateSourceValue(Arg<int>.Is.Equal(fieldUpdatesMock.ElementAt(1).Item2)));
            }
        }

        /// <summary>
        /// Tests that ManipulateData duplicates rows using a equal criteria.
        /// </summary>
        [Test]
        public void TestThatManipulateDataDuplicatesRowsUsingEqualCriteria()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (int)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    fieldMock.Expect(m => m.DatatypeOfSource)
                             .Return(typeof (int))
                             .Repeat.Any();
                    return fieldMock;
                }));

            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            tableMock.Expect(m => m.Fields)
                     .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(fixture.CreateMany<IField>(5).ToList())))
                     .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => tableMock));

            var fieldUpdatesMock = new Collection<Tuple<string, object>>
                {
                    new Tuple<string, object>(tableMock.Fields.ElementAt(0).NameSource, fixture.CreateAnonymous<int>()),
                    new Tuple<string, object>(tableMock.Fields.ElementAt(1).NameSource, fixture.CreateAnonymous<int>())
                };
            var criteriaConfigurationsMock = new Collection<Tuple<Type, string, object>>
                {
                    new Tuple<Type, string, object>(typeof (EqualCriteria<>), tableMock.Fields.ElementAt(0).NameSource, "1")
                };

            var dataCollectionMock = new Collection<IEnumerable<IDataObjectBase>>();
            while (dataCollectionMock.Count < 250)
            {
                var dataObjects = new Collection<IDataObjectBase>();
                for (var i = 0; i < tableMock.Fields.Count; i++)
                {
                    var clonedDataObjectMock = MockRepository.GenerateMock<IFieldData<int, int>>();
                    clonedDataObjectMock.Expect(m => m.Field)
                                        .Return(tableMock.Fields.ElementAt(i))
                                        .Repeat.Any();
                    var dataObjectMock = MockRepository.GenerateMock<IFieldData<int, int>>();
                    dataObjectMock.Expect(m => m.Field)
                                  .Return(tableMock.Fields.ElementAt(i))
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.SourceValue)
                                  .Return(i == 0 ? dataCollectionMock.Count%2 : fixture.CreateAnonymous<int>())
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.GetSourceValue<int>())
                                  .Return(dataObjectMock.SourceValue)
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.Clone())
                                  .Return(clonedDataObjectMock)
                                  .Repeat.Any();
                    dataObjects.Add(dataObjectMock);
                }
                dataCollectionMock.Add(dataObjects);
            }

            var rowDuplicator = new RowDuplicator(tableMock.NameSource, fieldUpdatesMock, criteriaConfigurationsMock);
            Assert.That(rowDuplicator, Is.Not.Null);

            var rowsToDuplicate = dataCollectionMock.Count(m => m.ElementAt(0).GetSourceValue<int>() == 1);
            var manipulatedData = rowDuplicator.ManipulateData(fixture.CreateAnonymous<ITable>(), dataCollectionMock).ToList();
            Assert.That(manipulatedData, Is.Not.Null);
            Assert.That(manipulatedData, Is.Not.Empty);
            Assert.That(manipulatedData.Count, Is.EqualTo(250 + rowsToDuplicate));
            for (var manipulatedDataRowNo = 0; manipulatedDataRowNo < manipulatedData.Count; manipulatedDataRowNo++)
            {
                if (manipulatedDataRowNo < dataCollectionMock.Count)
                {
                    if (manipulatedData.ElementAt(manipulatedDataRowNo).ElementAt(0).GetSourceValue<int>() > 0)
                    {
                        foreach (var dataObject in manipulatedData.ElementAt(manipulatedDataRowNo))
                        {
                            dataObject.AssertWasCalled(m => m.Clone());
                        }
                    }
                    continue;
                }
                manipulatedData.ElementAt(manipulatedDataRowNo).ElementAt(0).AssertWasCalled(m => m.UpdateSourceValue(Arg<int>.Is.Equal(fieldUpdatesMock.ElementAt(0).Item2)));
                manipulatedData.ElementAt(manipulatedDataRowNo).ElementAt(1).AssertWasCalled(m => m.UpdateSourceValue(Arg<int>.Is.Equal(fieldUpdatesMock.ElementAt(1).Item2)));
            }
        }

        /// <summary>
        /// Tests that ManipulateData duplicates rows using a pool criteria.
        /// </summary>
        [Test]
        public void TestThatManipulateDataDuplicatesRowsUsingPoolCriteria()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (int)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    fieldMock.Expect(m => m.DatatypeOfSource)
                             .Return(typeof (int))
                             .Repeat.Any();
                    return fieldMock;
                }));

            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            tableMock.Expect(m => m.Fields)
                     .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(fixture.CreateMany<IField>(5).ToList())))
                     .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => tableMock));

            var fieldUpdatesMock = new Collection<Tuple<string, object>>
                {
                    new Tuple<string, object>(tableMock.Fields.ElementAt(0).NameSource, fixture.CreateAnonymous<int>()),
                    new Tuple<string, object>(tableMock.Fields.ElementAt(1).NameSource, fixture.CreateAnonymous<int>())
                };
            var criteriaConfigurationsMock = new Collection<Tuple<Type, string, object>>
                {
                    new Tuple<Type, string, object>(typeof (PoolCriteria<>), tableMock.Fields.ElementAt(0).NameSource, new[] {"1", "2", "3"})
                };

            var dataCollectionMock = new Collection<IEnumerable<IDataObjectBase>>();
            while (dataCollectionMock.Count < 250)
            {
                var dataObjects = new Collection<IDataObjectBase>();
                for (var i = 0; i < tableMock.Fields.Count; i++)
                {
                    var clonedDataObjectMock = MockRepository.GenerateMock<IFieldData<int, int>>();
                    clonedDataObjectMock.Expect(m => m.Field)
                                        .Return(tableMock.Fields.ElementAt(i))
                                        .Repeat.Any();
                    var dataObjectMock = MockRepository.GenerateMock<IFieldData<int, int>>();
                    dataObjectMock.Expect(m => m.Field)
                                  .Return(tableMock.Fields.ElementAt(i))
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.SourceValue)
                                  .Return(i == 0 ? dataCollectionMock.Count%4 : fixture.CreateAnonymous<int>())
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.GetSourceValue<int>())
                                  .Return(dataObjectMock.SourceValue)
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.Clone())
                                  .Return(clonedDataObjectMock)
                                  .Repeat.Any();
                    dataObjects.Add(dataObjectMock);
                }
                dataCollectionMock.Add(dataObjects);
            }

            var rowDuplicator = new RowDuplicator(tableMock.NameSource, fieldUpdatesMock, criteriaConfigurationsMock);
            Assert.That(rowDuplicator, Is.Not.Null);

            var rowsToDuplicate = dataCollectionMock.Count(m => m.ElementAt(0).GetSourceValue<int>() > 0);
            var manipulatedData = rowDuplicator.ManipulateData(fixture.CreateAnonymous<ITable>(), dataCollectionMock).ToList();
            Assert.That(manipulatedData, Is.Not.Null);
            Assert.That(manipulatedData, Is.Not.Empty);
            Assert.That(manipulatedData.Count, Is.EqualTo(250 + rowsToDuplicate));
            for (var manipulatedDataRowNo = 0; manipulatedDataRowNo < manipulatedData.Count; manipulatedDataRowNo++)
            {
                if (manipulatedDataRowNo < dataCollectionMock.Count)
                {
                    if (manipulatedData.ElementAt(manipulatedDataRowNo).ElementAt(0).GetSourceValue<int>() > 0)
                    {
                        foreach (var dataObject in manipulatedData.ElementAt(manipulatedDataRowNo))
                        {
                            dataObject.AssertWasCalled(m => m.Clone());
                        }
                    }
                    continue;
                }
                manipulatedData.ElementAt(manipulatedDataRowNo).ElementAt(0).AssertWasCalled(m => m.UpdateSourceValue(Arg<int>.Is.Equal(fieldUpdatesMock.ElementAt(0).Item2)));
                manipulatedData.ElementAt(manipulatedDataRowNo).ElementAt(1).AssertWasCalled(m => m.UpdateSourceValue(Arg<int>.Is.Equal(fieldUpdatesMock.ElementAt(1).Item2)));
            }
        }

        /// <summary>
        /// Tests that ManipulateData duplicates rows using an interval criteria.
        /// </summary>
        [Test]
        public void TestThatManipulateDataDuplicatesRowsUsingIntervalCriteria()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (int)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    fieldMock.Expect(m => m.DatatypeOfSource)
                             .Return(typeof (int))
                             .Repeat.Any();
                    return fieldMock;
                }));

            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            tableMock.Expect(m => m.Fields)
                     .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(fixture.CreateMany<IField>(5).ToList())))
                     .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => tableMock));

            var fieldUpdatesMock = new Collection<Tuple<string, object>>
                {
                    new Tuple<string, object>(tableMock.Fields.ElementAt(0).NameSource, fixture.CreateAnonymous<int>()),
                    new Tuple<string, object>(tableMock.Fields.ElementAt(1).NameSource, fixture.CreateAnonymous<int>())
                };
            var criteriaConfigurationsMock = new Collection<Tuple<Type, string, object>>
                {
                    new Tuple<Type, string, object>(typeof (IntervalCriteria<>), tableMock.Fields.ElementAt(0).NameSource, new Tuple<string, string>("1", "3"))
                };

            var dataCollectionMock = new Collection<IEnumerable<IDataObjectBase>>();
            while (dataCollectionMock.Count < 250)
            {
                var dataObjects = new Collection<IDataObjectBase>();
                for (var i = 0; i < tableMock.Fields.Count; i++)
                {
                    var clonedDataObjectMock = MockRepository.GenerateMock<IFieldData<int, int>>();
                    clonedDataObjectMock.Expect(m => m.Field)
                                        .Return(tableMock.Fields.ElementAt(i))
                                        .Repeat.Any();
                    var dataObjectMock = MockRepository.GenerateMock<IFieldData<int, int>>();
                    dataObjectMock.Expect(m => m.Field)
                                  .Return(tableMock.Fields.ElementAt(i))
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.SourceValue)
                                  .Return(i == 0 ? dataCollectionMock.Count%4 : fixture.CreateAnonymous<int>())
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.GetSourceValue<int>())
                                  .Return(dataObjectMock.SourceValue)
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.Clone())
                                  .Return(clonedDataObjectMock)
                                  .Repeat.Any();
                    dataObjects.Add(dataObjectMock);
                }
                dataCollectionMock.Add(dataObjects);
            }

            var rowDuplicator = new RowDuplicator(tableMock.NameSource, fieldUpdatesMock, criteriaConfigurationsMock);
            Assert.That(rowDuplicator, Is.Not.Null);

            var rowsToDuplicate = dataCollectionMock.Count(m => m.ElementAt(0).GetSourceValue<int>() > 0);
            var manipulatedData = rowDuplicator.ManipulateData(fixture.CreateAnonymous<ITable>(), dataCollectionMock).ToList();
            Assert.That(manipulatedData, Is.Not.Null);
            Assert.That(manipulatedData, Is.Not.Empty);
            Assert.That(manipulatedData.Count, Is.EqualTo(250 + rowsToDuplicate));
            for (var manipulatedDataRowNo = 0; manipulatedDataRowNo < manipulatedData.Count; manipulatedDataRowNo++)
            {
                if (manipulatedDataRowNo < dataCollectionMock.Count)
                {
                    if (manipulatedData.ElementAt(manipulatedDataRowNo).ElementAt(0).GetSourceValue<int>() > 0)
                    {
                        foreach (var dataObject in manipulatedData.ElementAt(manipulatedDataRowNo))
                        {
                            dataObject.AssertWasCalled(m => m.Clone());
                        }
                    }
                    continue;
                }
                manipulatedData.ElementAt(manipulatedDataRowNo).ElementAt(0).AssertWasCalled(m => m.UpdateSourceValue(Arg<int>.Is.Equal(fieldUpdatesMock.ElementAt(0).Item2)));
                manipulatedData.ElementAt(manipulatedDataRowNo).ElementAt(1).AssertWasCalled(m => m.UpdateSourceValue(Arg<int>.Is.Equal(fieldUpdatesMock.ElementAt(1).Item2)));
            }
        }

        /// <summary>
        /// Test that IsManipulatingField returns true if the data manipulator is manipulating the given field.
        /// </summary>
        [Test]
        public void TestThatIsManipulatingFieldReturnsTrueIfDataManipulatorIsManipulatingFieldName()
        {
            var fixture = new Fixture();
            fixture.Customize<Tuple<string, object>>(e => e.FromFactory(() => new Tuple<string, object>(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<int>())));
            fixture.Customize<Tuple<Type, string, object>>(e => e.FromFactory(() => new Tuple<Type, string, object>(typeof(EqualCriteria<>), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<int>())));

            var fieldUpdates = fixture.CreateMany<Tuple<string, object>>(5).ToList();
            var rowDuplicator = new RowDuplicator(fixture.CreateAnonymous<string>(), fieldUpdates);
            Assert.That(rowDuplicator, Is.Not.Null);

            Assert.That(rowDuplicator.IsManipulatingField(fieldUpdates.ElementAt(0).Item1), Is.True);
        }

        /// <summary>
        /// Test that IsManipulatingField returns false if the data manipulator is not manipulating the given field.
        /// </summary>
        [Test]
        public void TestThatIsManipulatingFieldReturnsFalseIfDataManipulatorIsNotManipulatingFieldName()
        {
            var fixture = new Fixture();
            fixture.Customize<Tuple<string, object>>(e => e.FromFactory(() => new Tuple<string, object>(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<int>())));
            fixture.Customize<Tuple<Type, string, object>>(e => e.FromFactory(() => new Tuple<Type, string, object>(typeof(EqualCriteria<>), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<int>())));

            var fieldUpdates = fixture.CreateMany<Tuple<string, object>>(5).ToList();
            var rowDuplicator = new RowDuplicator(fixture.CreateAnonymous<string>(), fieldUpdates);
            Assert.That(rowDuplicator, Is.Not.Null);

            Assert.That(rowDuplicator.IsManipulatingField(fixture.CreateAnonymous<string>()), Is.False);
        }
    }
}
