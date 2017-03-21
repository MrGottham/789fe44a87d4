using DsiNext.DeliveryEngine.Domain.Metadata;
using NUnit.Framework;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Domain.Metadata
{
    /// <summary>
    /// Tests class for dynamic mapping.
    /// </summary>
    [TestFixture]
    public class DynamicMapTests
    {
        /// <summary>
        /// Test that the constructor initialize a dynamic map.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeDanymicMap()
        {
            var dynamicMap = new DynamicMap<int, string>();
            Assert.That(dynamicMap, Is.Not.Null);
            Assert.That(dynamicMap.ExceptionInfo, Is.Not.Null);
            Assert.That(dynamicMap.ExceptionInfo, Is.Not.Empty);
            Assert.That(dynamicMap.ExceptionInfo, Is.EqualTo(string.Format("{0}, TSource={1}, TTarget={2}", dynamicMap.GetType().Name, typeof (int).Name, typeof (string).Name)));
            Assert.That(dynamicMap.MappingObject, Is.Not.Null);
            Assert.That(dynamicMap.MappingObject, Is.EqualTo(dynamicMap));
            Assert.That(dynamicMap.MappingObjectData, Is.Null);
        }
    }
}
