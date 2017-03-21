using System;
using DsiNext.DeliveryEngine.Domain.Comparers;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Domain.Comparers
{
    /// <summary>
    /// Tests equality comparer for source name on named objects.
    /// </summary>
    [TestFixture]
    public class NameSourceComparerTests
    {
        /// <summary>
        /// Test that the constructor initialize the comparer.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeComparer()
        {
            var comparer = new NameSourceComparer();
            Assert.That(comparer, Is.Not.Null);
        }

        /// <summary>
        /// Test that Equals throws an ArgumentNullException if the first name object is null.
        /// </summary>
        [Test]
        public void TestThatEqualsThrowsArgumentNullExceptionIfXIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<INamedObject>(e => e.FromFactory(() => MockRepository.GenerateMock<INamedObject>()));

            var comparer = new NameSourceComparer();
            Assert.That(comparer, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => comparer.Equals(null, fixture.CreateAnonymous<INamedObject>()));
        }

        /// <summary>
        /// Test that Equals throws an ArgumentNullException if the second name object is null.
        /// </summary>
        [Test]
        public void TestThatEqualsThrowsArgumentNullExceptionIfYIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<INamedObject>(e => e.FromFactory(() => MockRepository.GenerateMock<INamedObject>()));

            var comparer = new NameSourceComparer();
            Assert.That(comparer, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => comparer.Equals(fixture.CreateAnonymous<INamedObject>(), null));
        }

        /// <summary>
        /// Test that Equals returns true if target name on the two named object is equal.
        /// </summary>
        [Test]
        public void TestThatEqualsReturnsTrueIfTargetNameIsEqualOnNameObjects()
        {
            var fixture = new Fixture();
            fixture.Customize<INamedObject>(e => e.FromFactory(() =>
                                                                   {
                                                                       var nameObjectMock = MockRepository.GenerateMock<INamedObject>();
                                                                       nameObjectMock.Expect(m => m.NameSource)
                                                                           .Return(fixture.CreateAnonymous<string>())
                                                                           .Repeat.Any();
                                                                       return nameObjectMock;
                                                                   }));

            var comparer = new NameSourceComparer();
            Assert.That(comparer, Is.Not.Null);

            var x = fixture.CreateAnonymous<INamedObject>();
            Assert.That(comparer.Equals(x, x), Is.True);

            x.AssertWasCalled(m => m.NameSource, opt => opt.Repeat.Times(2));
        }

        /// <summary>
        /// Test that Equals returns false if target name on the two named object is not equal.
        /// </summary>
        [Test]
        public void TestThatEqualsReturnsFalseIfTargetNameIsNotEqualOnNameObjects()
        {
            var fixture = new Fixture();
            fixture.Customize<INamedObject>(e => e.FromFactory(() =>
                                                                   {
                                                                       var nameObjectMock = MockRepository.GenerateMock<INamedObject>();
                                                                       nameObjectMock.Expect(m => m.NameSource)
                                                                           .Return(fixture.CreateAnonymous<string>())
                                                                           .Repeat.Any();
                                                                       return nameObjectMock;
                                                                   }));

            var comparer = new NameSourceComparer();
            Assert.That(comparer, Is.Not.Null);

            var x = fixture.CreateAnonymous<INamedObject>();
            var y = fixture.CreateAnonymous<INamedObject>();
            Assert.That(comparer.Equals(x, y), Is.False);

            x.AssertWasCalled(m => m.NameSource, opt => opt.Repeat.Times(1));
            y.AssertWasCalled(m => m.NameSource, opt => opt.Repeat.Times(1));
        }

        /// <summary>
        /// Test that GetHashCode throws an ArgumentNullException if the name object is null.
        /// </summary>
        [Test]
        public void TestThatGetHashCodeThrowsArgumentNullExceptionIfNameObjectIsNull()
        {
            var comparer = new NameSourceComparer();
            Assert.That(comparer, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => comparer.GetHashCode(null));
        }

        /// <summary>
        /// Test that GetHashCode throws an DeliveryEngineSystemException if target name on the named object is null.
        /// </summary>
        [Test]
        public void TestThatGetHashCodeThrowsDeliveryEngineSystemExceptionIfTargetNameOnNamedObjectIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<INamedObject>(e => e.FromFactory(() =>
                                                                   {
                                                                       var nameObjectMock = MockRepository.GenerateMock<INamedObject>();
                                                                       nameObjectMock.Expect(m => m.NameSource)
                                                                           .Return(null)
                                                                           .Repeat.Any();
                                                                       return nameObjectMock;
                                                                   }));

            var comparer = new NameSourceComparer();
            Assert.That(comparer, Is.Not.Null);

            var namedObject = fixture.CreateAnonymous<INamedObject>();
            Assert.Throws<DeliveryEngineSystemException>(() => comparer.GetHashCode(namedObject));

            namedObject.AssertWasCalled(m => m.NameSource, opt => opt.Repeat.Times(2));
        }

        /// <summary>
        /// Test that GetHashCode gets the hash code.
        /// </summary>
        [Test]
        public void TestThatGetHashCodeGetsHashCode()
        {
            var fixture = new Fixture();
            fixture.Customize<INamedObject>(e => e.FromFactory(() =>
                                                                   {
                                                                       var nameObjectMock = MockRepository.GenerateMock<INamedObject>();
                                                                       nameObjectMock.Expect(m => m.NameSource)
                                                                           .Return(fixture.CreateAnonymous<string>())
                                                                           .Repeat.Any();
                                                                       return nameObjectMock;
                                                                   }));

            var comparer = new NameSourceComparer();
            Assert.That(comparer, Is.Not.Null);

            var namedObject = fixture.CreateAnonymous<INamedObject>();
            Assert.That(comparer.GetHashCode(namedObject), Is.EqualTo(namedObject.NameSource.GetHashCode()));

            namedObject.AssertWasCalled(m => m.NameSource, opt => opt.Repeat.Times(3));
        }
    }
}
