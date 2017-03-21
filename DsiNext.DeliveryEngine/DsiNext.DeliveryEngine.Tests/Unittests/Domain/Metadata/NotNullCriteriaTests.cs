using System;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Domain.Metadata;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Domain.Metadata
{
    /// <summary>
    /// Tests the not null criteria..
    /// </summary>
    [TestFixture]
    public class NotNullCriteriaTests
    {
        /// <summary>
        /// Test that the constructor initialize the not null criteria.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeCriteria()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof(object)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() => MockRepository.GenerateMock<IField>()));
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));

            var fieldMock = fixture.CreateAnonymous<Field>();
            var criteria = new NotNullCriteria(fieldMock);
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
            Assert.Throws<ArgumentNullException>(() => new NotNullCriteria(null));
        }

        /// <summary>
        /// Test that AsString returns the string criteria where the data type is string.
        /// </summary>
        [Test]
        public void TestThatAsStringReturnsStringCriteriaWhereDataTypeIsString()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (string))
                     .Repeat.Any();

            var criteria = new NotNullCriteria(fieldMock);
            Assert.That(criteria, Is.Not.Null);
            Assert.That(criteria.AsString(), Is.Not.Null);
            Assert.That(criteria.AsString(), Is.Not.Empty);
            Assert.That(criteria.AsString(), Is.EqualTo(string.Format("Equals({0}.Value, null) == false\r\nEquals({0}.Value, string.Empty) == false", fieldMock.NameSource)));
        }

        /// <summary>
        /// Test that AsString returns the string criteria where the data type is integer.
        /// </summary>
        [Test]
        public void TestThatAsStringReturnsStringCriteriaWhereDataTypeIsInt()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (int?))
                     .Repeat.Any();

            var criteria = new NotNullCriteria(fieldMock);
            Assert.That(criteria, Is.Not.Null);
            Assert.That(criteria.AsString(), Is.Not.Null);
            Assert.That(criteria.AsString(), Is.Not.Empty);
            Assert.That(criteria.AsString(), Is.EqualTo(string.Format("Equals({0}.Value, null) == false\r\nEquals({0}.Value, 0) == false", fieldMock.NameSource)));
        }

        /// <summary>
        /// Test that AsString returns the string criteria where the data type is long.
        /// </summary>
        [Test]
        public void TestThatAsStringReturnsStringCriteriaWhereDataTypeIsLong()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (long))
                     .Repeat.Any();

            var criteria = new NotNullCriteria(fieldMock);
            Assert.That(criteria, Is.Not.Null);
            Assert.That(criteria.AsString(), Is.Not.Null);
            Assert.That(criteria.AsString(), Is.Not.Empty);
            Assert.That(criteria.AsString(), Is.EqualTo(string.Format("Equals({0}.Value, null) == false\r\nEquals({0}.Value, 0) == false", fieldMock.NameSource)));
        }

        /// <summary>
        /// Test that AsString returns the string criteria where the data type is object.
        /// </summary>
        [Test]
        public void TestThatAsStringReturnsStringCriteriaWhereDataTypeIsObject()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (object))
                     .Repeat.Any();

            var criteria = new NotNullCriteria(fieldMock);
            Assert.That(criteria, Is.Not.Null);
            Assert.That(criteria.AsString(), Is.Not.Null);
            Assert.That(criteria.AsString(), Is.Not.Empty);
            Assert.That(criteria.AsString(), Is.EqualTo(string.Format("Equals({0}.Value, null) == false", fieldMock.NameSource)));
        }

        /// <summary>
        /// Test that AsSql returns the SQL criteria where the data type is string.
        /// </summary>
        [Test]
        public void TestThatAsSqlReturnsSqlCriteriaWhereDataTypeIsString()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (string))
                     .Repeat.Any();

            var criteria = new NotNullCriteria(fieldMock);
            Assert.That(criteria, Is.Not.Null);
            Assert.That(criteria.AsSql(), Is.Not.Null);
            Assert.That(criteria.AsSql(), Is.Not.Empty);
            Assert.That(criteria.AsSql(), Is.EqualTo(string.Format("{0} IS NOT NULL AND LENGTH({0})>0", fieldMock.NameSource)));
        }

        /// <summary>
        /// Test that AsSql returns the SQL criteria where the data type is integer.
        /// </summary>
        [Test]
        public void TestThatAsSqlReturnsSqlCriteriaWhereDataTypeIsInt()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (int))
                     .Repeat.Any();

            var criteria = new NotNullCriteria(fieldMock);
            Assert.That(criteria, Is.Not.Null);
            Assert.That(criteria.AsSql(), Is.Not.Null);
            Assert.That(criteria.AsSql(), Is.Not.Empty);
            Assert.That(criteria.AsSql(), Is.EqualTo(string.Format("{0} IS NOT NULL AND {0}<>0", fieldMock.NameSource)));
        }

        /// <summary>
        /// Test that AsSql returns the SQL criteria where the data type is long.
        /// </summary>
        [Test]
        public void TestThatAsSqlReturnsSqlCriteriaWhereDataTypeIsLong()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (long))
                     .Repeat.Any();

            var criteria = new NotNullCriteria(fieldMock);
            Assert.That(criteria, Is.Not.Null);
            Assert.That(criteria.AsSql(), Is.Not.Null);
            Assert.That(criteria.AsSql(), Is.Not.Empty);
            Assert.That(criteria.AsSql(), Is.EqualTo(string.Format("{0} IS NOT NULL AND {0}<>0", fieldMock.NameSource)));
        }

        /// <summary>
        /// Test that AsSql returns the SQL criteria where the data type is long.
        /// </summary>
        [Test]
        public void TestThatAsSqlReturnsSqlCriteriaWhereDataTypeIsObject()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (object))
                     .Repeat.Any();

            var criteria = new NotNullCriteria(fieldMock);
            Assert.That(criteria, Is.Not.Null);
            Assert.That(criteria.AsSql(), Is.Not.Null);
            Assert.That(criteria.AsSql(), Is.Not.Empty);
            Assert.That(criteria.AsSql(), Is.EqualTo(string.Format("{0} IS NOT NULL", fieldMock.NameSource)));
        }

        /// <summary>
        /// Test that Exclude returns true if the value is null.
        /// </summary>
        [Test]
        public void TestThatExcludeReturnsTrueIfValueIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() => MockRepository.GenerateMock<IField>()));
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));

            var fieldMock = fixture.CreateAnonymous<Field>();
            var criteria = new NotNullCriteria(fieldMock);
            Assert.That(criteria, Is.Not.Null);

            Assert.That(criteria.Exclude(null), Is.True);
        }

        /// <summary>
        /// Test that Exclude returns true if the value is empty.
        /// </summary>
        [Test]
        public void TestThatExcludeReturnsTrueIfValueIsEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (string)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() => MockRepository.GenerateMock<IField>()));
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));

            var fieldMock = fixture.CreateAnonymous<Field>();
            var criteria = new NotNullCriteria(fieldMock);
            Assert.That(criteria, Is.Not.Null);

            Assert.That(criteria.Exclude(string.Empty), Is.True);
        }

        /// <summary>
        /// Test that Exclude returns true if the value is 0.
        /// </summary>
        [Test]
        public void TestThatExcludeReturnsTrueIfValueIsZero()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (int)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() => MockRepository.GenerateMock<IField>()));
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));

            var fieldMock = fixture.CreateAnonymous<Field>();
            var criteria = new NotNullCriteria(fieldMock);
            Assert.That(criteria, Is.Not.Null);

            Assert.That(criteria.Exclude(0), Is.True);
        }

        /// <summary>
        /// Test that Exclude returns false if the value is not null.
        /// </summary>
        [Test]
        public void TestThatExcludeReturnsFalseIfValueIsNotNull()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() => MockRepository.GenerateMock<IField>()));
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));

            var fieldMock = fixture.CreateAnonymous<Field>();
            var criteria = new NotNullCriteria(fieldMock);
            Assert.That(criteria, Is.Not.Null);

            Assert.That(criteria.Exclude(fixture.CreateAnonymous<string>()), Is.False);
        }
    }
}
