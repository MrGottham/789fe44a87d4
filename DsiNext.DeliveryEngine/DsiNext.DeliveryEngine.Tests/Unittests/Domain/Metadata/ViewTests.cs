using System;
using DsiNext.DeliveryEngine.Domain.Metadata;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Domain.Metadata
{
    /// <summary>
    /// Tests the information about a view (query).
    /// </summary>
    [TestFixture]
    public class ViewTests
    {
        /// <summary>
        /// Test that the constructor initialize a view without a description.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeViewWithoutDescription()
        {
            var fixture = new Fixture();
            var nameSource = fixture.CreateAnonymous<string>();
            var nameTarget = fixture.CreateAnonymous<string>();
            var sqlQuery = fixture.CreateAnonymous<string>();
            var view = new View(nameSource, nameTarget, sqlQuery);
            Assert.That(view, Is.Not.Null);
            Assert.That(view.NameSource, Is.Not.Null);
            Assert.That(view.NameSource, Is.Not.Empty);
            Assert.That(view.NameSource, Is.EqualTo(nameSource));
            Assert.That(view.NameTarget, Is.Not.Null);
            Assert.That(view.NameTarget, Is.Not.Empty);
            Assert.That(view.NameTarget, Is.EqualTo(nameTarget));
            Assert.That(view.SqlQuery, Is.Not.Null);
            Assert.That(view.SqlQuery, Is.Not.Empty);
            Assert.That(view.SqlQuery, Is.EqualTo(sqlQuery));
            Assert.That(view.Description, Is.Null);
        }

        /// <summary>
        /// Test that the constructor initialize a view with a description.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeViewWithDescription()
        {
            var fixture = new Fixture();
            var nameSource = fixture.CreateAnonymous<string>();
            var nameTarget = fixture.CreateAnonymous<string>();
            var sqlQuery = fixture.CreateAnonymous<string>();
            var description = fixture.CreateAnonymous<string>();
            var view = new View(nameSource, nameTarget, sqlQuery, description);
            Assert.That(view, Is.Not.Null);
            Assert.That(view.NameSource, Is.Not.Null);
            Assert.That(view.NameSource, Is.Not.Empty);
            Assert.That(view.NameSource, Is.EqualTo(nameSource));
            Assert.That(view.NameTarget, Is.Not.Null);
            Assert.That(view.NameTarget, Is.Not.Empty);
            Assert.That(view.NameTarget, Is.EqualTo(nameTarget));
            Assert.That(view.SqlQuery, Is.Not.Null);
            Assert.That(view.SqlQuery, Is.Not.Empty);
            Assert.That(view.SqlQuery, Is.EqualTo(sqlQuery));
            Assert.That(view.Description, Is.Not.Null);
            Assert.That(view.Description, Is.Not.Empty);
            Assert.That(view.Description, Is.EqualTo(description));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if name in the source repository is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameSourceIsNull()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new View(null, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if name in the source repository is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameSourceIsEmpty()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new View(string.Empty, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if name in the target repository is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameTargetIsNull()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new View(fixture.CreateAnonymous<string>(), null, fixture.CreateAnonymous<string>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if name in the target repository is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameTargetIsEmpty()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new View(fixture.CreateAnonymous<string>(), string.Empty, fixture.CreateAnonymous<string>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the sql query is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfSqlQueryIsNull()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new View(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), null));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the sql query is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfSqlQueryIsEmpty()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new View(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), string.Empty));
        }

        /// <summary>
        /// Test that the setter of SqlQuery throws an ArgumentNullException if the value is null.
        /// </summary>
        [Test]
        public void TestThatSqlQuerySetterThrowsArgumentNullExceptionIfValueIsNull()
        {
            var fixture = new Fixture();

            var view = new View(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(view, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => view.SqlQuery = null);
        }

        /// <summary>
        /// Test that the setter of SqlQuery throws an ArgumentNullException if the value is empty.
        /// </summary>
        [Test]
        public void TestThatSqlQuerySetterThrowsArgumentNullExceptionIfValueIsEmpty()
        {
            var fixture = new Fixture();

            var view = new View(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(view, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => view.SqlQuery = string.Empty);
        }

        /// <summary>
        /// Test that the setter of SqlQuery changed the value.
        /// </summary>
        [Test]
        public void TestThatSqlQuerySetterChangeValue()
        {
            var fixture = new Fixture();

            var view = new View(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(view, Is.Not.Null);

            var newValue = fixture.CreateAnonymous<string>();
            view.SqlQuery = newValue;

            Assert.That(view.SqlQuery, Is.Not.Null);
            Assert.That(view.SqlQuery, Is.Not.Empty);
            Assert.That(view.SqlQuery, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of SqlQuery raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatSqlQuerySetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();

            var view = new View(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(view, Is.Not.Null);

            var eventCalled = false;
            view.PropertyChanged += (s, e) =>
                                        {
                                            Assert.That(s, Is.Not.Null);
                                            Assert.That(e, Is.Not.Null);
                                            Assert.That(e.PropertyName, Is.Not.Null);
                                            Assert.That(e.PropertyName, Is.Not.Empty);
                                            Assert.That(e.PropertyName, Is.EqualTo("SqlQuery"));
                                            eventCalled = true;
                                        };

            view.SqlQuery = view.SqlQuery;
            Assert.That(eventCalled, Is.False);

            view.SqlQuery = fixture.CreateAnonymous<string>();
            Assert.That(eventCalled, Is.True);
        }
    }
}
