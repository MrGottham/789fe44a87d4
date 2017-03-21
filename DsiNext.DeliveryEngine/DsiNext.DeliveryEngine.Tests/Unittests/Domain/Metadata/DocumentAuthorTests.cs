using System;
using DsiNext.DeliveryEngine.Domain.Metadata;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Domain.Metadata
{
    /// <summary>
    /// Tests document author.
    /// </summary>
    [TestFixture]
    public class DocumentAuthorTests
    {
        /// <summary>
        /// Test that the constructor initialize a document author without name of the author.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeDocumentAuthorWithoutAuthor()
        {
            var fixture = new Fixture();
            var institution = fixture.CreateAnonymous<string>();
            var documentAuthor = new DocumentAuthor(institution);
            Assert.That(documentAuthor, Is.Not.Null);
            Assert.That(documentAuthor.Author, Is.Null);
            Assert.That(documentAuthor.Institution, Is.Not.Null);
            Assert.That(documentAuthor.Institution, Is.Not.Empty);
            Assert.That(documentAuthor.Institution, Is.EqualTo(institution));
        }

        /// <summary>
        /// Test that the constructor initialize a document author with name of the author.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeDocumentAuthorWithAuthor()
        {
            var fixture = new Fixture();
            var author = fixture.CreateAnonymous<string>();
            var institution = fixture.CreateAnonymous<string>();
            var documentAuthor = new DocumentAuthor(author, institution);
            Assert.That(documentAuthor, Is.Not.Null);
            Assert.That(documentAuthor.Author, Is.Not.Null);
            Assert.That(documentAuthor.Author, Is.Not.Empty);
            Assert.That(documentAuthor.Author, Is.EqualTo(author));
            Assert.That(documentAuthor.Institution, Is.Not.Null);
            Assert.That(documentAuthor.Institution, Is.Not.Empty);
            Assert.That(documentAuthor.Institution, Is.EqualTo(institution));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the name of the author is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfAuthorIsNull()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new DocumentAuthor(null, fixture.CreateAnonymous<string>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the name of the author is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfAuthorIsEmpty()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new DocumentAuthor(string.Empty, fixture.CreateAnonymous<string>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the institution of the author is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfInstitutionIsNull()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new DocumentAuthor(null));
            Assert.Throws<ArgumentNullException>(() => new DocumentAuthor(fixture.CreateAnonymous<string>(), null));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the institution of the author is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfInstitutionIsEmpty()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new DocumentAuthor(string.Empty));
            Assert.Throws<ArgumentNullException>(() => new DocumentAuthor(fixture.CreateAnonymous<string>(), string.Empty));
        }

        /// <summary>
        /// Test that the setter of Author changed the value.
        /// </summary>
        [Test]
        public void TestThatAuthorSetterChangeValue()
        {
            var fixture = new Fixture();

            var documentAuthor = new DocumentAuthor(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(documentAuthor, Is.Not.Null);

            var newValue = fixture.CreateAnonymous<string>();
            documentAuthor.Author = newValue;

            Assert.That(documentAuthor.Author, Is.Not.Null);
            Assert.That(documentAuthor.Author, Is.Not.Empty);
            Assert.That(documentAuthor.Author, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the seter of Author raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatAuthorSetterRaisesPropertyChanged()
        {
            var fixture = new Fixture();

            var documentAuthor = new DocumentAuthor(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(documentAuthor, Is.Not.Null);

            var eventCalled = false;
            documentAuthor.PropertyChanged += (s, e) =>
                {
                    Assert.That(e, Is.Not.Null);
                    Assert.That(e.PropertyName, Is.Not.Null);
                    Assert.That(e.PropertyName, Is.Not.Empty);
                    Assert.That(e.PropertyName, Is.EqualTo("Author"));
                    eventCalled = true;
                };

            documentAuthor.Author = documentAuthor.Author;
            Assert.That(eventCalled, Is.False);

            documentAuthor.Author = fixture.CreateAnonymous<string>();
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter of Author throws an ArgumentNullException if the value is null.
        /// </summary>
        [Test]
        public void TestThatAuthorSetterThrowsArgumentNullExceptionIfValueIsNull()
        {
            var fixture = new Fixture();

            var documentAuthor = new DocumentAuthor(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(documentAuthor, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => documentAuthor.Author = null);
        }

        /// <summary>
        /// Test that the setter of Author throws an ArgumentNullException if the value is empty.
        /// </summary>
        [Test]
        public void TestThatAuthorSetterThrowsArgumentNullExceptionIfValueIsEmpty()
        {
            var fixture = new Fixture();

            var documentAuthor = new DocumentAuthor(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(documentAuthor, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => documentAuthor.Author = string.Empty);
        }

        /// <summary>
        /// Test that the setter of Institution changed the value.
        /// </summary>
        [Test]
        public void TestThatInstitutionSetterChangeValue()
        {
            var fixture = new Fixture();

            var documentAuthor = new DocumentAuthor(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(documentAuthor, Is.Not.Null);

            var newValue = fixture.CreateAnonymous<string>();
            documentAuthor.Institution = newValue;

            Assert.That(documentAuthor.Institution, Is.Not.Null);
            Assert.That(documentAuthor.Institution, Is.Not.Empty);
            Assert.That(documentAuthor.Institution, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the seter of Institution raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatInstitutionSetterRaisesPropertyChanged()
        {
            var fixture = new Fixture();

            var documentAuthor = new DocumentAuthor(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(documentAuthor, Is.Not.Null);

            var eventCalled = false;
            documentAuthor.PropertyChanged += (s, e) =>
                {
                    Assert.That(e, Is.Not.Null);
                    Assert.That(e.PropertyName, Is.Not.Null);
                    Assert.That(e.PropertyName, Is.Not.Empty);
                    Assert.That(e.PropertyName, Is.EqualTo("Institution"));
                    eventCalled = true;
                };

            documentAuthor.Institution = documentAuthor.Institution;
            Assert.That(eventCalled, Is.False);

            documentAuthor.Institution = fixture.CreateAnonymous<string>();
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter of Institution throws an ArgumentNullException if the value is null.
        /// </summary>
        [Test]
        public void TestThatInstitutionSetterThrowsArgumentNullExceptionIfValueIsNull()
        {
            var fixture = new Fixture();

            var documentAuthor = new DocumentAuthor(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(documentAuthor, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => documentAuthor.Institution = null);
        }

        /// <summary>
        /// Test that the setter of Institution throws an ArgumentNullException if the value is empty.
        /// </summary>
        [Test]
        public void TestThatInstitutionSetterThrowsArgumentNullExceptionIfValueIsEmpty()
        {
            var fixture = new Fixture();

            var documentAuthor = new DocumentAuthor(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(documentAuthor, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => documentAuthor.Institution = string.Empty);
        }
    }
}
