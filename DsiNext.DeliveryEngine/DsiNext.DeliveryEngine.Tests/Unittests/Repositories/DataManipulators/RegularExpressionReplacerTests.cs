using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.DataManipulators;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Enums;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Repositories.DataManipulators
{
    /// <summary>
    /// Tests the regular expression replacer which can set, change or clear a field value.
    /// </summary>
    [TestFixture]
    public class RegularExpressionReplacerTests
    {
        /// <summary>
        /// Test that the constructor initialize a regular expression replacer with ApplyOnMatch.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeRegularExpressionReplacerWithApplyOnMatch()
        {
            var fixture = new Fixture();
            fixture.Customize<Regex>(e => e.FromFactory(() => new Regex("^[JjNn]$", RegexOptions.Compiled)));
            fixture.Customize<RegularExpressionApplyOn>(e => e.FromFactory(() => RegularExpressionApplyOn.ApplyOnMatch));

            var tableName = fixture.CreateAnonymous<string>();
            var regularExpression = fixture.CreateAnonymous<Regex>();
            var applyOn = fixture.CreateAnonymous<RegularExpressionApplyOn>();
            var fieldName = fixture.CreateAnonymous<string>();
            var fieldValue = fixture.CreateAnonymous<string>();
            var regularExpressionReplacer = new RegularExpressionReplacer(tableName, regularExpression, applyOn, fieldName, fieldValue);
            Assert.That(regularExpressionReplacer, Is.Not.Null);
            Assert.That(regularExpressionReplacer.TableName, Is.Not.Null);
            Assert.That(regularExpressionReplacer.TableName, Is.Not.Empty);
            Assert.That(regularExpressionReplacer.TableName, Is.EqualTo(tableName));
            Assert.That(regularExpressionReplacer, Is.Not.Null);
            Assert.That(regularExpressionReplacer.RegularExpression, Is.Not.Null);
            Assert.That(regularExpressionReplacer.RegularExpression, Is.EqualTo(regularExpression));
            Assert.That(regularExpressionReplacer.ApplyOn, Is.EqualTo(RegularExpressionApplyOn.ApplyOnMatch));
            Assert.That(regularExpressionReplacer.FieldName, Is.Not.Null);
            Assert.That(regularExpressionReplacer.FieldName, Is.Not.Empty);
            Assert.That(regularExpressionReplacer.FieldName, Is.EqualTo(fieldName));
            Assert.That(regularExpressionReplacer.FieldValue, Is.Not.Null);
            Assert.That(regularExpressionReplacer.FieldValue, Is.EqualTo(fieldValue));
        }

        /// <summary>
        /// Test that the constructor initialize a regular expression replacer with ApplyOnUnmatch.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeRegularExpressionReplacerWithApplyOnUnmatch()
        {
            var fixture = new Fixture();
            fixture.Customize<Regex>(e => e.FromFactory(() => new Regex("^[JjNn]$", RegexOptions.Compiled)));
            fixture.Customize<RegularExpressionApplyOn>(e => e.FromFactory(() => RegularExpressionApplyOn.ApplyOnUnmatch));

            var tableName = fixture.CreateAnonymous<string>();
            var regularExpression = fixture.CreateAnonymous<Regex>();
            var applyOn = fixture.CreateAnonymous<RegularExpressionApplyOn>();
            var fieldName = fixture.CreateAnonymous<string>();
            var fieldValue = fixture.CreateAnonymous<string>();
            var regularExpressionReplacer = new RegularExpressionReplacer(tableName, regularExpression, applyOn, fieldName, fieldValue);
            Assert.That(regularExpressionReplacer, Is.Not.Null);
            Assert.That(regularExpressionReplacer.TableName, Is.Not.Null);
            Assert.That(regularExpressionReplacer.TableName, Is.Not.Empty);
            Assert.That(regularExpressionReplacer.TableName, Is.EqualTo(tableName));
            Assert.That(regularExpressionReplacer, Is.Not.Null);
            Assert.That(regularExpressionReplacer.RegularExpression, Is.Not.Null);
            Assert.That(regularExpressionReplacer.RegularExpression, Is.EqualTo(regularExpression));
            Assert.That(regularExpressionReplacer.ApplyOn, Is.EqualTo(RegularExpressionApplyOn.ApplyOnUnmatch));
            Assert.That(regularExpressionReplacer.FieldName, Is.Not.Null);
            Assert.That(regularExpressionReplacer.FieldName, Is.Not.Empty);
            Assert.That(regularExpressionReplacer.FieldName, Is.EqualTo(fieldName));
            Assert.That(regularExpressionReplacer.FieldValue, Is.Not.Null);
            Assert.That(regularExpressionReplacer.FieldValue, Is.EqualTo(fieldValue));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if name of the table is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfTableNameIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<Regex>(e => e.FromFactory(() => new Regex("^[JjNn]$", RegexOptions.Compiled)));
            fixture.Customize<RegularExpressionApplyOn>(e => e.FromFactory(() => RegularExpressionApplyOn.ApplyOnMatch));

            Assert.Throws<ArgumentNullException>(() => new RegularExpressionReplacer(null, fixture.CreateAnonymous<Regex>(), fixture.CreateAnonymous<RegularExpressionApplyOn>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the regular expression if null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfRegularExpressionIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<RegularExpressionApplyOn>(e => e.FromFactory(() => RegularExpressionApplyOn.ApplyOnMatch));

            Assert.Throws<ArgumentNullException>(() => new RegularExpressionReplacer(fixture.CreateAnonymous<string>(), null, fixture.CreateAnonymous<RegularExpressionApplyOn>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if name of the field is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfFieldNameIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<Regex>(e => e.FromFactory(() => new Regex("^[JjNn]$", RegexOptions.Compiled)));
            fixture.Customize<RegularExpressionApplyOn>(e => e.FromFactory(() => RegularExpressionApplyOn.ApplyOnMatch));

            Assert.Throws<ArgumentNullException>(() => new RegularExpressionReplacer(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<Regex>(), fixture.CreateAnonymous<RegularExpressionApplyOn>(), null, fixture.CreateAnonymous<object>()));
        }

        /// <summary>
        /// Test that ManipulateData throws an ArgumentNullException if the table is null.
        /// </summary>
        [Test]
        public void TestThatManipulateDataThrowsArgumentNullExceptionIfTableIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<Regex>(e => e.FromFactory(() => new Regex("^[JjNn]$", RegexOptions.Compiled)));
            fixture.Customize<RegularExpressionApplyOn>(e => e.FromFactory(() => RegularExpressionApplyOn.ApplyOnMatch));

            var regularExpressionReplacer = new RegularExpressionReplacer(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<Regex>(), fixture.CreateAnonymous<RegularExpressionApplyOn>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>());
            Assert.That(regularExpressionReplacer, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => regularExpressionReplacer.ManipulateData(null, new List<IEnumerable<IDataObjectBase>>(0)));
        }

        /// <summary>
        /// Test that ManipulateData throws an ArgumentNullException if the data is null.
        /// </summary>
        [Test]
        public void TestThatManipulateDataThrowsArgumentNullExceptionIfDataIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<Regex>(e => e.FromFactory(() => new Regex("^[JjNn]$", RegexOptions.Compiled)));
            fixture.Customize<RegularExpressionApplyOn>(e => e.FromFactory(() => RegularExpressionApplyOn.ApplyOnMatch));
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));

            var regularExpressionReplacer = new RegularExpressionReplacer(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<Regex>(), fixture.CreateAnonymous<RegularExpressionApplyOn>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>());
            Assert.That(regularExpressionReplacer, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => regularExpressionReplacer.ManipulateData(fixture.CreateAnonymous<ITable>(), null));
        }

        /// <summary>
        /// Tests that ManipulateData returns without manipulating data if source name of the table does not match table name in the constructor.
        /// </summary>
        [Test]
        public void TestThatManipulateDataReturnsWithoutManipulatingDataIfTableSourceNameDoesNotMatchTableNameInTheConstructor()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<Regex>(e => e.FromFactory(() => new Regex("^[JjNn]$", RegexOptions.Compiled)));
            fixture.Customize<RegularExpressionApplyOn>(e => e.FromFactory(() => RegularExpressionApplyOn.ApplyOnMatch));
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

            var tableData = new List<IEnumerable<IDataObjectBase>>(250);
            while (tableData.Count < tableData.Capacity)
            {
                tableData.Add(tableMock.Fields.Select(field => MockRepository.GenerateMock<IDataObjectBase>()).ToList());
            }

            var regularExpressionReplacer = new RegularExpressionReplacer(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<Regex>(), fixture.CreateAnonymous<RegularExpressionApplyOn>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>());
            Assert.That(regularExpressionReplacer, Is.Not.Null);

            regularExpressionReplacer.ManipulateData(fixture.CreateAnonymous<ITable>(), tableData);
            foreach (var tableRow in tableData)
            {
                tableRow.ToList().ForEach(dataObject => dataObject.AssertWasNotCalled(m => m.UpdateSourceValue(Arg<object>.Is.NotNull)));
            }
        }

        /// <summary>
        /// Tests that ManipulateData throws an DeliveryEngineSystemException if the field name is not found on a data object.
        /// </summary>
        [Test]
        public void TestThatManipulateDataThrowsDeliveryEngineSystemExceptionIfFieldNameIsNotFoundOnDataObjects()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<Regex>(e => e.FromFactory(() => new Regex("^[JjNn]$", RegexOptions.Compiled)));
            fixture.Customize<RegularExpressionApplyOn>(e => e.FromFactory(() => RegularExpressionApplyOn.ApplyOnMatch));
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

            var tableData = new List<IEnumerable<IDataObjectBase>>(250);
            while (tableData.Count < tableData.Capacity)
            {
                tableData.Add(tableMock.Fields.Select(field =>
                    {
                        var dataObjectMock = MockRepository.GenerateMock<IDataObjectBase>();
                        dataObjectMock.Expect(m => m.Field)
                                      .Return(field)
                                      .Repeat.Any();
                        return dataObjectMock;
                    }).ToList());
            }

            var regularExpressionReplacer = new RegularExpressionReplacer(tableMock.NameSource, fixture.CreateAnonymous<Regex>(), fixture.CreateAnonymous<RegularExpressionApplyOn>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<object>());
            Assert.That(regularExpressionReplacer, Is.Not.Null);

            Assert.Throws<DeliveryEngineSystemException>(() => regularExpressionReplacer.ManipulateData(fixture.CreateAnonymous<ITable>(), tableData));
        }

        /// <summary>
        /// Tests that ManipulateData replace data where source values matches the regular expression.
        /// </summary>
        [Test]
        public void TestThatManipulateDataReplaceDataWhereSourceValuesMatchesRegularExpression()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<Regex>(e => e.FromFactory(() => new Regex("^[JjNn]$", RegexOptions.Compiled)));
            fixture.Customize<RegularExpressionApplyOn>(e => e.FromFactory(() => RegularExpressionApplyOn.ApplyOnMatch));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    fieldMock.Expect(m => m.DatatypeOfSource)
                             .Return(typeof (string))
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

            var tableData = new List<IEnumerable<IDataObjectBase>>(250);
            while (tableData.Count < tableData.Capacity)
            {
                tableData.Add(tableMock.Fields.Select(field =>
                    {
                        var dataObjectMock = MockRepository.GenerateMock<IDataObjectBase>();
                        dataObjectMock.Expect(m => m.Field)
                                      .Return(field)
                                      .Repeat.Any();
                        dataObjectMock.Expect(m => m.GetSourceValue<string>())
                                      .Return("n")
                                      .Repeat.Any();
                        return dataObjectMock;
                    }).ToList());
            }

            var regularExpressionReplacer = new RegularExpressionReplacer(tableMock.NameSource, fixture.CreateAnonymous<Regex>(), fixture.CreateAnonymous<RegularExpressionApplyOn>(), tableMock.Fields.ElementAt(0).NameSource, fixture.CreateAnonymous<string>());
            Assert.That(regularExpressionReplacer, Is.Not.Null);

            regularExpressionReplacer.ManipulateData(fixture.CreateAnonymous<ITable>(), tableData);
            foreach (var tableRow in tableData)
            {
                tableRow.ElementAt(0).AssertWasCalled(m => m.UpdateSourceValue(Arg<string>.Is.Equal(regularExpressionReplacer.FieldValue)));
            }
        }

        /// <summary>
        /// Tests that ManipulateData clear value where source values matches the regular expression.
        /// </summary>
        [Test]
        public void TestThatManipulateDataClearValueWhereSourceValuesMatchesRegularExpression()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<Regex>(e => e.FromFactory(() => new Regex("^[JjNn]$", RegexOptions.Compiled)));
            fixture.Customize<RegularExpressionApplyOn>(e => e.FromFactory(() => RegularExpressionApplyOn.ApplyOnMatch));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    fieldMock.Expect(m => m.DatatypeOfSource)
                             .Return(typeof (string))
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

            var tableData = new List<IEnumerable<IDataObjectBase>>(250);
            while (tableData.Count < tableData.Capacity)
            {
                tableData.Add(tableMock.Fields.Select(field =>
                {
                    var dataObjectMock = MockRepository.GenerateMock<IDataObjectBase>();
                    dataObjectMock.Expect(m => m.Field)
                                  .Return(field)
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.GetSourceValue<string>())
                                  .Return("n")
                                  .Repeat.Any();
                    return dataObjectMock;
                }).ToList());
            }

            var regularExpressionReplacer = new RegularExpressionReplacer(tableMock.NameSource, fixture.CreateAnonymous<Regex>(), fixture.CreateAnonymous<RegularExpressionApplyOn>(), tableMock.Fields.ElementAt(0).NameSource, null);
            Assert.That(regularExpressionReplacer, Is.Not.Null);

            regularExpressionReplacer.ManipulateData(fixture.CreateAnonymous<ITable>(), tableData);
            foreach (var tableRow in tableData)
            {
                tableRow.ElementAt(0).AssertWasCalled(m => m.UpdateSourceValue(Arg<object>.Is.Null));
            }
        }

        /// <summary>
        /// Tests that ManipulateData replace data where source values does not matches the regular expression.
        /// </summary>
        [Test]
        public void TestThatManipulateDataReplaceDataWhereSourceValuesDoesNotMatchesRegularExpression()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<Regex>(e => e.FromFactory(() => new Regex("^[JjNn]$", RegexOptions.Compiled)));
            fixture.Customize<RegularExpressionApplyOn>(e => e.FromFactory(() => RegularExpressionApplyOn.ApplyOnUnmatch));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    fieldMock.Expect(m => m.DatatypeOfSource)
                             .Return(typeof (string))
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

            var tableData = new List<IEnumerable<IDataObjectBase>>(250);
            while (tableData.Count < tableData.Capacity)
            {
                tableData.Add(tableMock.Fields.Select(field =>
                {
                    var dataObjectMock = MockRepository.GenerateMock<IDataObjectBase>();
                    dataObjectMock.Expect(m => m.Field)
                                  .Return(field)
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.GetSourceValue<string>())
                                  .Return("x")
                                  .Repeat.Any();
                    return dataObjectMock;
                }).ToList());
            }

            var regularExpressionReplacer = new RegularExpressionReplacer(tableMock.NameSource, fixture.CreateAnonymous<Regex>(), fixture.CreateAnonymous<RegularExpressionApplyOn>(), tableMock.Fields.ElementAt(0).NameSource, fixture.CreateAnonymous<string>());
            Assert.That(regularExpressionReplacer, Is.Not.Null);

            regularExpressionReplacer.ManipulateData(fixture.CreateAnonymous<ITable>(), tableData);
            foreach (var tableRow in tableData)
            {
                tableRow.ElementAt(0).AssertWasCalled(m => m.UpdateSourceValue(Arg<string>.Is.Equal(regularExpressionReplacer.FieldValue)));
            }
        }

        /// <summary>
        /// Tests that ManipulateData clear value where source values does not matches the regular expression.
        /// </summary>
        [Test]
        public void TestThatManipulateDataClearValueWhereSourceValuesDoesNotMatchesRegularExpression()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<Regex>(e => e.FromFactory(() => new Regex("^[JjNn]$", RegexOptions.Compiled)));
            fixture.Customize<RegularExpressionApplyOn>(e => e.FromFactory(() => RegularExpressionApplyOn.ApplyOnUnmatch));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    fieldMock.Expect(m => m.DatatypeOfSource)
                             .Return(typeof (string))
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

            var tableData = new List<IEnumerable<IDataObjectBase>>(250);
            while (tableData.Count < tableData.Capacity)
            {
                tableData.Add(tableMock.Fields.Select(field =>
                {
                    var dataObjectMock = MockRepository.GenerateMock<IDataObjectBase>();
                    dataObjectMock.Expect(m => m.Field)
                                  .Return(field)
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.GetSourceValue<string>())
                                  .Return("x")
                                  .Repeat.Any();
                    return dataObjectMock;
                }).ToList());
            }

            var regularExpressionReplacer = new RegularExpressionReplacer(tableMock.NameSource, fixture.CreateAnonymous<Regex>(), fixture.CreateAnonymous<RegularExpressionApplyOn>(), tableMock.Fields.ElementAt(0).NameSource, null);
            Assert.That(regularExpressionReplacer, Is.Not.Null);

            regularExpressionReplacer.ManipulateData(fixture.CreateAnonymous<ITable>(), tableData);
            foreach (var tableRow in tableData)
            {
                tableRow.ElementAt(0).AssertWasCalled(m => m.UpdateSourceValue(Arg<object>.Is.Null));
            }
        }

        /// <summary>
        /// Test that IsManipulatingField returns true if the data manipulator is manipulating the given field.
        /// </summary>
        [Test]
        public void TestThatIsManipulatingFieldReturnsTrueIfDataManipulatorIsManipulatingFieldName()
        {
            var fixture = new Fixture();
            fixture.Customize<Regex>(e => e.FromFactory(() => new Regex("^[JjNn]$", RegexOptions.Compiled)));
            fixture.Customize<RegularExpressionApplyOn>(e => e.FromFactory(() => RegularExpressionApplyOn.ApplyOnMatch));

            var fieldForDataManipulator = fixture.CreateAnonymous<string>();
            var regularExpressionReplacer = new RegularExpressionReplacer(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<Regex>(), fixture.CreateAnonymous<RegularExpressionApplyOn>(), fieldForDataManipulator, fixture.CreateAnonymous<object>());
            Assert.That(regularExpressionReplacer, Is.Not.Null);

            Assert.That(regularExpressionReplacer.IsManipulatingField(fieldForDataManipulator), Is.True);
        }

        /// <summary>
        /// Test that IsManipulatingField returns false if the data manipulator is not manipulating the given field.
        /// </summary>
        [Test]
        public void TestThatIsManipulatingFieldReturnsFalseIfDataManipulatorIsNotManipulatingFieldName()
        {
            var fixture = new Fixture();
            fixture.Customize<Regex>(e => e.FromFactory(() => new Regex("^[JjNn]$", RegexOptions.Compiled)));
            fixture.Customize<RegularExpressionApplyOn>(e => e.FromFactory(() => RegularExpressionApplyOn.ApplyOnMatch));

            var fieldForDataManipulator = fixture.CreateAnonymous<string>();
            var regularExpressionReplacer = new RegularExpressionReplacer(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<Regex>(), fixture.CreateAnonymous<RegularExpressionApplyOn>(), fieldForDataManipulator, fixture.CreateAnonymous<object>());
            Assert.That(regularExpressionReplacer, Is.Not.Null);

            Assert.That(regularExpressionReplacer.IsManipulatingField(fixture.CreateAnonymous<string>()), Is.False);
        }
    }
}
