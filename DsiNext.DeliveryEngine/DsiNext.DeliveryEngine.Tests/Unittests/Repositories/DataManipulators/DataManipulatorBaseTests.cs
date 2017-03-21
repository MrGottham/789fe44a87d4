using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Domain.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.DataManipulators;
using DsiNext.DeliveryEngine.Resources;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Repositories.DataManipulators
{
    /// <summary>
    /// Tests the basic data manipulator.
    /// </summary>
    [TestFixture]
    public class DataManipulatorBaseTests
    {
        /// <summary>
        /// Own class for testing the basic data manipulator.
        /// </summary>
        private class MyDataManipulator : DataManipulatorBase
        {
            #region Constructor

            /// <summary>
            /// Creates an data manipulator used for testing the basic data manipulator.
            /// </summary>
            /// <param name="tableName">Table name for the table on which the data should be manipulated.</param>
            public MyDataManipulator(string tableName)
                : base(tableName)
            {
            }

            #endregion

            #region Properties

            /// <summary>
            /// Indicates whether the data is manipulated.
            /// </summary>
            public bool DataIsManipulated
            {
                get;
                private set;
            }

            /// <summary>
            /// Indicates whether the data is finalized and manipulated.
            /// </summary>
            public bool DataIsFinalized
            {
                get;
                private set;
            }

            /// <summary>
            /// Indicates whether the ManipulatingField has been called.
            /// </summary>
            public bool ManipulatingFieldCalled
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Manipulates data for the table used by the data manipulator.
            /// </summary>
            /// <param name="table">Table on which to manipulate data.</param>
            /// <param name="dataToManipulate">Data which sould be manipulated.</param>
            /// <returns>Manipulated data for the table.</returns>
            protected override IEnumerable<IEnumerable<IDataObjectBase>> Manipulate(ITable table, IList<IEnumerable<IDataObjectBase>> dataToManipulate)
            {
                DataIsManipulated = true;
                return dataToManipulate;
            }

            /// <summary>
            /// Finalize data manipulation for the table used by the data manipulator.
            /// </summary>
            /// <param name="table">Table on which to finalize data manipulation.</param>
            /// <param name="dataToManipulate">The last manipulated data which has been received.</param>
            /// <returns>Finalized and manipulated data for the table.</returns>
            protected override IEnumerable<IEnumerable<IDataObjectBase>> Finalize(ITable table, IList<IEnumerable<IDataObjectBase>> dataToManipulate)
            {
                DataIsFinalized = true;
                return base.Finalize(table, dataToManipulate);
            }

            /// <summary>
            /// Indicates whether the data manipulator is manipulating a given field.
            /// </summary>
            /// <param name="fieldName">Name of the field on which to exam for use in the data manipulator.</param>
            /// <returns>True if the data manipulator use the field otherwise false.</returns>
            protected override bool ManipulatingField(string fieldName)
            {
                ManipulatingFieldCalled = true;
                return true;
            }

            /// <summary>
            /// Generates a filter for the table using the criteria configurations.
            /// </summary>
            /// <param name="table">Table on which to generate the filter.</param>
            /// <param name="criteriaConfigurations">Configuration for the criterias.</param>
            /// <returns>Filter for the table.</returns>
            public new IFilter GenerateFilter(ITable table, IEnumerable<Tuple<Type, string, object>> criteriaConfigurations)
            {
                return base.GenerateFilter(table, criteriaConfigurations);
            }

            #endregion
        }

        /// <summary>
        /// Tests that the constructor initialize the basic data manipulator.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeDataManipulator()
        {
            var fixture = new Fixture();
            var tableName = fixture.CreateAnonymous<string>();
            var dataManipulator = new MyDataManipulator(tableName);
            Assert.That(dataManipulator, Is.Not.Null);
            Assert.That(dataManipulator.TableName, Is.Not.Null);
            Assert.That(dataManipulator.TableName, Is.Not.Empty);
            Assert.That(dataManipulator.TableName, Is.EqualTo(tableName));
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentNullException if the table name is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfTableNameIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new MyDataManipulator(null));
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentNullException if the table name is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfTableNameIsEmpty()
        {
            Assert.Throws<ArgumentNullException>(() => new MyDataManipulator(string.Empty));
        }

        /// <summary>
        /// Tests that ManipulateData throws an ArgumentNullException if the table on which to manipulate data is null.
        /// </summary>
        [Test]
        public void TestThatManipulateDataThrowsArgumentNullExceptionIfTableIsNull()
        {
            var fixture = new Fixture();

            var dataManipulator = new MyDataManipulator(fixture.CreateAnonymous<string>());
            Assert.That(dataManipulator, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataManipulator.ManipulateData(null, new Collection<IEnumerable<IDataObjectBase>>()));
        }

        /// <summary>
        /// Tests that ManipulateData throws an ArgumentNullException if data for the table is null.
        /// </summary>
        [Test]
        public void TestThatManipulateDataThrowsArgumentNullExceptionIfDataIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));

            var dataManipulator = new MyDataManipulator(fixture.CreateAnonymous<string>());
            Assert.That(dataManipulator, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataManipulator.ManipulateData(fixture.CreateAnonymous<ITable>(), null));
        }

        /// <summary>
        /// Tests that ManipulateData returns without manipulating data if source name of the table does not match table name in the constructor.
        /// </summary>
        [Test]
        public void TestThatManipulateDataReturnsWithoutManipulatingDataIfTableSourceNameDoesNotMatchTableNameInTheConstructor()
        {
            var fixture = new Fixture();

            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => tableMock));

            var dataManipulator = new MyDataManipulator(fixture.CreateAnonymous<string>());
            Assert.That(dataManipulator, Is.Not.Null);
            Assert.That(dataManipulator.DataIsManipulated, Is.False);

            var data = new Collection<IEnumerable<IDataObjectBase>>();
            var manipulatedData = dataManipulator.ManipulateData(fixture.CreateAnonymous<ITable>(), data);
            Assert.That(manipulatedData, Is.Not.Null);
            Assert.That(manipulatedData, Is.EqualTo(data));

            Assert.That(dataManipulator.DataIsManipulated, Is.False);

            tableMock.AssertWasCalled(m => m.NameSource);
        }

        /// <summary>
        /// Tests that ManipulateData returns with manipulated data if source name of the table match table name in the constructor.
        /// </summary>
        [Test]
        public void TestThatManipulateDataReturnsWithManipulatedDataIfTableSourceNameMatchTableNameInTheConstructor()
        {
            var fixture = new Fixture();

            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => tableMock));

            var dataManipulator = new MyDataManipulator(tableMock.NameSource);
            Assert.That(dataManipulator, Is.Not.Null);
            Assert.That(dataManipulator.DataIsManipulated, Is.False);

            var data = new Collection<IEnumerable<IDataObjectBase>>();
            var manipulatedData = dataManipulator.ManipulateData(fixture.CreateAnonymous<ITable>(), data);
            Assert.That(manipulatedData, Is.Not.Null);
            Assert.That(manipulatedData, Is.EqualTo(data));

            Assert.That(dataManipulator.DataIsManipulated, Is.True);

            tableMock.AssertWasCalled(m => m.NameSource);
        }

        /// <summary>
        /// Tests that ManipulateData returns with manipulated data if target name of the table match table name in the constructor.
        /// </summary>
        [Test]
        public void TestThatManipulateDataReturnsWithManipulatedDataIfTableTargetNameMatchTableNameInTheConstructor()
        {
            var fixture = new Fixture();

            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameTarget)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => tableMock));

            var dataManipulator = new MyDataManipulator(tableMock.NameTarget);
            Assert.That(dataManipulator, Is.Not.Null);
            Assert.That(dataManipulator.DataIsManipulated, Is.False);

            var data = new Collection<IEnumerable<IDataObjectBase>>();
            var manipulatedData = dataManipulator.ManipulateData(fixture.CreateAnonymous<ITable>(), data);
            Assert.That(manipulatedData, Is.Not.Null);
            Assert.That(manipulatedData, Is.EqualTo(data));

            Assert.That(dataManipulator.DataIsManipulated, Is.True);

            tableMock.AssertWasCalled(m => m.NameTarget);
        }

        /// <summary>
        /// Tests that FinalizeDataManipulation throws an ArgumentNullException if the table on which to finalize data manipulation is null.
        /// </summary>
        [Test]
        public void TestThatFinalizeDataManipulationThrowsArgumentNullExceptionIfTableIsNull()
        {
            var fixture = new Fixture();

            var dataManipulator = new MyDataManipulator(fixture.CreateAnonymous<string>());
            Assert.That(dataManipulator, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataManipulator.FinalizeDataManipulation(null, new Collection<IEnumerable<IDataObjectBase>>()));
        }

        /// <summary>
        /// Tests that FinalizeDataManipulation throws an ArgumentNullException if data for the table is null.
        /// </summary>
        [Test]
        public void TestThatFinalizeDataManipulationThrowsArgumentNullExceptionIfDataIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));

            var dataManipulator = new MyDataManipulator(fixture.CreateAnonymous<string>());
            Assert.That(dataManipulator, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataManipulator.FinalizeDataManipulation(fixture.CreateAnonymous<ITable>(), null));
        }

        /// <summary>
        /// Tests that FinalizeDataManipulation returns without finalizing and manipulating data if source name of the table does not match table name in the constructor.
        /// </summary>
        [Test]
        public void TestThatFinalizeDataManipulationReturnsWithoutFinalizedDataIfTableSourceNameDoesNotMatchTableNameInTheConstructor()
        {
            var fixture = new Fixture();

            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => tableMock));

            var dataManipulator = new MyDataManipulator(fixture.CreateAnonymous<string>());
            Assert.That(dataManipulator, Is.Not.Null);
            Assert.That(dataManipulator.DataIsFinalized, Is.False);

            var data = new Collection<IEnumerable<IDataObjectBase>>();
            var manipulatedData = dataManipulator.FinalizeDataManipulation(fixture.CreateAnonymous<ITable>(), data);
            Assert.That(manipulatedData, Is.Not.Null);
            Assert.That(manipulatedData, Is.EqualTo(data));

            Assert.That(dataManipulator.DataIsFinalized, Is.False);

            tableMock.AssertWasCalled(m => m.NameSource);
        }

        /// <summary>
        /// Tests that FinalizeDataManipulation returns with manipulated and finalized data if source name of the table match table name in the constructor.
        /// </summary>
        [Test]
        public void TestThatFinalizeDataManipulationReturnsWithFinalizedDataIfTableSourceNameMatchTableNameInTheConstructor()
        {
            var fixture = new Fixture();

            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => tableMock));

            var dataManipulator = new MyDataManipulator(tableMock.NameSource);
            Assert.That(dataManipulator, Is.Not.Null);
            Assert.That(dataManipulator.DataIsFinalized, Is.False);

            var data = new Collection<IEnumerable<IDataObjectBase>>();
            var manipulatedData = dataManipulator.FinalizeDataManipulation(fixture.CreateAnonymous<ITable>(), data);
            Assert.That(manipulatedData, Is.Not.Null);
            Assert.That(manipulatedData, Is.EqualTo(data));

            Assert.That(dataManipulator.DataIsFinalized, Is.True);

            tableMock.AssertWasCalled(m => m.NameSource);
        }

        /// <summary>
        /// Tests that FinalizeDataManipulation returns with finalized and manipulated data if target name of the table match table name in the constructor.
        /// </summary>
        [Test]
        public void TestThatFinalizeDataManipulationReturnsWithFinalizedDataIfTableTargetNameMatchTableNameInTheConstructor()
        {
            var fixture = new Fixture();

            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameTarget)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => tableMock));

            var dataManipulator = new MyDataManipulator(tableMock.NameTarget);
            Assert.That(dataManipulator, Is.Not.Null);
            Assert.That(dataManipulator.DataIsFinalized, Is.False);

            var data = new Collection<IEnumerable<IDataObjectBase>>();
            var manipulatedData = dataManipulator.FinalizeDataManipulation(fixture.CreateAnonymous<ITable>(), data);
            Assert.That(manipulatedData, Is.Not.Null);
            Assert.That(manipulatedData, Is.EqualTo(data));

            Assert.That(dataManipulator.DataIsFinalized, Is.True);

            tableMock.AssertWasCalled(m => m.NameTarget);
        }

        /// <summary>
        /// Test that IsManipulatingTable throws an ArgumentNullException if name of the table is null.
        /// </summary>
        [Test]
        public void TestThatIsManipulatingTableThrowsArgumentNullExceptionIfTableNameIsNull()
        {
            var fixture = new Fixture();

            var dataManipulator = new MyDataManipulator(fixture.CreateAnonymous<string>());
            Assert.That(dataManipulator, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataManipulator.IsManipulatingTable(null));
        }

        /// <summary>
        /// Test that IsManipulatingTable throws an ArgumentNullException if name of the table is empty.
        /// </summary>
        [Test]
        public void TestThatIsManipulatingTableThrowsArgumentNullExceptionIfTableNameIsEmpty()
        {
            var fixture = new Fixture();

            var dataManipulator = new MyDataManipulator(fixture.CreateAnonymous<string>());
            Assert.That(dataManipulator, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataManipulator.IsManipulatingTable(string.Empty));
        }

        /// <summary>
        /// Test that IsManipulatingTable returns true if the name of the table matches the name of the data manipulators table.
        /// </summary>
        [Test]
        public void TestThatIsManipulatingTableReturnsTrueIfTableNameMatchTableNameForDataManipulator()
        {
            var fixture = new Fixture();

            var tableName = fixture.CreateAnonymous<string>();
            var dataManipulator = new MyDataManipulator(tableName);
            Assert.That(dataManipulator, Is.Not.Null);

            Assert.That(dataManipulator.IsManipulatingTable(tableName), Is.True);
        }

        /// <summary>
        /// Test that IsManipulatingTable returns false if the name of the table does not match the name of the data manipulators table.
        /// </summary>
        [Test]
        public void TestThatIsManipulatingTableReturnsFalseIfTableNameDoesNotMatchTableNameForDataManipulator()
        {
            var fixture = new Fixture();

            var tableName = fixture.CreateAnonymous<string>();
            var dataManipulator = new MyDataManipulator(tableName);
            Assert.That(dataManipulator, Is.Not.Null);

            Assert.That(dataManipulator.IsManipulatingTable(fixture.CreateAnonymous<string>()), Is.False);
        }

        /// <summary>
        /// Test that IsManipulatingField throws an ArgumentNullException if name of the field is null.
        /// </summary>
        [Test]
        public void TestThatIsManipulatingFieldThrowsArgumentNullExceptionIfFieldNameIsNull()
        {
            var fixture = new Fixture();

            var dataManipulator = new MyDataManipulator(fixture.CreateAnonymous<string>());
            Assert.That(dataManipulator, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataManipulator.IsManipulatingField(null));
        }

        /// <summary>
        /// Test that IsManipulatingField throws an ArgumentNullException if name of the field is empty.
        /// </summary>
        [Test]
        public void TestThatIsManipulatingFieldThrowsArgumentNullExceptionIfFieldNameIsEmpty()
        {
            var fixture = new Fixture();

            var dataManipulator = new MyDataManipulator(fixture.CreateAnonymous<string>());
            Assert.That(dataManipulator, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataManipulator.IsManipulatingField(string.Empty));
        }

        /// <summary>
        /// Test that IsManipulatingField calls and returns result from the abstract method ManipulatingField.
        /// </summary>
        [Test]
        public void TestThatIsManipulatingFieldCallsManipulatingField()
        {
            var fixture = new Fixture();

            var dataManipulator = new MyDataManipulator(fixture.CreateAnonymous<string>());
            Assert.That(dataManipulator, Is.Not.Null);

            Assert.That(dataManipulator.ManipulatingFieldCalled, Is.False);
            Assert.That(dataManipulator.IsManipulatingField(fixture.CreateAnonymous<string>()), Is.True);
            Assert.That(dataManipulator.ManipulatingFieldCalled, Is.True);
        }

        /// <summary>
        /// Tests that GenerateFilter throws an ArgumentNullException if the table is null.
        /// </summary>
        [Test]
        public void TestThatGenerateFilterThrowsArgumentNullExceptionIfTableIsNull()
        {
            var fixture = new Fixture();

            var dataManipulator = new MyDataManipulator(fixture.CreateAnonymous<string>());
            Assert.That(dataManipulator, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataManipulator.GenerateFilter(null, new Collection<Tuple<Type, string, object>>()));
        }

        /// <summary>
        /// Tests that GenerateFilter throws an ArgumentNullException if the configuration for the criterias is null.
        /// </summary>
        [Test]
        public void TestThatGenerateFilterThrowsArgumentNullExceptionIfCriteriaConfigurationsIsNull()
        {
            var fixture = new Fixture();

            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => tableMock));

            var dataManipulator = new MyDataManipulator(tableMock.NameSource);
            Assert.That(dataManipulator, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataManipulator.GenerateFilter(fixture.CreateAnonymous<ITable>(), null));
        }

        /// <summary>
        /// Tests that GenerateFilter creates a filter without criterias if the configuration for the criterias is empty.
        /// </summary>
        [Test]
        public void TestThatGenerateFilterCreatesFilterWithoutCriteriasIfCriteriaConfigurationsIsEmpty()
        {
            var fixture = new Fixture();

            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => tableMock));

            var dataManipulator = new MyDataManipulator(tableMock.NameSource);
            Assert.That(dataManipulator, Is.Not.Null);

            var filter = dataManipulator.GenerateFilter(fixture.CreateAnonymous<ITable>(), new Collection<Tuple<Type, string, object>>());
            Assert.That(filter, Is.Not.Null);
            Assert.That(filter.Criterias, Is.Not.Null);
            Assert.That(filter.Criterias, Is.Empty);
            Assert.That(filter.Criterias.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests that GenerateFilter creates a filter with some eqaul criterias.
        /// </summary>
        [Test]
        public void TestThatGenerateFilterCreatesFilterWithSomeEqualCriterias()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (int?)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    fieldMock.Expect(m => m.DatatypeOfSource)
                             .Return(typeof (int?))
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

            var dataManipulator = new MyDataManipulator(tableMock.NameSource);
            Assert.That(dataManipulator, Is.Not.Null);

            var criteriaConfigurations = new Collection<Tuple<Type, string, object>>
                {
                    new Tuple<Type, string, object>(typeof (EqualCriteria<>), tableMock.Fields.ElementAt(0).NameSource, fixture.CreateAnonymous<int>()),
                    new Tuple<Type, string, object>(typeof (EqualCriteria<>), tableMock.Fields.ElementAt(1).NameSource, fixture.CreateAnonymous<int>().ToString(CultureInfo.InvariantCulture))
                };

            var filter = dataManipulator.GenerateFilter(fixture.CreateAnonymous<ITable>(), criteriaConfigurations);
            Assert.That(filter, Is.Not.Null);
            Assert.That(filter.Criterias, Is.Not.Null);
            Assert.That(filter.Criterias, Is.Not.Empty);
            Assert.That(filter.Criterias.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests that GenerateFilter creates a filter with some pool criterias.
        /// </summary>
        [Test]
        public void TestThatGenerateFilterCreatesFilterWithSomePoolCriterias()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (int?)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    fieldMock.Expect(m => m.DatatypeOfSource)
                             .Return(typeof (int?))
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

            var dataManipulator = new MyDataManipulator(tableMock.NameSource);
            Assert.That(dataManipulator, Is.Not.Null);

            var criteriaConfigurations = new Collection<Tuple<Type, string, object>>
                {
                    new Tuple<Type, string, object>(typeof (PoolCriteria<>), tableMock.Fields.ElementAt(0).NameSource, new[] {fixture.CreateAnonymous<int>().ToString(CultureInfo.InvariantCulture), fixture.CreateAnonymous<int>().ToString(CultureInfo.InvariantCulture), fixture.CreateAnonymous<int>().ToString(CultureInfo.InvariantCulture)}),
                    new Tuple<Type, string, object>(typeof (PoolCriteria<>), tableMock.Fields.ElementAt(1).NameSource, new[] {fixture.CreateAnonymous<int>().ToString(CultureInfo.InvariantCulture), fixture.CreateAnonymous<int>().ToString(CultureInfo.InvariantCulture), fixture.CreateAnonymous<int>().ToString(CultureInfo.InvariantCulture)})
                };

            var filter = dataManipulator.GenerateFilter(fixture.CreateAnonymous<ITable>(), criteriaConfigurations);
            Assert.That(filter, Is.Not.Null);
            Assert.That(filter.Criterias, Is.Not.Null);
            Assert.That(filter.Criterias, Is.Not.Empty);
            Assert.That(filter.Criterias.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests that GenerateFilter creates a filter with some interval criterias.
        /// </summary>
        [Test]
        public void TestThatGenerateFilterCreatesFilterWithSomeIntervalCriterias()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (int?)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    fieldMock.Expect(m => m.DatatypeOfSource)
                             .Return(typeof (int?))
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

            var dataManipulator = new MyDataManipulator(tableMock.NameSource);
            Assert.That(dataManipulator, Is.Not.Null);

            var criteriaConfigurations = new Collection<Tuple<Type, string, object>>
                {
                    new Tuple<Type, string, object>(typeof (IntervalCriteria<>), tableMock.Fields.ElementAt(0).NameSource, new Tuple<string, string>(fixture.CreateAnonymous<int>().ToString(CultureInfo.InvariantCulture), fixture.CreateAnonymous<int>().ToString(CultureInfo.InvariantCulture))),
                    new Tuple<Type, string, object>(typeof (IntervalCriteria<>), tableMock.Fields.ElementAt(1).NameSource, new Tuple<string, string>(fixture.CreateAnonymous<int>().ToString(CultureInfo.InvariantCulture), fixture.CreateAnonymous<int>().ToString(CultureInfo.InvariantCulture)))
                };

            var filter = dataManipulator.GenerateFilter(fixture.CreateAnonymous<ITable>(), criteriaConfigurations);
            Assert.That(filter, Is.Not.Null);
            Assert.That(filter.Criterias, Is.Not.Null);
            Assert.That(filter.Criterias, Is.Not.Empty);
            Assert.That(filter.Criterias.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests that GenerateFilter throws an DeliveryEngineSystemException if a field is not found on the table.
        /// </summary>
        [Test]
        public void TestThatGenerateFilterThrowsDeliveryEngineSystemExceptionIfFieldNotFoundOnTable()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (int?)));
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

            var dataManipulator = new MyDataManipulator(tableMock.NameSource);
            Assert.That(dataManipulator, Is.Not.Null);

            var criteriaConfigurations = new Collection<Tuple<Type, string, object>>
                {
                    new Tuple<Type, string, object>(typeof (EqualCriteria<>), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<int>())
                };

            var exception = Assert.Throws<DeliveryEngineSystemException>(() => dataManipulator.GenerateFilter(fixture.CreateAnonymous<ITable>(), criteriaConfigurations));
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Empty);
            Assert.That(exception.Message, Is.EqualTo(Resource.GetExceptionMessage(ExceptionMessage.FieldNotFound, criteriaConfigurations.ElementAt(0).Item2)));
        }

        /// <summary>
        /// Tests that GenerateFilter throws an DeliveryEngineSystemException if a data type in the criteria configurations is not supported.
        /// </summary>
        [Test]
        public void TestThatGenerateFilterThrowsDeliveryEngineSystemExceptionIfDataTypeInCriteriaConfigurationsIsNotSupported()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (int?)));
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

            var dataManipulator = new MyDataManipulator(tableMock.NameSource);
            Assert.That(dataManipulator, Is.Not.Null);

            var criteriaConfigurations = new Collection<Tuple<Type, string, object>>
                {
                    new Tuple<Type, string, object>(typeof (object), tableMock.Fields.ElementAt(0).NameSource, fixture.CreateAnonymous<int>())
                };

            var exception = Assert.Throws<DeliveryEngineSystemException>(() => dataManipulator.GenerateFilter(fixture.CreateAnonymous<ITable>(), criteriaConfigurations));
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Empty);
            Assert.That(exception.Message, Is.EqualTo(Resource.GetExceptionMessage(ExceptionMessage.DataTypeNotSupported, criteriaConfigurations.ElementAt(0).Item1.Name)));
        }
    }
}
