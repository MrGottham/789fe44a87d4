using System;
using DsiNext.DeliveryEngine.Domain.Metadata;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Domain.Metadata
{
    /// <summary>
    /// Tests the named matadata object in the delivery engine.
    /// </summary>
    [TestFixture]
    public class NamedObjectTests
    {
        /// <summary>
        /// Test that the constructor initialize a named object without a description.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeNamedObjectWithoutDescription()
        {
            var fixture = new Fixture();
            var nameSource = fixture.CreateAnonymous<string>();
            var nameTarget = fixture.CreateAnonymous<string>();

            var namedObject = new NamedObject(nameSource, nameTarget);
            Assert.That(namedObject, Is.Not.Null);
            Assert.That(namedObject.NameSource, Is.Not.Null);
            Assert.That(namedObject.NameSource, Is.Not.Empty);
            Assert.That(namedObject.NameSource, Is.EqualTo(nameSource));
            Assert.That(namedObject.NameTarget, Is.Not.Null);
            Assert.That(namedObject.NameTarget, Is.Not.Empty);
            Assert.That(namedObject.NameTarget, Is.EqualTo(nameTarget));
            Assert.That(namedObject.Description, Is.Null);
            Assert.That(namedObject.ExceptionInfo, Is.Not.Null);
            Assert.That(namedObject.ExceptionInfo, Is.Not.Empty);
            Assert.That(namedObject.ExceptionInfo, Is.EqualTo(string.Format("{0}, NameSource={1}, NameTarget={2}, Description={3}", namedObject.GetType().Name, namedObject.NameSource, namedObject.NameTarget, namedObject.Description)));
            Assert.That(namedObject.MetadataObject, Is.Not.Null);
            Assert.That(namedObject.MetadataObject, Is.EqualTo(namedObject));
        }

        /// <summary>
        /// Test that the constructor initialize a named object with a description.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeNamedObjectWithDescription()
        {
            var fixture = new Fixture();
            var nameSource = fixture.CreateAnonymous<string>();
            var nameTarget = fixture.CreateAnonymous<string>();
            var description = fixture.CreateAnonymous<string>();

            var namedObject = new NamedObject(nameSource, nameTarget, description);
            Assert.That(namedObject, Is.Not.Null);
            Assert.That(namedObject.NameSource, Is.Not.Null);
            Assert.That(namedObject.NameSource, Is.Not.Empty);
            Assert.That(namedObject.NameSource, Is.EqualTo(nameSource));
            Assert.That(namedObject.NameTarget, Is.Not.Null);
            Assert.That(namedObject.NameTarget, Is.Not.Empty);
            Assert.That(namedObject.NameTarget, Is.EqualTo(nameTarget));
            Assert.That(namedObject.Description, Is.Not.Null);
            Assert.That(namedObject.Description, Is.Not.Empty);
            Assert.That(namedObject.Description, Is.EqualTo(description));
            Assert.That(namedObject.ExceptionInfo, Is.Not.Null);
            Assert.That(namedObject.ExceptionInfo, Is.Not.Empty);
            Assert.That(namedObject.ExceptionInfo, Is.EqualTo(string.Format("{0}, NameSource={1}, NameTarget={2}, Description={3}", namedObject.GetType().Name, namedObject.NameSource, namedObject.NameTarget, namedObject.Description)));
            Assert.That(namedObject.MetadataObject, Is.Not.Null);
            Assert.That(namedObject.MetadataObject, Is.EqualTo(namedObject));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if name in the source repository is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameSourceIsNull()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new NamedObject(null, fixture.CreateAnonymous<string>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if name in the source repository is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameSourceIsEmpty()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new NamedObject(string.Empty, fixture.CreateAnonymous<string>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if name in the target repository is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameTargetIsNull()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new NamedObject(fixture.CreateAnonymous<string>(), null));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if name in the target repository is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameTargetIsEmpty()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new NamedObject(fixture.CreateAnonymous<string>(), string.Empty));
        }

        /// <summary>
        /// Test that the setter for NameSource raise the PropertyChanged event when value is changed.
        /// </summary>
        [Test]
        public void TestThatNameSourceSetterRaisePropertyChangedIfValueIsChanged()
        {
            var fixture = new Fixture();

            var namedObject = new NamedObject(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(namedObject, Is.Not.Null);

            var eventCalled = false;
            namedObject.PropertyChanged += ((s, e) =>
                                                {
                                                    Assert.That(s, Is.Not.Null);
                                                    Assert.That(e, Is.Not.Null);
                                                    Assert.That(e.PropertyName, Is.Not.Null);
                                                    Assert.That(e.PropertyName, Is.Not.Empty);
                                                    if (e.PropertyName.CompareTo("NameSource") == 0)
                                                    {
                                                        eventCalled = true;
                                                    }
                                                });

            namedObject.NameSource = namedObject.NameSource;
            Assert.That(eventCalled, Is.False);

            var newValue = fixture.CreateAnonymous<string>();
            namedObject.NameSource = newValue;
            Assert.That(namedObject.NameSource, Is.EqualTo(newValue));
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter for NameSource throws an ArgumentNullException if the value is null.
        /// </summary>
        [Test]
        public void TestThatNameSourceSetterThrowsArgumentNullExceptionIfValueIsNull()
        {
            var fixture = new Fixture();

            var namedObject = new NamedObject(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(namedObject, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => namedObject.NameSource = null);
        }

        /// <summary>
        /// Test that the setter for NameSource throws an ArgumentNullException if the value is empty.
        /// </summary>
        [Test]
        public void TestThatNameSourceSetterThrowsArgumentNullExceptionIfValueIsEmpty()
        {
            var fixture = new Fixture();

            var namedObject = new NamedObject(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(namedObject, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => namedObject.NameSource = string.Empty);
        }

        /// <summary>
        /// Test that the setter for NameTarget raise the PropertyChanged event when value is changed.
        /// </summary>
        [Test]
        public void TestThatNameTargetSetterRaisePropertyChangedIfValueIsChanged()
        {
            var fixture = new Fixture();

            var namedObject = new NamedObject(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(namedObject, Is.Not.Null);

            var eventCalled = false;
            namedObject.PropertyChanged += ((s, e) =>
                                                {
                                                    Assert.That(s, Is.Not.Null);
                                                    Assert.That(e, Is.Not.Null);
                                                    Assert.That(e.PropertyName, Is.Not.Null);
                                                    Assert.That(e.PropertyName, Is.Not.Empty);
                                                    if (e.PropertyName.CompareTo("NameTarget") == 0)
                                                    {
                                                        eventCalled = true;
                                                    }
                                                });

            namedObject.NameTarget = namedObject.NameTarget;
            Assert.That(eventCalled, Is.False);

            var newValue = fixture.CreateAnonymous<string>();
            namedObject.NameTarget = newValue;
            Assert.That(namedObject.NameTarget, Is.EqualTo(newValue));
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter for NameTarget throws an ArgumentNullException if the value is null.
        /// </summary>
        [Test]
        public void TestThatNameTargetSetterThrowsArgumentNullExceptionIfValueIsNull()
        {
            var fixture = new Fixture();

            var namedObject = new NamedObject(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(namedObject, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => namedObject.NameTarget = null);
        }

        /// <summary>
        /// Test that the setter for NameTarget throws an ArgumentNullException if the value is empty.
        /// </summary>
        [Test]
        public void TestThatNameTargetSetterThrowsArgumentNullExceptionIfValueIsEmpty()
        {
            var fixture = new Fixture();

            var namedObject = new NamedObject(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(namedObject, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => namedObject.NameTarget = string.Empty);
        }

        /// <summary>
        /// Test that the setter for Description raise the PropertyChanged event when value is changed.
        /// </summary>
        [Test]
        public void TestThatDescriptionSetterRaisePropertyChangedIfValueIsChanged()
        {
            var fixture = new Fixture();

            var namedObject = new NamedObject(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(namedObject, Is.Not.Null);

            var eventCalled = false;
            namedObject.PropertyChanged += ((s, e) =>
                                                {
                                                    Assert.That(s, Is.Not.Null);
                                                    Assert.That(e, Is.Not.Null);
                                                    Assert.That(e.PropertyName, Is.Not.Null);
                                                    Assert.That(e.PropertyName, Is.Not.Empty);
                                                    if (e.PropertyName.CompareTo("Description") == 0)
                                                    {
                                                        eventCalled = true;
                                                    }
                                                });

            namedObject.Description = namedObject.Description;
            Assert.That(eventCalled, Is.False);

            var newValue = fixture.CreateAnonymous<string>();
            namedObject.Description = newValue;
            Assert.That(namedObject.Description, Is.EqualTo(newValue));
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter for Description throws an ArgumentNullException if the value is null.
        /// </summary>
        [Test]
        public void TestThatDescriptionSetterThrowsArgumentNullExceptionIfValueIsNull()
        {
            var fixture = new Fixture();

            var namedObject = new NamedObject(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(namedObject, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => namedObject.Description = null);
        }

        /// <summary>
        /// Test that the setter for Description throws an ArgumentNullException if the value is empty.
        /// </summary>
        [Test]
        public void TestThatDescriptionSetterThrowsArgumentNullExceptionIfValueIsEmpty()
        {
            var fixture = new Fixture();

            var namedObject = new NamedObject(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(namedObject, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => namedObject.Description = string.Empty);
        }
    }
}
