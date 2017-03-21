using DsiNext.DeliveryEngine.Domain.Metadata;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Domain.Metadata
{
    /// <summary>
    /// Tests the exclusion criteria.
    /// </summary>
    [TestFixture]
    public class ExcludeCriteriaTests
    {
        /// <summary>
        /// Test that the constructor initialize the exclusion criteria.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeCriteria()
        {
            var criteria = new ExcludeCriteria();
            Assert.That(criteria, Is.Not.Null);
        }

        /// <summary>
        /// Test that AsString returns the string representation of the exclusion criteria.
        /// </summary>
        [Test]
        public void TestThatAsStringReturnsStringCriteria()
        {
            var criteria = new ExcludeCriteria();
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
            var criteria = new ExcludeCriteria();
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
            var criteria = new ExcludeCriteria();
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

            var criteria = new ExcludeCriteria();
            Assert.That(criteria, Is.Not.Null);

            Assert.That(criteria.Exclude(fixture.CreateAnonymous<object>()), Is.True);
        }
    }
}
