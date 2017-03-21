using System;
using System.Linq;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Domain.Metadata;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Domain.Metadata
{
    /// <summary>
    /// Tests the pool criteria.
    /// </summary>
    [TestFixture]
    public class PoolCriteriaTests
    {
        /// <summary>
        /// Test that the constructor initialize the pool criteria.
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
            var criteria = new PoolCriteria<object>(fieldMock, fixture.CreateMany<object>(5).ToList());
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

            Assert.Throws<ArgumentNullException>(() => new PoolCriteria<object>(null, fixture.CreateMany<object>(5).ToList()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the pool values is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfPoolValuesIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof(object)));
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));
            fixture.Customize<IField>(e => e.FromFactory(() => MockRepository.GenerateMock<IField>()));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));

            Assert.Throws<ArgumentNullException>(() => new PoolCriteria<object>(fixture.CreateAnonymous<IField>(), null));
        }

        /// <summary>
        /// Test that AsString returns the string criteria where the values are strings.
        /// </summary>
        [Test]
        public void TestThatAsStringReturnsStringCriteriaForStringValues()
        {
            var fixture = new Fixture();
            var poolValues = fixture.CreateMany<string>(3).ToList();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();

            var criteria = new PoolCriteria<string>(fieldMock, poolValues);
            Assert.That(criteria, Is.Not.Null);

            var stringCriteria = criteria.AsString();
            Assert.That(stringCriteria, Is.Not.Null);
            Assert.That(stringCriteria, Is.Not.Empty);
            Assert.That(stringCriteria, Is.EqualTo(string.Format("Regex.IsMatch({0}.Value, \"^{1}|{2}|{3}$\")", fieldMock.NameSource, poolValues.ElementAt(0), poolValues.ElementAt(1), poolValues.ElementAt(2))));
        }

        /// <summary>
        /// Test that AsString returns the string criteria where the values are integers.
        /// </summary>
        [Test]
        public void TestThatAsStringReturnsStringCriteriaForIntegerValues()
        {
            var fixture = new Fixture();
            var poolValues = fixture.CreateMany<int>(3).ToList();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();

            var criteria = new PoolCriteria<int>(fieldMock, poolValues);
            Assert.That(criteria, Is.Not.Null);

            var stringCriteria = criteria.AsString();
            Assert.That(stringCriteria, Is.Not.Null);
            Assert.That(stringCriteria, Is.Not.Empty);
            Assert.That(stringCriteria, Is.EqualTo(string.Format("Regex.IsMatch({0}.Value.ToString(), \"^{1}|{2}|{3}$\")", fieldMock.NameSource, poolValues.ElementAt(0), poolValues.ElementAt(1), poolValues.ElementAt(2))));
        }

        /// <summary>
        /// Test that AsSql returns the sql criteria where the values are strings.
        /// </summary>
        [Test]
        public void TestThatAsSqlReturnsSqlCriteriaForStringValues()
        {
            var fixture = new Fixture();
            var poolValues = fixture.CreateMany<string>(3).ToList();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();

            var criteria = new PoolCriteria<string>(fieldMock, poolValues);
            Assert.That(criteria, Is.Not.Null);

            var sqlCriteria = criteria.AsSql();
            Assert.That(sqlCriteria, Is.Not.Null);
            Assert.That(sqlCriteria, Is.Not.Empty);
            Assert.That(sqlCriteria, Is.EqualTo(string.Format("{0} IN ('{1}', '{2}', '{3}')", fieldMock.NameSource, poolValues.ElementAt(0), poolValues.ElementAt(1), poolValues.ElementAt(2))));
        }

        /// <summary>
        /// Test that AsSql returns the sql criteria where the values are integer.
        /// </summary>
        [Test]
        public void TestThatAsSqlReturnsSqlCriteriaForIntegerValues()
        {
            var fixture = new Fixture();
            var poolValues = fixture.CreateMany<int>(3).ToList();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameSource)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();

            var criteria = new PoolCriteria<int>(fieldMock, poolValues);
            Assert.That(criteria, Is.Not.Null);

            var sqlCriteria = criteria.AsSql();
            Assert.That(sqlCriteria, Is.Not.Null);
            Assert.That(sqlCriteria, Is.Not.Empty);
            Assert.That(sqlCriteria, Is.EqualTo(string.Format("{0} IN ({1}, {2}, {3})", fieldMock.NameSource, poolValues.ElementAt(0), poolValues.ElementAt(1), poolValues.ElementAt(2))));
        }

        /// <summary>
        /// Test that Exclude returns false if the string value is in the pool.
        /// </summary>
        [Test]
        public void TestThatExcludeReturnsFalseIfStringValueIsInThePool()
        {
            var fixture = new Fixture();
            var poolValues = fixture.CreateMany<string>(3).ToList();

            var fieldMock = MockRepository.GenerateMock<IField>();

            var criteria = new PoolCriteria<string>(fieldMock, poolValues);
            Assert.That(criteria, Is.Not.Null);

            Assert.That(criteria.Exclude(poolValues.ElementAt(1)), Is.False);
        }

        /// <summary>
        /// Test that Exclude returns true if the string value is not in the pool.
        /// </summary>
        [Test]
        public void TestThatExcludeReturnsTrueIfStringValueIsNotInThePool()
        {
            var fixture = new Fixture();
            var poolValues = fixture.CreateMany<string>(3).ToList();

            var fieldMock = MockRepository.GenerateMock<IField>();

            var criteria = new PoolCriteria<string>(fieldMock, poolValues);
            Assert.That(criteria, Is.Not.Null);

            Assert.That(criteria.Exclude(fixture.CreateAnonymous<string>()), Is.True);
        }

        /// <summary>
        /// Test that Exclude returns false if the integer value is in the pool.
        /// </summary>
        [Test]
        public void TestThatExcludeReturnsFalseIfIntegerValueIsInThePool()
        {
            var fixture = new Fixture();
            var poolValues = fixture.CreateMany<int>(3).ToList();

            var fieldMock = MockRepository.GenerateMock<IField>();

            var criteria = new PoolCriteria<int>(fieldMock, poolValues);
            Assert.That(criteria, Is.Not.Null);

            Assert.That(criteria.Exclude(poolValues.ElementAt(1)), Is.False);
        }

        /// <summary>
        /// Test that Exclude returns true if the integer value is not in the pool.
        /// </summary>
        [Test]
        public void TestThatExcludeReturnsTrueIfIntegerValueIsNotInThePool()
        {
            var fixture = new Fixture();
            var poolValues = fixture.CreateMany<int>(3).ToList();

            var fieldMock = MockRepository.GenerateMock<IField>();

            var criteria = new PoolCriteria<int>(fieldMock, poolValues);
            Assert.That(criteria, Is.Not.Null);

            Assert.That(criteria.Exclude(fixture.CreateAnonymous<int>()), Is.True);
        }
    }
}
