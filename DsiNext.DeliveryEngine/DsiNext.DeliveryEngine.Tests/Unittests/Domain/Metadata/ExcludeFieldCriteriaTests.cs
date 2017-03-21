using System;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Domain.Metadata;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Domain.Metadata
{
    /// <summary>
    /// Tests the exclusion criteria on a field.
    /// </summary>
    [TestFixture]
    public class ExcludeFieldCriteriaTests
    {
        /// <summary>
        /// Test that the constructor initialize the exclusion criteria.
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
            var criteria = new ExcludeFieldCriteria(fieldMock);
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
            Assert.Throws<ArgumentNullException>(() => new ExcludeFieldCriteria(null));
        }

        /// <summary>
        /// Test that AsString returns the string representation of the exclusion criteria.
        /// </summary>
        [Test]
        public void TestThatAsStringReturnsStringCriteria()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof(object)));
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));
            fixture.Customize<IField>(e => e.FromFactory(() => MockRepository.GenerateMock<IField>()));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));

            var criteria = new ExcludeFieldCriteria(fixture.CreateAnonymous<IField>());
            Assert.That(criteria, Is.Not.Null);

            var stringCriteria = criteria.AsString();
            Assert.That(stringCriteria, Is.Not.Null);
            Assert.That(stringCriteria, Is.Empty);
        }

        /// <summary>
        /// Test that AsSql returns the SQL representation of the exclusion criteria.
        /// </summary>
        [Test]
        public void TestThatAsSqlReturnsStringCriteria()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof(object)));
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));
            fixture.Customize<IField>(e => e.FromFactory(() => MockRepository.GenerateMock<IField>()));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));

            var criteria = new ExcludeFieldCriteria(fixture.CreateAnonymous<IField>());
            Assert.That(criteria, Is.Not.Null);

            var sqlCriteria = criteria.AsSql();
            Assert.That(sqlCriteria, Is.Not.Null);
            Assert.That(sqlCriteria, Is.Empty);
        }

        /// <summary>
        /// Test that Exclude returns true if the value is null.
        /// </summary>
        [Test]
        public void TestThatExcludeReturnTrueIfValueIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof(object)));
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));
            fixture.Customize<IField>(e => e.FromFactory(() => MockRepository.GenerateMock<IField>()));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));

            var criteria = new ExcludeFieldCriteria(fixture.CreateAnonymous<IField>());
            Assert.That(criteria, Is.Not.Null);

            Assert.That(criteria.Exclude(null), Is.True);
        }

        /// <summary>
        /// Test that Exclude returns true if the value is not null.
        /// </summary>
        [Test]
        public void TestThatExcludeReturnTrueIfValueIsNotNull()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof(object)));
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));
            fixture.Customize<IField>(e => e.FromFactory(() => MockRepository.GenerateMock<IField>()));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));

            var criteria = new ExcludeFieldCriteria(fixture.CreateAnonymous<IField>());
            Assert.That(criteria, Is.Not.Null);

            Assert.That(criteria.Exclude(fixture.CreateAnonymous<object>()), Is.True);
        }
    }
}
