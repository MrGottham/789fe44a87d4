using System;
using DsiNext.DeliveryEngine.Domain.Metadata;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Domain.Metadata
{
    /// <summary>
    /// Tests basic document.
    /// </summary>
    [TestFixture]
    public class DocumentBaseTests
    {
        /// <summary>
        /// Privat class for testing the basic document.
        /// </summary>
        private class MyDocument : DocumentBase
        {
            #region Constructors

            /// <summary>
            /// Creates a private document for testing the basic document.
            /// </summary>
            /// <param name="id">Unique ID for the document.</param>
            /// <param name="nameSource">Document name in the source repository.</param>
            /// <param name="nameTarget">Document name in the target repository.</param>
            /// <param name="reference">Reference to the document.</param>
            public MyDocument(int id, string nameSource, string nameTarget, string reference)
                : base(id, nameSource, nameTarget, reference)
            {
            }

            /// <summary>
            /// Creates a private document for testing the basic document.
            /// </summary>
            /// <param name="id">Unique ID for the document.</param>
            /// <param name="nameSource">Document name in the source repository.</param>
            /// <param name="nameTarget">Document name in the target repository.</param>
            /// <param name="reference">Reference to the document.</param>
            /// <param name="description">Description.</param>
            public MyDocument(int id, string nameSource, string nameTarget, string reference, string description)
                : base(id, nameSource, nameTarget, reference, description)
            {
            }

            #endregion
        }

        /// <summary>
        /// Test that the constructor initialize a document without a description.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeDocumentWithoutDescription()
        {
            var fixture = new Fixture();

            var id = fixture.CreateAnonymous<int>();
            var nameSource = fixture.CreateAnonymous<string>();
            var nameTarget = fixture.CreateAnonymous<string>();
            var reference = fixture.CreateAnonymous<string>();
            var document = new MyDocument(id, nameSource, nameTarget, reference);
            Assert.That(document, Is.Not.Null);
            Assert.That(document.Id, Is.EqualTo(id));
            Assert.That(document.NameSource, Is.Not.Null);
            Assert.That(document.NameSource, Is.Not.Empty);
            Assert.That(document.NameSource, Is.EqualTo(nameSource));
            Assert.That(document.NameTarget, Is.Not.Null);
            Assert.That(document.NameTarget, Is.Not.Empty);
            Assert.That(document.NameTarget, Is.EqualTo(nameTarget));
            Assert.That(document.Reference, Is.Not.Null);
            Assert.That(document.Reference, Is.Not.Empty);
            Assert.That(document.Reference, Is.EqualTo(reference));
            Assert.That(document.Description, Is.Null);
        }

        /// <summary>
        /// Test that the constructor initialize a document with a description.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeDocumentWithDescription()
        {
            var fixture = new Fixture();

            var id = fixture.CreateAnonymous<int>();
            var nameSource = fixture.CreateAnonymous<string>();
            var nameTarget = fixture.CreateAnonymous<string>();
            var reference = fixture.CreateAnonymous<string>();
            var description = fixture.CreateAnonymous<string>();
            var document = new MyDocument(id, nameSource, nameTarget, reference, description);
            Assert.That(document, Is.Not.Null);
            Assert.That(document.Id, Is.EqualTo(id));
            Assert.That(document.NameSource, Is.Not.Null);
            Assert.That(document.NameSource, Is.Not.Empty);
            Assert.That(document.NameSource, Is.EqualTo(nameSource));
            Assert.That(document.NameTarget, Is.Not.Null);
            Assert.That(document.NameTarget, Is.Not.Empty);
            Assert.That(document.NameTarget, Is.EqualTo(nameTarget));
            Assert.That(document.Reference, Is.Not.Null);
            Assert.That(document.Reference, Is.Not.Empty);
            Assert.That(document.Reference, Is.EqualTo(reference));
            Assert.That(document.Description, Is.Not.Null);
            Assert.That(document.Description, Is.Not.Empty);
            Assert.That(document.Description, Is.EqualTo(description));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if document name in the source repository is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameSourceIsNull()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new MyDocument(fixture.CreateAnonymous<int>(), null, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if document name in the source repository is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameSourceIsEmpty()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new MyDocument(fixture.CreateAnonymous<int>(), string.Empty, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if document name in the target repository is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameTargetIsNull()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new MyDocument(fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), null, fixture.CreateAnonymous<string>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if document name in the target repository is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfNameTargetIsEmpty()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new MyDocument(fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), string.Empty, fixture.CreateAnonymous<string>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if reference to the document is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfReferenceIsNull()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new MyDocument(fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), null));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if reference to the document is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfReferenceIsEmpty()
        {
            var fixture = new Fixture();
            Assert.Throws<ArgumentNullException>(() => new MyDocument(fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), string.Empty));
        }

        /// <summary>
        /// Test that the setter of Id changed the value.
        /// </summary>
        [Test]
        public void TestThatIdSetterChangedValue()
        {
            var fixture = new Fixture();

            var document = new MyDocument(fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(document, Is.Not.Null);

            var newValue = fixture.CreateAnonymous<int>();
            document.Id = newValue;

            Assert.That(document.Id, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of Id raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatIdSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();

            var document = new MyDocument(fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(document, Is.Not.Null);

            var eventCalled = false;
            document.PropertyChanged += (s, e) =>
                                            {
                                                Assert.That(s, Is.Not.Null);
                                                Assert.That(e, Is.Not.Null);
                                                Assert.That(e.PropertyName, Is.Not.Null);
                                                Assert.That(e.PropertyName, Is.Not.Empty);
                                                Assert.That(e.PropertyName, Is.EqualTo("Id"));
                                                eventCalled = true;
                                            };

            document.Id = document.Id;
            Assert.That(eventCalled, Is.False);

            document.Id = fixture.CreateAnonymous<int>();
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that the setter of Reference throws an ArgumentNullException if value is null.
        /// </summary>
        [Test]
        public void TestThatReferenceSetterThrowsArgumentNullExceptionIfValueIsNull()
        {
            var fixture = new Fixture();

            var document = new MyDocument(fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(document, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => document.Reference = null);
        }

        /// <summary>
        /// Test that the setter of Reference throws an ArgumentNullException if value is empty.
        /// </summary>
        [Test]
        public void TestThatReferenceSetterThrowsArgumentNullExceptionIfValueIsEmpty()
        {
            var fixture = new Fixture();

            var document = new MyDocument(fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(document, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => document.Reference = string.Empty);
        }

        /// <summary>
        /// Test that the setter of Reference changed the value.
        /// </summary>
        [Test]
        public void TestThatReferenceSetterChangedValue()
        {
            var fixture = new Fixture();

            var document = new MyDocument(fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(document, Is.Not.Null);

            var newValue = fixture.CreateAnonymous<string>();
            document.Reference = newValue;

            Assert.That(document.Reference, Is.Not.Null);
            Assert.That(document.Reference, Is.Not.Empty);
            Assert.That(document.Reference, Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that the setter of Reference raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatReferenceSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();

            var document = new MyDocument(fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(document, Is.Not.Null);

            var eventCalled = false;
            document.PropertyChanged += (s, e) =>
                                            {
                                                Assert.That(s, Is.Not.Null);
                                                Assert.That(e, Is.Not.Null);
                                                Assert.That(e.PropertyName, Is.Not.Null);
                                                Assert.That(e.PropertyName, Is.Not.Empty);
                                                Assert.That(e.PropertyName, Is.EqualTo("Reference"));
                                                eventCalled = true;
                                            };

            document.Reference = document.Reference;
            Assert.That(eventCalled, Is.False);

            document.Reference = fixture.CreateAnonymous<string>();
            Assert.That(eventCalled, Is.True);
        }
    }
}
