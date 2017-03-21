using System;
using Domstolene.JFS.CommonLibrary.IoC.Interfaces;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.DataValidators;
using NUnit.Framework;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.BusinessLogic.DataValidators
{
    /// <summary>
    /// Tests the collection of data validators for the delivery engine.
    /// </summary>
    [TestFixture]
    public class DataValidatorsTests
    {
        /// <summary>
        /// Test that the constructor initialize the collection of data validators for the delivery engine.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeDataValidators()
        {
            var containerMock = MockRepository.GenerateMock<IContainer>();
            containerMock.Expect(m => m.ResolveAll<IDataValidator>())
                .Return(new IDataValidator[]
                    {
                        MockRepository.GenerateMock<IPrimaryKeyDataValidator>(),
                        MockRepository.GenerateMock<IForeignKeysDataValidator>()
                    })
                .Repeat.Any();

            var dataValidators = new DeliveryEngine.BusinessLogic.DataValidators.DataValidators(containerMock);
            Assert.That(dataValidators, Is.Not.Null);
            Assert.That(dataValidators.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the container to inversion of control is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfContainerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new DeliveryEngine.BusinessLogic.DataValidators.DataValidators(null));
        }
    }
}
