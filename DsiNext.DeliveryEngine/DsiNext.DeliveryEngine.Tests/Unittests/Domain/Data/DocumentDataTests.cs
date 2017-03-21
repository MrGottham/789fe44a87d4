using DsiNext.DeliveryEngine.Domain.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Domain.Data
{
    /// <summary>
    /// Tests data object for a document.
    /// </summary>
    [TestFixture]
    public class DocumentDataTests
    {
        /// <summary>
        /// Test that constructor initialize document data.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeDocumentData()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();
            var documentMock = MockRepository.GenerateMock<IDocument>();
            documentMock.Expect(m => m.Reference)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();

            var documentData = new DocumentData(fieldMock, documentMock);
            Assert.That(documentData, Is.Not.Null);
            Assert.That(documentData.Document, Is.Not.Null);
            Assert.That(documentData.Document, Is.EqualTo(documentMock));
            Assert.That(documentData.Reference, Is.Not.Null);
            Assert.That(documentData.Reference, Is.Not.Empty);
            Assert.That(documentData.Reference, Is.EqualTo(documentMock.Reference));
        }

        /// <summary>
        /// Test that Reference returns the reference to the document.
        /// </summary>
        [Test]
        public void TestThatReferenceReturnsReferenceToDocument()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();
            var documentMock = MockRepository.GenerateMock<IDocument>();
            documentMock.Expect(m => m.Reference)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();

            var documentData = new DocumentData(fieldMock, documentMock);
            Assert.That(documentData.Reference, Is.Not.Null);
            Assert.That(documentData.Reference, Is.Not.Empty);
            Assert.That(documentData.Reference, Is.EqualTo(documentMock.Reference));
        }

        /// <summary>
        /// Test that Reference returns null if the document is null.
        /// </summary>
        [Test]
        public void TestThatReferenceReturnsNullIfDocumentIsNull()
        {
            var fieldMock = MockRepository.GenerateMock<IField>();

            var documentData = new DocumentData(fieldMock, null);
            Assert.That(documentData.Reference, Is.Null);
        }

        /// <summary>
        /// Test that the setter of Document raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatDocumentSetterRaisesPropertyChangedEvent()
        {
            var fieldMock = MockRepository.GenerateMock<IField>();
            var documentMock = MockRepository.GenerateMock<IDocument>();

            var documentData = new DocumentData(fieldMock, documentMock);
            Assert.That(documentData, Is.Not.Null);

            var eventCalledForDocument = false;
            var eventCalledForReference = false;
            documentData.PropertyChanged += (s, e) =>
                                                {
                                                    Assert.That(s, Is.Not.Null);
                                                    Assert.That(e, Is.Not.Null);
                                                    Assert.That(e.PropertyName, Is.Not.Null);
                                                    Assert.That(e.PropertyName, Is.Not.Empty);
                                                    if (e.PropertyName.Equals("Document"))
                                                    {
                                                        eventCalledForDocument = true;
                                                    }
                                                    if (e.PropertyName.Equals("Reference"))
                                                    {
                                                        eventCalledForReference = true;
                                                    }
                                                };

            documentData.Document = documentData.Document;
            Assert.That(eventCalledForDocument, Is.False);
            Assert.That(eventCalledForReference, Is.False);

            documentData.Document = MockRepository.GenerateMock<IDocument>();
            Assert.That(eventCalledForDocument, Is.True);
            Assert.That(eventCalledForReference, Is.True);
        }

        /// <summary>
        /// Test that GetSourceValue throws an DeliveryEngineSystemException if the generic target type is invalid.
        /// </summary>
        [Test]
        public void TestThatGetSourceValueThrowsDeliveryEngineSystemExceptionIfGenericTargetTypeIsInvalid()
        {
            var fieldMock = MockRepository.GenerateMock<IField>();
            var documentMock = MockRepository.GenerateMock<IDocument>();

            var documentData = new DocumentData(fieldMock, documentMock);
            Assert.That(documentData, Is.Not.Null);

            Assert.Throws<DeliveryEngineSystemException>(() => documentData.GetSourceValue<decimal>());
        }

        /// <summary>
        /// Test that GetSourceValue get the source document.
        /// </summary>
        [Test]
        public void TestThatGetSourceValueGetsTheSourceDocument()
        {
            var fieldMock = MockRepository.GenerateMock<IField>();
            var documentMock = MockRepository.GenerateMock<IDocument>();

            var documentData = new DocumentData(fieldMock, documentMock);
            Assert.That(documentData, Is.Not.Null);

            var sourceDocument = documentData.GetSourceValue<IDocument>();
            Assert.That(sourceDocument, Is.Not.Null);
            Assert.That(sourceDocument, Is.EqualTo(documentData.Document));
        }

        /// <summary>
        /// Test that GetTargetValue throws an DeliveryEngineSystemException if the generic target type is invalid.
        /// </summary>
        [Test]
        public void TestThatGetTargetValueThrowsDeliveryEngineSystemExceptionIfGenericTargetTypeIsInvalid()
        {
            var fieldMock = MockRepository.GenerateMock<IField>();
            var documentMock = MockRepository.GenerateMock<IDocument>();

            var documentData = new DocumentData(fieldMock, documentMock);
            Assert.That(documentData, Is.Not.Null);

            Assert.Throws<DeliveryEngineSystemException>(() => documentData.GetTargetValue<decimal>());
        }

        /// <summary>
        /// Test that GetTargetValue get the target document.
        /// </summary>
        [Test]
        public void TestThatGetTargetValueGetsTheTargetDocument()
        {
            var fieldMock = MockRepository.GenerateMock<IField>();
            var documentMock = MockRepository.GenerateMock<IDocument>();

            var documentData = new DocumentData(fieldMock, documentMock);
            Assert.That(documentData, Is.Not.Null);

            var targetDocument = documentData.GetTargetValue<IDocument>();
            Assert.That(targetDocument, Is.Not.Null);
            Assert.That(targetDocument, Is.EqualTo(documentData.Document));
        }

        /// <summary>
        /// Test that UpdateSourceValue throws an DeliveryEngineSystemException if the generic target type is invalid.
        /// </summary>
        [Test]
        public void TestThatUpdateSourceValueThrowsDeliveryEngineSystemExceptionIfGenericTargetTypeIsInvalid()
        {
            var fixture = new Fixture();
            var fieldMock = MockRepository.GenerateMock<IField>();
            var documentMock = MockRepository.GenerateMock<IDocument>();

            var documentData = new DocumentData(fieldMock, documentMock);
            Assert.That(documentData, Is.Not.Null);

            Assert.Throws<DeliveryEngineSystemException>(() => documentData.UpdateSourceValue(fixture.CreateAnonymous<decimal>()));
        }

        /// <summary>
        /// Test that UpdateSourceValue updates the source value.
        /// </summary>
        [Test]
        public void TestThatUpdateSourceValueUpdatesSourceValue()
        {
            var fieldMock = MockRepository.GenerateMock<IField>();
            var documentMock = MockRepository.GenerateMock<IDocument>();

            var documentData = new DocumentData(fieldMock, documentMock);
            Assert.That(documentData, Is.Not.Null);

            var newDocumentMock = MockRepository.GenerateMock<IDocument>();
            documentData.UpdateSourceValue(newDocumentMock);

            var sourceDocument = documentData.GetSourceValue<IDocument>();
            Assert.That(sourceDocument, Is.Not.Null);
            Assert.That(sourceDocument, Is.EqualTo(newDocumentMock));
        }

        /// <summary>
        /// Test that UpdateSourceValue can set the source value to null.
        /// </summary>
        [Test]
        public void TestThatUpdateSourceValueCanSetSourceValueToNull()
        {
            var fieldMock = MockRepository.GenerateMock<IField>();
            var documentMock = MockRepository.GenerateMock<IDocument>();

            var documentData = new DocumentData(fieldMock, documentMock);
            Assert.That(documentData, Is.Not.Null);

            documentData.UpdateSourceValue<object>(null);

            var sourceDocument = documentData.GetSourceValue<IDocument>();
            Assert.That(sourceDocument, Is.Null);
        }

        /// <summary>
        /// Test that Clone clones the document data.
        /// </summary>
        [Test]
        public void TestThatCloneClonesDocumentData()
        {
            var fieldMock = MockRepository.GenerateMock<IField>();
            var documentMock = MockRepository.GenerateMock<IDocument>();

            var documentData = new DocumentData(fieldMock, documentMock);
            Assert.That(documentData, Is.Not.Null);

            var clonedDocumentData = (IDocumentData) documentData.Clone();
            Assert.That(clonedDocumentData, Is.Not.Null);
            Assert.That(clonedDocumentData.Field, Is.Not.Null);
            Assert.That(clonedDocumentData.Field, Is.EqualTo(documentData.Field));
            Assert.That(clonedDocumentData.Document, Is.Not.Null);
            Assert.That(clonedDocumentData.Document, Is.EqualTo(documentData.Document));
            Assert.That(clonedDocumentData.Reference, Is.EqualTo(documentData.Reference));

            documentData.UpdateSourceValue<object>(null);

            var sourceDocument = documentData.GetSourceValue<IDocument>();
            Assert.That(sourceDocument, Is.Null);
        }
    }
}
