using DsiNext.DeliveryEngine.BusinessLogic.Events;
using NUnit.Framework;

namespace DsiNext.DeliveryEngine.Tests.Unittests.BusinessLogic.Events
{
    /// <summary>
    /// Tests arguments to the event raised before getting the data source.
    /// </summary>
    [TestFixture]
    public class GetDataSourceEventArgsTests 
    {
        /// <summary>
        /// Test that the constructor initialize arguments to the event raised before getting the data source.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeEventArgs()
        {
            var eventArgs = new GetDataSourceEventArgs();
            Assert.That(eventArgs, Is.Not.Null);
        }
    }
}
