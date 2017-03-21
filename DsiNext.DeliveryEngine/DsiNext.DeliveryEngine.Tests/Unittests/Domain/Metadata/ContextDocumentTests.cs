using System;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Domain.Metadata;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Domain.Metadata
{
    /// <summary>
    /// Tests context document.
    /// </summary>
    [TestFixture]
    public class ContextDocumentTests
    {
        /// <summary>
        /// Test that the constructor initialize a context document without a description.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeContextDocumentWithoutDescription()
        {
            var fixture = new Fixture();
            fixture.Customize<ContextDocumentCategories>(e => e.FromFactory(() => ContextDocumentCategories.SystemInformationOther));

            var id = fixture.CreateAnonymous<int>();
            var nameSource = fixture.CreateAnonymous<string>();
            var nameTarget = fixture.CreateAnonymous<string>();
            var reference = fixture.CreateAnonymous<string>();
            var category = fixture.CreateAnonymous<ContextDocumentCategories>();
            var contextDocument = new ContextDocument(id, nameSource, nameTarget, reference, category);
            Assert.That(contextDocument, Is.Not.Null);
            Assert.That(contextDocument.Id, Is.EqualTo(id));
            Assert.That(contextDocument.NameSource, Is.Not.Null);
            Assert.That(contextDocument.NameSource, Is.Not.Empty);
            Assert.That(contextDocument.NameSource, Is.EqualTo(nameSource));
            Assert.That(contextDocument.NameTarget, Is.Not.Null);
            Assert.That(contextDocument.NameTarget, Is.Not.Empty);
            Assert.That(contextDocument.NameTarget, Is.EqualTo(nameTarget));
            Assert.That(contextDocument.Reference, Is.Not.Null);
            Assert.That(contextDocument.Reference, Is.Not.Empty);
            Assert.That(contextDocument.Reference, Is.EqualTo(reference));
            Assert.That(contextDocument.Description, Is.Null);
            Assert.That(contextDocument.DocumentDate, Is.Null);
            Assert.That(contextDocument.DocumentAuthors, Is.Not.Null);
            Assert.That(contextDocument.DocumentAuthors.Count, Is.EqualTo(0));
            Assert.That(contextDocument.Categories, Is.Not.Null);
            Assert.That(contextDocument.Categories.Count, Is.EqualTo(1));
            Assert.That(contextDocument.Categories.Contains(category), Is.True);
        }

        /// <summary>
        /// Test that the constructor initialize a context document with a description.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeContextDocumentWithDescription()
        {
            var fixture = new Fixture();
            fixture.Customize<ContextDocumentCategories>(e => e.FromFactory(() => ContextDocumentCategories.SystemInformationOther));

            var id = fixture.CreateAnonymous<int>();
            var nameSource = fixture.CreateAnonymous<string>();
            var nameTarget = fixture.CreateAnonymous<string>();
            var reference = fixture.CreateAnonymous<string>();
            var description = fixture.CreateAnonymous<string>();
            var category = fixture.CreateAnonymous<ContextDocumentCategories>();
            var contextDocument = new ContextDocument(id, nameSource, nameTarget, reference, description, category);
            Assert.That(contextDocument, Is.Not.Null);
            Assert.That(contextDocument.Id, Is.EqualTo(id));
            Assert.That(contextDocument.NameSource, Is.Not.Null);
            Assert.That(contextDocument.NameSource, Is.Not.Empty);
            Assert.That(contextDocument.NameSource, Is.EqualTo(nameSource));
            Assert.That(contextDocument.NameTarget, Is.Not.Null);
            Assert.That(contextDocument.NameTarget, Is.Not.Empty);
            Assert.That(contextDocument.NameTarget, Is.EqualTo(nameTarget));
            Assert.That(contextDocument.Reference, Is.Not.Null);
            Assert.That(contextDocument.Reference, Is.Not.Empty);
            Assert.That(contextDocument.Reference, Is.EqualTo(reference));
            Assert.That(contextDocument.Description, Is.Not.Null);
            Assert.That(contextDocument.Description, Is.Not.Empty);
            Assert.That(contextDocument.Description, Is.EqualTo(description));
            Assert.That(contextDocument.DocumentDate, Is.Null);
            Assert.That(contextDocument.DocumentAuthors, Is.Not.Null);
            Assert.That(contextDocument.DocumentAuthors.Count, Is.EqualTo(0));
            Assert.That(contextDocument.Categories, Is.Not.Null);
            Assert.That(contextDocument.Categories.Count, Is.EqualTo(1));
            Assert.That(contextDocument.Categories.Contains(category), Is.True);
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if document name in the source repository is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameSourceIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<ContextDocumentCategories>(e => e.FromFactory(() => ContextDocumentCategories.SystemInformationOther));
            Assert.Throws<ArgumentNullException>(() => new ContextDocument(fixture.CreateAnonymous<int>(), null, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<ContextDocumentCategories>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if document name in the source repository is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameSourceIsEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize<ContextDocumentCategories>(e => e.FromFactory(() => ContextDocumentCategories.SystemInformationOther));
            Assert.Throws<ArgumentNullException>(() => new ContextDocument(fixture.CreateAnonymous<int>(), string.Empty, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<ContextDocumentCategories>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if document name in the target repository is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameTargetIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<ContextDocumentCategories>(e => e.FromFactory(() => ContextDocumentCategories.SystemInformationOther));
            Assert.Throws<ArgumentNullException>(() => new ContextDocument(fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), null, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<ContextDocumentCategories>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if document name in the target repository is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameTargetIsEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize<ContextDocumentCategories>(e => e.FromFactory(() => ContextDocumentCategories.SystemInformationOther));
            Assert.Throws<ArgumentNullException>(() => new ContextDocument(fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), string.Empty, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<ContextDocumentCategories>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if reference to the document is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfReferenceIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<ContextDocumentCategories>(e => e.FromFactory(() => ContextDocumentCategories.SystemInformationOther));
            Assert.Throws<ArgumentNullException>(() => new ContextDocument(fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), null, fixture.CreateAnonymous<ContextDocumentCategories>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if reference to the document is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfReferenceIsEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize<ContextDocumentCategories>(e => e.FromFactory(() => ContextDocumentCategories.SystemInformationOther));
            Assert.Throws<ArgumentNullException>(() => new ContextDocument(fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), string.Empty, fixture.CreateAnonymous<ContextDocumentCategories>()));
        }

        /// <summary>
        /// Test that the setter of DocumentDate changed the value.
        /// </summary>
        [Test]
        public void TestThatDocumentDateSetterChangedValue()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));
            fixture.Customize<ContextDocumentCategories>(e => e.FromFactory(() => ContextDocumentCategories.SystemInformationOther));

            var contextDocument = new ContextDocument(fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<ContextDocumentCategories>());
            Assert.That(contextDocument, Is.Not.Null);

            var newValue = fixture.CreateAnonymous<DateTime>();
            contextDocument.DocumentDate = newValue;

            Assert.That(contextDocument.DocumentDate, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of DocumentDate raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatDocumentDateSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(e => e.FromFactory(() => DateTime.Now));
            fixture.Customize<ContextDocumentCategories>(e => e.FromFactory(() => ContextDocumentCategories.SystemInformationOther));

            var contextDocument = new ContextDocument(fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<ContextDocumentCategories>());
            Assert.That(contextDocument, Is.Not.Null);

            var eventCalled = false;
            contextDocument.PropertyChanged += (s, e) =>
                                                   {
                                                       Assert.That(s, Is.Not.Null);
                                                       Assert.That(e, Is.Not.Null);
                                                       Assert.That(e.PropertyName, Is.Not.Null);
                                                       Assert.That(e.PropertyName, Is.Not.Empty);
                                                       Assert.That(e.PropertyName, Is.EqualTo("DocumentDate"));
                                                       eventCalled = true;
                                                   };

            contextDocument.DocumentDate = contextDocument.DocumentDate;
            Assert.That(eventCalled, Is.False);

            contextDocument.DocumentDate = fixture.CreateAnonymous<DateTime>();
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that AddDocumentAuthor throws an ArgumentNullException if the document author is null.
        /// </summary>
        [Test]
        public void TestThatAddDocumentAuthorThrowsArgumentNullExceptionIfDocumentAuthorIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<ContextDocumentCategories>(e => e.FromFactory(() => ContextDocumentCategories.SystemInformationOther));

            var contextDocument = new ContextDocument(fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<ContextDocumentCategories>());
            Assert.That(contextDocument, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => contextDocument.AddDocumentAuthor(null));
        }

        /// <summary>
        /// Test that AddDocumentAuthor adds a document author.
        /// </summary>
        [Test]
        public void TestThatAddDocumentAuthorAddsDocumentAuthor()
        {
            var fixture = new Fixture();
            fixture.Customize<ContextDocumentCategories>(e => e.FromFactory(() => ContextDocumentCategories.SystemInformationOther));

            var contextDocument = new ContextDocument(fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<ContextDocumentCategories>());
            Assert.That(contextDocument, Is.Not.Null);
            Assert.That(contextDocument.DocumentAuthors, Is.Not.Null);
            Assert.That(contextDocument.DocumentAuthors.Count, Is.EqualTo(0));

            var documentAuthorMock = MockRepository.GenerateMock<IDocumentAuthor>();
            contextDocument.AddDocumentAuthor(documentAuthorMock);
            Assert.That(contextDocument, Is.Not.Null);
            Assert.That(contextDocument.DocumentAuthors, Is.Not.Null);
            Assert.That(contextDocument.DocumentAuthors.Count, Is.EqualTo(1));
            Assert.That(contextDocument.DocumentAuthors.Contains(documentAuthorMock), Is.True);
        }

        /// <summary>
        /// Test that AddAuthor adds a category to the context document.
        /// </summary>
        [Test]
        public void TestAtAddCategoryAddInstitution()
        {
            var fixture = new Fixture();
            fixture.Customize<ContextDocumentCategories>(e => e.FromFactory(() => ContextDocumentCategories.SystemInformationOther));

            var contextDocument = new ContextDocument(fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<ContextDocumentCategories>());
            Assert.That(contextDocument, Is.Not.Null);
            Assert.That(contextDocument.Categories, Is.Not.Null);
            Assert.That(contextDocument.Categories.Count, Is.EqualTo(1));

            contextDocument.AddCategory(ContextDocumentCategories.SystemPurpose);
            Assert.That(contextDocument.Categories.Count, Is.EqualTo(2));
            Assert.That(contextDocument.Categories.Contains(ContextDocumentCategories.SystemPurpose), Is.True);
        }
    }
}
