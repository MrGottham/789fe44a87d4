using System;
using System.Collections.Generic;
using System.Linq;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Domain.Metadata;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Domain.Metadata
{
    /// <summary>
    /// a Test filter containing criterias.
    /// </summary>
    [TestFixture]
    public class FilterTests
    {
        /// <summary>
        /// Test that the constructor initialize a filter containing criterias.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeFilter()
        {
            var filter = new Filter();
            Assert.That(filter, Is.Not.Null);
            Assert.That(filter.Criterias, Is.Not.Null);
            Assert.That(filter.Criterias.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Test that AsString returns Empty if the filter does not contains any criterias.
        /// </summary>
        [Test]
        public void TestThatAsStringReturnsEmptyIfFilterDoesNotContainsCriterias()
        {
            var filter = new Filter();
            Assert.That(filter, Is.Not.Null);
            Assert.That(filter.Criterias, Is.Not.Null);
            Assert.That(filter.Criterias.Count, Is.EqualTo(0));

            var asString = filter.AsString();
            Assert.That(asString, Is.Not.Null);
            Assert.That(asString, Is.Empty);
        }

        /// <summary>
        /// Test that AsString returns the string representation of the criterias in the filter.
        /// </summary>
        [Test]
        public void TestThatAsStringReturnsStringRepresentationOfCriterias()
        {
            var fixture = new Fixture();
            fixture.Customize<ICriteria>(e => e.FromFactory(() =>
                {
                    var criteriaMock = MockRepository.GenerateMock<ICriteria>();
                    criteriaMock.Expect(m => m.AsString())
                        .Return(fixture.CreateAnonymous<string>())
                        .Repeat.Any();
                    return criteriaMock;
                }));

            var filter = new Filter();
            Assert.That(filter, Is.Not.Null);
            Assert.That(filter.Criterias, Is.Not.Null);
            Assert.That(filter.Criterias.Count, Is.EqualTo(0));

            filter.AddCriteria(fixture.CreateAnonymous<ICriteria>());
            filter.AddCriteria(fixture.CreateAnonymous<ICriteria>());
            filter.AddCriteria(fixture.CreateAnonymous<ICriteria>());
            Assert.That(filter.Criterias, Is.Not.Null);
            Assert.That(filter.Criterias.Count, Is.EqualTo(3));

            var asString = filter.AsString();
            Assert.That(asString, Is.Not.Null);
            Assert.That(asString, Is.Not.Empty);
            Assert.That(asString, Is.EqualTo(string.Format("{0}\r\n{1}\r\n{2}", filter.Criterias.ElementAt(0).AsString(), filter.Criterias.ElementAt(1).AsString(), filter.Criterias.ElementAt(2).AsString())));

            filter.Criterias.ElementAt(0).AssertWasCalled(m => m.AsString());
            filter.Criterias.ElementAt(1).AssertWasCalled(m => m.AsString());
            filter.Criterias.ElementAt(2).AssertWasCalled(m => m.AsString());
        }

        /// <summary>
        /// Test that AsSql returns Empty if the filter does not contains any criterias.
        /// </summary>
        [Test]
        public void TestThatAsSqlReturnsEmptyIfFilterDoesNotContainsCriterias()
        {
            var filter = new Filter();
            Assert.That(filter, Is.Not.Null);
            Assert.That(filter.Criterias, Is.Not.Null);
            Assert.That(filter.Criterias.Count, Is.EqualTo(0));

            var asSql = filter.AsString();
            Assert.That(asSql, Is.Not.Null);
            Assert.That(asSql, Is.Empty);
        }

        /// <summary>
        /// Test that AsSql returns the SQL representation of the criterias in the filter.
        /// </summary>
        [Test]
        public void TestThatAsSqlReturnsSqlRepresentationOfCriterias()
        {
            var fixture = new Fixture();
            fixture.Customize<ICriteria>(e => e.FromFactory(() =>
                {
                    var criteriaMock = MockRepository.GenerateMock<ICriteria>();
                    criteriaMock.Expect(m => m.AsSql())
                        .Return(fixture.CreateAnonymous<string>())
                        .Repeat.Any();
                    return criteriaMock;
                }));

            var filter = new Filter();
            Assert.That(filter, Is.Not.Null);
            Assert.That(filter.Criterias, Is.Not.Null);
            Assert.That(filter.Criterias.Count, Is.EqualTo(0));

            filter.AddCriteria(fixture.CreateAnonymous<ICriteria>());
            filter.AddCriteria(fixture.CreateAnonymous<ICriteria>());
            filter.AddCriteria(fixture.CreateAnonymous<ICriteria>());
            Assert.That(filter.Criterias, Is.Not.Null);
            Assert.That(filter.Criterias.Count, Is.EqualTo(3));

            var asSql = filter.AsSql();
            Assert.That(asSql, Is.Not.Null);
            Assert.That(asSql, Is.Not.Empty);
            Assert.That(asSql, Is.EqualTo(string.Format("({0}) AND ({1}) AND ({2})", filter.Criterias.ElementAt(0).AsSql(), filter.Criterias.ElementAt(1).AsSql(), filter.Criterias.ElementAt(2).AsSql())));

            filter.Criterias.ElementAt(0).AssertWasCalled(m => m.AsSql());
            filter.Criterias.ElementAt(1).AssertWasCalled(m => m.AsSql());
            filter.Criterias.ElementAt(2).AssertWasCalled(m => m.AsSql());
        }

        /// <summary>
        /// Test that Exclude throws an ArgumentNullException if the data objects for the record is null.
        /// </summary>
        [Test]
        public void TestThatExcludeThrowsArgumentNullExceptionIfRecordDataObjectsIsNull()
        {
            var filter = new Filter();
            Assert.That(filter, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => filter.Exclude((IEnumerable<IDataObjectBase>) null));
        }

        /// <summary>
        /// Test that Exclude returns False if the data objects for the record matches the criteria.
        /// </summary>
        [Test]
        public void TestThatExcludeReturnsFalseIfRecordDataObjectsMatchesTheCriteria()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
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

            var filter = new Filter();
            Assert.That(filter, Is.Not.Null);
            Assert.That(filter.Criterias, Is.Not.Null);
            Assert.That(filter.Criterias.Count, Is.EqualTo(0));

            var fields = fixture.CreateMany<IField>(5).ToList();

            var fieldCriteriaMock = MockRepository.GenerateMock<IFieldCriteria>();
            fieldCriteriaMock.Expect(m => m.Field)
                .Return(fields.ElementAt(1))
                .Repeat.Any();
            fieldCriteriaMock.Expect(m => m.Exclude(Arg<object>.Is.NotNull))
                .Return(false)
                .Repeat.Any();
            filter.AddCriteria(fieldCriteriaMock);
            Assert.That(filter.Criterias, Is.Not.Null);
            Assert.That(filter.Criterias.Count, Is.EqualTo(1));

            var dataObjects = new List<IDataObjectBase>(fields.Count);
            foreach (var fieldMock in fields)
            {
                var dataObjectMock = MockRepository.GenerateMock<IDataObjectBase>();
                dataObjectMock.Expect(m => m.Field)
                    .Return(fieldMock)
                    .Repeat.Any();
                dataObjectMock.Expect(m => m.GetSourceValue<string>())
                    .Return(fixture.CreateAnonymous<string>())
                    .Repeat.Any();
                dataObjects.Add(dataObjectMock);
            }

            Assert.That(filter.Exclude(dataObjects), Is.False);

            fieldCriteriaMock.AssertWasCalled(m => m.Field);
            fieldCriteriaMock.AssertWasCalled(m => m.Exclude(Arg<object>.Is.NotNull));
        }

        /// <summary>
        /// Test that Exclude returns True if the data objects for the record does not matches the criteria.
        /// </summary>
        [Test]
        public void TestThatExcludeReturnsTrueIfRecordDataObjectsDoesNotMatchesTheCriteria()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof(object)));
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

            var filter = new Filter();
            Assert.That(filter, Is.Not.Null);
            Assert.That(filter.Criterias, Is.Not.Null);
            Assert.That(filter.Criterias.Count, Is.EqualTo(0));

            var fields = fixture.CreateMany<IField>(5).ToList();

            var fieldCriteriaMock = MockRepository.GenerateMock<IFieldCriteria>();
            fieldCriteriaMock.Expect(m => m.Field)
                .Return(fields.ElementAt(1))
                .Repeat.Any();
            fieldCriteriaMock.Expect(m => m.Exclude(Arg<object>.Is.NotNull))
                .Return(true)
                .Repeat.Any();
            filter.AddCriteria(fieldCriteriaMock);
            Assert.That(filter.Criterias, Is.Not.Null);
            Assert.That(filter.Criterias.Count, Is.EqualTo(1));

            var dataObjects = new List<IDataObjectBase>(fields.Count);
            foreach (var fieldMock in fields)
            {
                var dataObjectMock = MockRepository.GenerateMock<IDataObjectBase>();
                dataObjectMock.Expect(m => m.Field)
                    .Return(fieldMock)
                    .Repeat.Any();
                dataObjectMock.Expect(m => m.GetSourceValue<string>())
                    .Return(fixture.CreateAnonymous<string>())
                    .Repeat.Any();
                dataObjects.Add(dataObjectMock);
            }

            Assert.That(filter.Exclude(dataObjects), Is.True);

            fieldCriteriaMock.AssertWasCalled(m => m.Field);
            fieldCriteriaMock.AssertWasCalled(m => m.Exclude(Arg<object>.Is.NotNull));
        }

        /// <summary>
        /// Test that Exclude throws an ArgumentNullException if the field is null.
        /// </summary>
        [Test]
        public void TestThatExcludeThrowsArgumentNullExceptionIfFieldIsNull()
        {
            var filter = new Filter();
            Assert.That(filter, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => filter.Exclude((IField) null));
        }

        /// <summary>
        /// Test that Exclude returns False if the field matches the criteria.
        /// </summary>
        [Test]
        public void TestThatExcludeReturnsFalseIfFieldMatchesTheCriteria()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof(object)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameSource)
                        .Return(fixture.CreateAnonymous<string>())
                        .Repeat.Any();
                    return fieldMock;
                }));

            var filter = new Filter();
            Assert.That(filter, Is.Not.Null);
            Assert.That(filter.Criterias, Is.Not.Null);
            Assert.That(filter.Criterias.Count, Is.EqualTo(0));

            var excludeFieldCriteriaMock = MockRepository.GenerateMock<IExcludeFieldCriteria>();
            excludeFieldCriteriaMock.Expect(m => m.Field)
                .Return(fixture.CreateAnonymous<IField>())
                .Repeat.Any();
            excludeFieldCriteriaMock.Expect(m => m.Exclude(Arg<IField>.Is.NotNull))
                .Return(false)
                .Repeat.Any();
            filter.AddCriteria(excludeFieldCriteriaMock);
            Assert.That(filter.Criterias, Is.Not.Null);
            Assert.That(filter.Criterias.Count, Is.EqualTo(1));

            Assert.That(filter.Exclude(excludeFieldCriteriaMock.Field), Is.False);

            excludeFieldCriteriaMock.AssertWasCalled(m => m.Field);
            excludeFieldCriteriaMock.AssertWasCalled(m => m.Exclude(Arg<IField>.Is.Equal(excludeFieldCriteriaMock.Field)));
        }

        /// <summary>
        /// Test that Exclude returns True if the field does not matches the criteria.
        /// </summary>
        [Test]
        public void TestThatExcludeReturnsFalseIfFieldDoesNotMatchesTheCriteria()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof(object)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameSource)
                        .Return(fixture.CreateAnonymous<string>())
                        .Repeat.Any();
                    return fieldMock;
                }));

            var filter = new Filter();
            Assert.That(filter, Is.Not.Null);
            Assert.That(filter.Criterias, Is.Not.Null);
            Assert.That(filter.Criterias.Count, Is.EqualTo(0));

            var excludeFieldCriteriaMock = MockRepository.GenerateMock<IExcludeFieldCriteria>();
            excludeFieldCriteriaMock.Expect(m => m.Field)
                .Return(fixture.CreateAnonymous<IField>())
                .Repeat.Any();
            excludeFieldCriteriaMock.Expect(m => m.Exclude(Arg<IField>.Is.NotNull))
                .Return(true)
                .Repeat.Any();
            filter.AddCriteria(excludeFieldCriteriaMock);
            Assert.That(filter.Criterias, Is.Not.Null);
            Assert.That(filter.Criterias.Count, Is.EqualTo(1));

            Assert.That(filter.Exclude(excludeFieldCriteriaMock.Field), Is.True);

            excludeFieldCriteriaMock.AssertWasCalled(m => m.Field);
            excludeFieldCriteriaMock.AssertWasCalled(m => m.Exclude(Arg<IField>.Is.Equal(excludeFieldCriteriaMock.Field)));
        }

        /// <summary>
        /// Test that AddCriteria throws an ArgumentNullException if the criteria is null.
        /// </summary>
        [Test]
        public void TestThatAddCriteriaThrowsArgumentNullExceptionIfCriteriaIsNull()
        {
            var filter = new Filter();
            Assert.That(filter, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => filter.AddCriteria(null));
        }

        /// <summary>
        /// Test that AddCriteria adds a criteria to the filter.
        /// </summary>
        [Test]
        public void TestThatAddCriteriaAddsCriteriaToFilter()
        {
            var fixture = new Fixture();
            fixture.Customize<ICriteria>(e => e.FromFactory(() => MockRepository.GenerateMock<ICriteria>()));

            var filter = new Filter();
            Assert.That(filter, Is.Not.Null);
            Assert.That(filter.Criterias, Is.Not.Null);
            Assert.That(filter.Criterias.Count, Is.EqualTo(0));

            filter.AddCriteria(fixture.CreateAnonymous<ICriteria>());
            Assert.That(filter.Criterias, Is.Not.Null);
            Assert.That(filter.Criterias.Count, Is.EqualTo(1));
        }
    }
}
