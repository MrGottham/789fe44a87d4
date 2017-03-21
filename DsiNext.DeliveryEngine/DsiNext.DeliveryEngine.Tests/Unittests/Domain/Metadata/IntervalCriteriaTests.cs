using System;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Domain.Metadata;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Domain.Metadata
{
    /// <summary>
    /// Tests the interval criteria.
    /// </summary>
    [TestFixture]
    public class IntervalCriteriaTests
    {
        /// <summary>
        /// Test that the constructor initialize the interval criteria.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeCriteria()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof(object)));
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));
            fixture.Customize<IField>(e => e.FromFactory(() => MockRepository.GenerateMock<IField>()));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));

            var fieldMock = fixture.CreateAnonymous<Field>();
            var criteria = new IntervalCriteria<string>(fieldMock, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(criteria, Is.Not.Null);
            Assert.That(criteria.Field, Is.Not.Null);
            Assert.That(criteria.Field, Is.EqualTo(fieldMock));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the field is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfFieldIsNull()
        {
            var fixture = new Fixture();

            Assert.Throws<ArgumentNullException>(() => new IntervalCriteria<string>(null, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the from value is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfFromValueIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof(object)));
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));
            fixture.Customize<IField>(e => e.FromFactory(() => MockRepository.GenerateMock<IField>()));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));

            Assert.Throws<ArgumentNullException>(() => new IntervalCriteria<string>(fixture.CreateAnonymous<IField>(), null, fixture.CreateAnonymous<string>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the to value is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfToValueIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof(object)));
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));
            fixture.Customize<IField>(e => e.FromFactory(() => MockRepository.GenerateMock<IField>()));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));

            Assert.Throws<ArgumentNullException>(() => new IntervalCriteria<string>(fixture.CreateAnonymous<IField>(), fixture.CreateAnonymous<string>(), null));
        }

        /// <summary>
        /// Test that AsString returns the string criteria where the from and to values are string.
        /// </summary>
        [Test]
        public void TestThatAsStringReturnsStringCriteriaForStringValues()
        {
            var fixture = new Fixture();
            var fromValue = fixture.CreateAnonymous<string>();
            var toValue = fixture.CreateAnonymous<string>();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();

            var criteria = new IntervalCriteria<string>(fieldMock, fromValue, toValue);
            Assert.That(criteria, Is.Not.Null);

            var stringCriteria = criteria.AsString();
            Assert.That(stringCriteria, Is.Not.Null);
            Assert.That(stringCriteria, Is.Not.Empty);

            Assert.That(stringCriteria, Is.EqualTo(string.Format("{0}.Value.CompareTo(\"{1}\") >= 0\r\n{0}.Value.CompareTo(\"{2}\") <= 0", fieldMock.NameSource, fromValue, toValue)));
        }

        /// <summary>
        /// Test that AsString returns the string criteria where the from and to values are integer.
        /// </summary>
        [Test]
        public void TestThatAsStringReturnsStringCriteriaForIntegerValues()
        {
            var fixture = new Fixture();
            var fromValue = fixture.CreateAnonymous<int>();
            var toValue = fixture.CreateAnonymous<int>();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();

            var criteria = new IntervalCriteria<int>(fieldMock, fromValue, toValue);
            Assert.That(criteria, Is.Not.Null);

            var stringCriteria = criteria.AsString();
            Assert.That(stringCriteria, Is.Not.Null);
            Assert.That(stringCriteria, Is.Not.Empty);

            Assert.That(stringCriteria, Is.EqualTo(string.Format("{0}.Value.CompareTo({1}) >= 0\r\n{0}.Value.CompareTo({2}) <= 0", fieldMock.NameSource, fromValue, toValue)));
        }

        /// <summary>
        /// Test that AsSql returns the sql criteria where the from and to values are string.
        /// </summary>
        [Test]
        public void TestThatAsSqlReturnsSqlCriteriaForStringValues()
        {
            var fixture = new Fixture();
            var fromValue = fixture.CreateAnonymous<string>();
            var toValue = fixture.CreateAnonymous<string>();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();

            var criteria = new IntervalCriteria<string>(fieldMock, fromValue, toValue);
            Assert.That(criteria, Is.Not.Null);

            var stringCriteria = criteria.AsSql();
            Assert.That(stringCriteria, Is.Not.Null);
            Assert.That(stringCriteria, Is.Not.Empty);

            Assert.That(stringCriteria, Is.EqualTo(string.Format("{0} BETWEEN '{1}' AND '{2}'", fieldMock.NameSource, fromValue, toValue)));
        }

        /// <summary>
        /// Test that AsSql returns the sql criteria where the from and to values are integer.
        /// </summary>
        [Test]
        public void TestThatAsSqlReturnsSqlCriteriaForIntegerValues()
        {
            var fixture = new Fixture();
            var fromValue = fixture.CreateAnonymous<int>();
            var toValue = fixture.CreateAnonymous<int>();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();

            var criteria = new IntervalCriteria<int>(fieldMock, fromValue, toValue);
            Assert.That(criteria, Is.Not.Null);

            var stringCriteria = criteria.AsSql();
            Assert.That(stringCriteria, Is.Not.Null);
            Assert.That(stringCriteria, Is.Not.Empty);

            Assert.That(stringCriteria, Is.EqualTo(string.Format("{0} BETWEEN {1} AND {2}", fieldMock.NameSource, fromValue, toValue)));
        }

        /// <summary>
        /// Test that Exclude returns false if the string value is between from and to values.
        /// </summary>
        [Test]
        public void TestThatExcludeReturnsFalseIfStringValueIsBetweenValues()
        {
            var fixture = new Fixture();
            var value = fixture.CreateAnonymous<string>();
            var fromValue = string.Format("{0}0", value);
            var toValue = string.Format("{0}9", value);

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();

            var criteria = new IntervalCriteria<string>(fieldMock, fromValue, toValue);
            Assert.That(criteria, Is.Not.Null);

            Assert.That(criteria.Exclude(string.Format("{0}5", value)), Is.False);
        }

        /// <summary>
        /// Test that Exclude returns true if the string value is not between from and to values.
        /// </summary>
        [Test]
        public void TestThatExcludeReturnsTrueIfStringValueIsNotBetweenValues()
        {
            var fixture = new Fixture();
            var value = fixture.CreateAnonymous<string>();
            var fromValue = string.Format("{0}0", value);
            var toValue = string.Format("{0}9", value);

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();

            var criteria = new IntervalCriteria<string>(fieldMock, fromValue, toValue);
            Assert.That(criteria, Is.Not.Null);

            Assert.That(criteria.Exclude(string.Format("{0}A", value)), Is.True);
        }

        /// <summary>
        /// Test that Exclude returns false if the integer value is between from and to values.
        /// </summary>
        [Test]
        public void TestThatExcludeReturnsFalseIfIntegerValueIsBetweenValues()
        {
            var fixture = new Fixture();
            const int fromValue = 0;
            const int toValue = 9;

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();

            var criteria = new IntervalCriteria<int>(fieldMock, fromValue, toValue);
            Assert.That(criteria, Is.Not.Null);

            Assert.That(criteria.Exclude(5), Is.False);
        }

        /// <summary>
        /// Test that Exclude returns true if the integer value is not between from and to values.
        /// </summary>
        [Test]
        public void TestThatExcludeReturnsTrueIfIntegerValueIsNotBetweenValues()
        {
            var fixture = new Fixture();
            const int fromValue = 0;
            const int toValue = 9;

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();

            var criteria = new IntervalCriteria<int>(fieldMock, fromValue, toValue);
            Assert.That(criteria, Is.Not.Null);

            Assert.That(criteria.Exclude(10), Is.True);
        }
    }
}
