using System;
using DsiNext.DeliveryEngine.Domain.Metadata;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Domain.Metadata
{
    /// <summary>
    /// Tests informations about a FORM classification.
    /// </summary>
    [TestFixture]
    public class FormClassTests
    {
        /// <summary>
        /// Test that the constructor initialize informations about a FORM classification.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeFormClass()
        {
            var fixture = new Fixture();

            var nameSource = fixture.CreateAnonymous<string>();
            var nameTarget = fixture.CreateAnonymous<string>();
            var description = fixture.CreateAnonymous<string>();
            var formClass = new FormClass(nameSource, nameTarget, description);
            Assert.That(formClass, Is.Not.Null);
            Assert.That(formClass.NameSource, Is.Not.Null);
            Assert.That(formClass.NameSource, Is.Not.Empty);
            Assert.That(formClass.NameSource, Is.EqualTo(nameSource));
            Assert.That(formClass.NameTarget, Is.Not.Null);
            Assert.That(formClass.NameTarget, Is.Not.Empty);
            Assert.That(formClass.NameTarget, Is.EqualTo(nameTarget));
            Assert.That(formClass.Description, Is.Not.Null);
            Assert.That(formClass.Description, Is.Not.Empty);
            Assert.That(formClass.Description, Is.EqualTo(description));
            Assert.That(formClass.FormClassName, Is.Not.Null);
            Assert.That(formClass.FormClassName, Is.Not.Empty);
            Assert.That(formClass.FormClassName, Is.EqualTo(nameTarget));
            Assert.That(formClass.FormClassText, Is.Not.Null);
            Assert.That(formClass.FormClassText, Is.Not.Empty);
            Assert.That(formClass.FormClassText, Is.EqualTo(description));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if FORM classification in the source repository is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameSourceIsNull()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new FormClass(null, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if FORM classification in the source repository is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameSourceIsEmpty()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new FormClass(string.Empty, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if FORM classification in the target repository is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameTargetIsNull()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new FormClass(fixture.CreateAnonymous<string>(), null, fixture.CreateAnonymous<string>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if FORM classification in the target repository is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameTargetIsEmpty()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new FormClass(fixture.CreateAnonymous<string>(), string.Empty, fixture.CreateAnonymous<string>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if text for FORM classification is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfDescriptionIsNull()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new FormClass(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), null));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if text for FORM classification is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfDescriptionIsEmpty()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new FormClass(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), string.Empty));
        }
    }
}
