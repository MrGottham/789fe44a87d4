using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Domstolene.JFS.CommonLibrary.IoC.Interfaces;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Repositories.DataManipulators
{
    /// <summary>
    /// Tests the collection of data manipulators.
    /// </summary>
    [TestFixture]
    public class DataManipulatorsTests
    {
        /// <summary>
        /// Tests that the constructor initialize the collection of data manipulators.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeDataManipulators()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataManipulator>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataManipulator>()));

            var containerMock = MockRepository.GenerateMock<IContainer>();
            containerMock.Expect(m => m.ResolveAll<IDataManipulator>())
                         .Return(fixture.CreateMany<IDataManipulator>(3).ToArray())
                         .Repeat.Any();

            var dataManipulators = new DeliveryEngine.Repositories.DataManipulators.DataManipulators(containerMock);
            Assert.That(dataManipulators, Is.Not.Null);
            Assert.That(dataManipulators.Count, Is.EqualTo(3));

            containerMock.AssertWasCalled(m => m.ResolveAll<IDataManipulator>());
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentNullException if the container for Inversion of Control is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfContainerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new DeliveryEngine.Repositories.DataManipulators.DataManipulators(null));
        }

        /// <summary>
        /// Tests that ManipulateData throws an ArgumentNullException if the table is null.
        /// </summary>
        [Test]
        public void TestThatManipulateDataThrowsArgumentNullExceptionIfTableIsNull()
        {
            var containerMock = MockRepository.GenerateMock<IContainer>();
            containerMock.Expect(m => m.ResolveAll<IDataManipulator>())
                         .Return(new Collection<IDataManipulator>().ToArray())
                         .Repeat.Any();
            
            var dataManipulators = new DeliveryEngine.Repositories.DataManipulators.DataManipulators(containerMock);
            Assert.That(dataManipulators, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataManipulators.ManipulateData(null, new Collection<IEnumerable<IDataObjectBase>>(), true));
        }

        /// <summary>
        /// Tests that ManipulateData throws an ArgumentNullException if the data is null.
        /// </summary>
        [Test]
        public void TestThatManipulateDataThrowsArgumentNullExceptionIfDataIsNull()
        {
            var containerMock = MockRepository.GenerateMock<IContainer>();
            containerMock.Expect(m => m.ResolveAll<IDataManipulator>())
                         .Return(new Collection<IDataManipulator>().ToArray())
                         .Repeat.Any();

            var dataManipulators = new DeliveryEngine.Repositories.DataManipulators.DataManipulators(containerMock);
            Assert.That(dataManipulators, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataManipulators.ManipulateData(MockRepository.GenerateMock<ITable>(), null, true));
        }

        /// <summary>
        /// Tests that ManipulateData calls ManipulateData for each data manipulator.
        /// </summary>
        [Test]
        public void TestThatManipulateDataCallManipulateDataForEachDataManipulator()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataManipulator>(e => e.FromFactory(() =>
                {
                    var dataManipulatorMock = MockRepository.GenerateMock<IDataManipulator>();
                    dataManipulatorMock.Expect(m => m.ManipulateData(Arg<ITable>.Is.NotNull, Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull))
                                       .Return(new Collection<IEnumerable<IDataObjectBase>>())
                                       .Repeat.Any();
                    dataManipulatorMock.Expect(m => m.FinalizeDataManipulation(Arg<ITable>.Is.NotNull, Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull))
                                       .Return(new List<IEnumerable<IDataObjectBase>>())
                                       .Repeat.Any();
                    return dataManipulatorMock;
                }));

            var dataManipulatorCollectionMock = fixture.CreateMany<IDataManipulator>(3).ToList();
            var containerMock = MockRepository.GenerateMock<IContainer>();
            containerMock.Expect(m => m.ResolveAll<IDataManipulator>())
                         .Return(dataManipulatorCollectionMock.ToArray())
                         .Repeat.Any();

            var dataManipulators = new DeliveryEngine.Repositories.DataManipulators.DataManipulators(containerMock);
            Assert.That(dataManipulators, Is.Not.Null);

            var tableMock = MockRepository.GenerateMock<ITable>();
            var data = new Collection<IEnumerable<IDataObjectBase>>();

            var result = dataManipulators.ManipulateData(tableMock, data, true);
            Assert.That(result, Is.Not.Null);

            foreach (var dataManipulator in dataManipulators)
            {
                dataManipulator.AssertWasCalled(m => m.ManipulateData(tableMock, data));
                dataManipulator.AssertWasCalled(m => m.FinalizeDataManipulation(tableMock, data));
            }
        }

        /// <summary>
        /// Test, that ManipulateData calls FinalizeDataManipulation for each data manipulator when the last data has been received.
        /// </summary>
        [Test]
        public void TestThatManipulateDataCallFinalizeDataManipulationOnEachDataManipulatorWhenEndOfData()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataManipulator>(e => e.FromFactory(() =>
                {
                    var dataManipulatorMock = MockRepository.GenerateMock<IDataManipulator>();
                    dataManipulatorMock.Expect(m => m.ManipulateData(Arg<ITable>.Is.NotNull, Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull))
                                       .Return(new Collection<IEnumerable<IDataObjectBase>>())
                                       .Repeat.Any();
                    dataManipulatorMock.Expect(m => m.FinalizeDataManipulation(Arg<ITable>.Is.NotNull, Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull))
                                       .Return(new List<IEnumerable<IDataObjectBase>>())
                                       .Repeat.Any();
                    return dataManipulatorMock;
                }));

            var dataManipulatorCollectionMock = fixture.CreateMany<IDataManipulator>(3).ToList();
            var containerMock = MockRepository.GenerateMock<IContainer>();
            containerMock.Expect(m => m.ResolveAll<IDataManipulator>())
                         .Return(dataManipulatorCollectionMock.ToArray())
                         .Repeat.Any();

            var dataManipulators = new DeliveryEngine.Repositories.DataManipulators.DataManipulators(containerMock);
            Assert.That(dataManipulators, Is.Not.Null);

            var tableMock = MockRepository.GenerateMock<ITable>();
            var data = new Collection<IEnumerable<IDataObjectBase>>();

            var result = dataManipulators.ManipulateData(tableMock, data, true);
            Assert.That(result, Is.Not.Null);

            foreach (var dataManipulator in dataManipulators)
            {
                dataManipulator.AssertWasCalled(m => m.ManipulateData(tableMock, data));
                dataManipulator.AssertWasCalled(m => m.FinalizeDataManipulation(tableMock, data));
            }
        }

        /// <summary>
        /// Test, that ManipulateData does not call FinalizeDataManipulation for each data manipulator if the last data bas not been received.
        /// </summary>
        [Test]
        public void TestThatManipulateDataDontCallFinalizeDataManipulationOnEachDataManipulatorIfNotEndOfData()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataManipulator>(e => e.FromFactory(() =>
                {
                    var dataManipulatorMock = MockRepository.GenerateMock<IDataManipulator>();
                    dataManipulatorMock.Expect(m => m.ManipulateData(Arg<ITable>.Is.NotNull, Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull))
                                       .Return(new Collection<IEnumerable<IDataObjectBase>>())
                                       .Repeat.Any();
                    return dataManipulatorMock;
                }));

            var dataManipulatorCollectionMock = fixture.CreateMany<IDataManipulator>(3).ToList();
            var containerMock = MockRepository.GenerateMock<IContainer>();
            containerMock.Expect(m => m.ResolveAll<IDataManipulator>())
                         .Return(dataManipulatorCollectionMock.ToArray())
                         .Repeat.Any();

            var dataManipulators = new DeliveryEngine.Repositories.DataManipulators.DataManipulators(containerMock);
            Assert.That(dataManipulators, Is.Not.Null);

            var tableMock = MockRepository.GenerateMock<ITable>();
            var data = new Collection<IEnumerable<IDataObjectBase>>();

            var result = dataManipulators.ManipulateData(tableMock, data, false);
            Assert.That(result, Is.Not.Null);

            foreach (var dataManipulator in dataManipulators)
            {
                dataManipulator.AssertWasCalled(m => m.ManipulateData(tableMock, data));
                dataManipulator.AssertWasNotCalled(m => m.FinalizeDataManipulation(tableMock, data));
            }
        }
    }
}
