using System;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Domain.Metadata;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Domain.Metadata
{
    /// <summary>
    /// Tests the equal criteria.
    /// </summary>
    [TestFixture]
    public class EqualCriteriaTests
    {
        /// <summary>
        /// Test that the constructor initialize the equal criteria.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeCriteria()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));
            fixture.Customize<IField>(e => e.FromFactory(() => MockRepository.GenerateMock<IField>()));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));

            var fieldMock = fixture.CreateAnonymous<Field>();
            var criteria = new EqualCriteria<object>(fieldMock, fixture.CreateAnonymous<object>());
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

            Assert.Throws<ArgumentNullException>(() => new EqualCriteria<object>(null, fixture.CreateAnonymous<object>()));
        }

        /// <summary>
        /// Test that the constructor returns the string criteria where value is null.
        /// </summary>
        [Test]
        public void TestThatAsStringReturnsStringCriteriaWhereValueIsNull()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();

            var criteria = new EqualCriteria<string>(fieldMock, null);
            Assert.That(criteria, Is.Not.Null);

            var stringCriteria = criteria.AsString();
            Assert.That(stringCriteria, Is.Not.Null);
            Assert.That(stringCriteria, Is.Not.Empty);
            Assert.That(stringCriteria, Is.EqualTo(string.Format("{0}.Value == null", fieldMock.NameSource)));
        }

        /// <summary>
        /// Test that the constructor returns the string criteria where value is a string.
        /// </summary>
        [Test]
        public void TestThatAsStringReturnsStringCriteriaWhereValueIsString()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();

            var criteria = new EqualCriteria<string>(fieldMock, "Xyz");
            Assert.That(criteria, Is.Not.Null);

            var stringCriteria = criteria.AsString();
            Assert.That(stringCriteria, Is.Not.Null);
            Assert.That(stringCriteria, Is.Not.Empty);
            Assert.That(stringCriteria, Is.EqualTo(string.Format("Regex.IsMatch({0}.Value, \"^Xyz$\")", fieldMock.NameSource)));
        }

        /// <summary>
        /// Test that the constructor returns the string criteria where value is an integer.
        /// </summary>
        [Test]
        public void TestThatAsStringReturnsStringCriteriaWhereValueIsInteger()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();

            var criteria = new EqualCriteria<int>(fieldMock, 24);
            Assert.That(criteria, Is.Not.Null);

            var stringCriteria = criteria.AsString();
            Assert.That(stringCriteria, Is.Not.Null);
            Assert.That(stringCriteria, Is.Not.Empty);
            Assert.That(stringCriteria, Is.EqualTo(string.Format("Regex.IsMatch({0}.Value.ToString(), \"^24$\")", fieldMock.NameSource)));
        }

        /// <summary>
        /// Test that the constructor returns the SQL criteria where value is null.
        /// </summary>
        [Test]
        public void TestThatAsSqlReturnsSqlCriteriaWhereValueIsNull()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();

            var criteria = new EqualCriteria<string>(fieldMock, null);
            Assert.That(criteria, Is.Not.Null);

            var sqlCriteria = criteria.AsSql();
            Assert.That(sqlCriteria, Is.Not.Null);
            Assert.That(sqlCriteria, Is.Not.Empty);
            Assert.That(sqlCriteria, Is.EqualTo(string.Format("{0} IS NULL", fieldMock.NameSource)));
        }

        /// <summary>
        /// Test that the constructor returns the SQL criteria where value is a string.
        /// </summary>
        [Test]
        public void TestThatAsSqlReturnsSqlCriteriaWhereValueIsString()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();

            var criteria = new EqualCriteria<string>(fieldMock, "Xyz");
            Assert.That(criteria, Is.Not.Null);

            var sqlCriteria = criteria.AsSql();
            Assert.That(sqlCriteria, Is.Not.Null);
            Assert.That(sqlCriteria, Is.Not.Empty);
            Assert.That(sqlCriteria, Is.EqualTo(string.Format("{0}='Xyz'", fieldMock.NameSource)));
        }

        /// <summary>
        /// Test that the constructor returns the SQL criteria where value is an integer.
        /// </summary>
        [Test]
        public void TestThatAsSqlReturnsSqlCriteriaWhereValueIsInteger()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();

            var criteria = new EqualCriteria<int>(fieldMock, 24);
            Assert.That(criteria, Is.Not.Null);

            var sqlCriteria = criteria.AsSql();
            Assert.That(sqlCriteria, Is.Not.Null);
            Assert.That(sqlCriteria, Is.Not.Empty);
            Assert.That(sqlCriteria, Is.EqualTo(string.Format("{0}=24", fieldMock.NameSource)));
        }

        /// <summary>
        /// Test that Exclude returns false if the value equals null.
        /// </summary>
        [Test]
        public void TestThatExcludeReturnsFalseIfValueEqualsNull()
        {
            var fieldMock = MockRepository.GenerateMock<IField>();

            var criteria = new EqualCriteria<object>(fieldMock, null);
            Assert.That(criteria, Is.Not.Null);

            Assert.That(criteria.Exclude(null), Is.False);
        }

        /// <summary>
        /// Test that Exclude returns true if the value don't equals null.
        /// </summary>
        [Test]
        public void TestThatExcludeReturnsTrueIfValueDoesNotEqualsNull()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();

            var criteria = new EqualCriteria<object>(fieldMock, null);
            Assert.That(criteria, Is.Not.Null);

            Assert.That(criteria.Exclude(fixture.CreateAnonymous<object>()), Is.True);
        }

        /// <summary>
        /// Test that Exclude returns false if the value equals a string value.
        /// </summary>
        [Test]
        public void TestThatExcludeReturnsFalseIfValueEqualsStringValue()
        {
            var fieldMock = MockRepository.GenerateMock<IField>();

            var criteria = new EqualCriteria<string>(fieldMock, "Xyz");
            Assert.That(criteria, Is.Not.Null);

            Assert.That(criteria.Exclude("Xyz"), Is.False);
        }

        /// <summary>
        /// Test that Exclude returns true if the value don't equals a string value.
        /// </summary>
        [Test]
        public void TestThatExcludeReturnsTrueIfValueDoesNotEqualsStringValue()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();

            var criteria = new EqualCriteria<string>(fieldMock, "Xyz");
            Assert.That(criteria, Is.Not.Null);

            Assert.That(criteria.Exclude(fixture.CreateAnonymous<string>()), Is.True);
        }

        /// <summary>
        /// Test that Exclude returns false if the value equals a integer value.
        /// </summary>
        [Test]
        public void TestThatExcludeReturnsFalseIfValueEqualsIntegerValue()
        {
            var fieldMock = MockRepository.GenerateMock<IField>();

            var criteria = new EqualCriteria<int>(fieldMock, 24);
            Assert.That(criteria, Is.Not.Null);

            Assert.That(criteria.Exclude(24), Is.False);
        }

        /// <summary>
        /// Test that Exclude returns true if the value equals a integer value.
        /// </summary>
        [Test]
        public void TestThatExcludeReturnsTrueIfValueDoesNotEqualsIntegerValue()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();

            var criteria = new EqualCriteria<int>(fieldMock, 24);
            Assert.That(criteria, Is.Not.Null);

            Assert.That(criteria.Exclude(fixture.CreateAnonymous<int>()), Is.True);
        }
    }
}
