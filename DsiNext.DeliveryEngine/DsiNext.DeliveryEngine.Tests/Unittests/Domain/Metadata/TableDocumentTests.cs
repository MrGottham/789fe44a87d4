using System;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Domain.Metadata;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Domain.Metadata
{
    /// <summary>
    /// Tests document which are associated to a table.
    /// </summary>
    [TestFixture]
    public class TableDocumentTests
    {
        /// <summary>
        /// Test that the constructor initialize a table document without a description.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeTableDocumentWithoutDescription()
        {
            var fixture = new Fixture();

            var id = fixture.CreateAnonymous<int>();
            var nameSource = fixture.CreateAnonymous<string>();
            var nameTarget = fixture.CreateAnonymous<string>();
            var reference = fixture.CreateAnonymous<string>();
            var tableDocument = new TableDocument(id, nameSource, nameTarget, reference);
            Assert.That(tableDocument, Is.Not.Null);
            Assert.That(tableDocument.Id, Is.EqualTo(id));
            Assert.That(tableDocument.NameSource, Is.Not.Null);
            Assert.That(tableDocument.NameSource, Is.Not.Empty);
            Assert.That(tableDocument.NameSource, Is.EqualTo(nameSource));
            Assert.That(tableDocument.NameTarget, Is.Not.Null);
            Assert.That(tableDocument.NameTarget, Is.Not.Empty);
            Assert.That(tableDocument.NameTarget, Is.EqualTo(nameTarget));
            Assert.That(tableDocument.Reference, Is.Not.Null);
            Assert.That(tableDocument.Reference, Is.Not.Empty);
            Assert.That(tableDocument.Reference, Is.EqualTo(reference));
            Assert.That(tableDocument.Description, Is.Null);
            Assert.That(tableDocument.Field, Is.Null);
        }

        /// <summary>
        /// Test that the constructor initialize a table document with a description.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeTableDocumentWithDescription()
        {
            var fixture = new Fixture();

            var id = fixture.CreateAnonymous<int>();
            var nameSource = fixture.CreateAnonymous<string>();
            var nameTarget = fixture.CreateAnonymous<string>();
            var reference = fixture.CreateAnonymous<string>();
            var description = fixture.CreateAnonymous<string>();
            var tableDocument = new TableDocument(id, nameSource, nameTarget, reference, description);
            Assert.That(tableDocument, Is.Not.Null);
            Assert.That(tableDocument.Id, Is.EqualTo(id));
            Assert.That(tableDocument.NameSource, Is.Not.Null);
            Assert.That(tableDocument.NameSource, Is.Not.Empty);
            Assert.That(tableDocument.NameSource, Is.EqualTo(nameSource));
            Assert.That(tableDocument.NameTarget, Is.Not.Null);
            Assert.That(tableDocument.NameTarget, Is.Not.Empty);
            Assert.That(tableDocument.NameTarget, Is.EqualTo(nameTarget));
            Assert.That(tableDocument.Reference, Is.Not.Null);
            Assert.That(tableDocument.Reference, Is.Not.Empty);
            Assert.That(tableDocument.Reference, Is.EqualTo(reference));
            Assert.That(tableDocument.Description, Is.Not.Null);
            Assert.That(tableDocument.Description, Is.Not.Empty);
            Assert.That(tableDocument.Description, Is.EqualTo(description));
            Assert.That(tableDocument.Field, Is.Null);
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if document name in the source repository is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameSourceIsNull()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new TableDocument(fixture.CreateAnonymous<int>(), null, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if document name in the source repository is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameSourceIsEmpty()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new TableDocument(fixture.CreateAnonymous<int>(), string.Empty, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if document name in the target repository is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameTargetIsNull()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new TableDocument(fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), null, fixture.CreateAnonymous<string>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if document name in the target repository is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameTargetIsEmpty()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new TableDocument(fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), string.Empty, fixture.CreateAnonymous<string>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if reference to the document is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfReferenceIsNull()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new TableDocument(fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), null));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if reference to the document is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfReferenceIsEmpty()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new TableDocument(fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), string.Empty));
        }

        /// <summary>
        /// Test that the setter of Field changed the value.
        /// </summary>
        [Test]
        public void TestThatFieldSetterChangedValue()
        {
            var fixture = new Fixture();

            var tableDocument = new TableDocument(fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(tableDocument, Is.Not.Null);

            var newValue = MockRepository.GenerateMock<IField>();
            tableDocument.Field = newValue;

            Assert.That(tableDocument.Field, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of Field raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatFieldSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();

            var tableDocument = new TableDocument(fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(tableDocument, Is.Not.Null);

            var eventCalled = false;
            tableDocument.PropertyChanged += (s, e) =>
                                                 {
                                                     Assert.That(s, Is.Not.Null);
                                                     Assert.That(e, Is.Not.Null);
                                                     Assert.That(e.PropertyName, Is.Not.Null);
                                                     Assert.That(e.PropertyName, Is.Not.Empty);
                                                     Assert.That(e.PropertyName, Is.EqualTo("Field"));
                                                     eventCalled = true;
                                                 };

            tableDocument.Field = tableDocument.Field;
            Assert.That(eventCalled, Is.False);

            tableDocument.Field = MockRepository.GenerateMock<IField>();
            Assert.That(eventCalled, Is.True);
        }
    }
}
