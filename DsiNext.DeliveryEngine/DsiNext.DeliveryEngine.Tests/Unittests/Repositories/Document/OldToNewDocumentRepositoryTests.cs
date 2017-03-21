using DsiNext.DeliveryEngine.Repositories.Document.OldToNew;
using NUnit.Framework;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Repositories.Document
{
    /// <summary>
    /// Tests the document repository for converting old delivery format to the new delivery format.
    /// </summary>
    [TestFixture]
    public class OldToNewDocumentRepositoryTests
    {
        /// <summary>
        /// Test that the constructor initialize the document repository.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeOldToNewDocumentRepository()
        {
            var oldToNewDocumentRepository = new OldToNewDocumentRepository();
            Assert.That(oldToNewDocumentRepository, Is.Not.Null);
        }
    }
}
