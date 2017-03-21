using DsiNext.DeliveryEngine.Domain.Metadata;
using NUnit.Framework;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Domain.Metadata
{
    /// <summary>
    /// Test the class for static mapping.
    /// </summary>
    [TestFixture]
    public class StaticMapTests
    {
        /// <summary>
        /// Test that the constructor initialize a static map.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeStaticMap()
        {
            var staticMap = new StaticMap<int, string>();
            Assert.That(staticMap, Is.Not.Null);
            Assert.That(staticMap.ExceptionInfo, Is.Not.Null);
            Assert.That(staticMap.ExceptionInfo, Is.Not.Empty);
            Assert.That(staticMap.ExceptionInfo, Is.EqualTo(string.Format("{0}, TSource={1}, TTarget={2}", staticMap.GetType().Name, typeof (int).Name, typeof (string).Name)));
            Assert.That(staticMap.MappingObject, Is.Not.Null);
            Assert.That(staticMap.MappingObject, Is.EqualTo(staticMap));
            Assert.That(staticMap.MappingObjectData, Is.Null);
        }
    }
}
