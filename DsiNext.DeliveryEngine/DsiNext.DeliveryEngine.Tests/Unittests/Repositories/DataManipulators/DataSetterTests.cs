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
    /// Tests the data setter which can set or change a field value.
    /// </summary>
    [TestFixture]
    public class DataSetterTests
    {
        /// <summary>
        /// Test that the constructor initialize a data setter without configuration for criterias.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeDataSetterWithoutCriteriaConfigurations()
        {
            var fixture = new Fixture();
            var tableName = fixture.CreateAnonymous<string>();
            var fieldName = fixture.CreateAnonymous<string>();
            var fieldValue = fixture.CreateAnonymous<object>();

            var dataSetter = new DataSetter(tableName, fieldName, fieldValue);
            Assert.That(dataSetter, Is.Not.Null);
            Assert.That(dataSetter.TableName, Is.Not.Null);
            Assert.That(dataSetter.TableName, Is.Not.Empty);
            Assert.That(dataSetter.TableName, Is.EqualTo(tableName));
            Assert.That(dataSetter.FieldName, Is.Not.Null);
            Assert.That(dataSetter.FieldName, Is.Not.Empty);
            Assert.That(dataSetter.FieldName, Is.EqualTo(fieldName));
            Assert.That(dataSetter.FieldValue, Is.Not.Null);
            Assert.That(dataSetter.FieldValue, Is.EqualTo(fieldValue));
            Assert.That(dataSetter.CriteriaConfigurations, Is.Not.Null);
            Assert.That(dataSetter.CriteriaConfigurations, Is.Empty);
        }

        /// <summary>
        /// Test that the constructor initialize a data setter with configuration for criterias.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeDataSetterWithCriteriaConfigurations()
        {
            var fixture = new Fixture();
            fixture.Customize<Tuple<Type, string, object>>(e => e.FromFactory(() => new Tuple<Type, string, object>(typeof (EqualCriteria<>), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>())));
            
            var tableName = fixture.CreateAnonymous<string>();
            var fieldName = fixture.CreateAnonymous<string>();
            var fieldValue = fixture.CreateAnonymous<string>();
            var criteriaConfigurations = fixture.CreateMany<Tuple<Type, string, object>>(5).ToList();

            var dataSetter = new DataSetter(tableName, fieldName, fieldValue, criteriaConfigurations);
            Assert.That(dataSetter, Is.Not.Null);
            Assert.That(dataSetter.TableName, Is.Not.Null);
            Assert.That(dataSetter.TableName, Is.Not.Empty);
            Assert.That(dataSetter.TableName, Is.EqualTo(tableName));
            Assert.That(dataSetter.FieldName, Is.Not.Null);
            Assert.That(dataSetter.FieldName, Is.Not.Empty);
            Assert.That(dataSetter.FieldName, Is.EqualTo(fieldName));
            Assert.That(dataSetter.FieldValue, Is.Not.Null);
            Assert.That(dataSetter.FieldValue, Is.EqualTo(fieldValue));
            Assert.That(dataSetter.CriteriaConfigurations, Is.Not.Null);
            Assert.That(dataSetter.CriteriaConfigurations, Is.Not.Empty);
            Assert.That(dataSetter.CriteriaConfigurations, Is.EqualTo(criteriaConfigurations));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if name of the table is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfTableNameIsNull()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new DataSetter(null, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>()));
            Assert.Throws<ArgumentNullException>(() => new DataSetter(null, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>(), new Collection<Tuple<Type, string, object>>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if name of the table is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfTableNameIsEmpty()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new DataSetter(string.Empty, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>()));
            Assert.Throws<ArgumentNullException>(() => new DataSetter(string.Empty, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>(), new Collection<Tuple<Type, string, object>>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if name of the field is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfFieldNameIsNull()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new DataSetter(fixture.CreateAnonymous<string>(), null, fixture.CreateAnonymous<object>()));
            Assert.Throws<ArgumentNullException>(() => new DataSetter(fixture.CreateAnonymous<string>(), null, fixture.CreateAnonymous<object>(), new Collection<Tuple<Type, string, object>>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if name of the field is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfFieldNameIsEmpty()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new DataSetter(fixture.CreateAnonymous<string>(), string.Empty, fixture.CreateAnonymous<object>()));
            Assert.Throws<ArgumentNullException>(() => new DataSetter(fixture.CreateAnonymous<string>(), string.Empty, fixture.CreateAnonymous<object>(), new Collection<Tuple<Type, string, object>>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if value of the field is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfFieldValueIsNull()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new DataSetter(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), null));
            Assert.Throws<ArgumentNullException>(() => new DataSetter(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), null, new Collection<Tuple<Type, string, object>>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if configuration for criterias is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfCriteriaConfigurationsIsNull()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new DataSetter(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>(), null));
        }

        /// <summary>
        /// Test that ManipulateData throws an ArgumentNullException if the table is null.
        /// </summary>
        [Test]
        public void TestThatManipulateDataThrowsArgumentNullExceptionIfTableIsNull()
        {
            var fixture = new Fixture();

            var dataSetter = new DataSetter(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>());
            Assert.That(dataSetter, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSetter.ManipulateData(null, new Collection<IEnumerable<IDataObjectBase>>()));
        }

        /// <summary>
        /// Test that ManipulateData throws an ArgumentNullException if the data is null.
        /// </summary>
        [Test]
        public void TestThatManipulateDataThrowsArgumentNullExceptionIfDataIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));

            var dataSetter = new DataSetter(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>());
            Assert.That(dataSetter, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataSetter.ManipulateData(fixture.CreateAnonymous<ITable>(), null));
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
            fixture.Customize<IField>(e => e.FromFactory(() => MockRepository.GenerateMock<IField>()));

            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            tableMock.Expect(m => m.Fields)
                     .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(fixture.CreateMany<IField>(5).ToList())))
                     .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => tableMock));

            var dataCollectionMock = new Collection<IEnumerable<IDataObjectBase>>();
            while (dataCollectionMock.Count < 250)
            {
                var dataObjects = new Collection<IDataObjectBase>();
                for (var i = 0; i < tableMock.Fields.Count; i++)
                {
                    var dataObject = MockRepository.GenerateMock<IFieldData<int, int>>();
                    dataObject.Expect(m => m.GetSourceValue<int>())
                              .Return(i)
                              .Repeat.Any();
                    dataObjects.Add(dataObject);
                }
                dataCollectionMock.Add(dataObjects);
            }

            var dataSetter = new DataSetter(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>());
            Assert.That(dataSetter, Is.Not.Null);

            var manipulatedData = dataSetter.ManipulateData(fixture.CreateAnonymous<ITable>(), dataCollectionMock);
            Assert.That(manipulatedData, Is.Not.Null);

            foreach (var dataObjects in manipulatedData)
            {
                Assert.That(dataObjects.ElementAt(3).GetSourceValue<int>(), Is.EqualTo(3));
            }
        }

        /// <summary>
        /// Tests that ManipulateData changes a field value without using criterias.
        /// </summary>
        [Test]
        public void TestThatManipulateDataChangesFieldValueWithoutUsingCriterias()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof(int)));
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

            var dataCollectionMock = new Collection<IEnumerable<IDataObjectBase>>();
            while (dataCollectionMock.Count < 250)
            {
                var dataObjects = new Collection<IDataObjectBase>();
                for (var i = 0; i < tableMock.Fields.Count; i++)
                {
                    var dataObject = MockRepository.GenerateMock<IFieldData<int, int>>();
                    dataObject.Expect(m => m.Field)
                              .Return(tableMock.Fields.ElementAt(i))
                              .Repeat.Any();
                    dataObjects.Add(dataObject);
                }
                dataCollectionMock.Add(dataObjects);
            }

            var newFieldValue = fixture.CreateAnonymous<int>();
            var dataSetter = new DataSetter(tableMock.NameSource, tableMock.Fields.ElementAt(3).NameSource, newFieldValue);
            Assert.That(dataSetter, Is.Not.Null);

            var manipulatedData = dataSetter.ManipulateData(fixture.CreateAnonymous<ITable>(), dataCollectionMock);
            Assert.That(manipulatedData, Is.Not.Null);

            foreach (var dataObjects in manipulatedData)
            {
                dataObjects.ElementAt(3).AssertWasCalled(m => m.UpdateSourceValue(Arg<int>.Is.Equal(newFieldValue)));
            }
        }

        /// <summary>
        /// Tests that ManipulateData changes a field value using a equal criterias.
        /// </summary>
        [Test]
        public void TestThatManipulateDataChangesFieldValueUsingEqualCriterias()
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

            var criteriaCollectionMock = new Collection<Tuple<Type, string, object>>
                {
                    new Tuple<Type, string, object>(typeof (EqualCriteria<>), tableMock.Fields.ElementAt(0).NameSource, "1")
                };

            var dataCollectionMock = new Collection<IEnumerable<IDataObjectBase>>();
            while (dataCollectionMock.Count < 250)
            {
                var dataObjects = new Collection<IDataObjectBase>();
                for (var i = 0; i < tableMock.Fields.Count; i++)
                {
                    var dataObject = MockRepository.GenerateMock<IFieldData<int, int>>();
                    dataObject.Expect(m => m.Field)
                              .Return(tableMock.Fields.ElementAt(i))
                              .Repeat.Any();
                    dataObject.Expect(m => m.SourceValue)
                              .Return(i == 0 ? dataCollectionMock.Count%2 : i)
                              .Repeat.Any();
                    dataObject.Expect(m => m.GetSourceValue<int>())
                              .Return(dataObject.SourceValue)
                              .Repeat.Any();
                    dataObjects.Add(dataObject);
                }
                dataCollectionMock.Add(dataObjects);
            }

            var newFieldValue = fixture.CreateAnonymous<int>();
            var dataSetter = new DataSetter(tableMock.NameSource, tableMock.Fields.ElementAt(3).NameSource, newFieldValue, criteriaCollectionMock);
            Assert.That(dataSetter, Is.Not.Null);

            var manipulatedData = dataSetter.ManipulateData(fixture.CreateAnonymous<ITable>(), dataCollectionMock);
            Assert.That(manipulatedData, Is.Not.Null);

            var dataRows = manipulatedData.ToList();
            for (var i = 0; i < dataRows.Count; i++)
            {
                if (dataRows.ElementAt(i).ElementAt(0).GetSourceValue<int>() > 0)
                {
                    dataRows.ElementAt(i).ElementAt(3).AssertWasCalled(m => m.UpdateSourceValue(Arg<int>.Is.Equal(newFieldValue)));
                    continue;
                }
                dataRows.ElementAt(i).ElementAt(3).AssertWasNotCalled(m => m.UpdateSourceValue(Arg<int>.Is.Anything));
            }
        }

        /// <summary>
        /// Tests that ManipulateData changes a field value using a pool criterias.
        /// </summary>
        [Test]
        public void TestThatManipulateDataChangesFieldValueUsingPoolCriterias()
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

            var criteriaCollectionMock = new Collection<Tuple<Type, string, object>>
                {
                    new Tuple<Type, string, object>(typeof (PoolCriteria<>), tableMock.Fields.ElementAt(0).NameSource, new[] {"1", "2", "3"})
                };

            var dataCollectionMock = new Collection<IEnumerable<IDataObjectBase>>();
            while (dataCollectionMock.Count < 250)
            {
                var dataObjects = new Collection<IDataObjectBase>();
                for (var i = 0; i < tableMock.Fields.Count; i++)
                {
                    var dataObject = MockRepository.GenerateMock<IFieldData<int, int>>();
                    dataObject.Expect(m => m.Field)
                              .Return(tableMock.Fields.ElementAt(i))
                              .Repeat.Any();
                    dataObject.Expect(m => m.SourceValue)
                              .Return(i == 0 ? dataCollectionMock.Count%4 : i)
                              .Repeat.Any();
                    dataObject.Expect(m => m.GetSourceValue<int>())
                              .Return(dataObject.SourceValue)
                              .Repeat.Any();
                    dataObjects.Add(dataObject);
                }
                dataCollectionMock.Add(dataObjects);
            }

            var newFieldValue = fixture.CreateAnonymous<int>();
            var dataSetter = new DataSetter(tableMock.NameSource, tableMock.Fields.ElementAt(3).NameSource, newFieldValue, criteriaCollectionMock);
            Assert.That(dataSetter, Is.Not.Null);

            var manipulatedData = dataSetter.ManipulateData(fixture.CreateAnonymous<ITable>(), dataCollectionMock);
            Assert.That(manipulatedData, Is.Not.Null);

            var dataRows = manipulatedData.ToList();
            for (var i = 0; i < dataRows.Count; i++)
            {
                if (dataRows.ElementAt(i).ElementAt(0).GetSourceValue<int>() > 0)
                {
                    dataRows.ElementAt(i).ElementAt(3).AssertWasCalled(m => m.UpdateSourceValue(Arg<int>.Is.Equal(newFieldValue)));
                    continue;
                }
                dataRows.ElementAt(i).ElementAt(3).AssertWasNotCalled(m => m.UpdateSourceValue(Arg<int>.Is.Anything));
            }
        }

        /// <summary>
        /// Tests that ManipulateData changes a field value using an interval criterias.
        /// </summary>
        [Test]
        public void TestThatManipulateDataChangesFieldValueUsingIntervalCriterias()
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

            var criteriaCollectionMock = new Collection<Tuple<Type, string, object>>
                {
                    new Tuple<Type, string, object>(typeof (IntervalCriteria<>), tableMock.Fields.ElementAt(0).NameSource, new Tuple<string, string>("1", "3"))
                };

            var dataCollectionMock = new Collection<IEnumerable<IDataObjectBase>>();
            while (dataCollectionMock.Count < 250)
            {
                var dataObjects = new Collection<IDataObjectBase>();
                for (var i = 0; i < tableMock.Fields.Count; i++)
                {
                    var dataObject = MockRepository.GenerateMock<IFieldData<int, int>>();
                    dataObject.Expect(m => m.Field)
                              .Return(tableMock.Fields.ElementAt(i))
                              .Repeat.Any();
                    dataObject.Expect(m => m.SourceValue)
                              .Return(i == 0 ? dataCollectionMock.Count%4 : i)
                              .Repeat.Any();
                    dataObject.Expect(m => m.GetSourceValue<int>())
                              .Return(dataObject.SourceValue)
                              .Repeat.Any();
                    dataObjects.Add(dataObject);
                }
                dataCollectionMock.Add(dataObjects);
            }

            var newFieldValue = fixture.CreateAnonymous<int>();
            var dataSetter = new DataSetter(tableMock.NameSource, tableMock.Fields.ElementAt(3).NameSource, newFieldValue, criteriaCollectionMock);
            Assert.That(dataSetter, Is.Not.Null);

            var manipulatedData = dataSetter.ManipulateData(fixture.CreateAnonymous<ITable>(), dataCollectionMock);
            Assert.That(manipulatedData, Is.Not.Null);

            var dataRows = manipulatedData.ToList();
            for (var i = 0; i < dataRows.Count; i++)
            {
                if (dataRows.ElementAt(i).ElementAt(0).GetSourceValue<int>() > 0)
                {
                    dataRows.ElementAt(i).ElementAt(3).AssertWasCalled(m => m.UpdateSourceValue(Arg<int>.Is.Equal(newFieldValue)));
                    continue;
                }
                dataRows.ElementAt(i).ElementAt(3).AssertWasNotCalled(m => m.UpdateSourceValue(Arg<int>.Is.Anything));
            }
        }

        /// <summary>
        /// Test that IsManipulatingField returns true if the data manipulator is manipulating the given field.
        /// </summary>
        [Test]
        public void TestThatIsManipulatingFieldReturnsTrueIfDataManipulatorIsManipulatingFieldName()
        {
            var fixture = new Fixture();

            var fieldForDataManipulator = fixture.CreateAnonymous<string>();
            var dataSetter = new DataSetter(fixture.CreateAnonymous<string>(), fieldForDataManipulator, fixture.CreateAnonymous<object>());
            Assert.That(dataSetter, Is.Not.Null);

            Assert.That(dataSetter.IsManipulatingField(fieldForDataManipulator), Is.True);
        }

        /// <summary>
        /// Test that IsManipulatingField returns false if the data manipulator is not manipulating the given field.
        /// </summary>
        [Test]
        public void TestThatIsManipulatingFieldReturnsFalseIfDataManipulatorIsNotManipulatingFieldName()
        {
            var fixture = new Fixture();

            var fieldForDataManipulator = fixture.CreateAnonymous<string>();
            var dataSetter = new DataSetter(fixture.CreateAnonymous<string>(), fieldForDataManipulator, fixture.CreateAnonymous<object>());
            Assert.That(dataSetter, Is.Not.Null);

            Assert.That(dataSetter.IsManipulatingField(fixture.CreateAnonymous<string>()), Is.False);
        }
    }
}
