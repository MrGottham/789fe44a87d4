using DsiNext.DeliveryEngine.Domain.Metadata;
using NUnit.Framework;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Domain.Metadata
{
    /// <summary>
    /// Tests the basic metadata object in the delivery engine.
    /// </summary>
    [TestFixture]
    public class MetadataObjectBaseTests
    {
        /// <summary>
        /// Own class for testing the basic metadata object in the delivery engine.
        /// </summary>
        private class MyMetadataObject : MetadataObjectBase
        {
        }

        /// <summary>
        /// Test that the constructor initialize an basic metadata object.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeAnMetadataObject()
        {
            var metadataObject = new MyMetadataObject();
            Assert.That(metadataObject, Is.Not.Null);
        }
    }
}
