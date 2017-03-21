using System;
using DsiNext.DeliveryEngine.Domain.Metadata;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Domain.Metadata
{
    /// <summary>
    /// Tests creator of the archive.
    /// </summary>
    [TestFixture]
    public class CreatorTests
    {
        /// <summary>
        /// Test that the constructor initialize a creator without a description.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeCreatorWithoutDescription()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var nameSource = fixture.CreateAnonymous<string>();
            var nameTarget = fixture.CreateAnonymous<string>();
            var periodStart = fixture.CreateAnonymous<DateTime>();
            var periodEnd = fixture.CreateAnonymous<DateTime>().AddDays(7);
            var creator = new Creator(nameSource, nameTarget, periodStart, periodEnd);
            Assert.That(creator, Is.Not.Null);
            Assert.That(creator.NameSource, Is.Not.Null);
            Assert.That(creator.NameSource, Is.Not.Empty);
            Assert.That(creator.NameSource, Is.EqualTo(nameSource));
            Assert.That(creator.NameTarget, Is.Not.Null);
            Assert.That(creator.NameTarget, Is.Not.Empty);
            Assert.That(creator.NameTarget, Is.EqualTo(nameTarget));
            Assert.That(creator.Description, Is.Null);
            Assert.That(creator.PeriodStart, Is.EqualTo(periodStart));
            Assert.That(creator.PeriodEnd, Is.EqualTo(periodEnd));
        }

        /// <summary>
        /// Test that the constructor initialize a creator with a description.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeCreatorWithDescription()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var nameSource = fixture.CreateAnonymous<string>();
            var nameTarget = fixture.CreateAnonymous<string>();
            var description = fixture.CreateAnonymous<string>();
            var periodStart = fixture.CreateAnonymous<DateTime>();
            var periodEnd = fixture.CreateAnonymous<DateTime>().AddDays(7);
            var creator = new Creator(nameSource, nameTarget, description, periodStart, periodEnd);
            Assert.That(creator, Is.Not.Null);
            Assert.That(creator.NameSource, Is.Not.Null);
            Assert.That(creator.NameSource, Is.Not.Empty);
            Assert.That(creator.NameSource, Is.EqualTo(nameSource));
            Assert.That(creator.NameTarget, Is.Not.Null);
            Assert.That(creator.NameTarget, Is.Not.Empty);
            Assert.That(creator.NameTarget, Is.EqualTo(nameTarget));
            Assert.That(creator.Description, Is.Not.Null);
            Assert.That(creator.Description, Is.Not.Empty);
            Assert.That(creator.Description, Is.EqualTo(description));
            Assert.That(creator.PeriodStart, Is.EqualTo(periodStart));
            Assert.That(creator.PeriodEnd, Is.EqualTo(periodEnd));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if name of the creator in the source repository is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameSourceIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));
            Assert.Throws<ArgumentNullException>(() => new Creator(null, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7)));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if name of the creator in the source repository is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameSourceIsEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));
            Assert.Throws<ArgumentNullException>(() => new Creator(string.Empty, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7)));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if name of the creator in the target repository is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameTargetIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));
            Assert.Throws<ArgumentNullException>(() => new Creator(fixture.CreateAnonymous<string>(), null, fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7)));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if name of the creator in the target repository is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameTargetIsEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));
            Assert.Throws<ArgumentNullException>(() => new Creator(fixture.CreateAnonymous<string>(), string.Empty, fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7)));
        }

        /// <summary>
        /// Test that the setter of PeriodStart changed the value.
        /// </summary>
        [Test]
        public void TestThatPeriodStartSetterChangedValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var creator = new Creator(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(creator, Is.Not.Null);

            var newValue = fixture.CreateAnonymous<DateTime>().AddDays(1);
            creator.PeriodStart = newValue;

            Assert.That(creator.PeriodStart, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of PeriodStart raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatPeriodStartSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var creator = new Creator(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(creator, Is.Not.Null);

            var eventCalled = false;
            creator.PropertyChanged += (s, e) =>
                                           {
                                               Assert.That(s, Is.Not.Null);
                                               Assert.That(e, Is.Not.Null);
                                               Assert.That(e.PropertyName, Is.Not.Null);
                                               Assert.That(e.PropertyName, Is.Not.Empty);
                                               Assert.That(e.PropertyName, Is.EqualTo("PeriodStart"));
                                               eventCalled = true;
                                           };

            creator.PeriodStart = creator.PeriodStart;
            Assert.That(eventCalled, Is.False);

            creator.PeriodStart = creator.PeriodStart.AddDays(1);
            Assert.That(eventCalled, Is.True);

        }

        /// <summary>
        /// Test that the setter of PeriodEnd changed the value.
        /// </summary>
        [Test]
        public void TestThatPeriodEndSetterChangedValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var creator = new Creator(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(creator, Is.Not.Null);

            var newValue = fixture.CreateAnonymous<DateTime>().AddDays(1);
            creator.PeriodEnd = newValue;

            Assert.That(creator.PeriodEnd, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of PeriodEnd raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatPeriodEndSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));

            var creator = new Creator(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<DateTime>(), fixture.CreateAnonymous<DateTime>().AddDays(7));
            Assert.That(creator, Is.Not.Null);

            var eventCalled = false;
            creator.PropertyChanged += (s, e) =>
                                           {
                                               Assert.That(s, Is.Not.Null);
                                               Assert.That(e, Is.Not.Null);
                                               Assert.That(e.PropertyName, Is.Not.Null);
                                               Assert.That(e.PropertyName, Is.Not.Empty);
                                               Assert.That(e.PropertyName, Is.EqualTo("PeriodEnd"));
                                               eventCalled = true;
                                           };

            creator.PeriodEnd = creator.PeriodEnd;
            Assert.That(eventCalled, Is.False);

            creator.PeriodEnd = creator.PeriodEnd.AddDays(1);
            Assert.That(eventCalled, Is.True);
        }
    }
}
