using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Events;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.Data.Oracle;
using DsiNext.DeliveryEngine.Repositories.Data.Oracle.Events;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Events;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Repositories.Data
{
    /// <summary>
    /// Tests data repository for reading data from an Oracle database.
    /// </summary>
    [TestFixture]
    public class OracleDataRepositoryTests
    {
        /// <summary>
        /// Tests that the constructor initialize the data repository for reading data from an Oracle database.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeDataRepository()
        {
            var fixture = new Fixture();
            fixture.Customize<IOracleClientFactory>(e => e.FromFactory(() => MockRepository.GenerateMock<IOracleClientFactory>()));
            fixture.Customize<IDataManipulators>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataManipulators>()));

            var dataRepository = new OracleDataRepository(fixture.CreateAnonymous<IOracleClientFactory>(), fixture.CreateAnonymous<IDataManipulators>());
            Assert.That(dataRepository, Is.Not.Null);
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentNullException if the factory to creates Oracle clients is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfOracleClientFactoryIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataManipulators>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataManipulators>()));

            Assert.Throws<ArgumentNullException>(() => new OracleDataRepository(null, fixture.CreateAnonymous<IDataManipulators>()));
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentNullException if the collection of data manipulators is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfDataManipulatorsIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IOracleClientFactory>(e => e.FromFactory(() => MockRepository.GenerateMock<IOracleClientFactory>()));

            Assert.Throws<ArgumentNullException>(() => new OracleDataRepository(fixture.CreateAnonymous<IOracleClientFactory>(), null));
        }

        /// <summary>
        /// Tests that DataGetForTargetTable throws an ArgumentNullException if name of the target table is null.
        /// </summary>
        [Test]
        public void TestThatDataGetForTargetTableThrowsArgumentNullExceptionIfTargetTableNameIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IOracleClientFactory>(e => e.FromFactory(() => MockRepository.GenerateMock<IOracleClientFactory>()));
            fixture.Customize<IDataManipulators>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataManipulators>()));
            fixture.Customize<IDataSource>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataSource>()));

            var dataRepository = new OracleDataRepository(fixture.CreateAnonymous<IOracleClientFactory>(), fixture.CreateAnonymous<IDataManipulators>());
            Assert.That(dataRepository, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataRepository.DataGetForTargetTable(null, fixture.CreateAnonymous<IDataSource>()));
        }

        /// <summary>
        /// Tests that DataGetForTargetTable throws an ArgumentNullException if name of the target table is empty.
        /// </summary>
        [Test]
        public void TestThatDataGetForTargetTableThrowsArgumentNullExceptionIfTargetTableNameIsEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize<IOracleClientFactory>(e => e.FromFactory(() => MockRepository.GenerateMock<IOracleClientFactory>()));
            fixture.Customize<IDataManipulators>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataManipulators>()));
            fixture.Customize<IDataSource>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataSource>()));

            var dataRepository = new OracleDataRepository(fixture.CreateAnonymous<IOracleClientFactory>(), fixture.CreateAnonymous<IDataManipulators>());
            Assert.That(dataRepository, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataRepository.DataGetForTargetTable(string.Empty, fixture.CreateAnonymous<IDataSource>()));
        }

        /// <summary>
        /// Tests that DataGetForTargetTable throws an ArgumentNullException if the data source is null.
        /// </summary>
        [Test]
        public void TestThatDataGetForTargetTableThrowsArgumentNullExceptionIfDataSourceIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IOracleClientFactory>(e => e.FromFactory(() => MockRepository.GenerateMock<IOracleClientFactory>()));
            fixture.Customize<IDataManipulators>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataManipulators>()));

            var dataRepository = new OracleDataRepository(fixture.CreateAnonymous<IOracleClientFactory>(), fixture.CreateAnonymous<IDataManipulators>());
            Assert.That(dataRepository, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataRepository.DataGetForTargetTable(fixture.CreateAnonymous<string>(), null));
        }

        /// <summary>
        /// Tests that DataGetFromTable throws an ArgumentNullException if the table is null.
        /// </summary>
        [Test]
        public void TestThatDataGetFromTableThrowsArgumentNullExceptionIfTableIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IOracleClientFactory>(e => e.FromFactory(() => MockRepository.GenerateMock<IOracleClientFactory>()));
            fixture.Customize<IDataManipulators>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataManipulators>()));

            var dataRepository = new OracleDataRepository(fixture.CreateAnonymous<IOracleClientFactory>(), fixture.CreateAnonymous<IDataManipulators>());
            Assert.That(dataRepository, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataRepository.DataGetFromTable(null));
        }

        /// <summary>
        /// Test that DataGetForTargetTable gets data from one or more source tables. 
        /// </summary>
        [Test]
        public void TestThatDataGetForTargetTableGetDataFromOneOrMoreSourceTables()
        {
            var fixture = new Fixture();
            fixture.Customize<IEnumerable<IDataObjectBase>>(e => e.FromFactory(() => new List<IDataObjectBase>(0)));

            var firstTableMock = MockRepository.GenerateMock<ITable>();
            firstTableMock.Expect(m => m.NameTarget)
                          .Return(fixture.CreateAnonymous<string>())
                          .Repeat.Any();
            firstTableMock.Expect(m => m.RecordFilters)
                          .Return(new ReadOnlyObservableCollection<IFilter>(new ObservableCollection<IFilter>(new List<IFilter>(0))))
                          .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => firstTableMock));

            var secondTableMock = MockRepository.GenerateMock<ITable>();
            secondTableMock.Expect(m => m.NameTarget)
                           .Return(firstTableMock.NameTarget)
                           .Repeat.Any();
            secondTableMock.Expect(m => m.RecordFilters)
                           .Return(new ReadOnlyObservableCollection<IFilter>(new ObservableCollection<IFilter>(new List<IFilter>(0))))
                           .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => secondTableMock));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(new List<ITable> {firstTableMock, secondTableMock})))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var oracleClientMock = MockRepository.GenerateMock<IOracleClient>();
            oracleClientMock.Expect(m => m.GetData(Arg<ITable>.Is.NotNull, Arg<DeliveryEngineEventHandler<IHandleDataEventArgs>>.Is.NotNull))
                            .WhenCalled(e =>
                                {
                                    var eventHandler = (DeliveryEngineEventHandler<IHandleDataEventArgs>) e.Arguments[1];
                                    eventHandler.Invoke(this, new HandleOracleDataEventArgs(e.Arguments[0] as ITable, fixture.CreateMany<IEnumerable<IDataObjectBase>>(250).ToList(), true));
                                })
                            .Repeat.Any();
            fixture.Customize<IOracleClient>(e => e.FromFactory(() => oracleClientMock));

            var oracleClientFactoryMock = MockRepository.GenerateMock<IOracleClientFactory>();
            oracleClientFactoryMock.Expect(m => m.Create())
                                   .Return(oracleClientMock)
                                   .Repeat.Any();
            fixture.Customize<IOracleClientFactory>(e => e.FromFactory(() => oracleClientFactoryMock));

            var dataManipulatorsMock = MockRepository.GenerateMock<IDataManipulators>();
            dataManipulatorsMock.Expect(m => m.ManipulateData(Arg<ITable>.Is.NotNull, Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull, Arg<bool>.Is.Anything))
                                .WhenCalled(e => e.ReturnValue = e.Arguments[1])
                                .Repeat.Any();
            fixture.Customize<IDataManipulators>(e => e.FromFactory(() => dataManipulatorsMock));

            var dataRepository = new OracleDataRepository(fixture.CreateAnonymous<IOracleClientFactory>(), fixture.CreateAnonymous<IDataManipulators>());
            Assert.That(dataRepository, Is.Not.Null);

            var eventCalled = 0;
            dataRepository.OnHandleData += (s, e) =>
                {
                    Assert.That(s, Is.Not.Null);
                    Assert.That(e, Is.Not.Null);
                    Assert.That(e.Table, Is.Not.Null);
                    Assert.That(e.Data, Is.Not.Null);
                    Assert.That(e.Data, Is.Not.Empty);
                    Assert.That(e.Data.Count(), Is.EqualTo(250));
                    Assert.That(e.EndOfData, Is.True);
                    eventCalled++;
                };
            dataRepository.DataGetForTargetTable(firstTableMock.NameTarget, fixture.CreateAnonymous<IDataSource>());
            Assert.That(eventCalled, Is.EqualTo(2));

            dataSourceMock.AssertWasCalled(m => m.Tables);

            oracleClientFactoryMock.AssertWasCalled(m => m.Create(), opt => opt.Repeat.Times(2));
            oracleClientMock.AssertWasCalled(m => m.ValidateTable(Arg<ITable>.Is.NotNull), opt => opt.Repeat.Times(2));
            oracleClientMock.AssertWasCalled(m => m.GetData(Arg<ITable>.Is.NotNull, Arg<DeliveryEngineEventHandler<IHandleDataEventArgs>>.Is.NotNull), opt => opt.Repeat.Times(2));
            oracleClientMock.AssertWasCalled(m => m.Dispose(), opt => opt.Repeat.Times(4));

            firstTableMock.AssertWasCalled(m => m.RecordFilters);
            secondTableMock.AssertWasCalled(m => m.RecordFilters);

            dataManipulatorsMock.AssertWasCalled(m => m.ManipulateData(Arg<ITable>.Is.Equal(firstTableMock), Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull, Arg<bool>.Is.Equal(true)));
            dataManipulatorsMock.AssertWasCalled(m => m.ManipulateData(Arg<ITable>.Is.Equal(secondTableMock), Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull, Arg<bool>.Is.Equal(true)));
        }

        /// <summary>
        /// Test that DataGetForTargetTable throws an DeliveryEngineMetadataException, if the oracle client throws an DeliveryEngineMetadataException.
        /// </summary>
        [Test]
        public void TestThatDataGetForTargetTableThrowsDeliveryEngineMetadataExceptionIfOracleClientThrowsDeliveryEngineMetadataException()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineMetadataExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineMetadataExceptionInfo>()));

            var firstTableMock = MockRepository.GenerateMock<ITable>();
            firstTableMock.Expect(m => m.NameTarget)
                          .Return(fixture.CreateAnonymous<string>())
                          .Repeat.Any();
            firstTableMock.Expect(m => m.RecordFilters)
                          .Return(new ReadOnlyObservableCollection<IFilter>(new ObservableCollection<IFilter>(new List<IFilter>(0))))
                          .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<ITable>(e => e.FromFactory(() => firstTableMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var secondTableMock = MockRepository.GenerateMock<ITable>();
            secondTableMock.Expect(m => m.NameTarget)
                           .Return(firstTableMock.NameTarget)
                           .Repeat.Any();
            secondTableMock.Expect(m => m.RecordFilters)
                           .Return(new ReadOnlyObservableCollection<IFilter>(new ObservableCollection<IFilter>(new List<IFilter>(0))))
                           .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => secondTableMock));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(new List<ITable> {firstTableMock, secondTableMock})))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var oracleClientMock = MockRepository.GenerateMock<IOracleClient>();
            oracleClientMock.Expect(m => m.GetData(Arg<ITable>.Is.NotNull, Arg<DeliveryEngineEventHandler<IHandleDataEventArgs>>.Is.NotNull))
                            .Throw(fixture.CreateAnonymous<DeliveryEngineMetadataException>())
                            .Repeat.Any();
            fixture.Customize<IOracleClient>(e => e.FromFactory(() => oracleClientMock));

            var oracleClientFactoryMock = MockRepository.GenerateMock<IOracleClientFactory>();
            oracleClientFactoryMock.Expect(m => m.Create())
                                   .Return(oracleClientMock)
                                   .Repeat.Any();
            fixture.Customize<IOracleClientFactory>(e => e.FromFactory(() => oracleClientFactoryMock));

            var dataManipulatorsMock = MockRepository.GenerateMock<IDataManipulators>();
            fixture.Customize<IDataManipulators>(e => e.FromFactory(() => dataManipulatorsMock));

            var dataRepository = new OracleDataRepository(fixture.CreateAnonymous<IOracleClientFactory>(), fixture.CreateAnonymous<IDataManipulators>());
            Assert.That(dataRepository, Is.Not.Null);

            Assert.Throws<DeliveryEngineMetadataException>(() => dataRepository.DataGetForTargetTable(firstTableMock.NameTarget, fixture.CreateAnonymous<IDataSource>()));

            dataSourceMock.AssertWasCalled(m => m.Tables);

            oracleClientFactoryMock.AssertWasCalled(m => m.Create());
            oracleClientMock.AssertWasCalled(m => m.ValidateTable(Arg<ITable>.Is.NotNull));
            oracleClientMock.AssertWasCalled(m => m.GetData(Arg<ITable>.Is.NotNull, Arg<DeliveryEngineEventHandler<IHandleDataEventArgs>>.Is.NotNull));
            oracleClientMock.AssertWasCalled(m => m.Dispose());

            firstTableMock.AssertWasNotCalled(m => m.RecordFilters);
            secondTableMock.AssertWasNotCalled(m => m.RecordFilters);

            // ReSharper disable ImplicitlyCapturedClosure
            dataManipulatorsMock.AssertWasNotCalled(m => m.ManipulateData(Arg<ITable>.Is.Equal(firstTableMock), Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull, Arg<bool>.Is.Anything));
            // ReSharper restore ImplicitlyCapturedClosure
            dataManipulatorsMock.AssertWasNotCalled(m => m.ManipulateData(Arg<ITable>.Is.Equal(secondTableMock), Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull, Arg<bool>.Is.Anything));
        }

        /// <summary>
        /// Test that DataGetForTargetTable throws an DeliveryEngineRepositoryException, if the oracle client throws an DeliveryEngineRepositoryException.
        /// </summary>
        [Test]
        public void TestThatDataGetForTargetTableThrowsDeliveryEngineRepositoryExceptionIfOracleClientThrowsDeliveryEngineRepositoryException()
        {
            var fixture = new Fixture();

            var firstTableMock = MockRepository.GenerateMock<ITable>();
            firstTableMock.Expect(m => m.NameTarget)
                          .Return(fixture.CreateAnonymous<string>())
                          .Repeat.Any();
            firstTableMock.Expect(m => m.RecordFilters)
                          .Return(new ReadOnlyObservableCollection<IFilter>(new ObservableCollection<IFilter>(new List<IFilter>(0))))
                          .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<ITable>(e => e.FromFactory(() => firstTableMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var secondTableMock = MockRepository.GenerateMock<ITable>();
            secondTableMock.Expect(m => m.NameTarget)
                           .Return(firstTableMock.NameTarget)
                           .Repeat.Any();
            secondTableMock.Expect(m => m.RecordFilters)
                           .Return(new ReadOnlyObservableCollection<IFilter>(new ObservableCollection<IFilter>(new List<IFilter>(0))))
                           .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => secondTableMock));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(new List<ITable> {firstTableMock, secondTableMock})))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var oracleClientMock = MockRepository.GenerateMock<IOracleClient>();
            oracleClientMock.Expect(m => m.GetData(Arg<ITable>.Is.NotNull, Arg<DeliveryEngineEventHandler<IHandleDataEventArgs>>.Is.NotNull))
                            .Throw(fixture.CreateAnonymous<DeliveryEngineRepositoryException>())
                            .Repeat.Any();
            fixture.Customize<IOracleClient>(e => e.FromFactory(() => oracleClientMock));

            var oracleClientFactoryMock = MockRepository.GenerateMock<IOracleClientFactory>();
            oracleClientFactoryMock.Expect(m => m.Create())
                                   .Return(oracleClientMock)
                                   .Repeat.Any();
            fixture.Customize<IOracleClientFactory>(e => e.FromFactory(() => oracleClientFactoryMock));

            var dataManipulatorsMock = MockRepository.GenerateMock<IDataManipulators>();
            fixture.Customize<IDataManipulators>(e => e.FromFactory(() => dataManipulatorsMock));

            var dataRepository = new OracleDataRepository(fixture.CreateAnonymous<IOracleClientFactory>(), fixture.CreateAnonymous<IDataManipulators>());
            Assert.That(dataRepository, Is.Not.Null);

            Assert.Throws<DeliveryEngineRepositoryException>(() => dataRepository.DataGetForTargetTable(firstTableMock.NameTarget, fixture.CreateAnonymous<IDataSource>()));

            dataSourceMock.AssertWasCalled(m => m.Tables);

            oracleClientFactoryMock.AssertWasCalled(m => m.Create());
            oracleClientMock.AssertWasCalled(m => m.ValidateTable(Arg<ITable>.Is.NotNull));
            oracleClientMock.AssertWasCalled(m => m.GetData(Arg<ITable>.Is.NotNull, Arg<DeliveryEngineEventHandler<IHandleDataEventArgs>>.Is.NotNull));
            oracleClientMock.AssertWasCalled(m => m.Dispose());

            firstTableMock.AssertWasNotCalled(m => m.RecordFilters);
            secondTableMock.AssertWasNotCalled(m => m.RecordFilters);
            
            // ReSharper disable ImplicitlyCapturedClosure
            dataManipulatorsMock.AssertWasNotCalled(m => m.ManipulateData(Arg<ITable>.Is.Equal(firstTableMock), Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull, Arg<bool>.Is.Anything));
            // ReSharper restore ImplicitlyCapturedClosure
            dataManipulatorsMock.AssertWasNotCalled(m => m.ManipulateData(Arg<ITable>.Is.Equal(secondTableMock), Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull, Arg<bool>.Is.Anything));
        }

        /// <summary>
        /// Test that DataGetForTargetTable throws an DeliveryEngineRepositoryException, if the oracle client throws an Exception.
        /// </summary>
        [Test]
        public void TestThatDataGetForTargetTableThrowsDeliveryEngineRepositoryExceptionIfOracleClientThrowsException()
        {
            var fixture = new Fixture();

            var firstTableMock = MockRepository.GenerateMock<ITable>();
            firstTableMock.Expect(m => m.NameTarget)
                          .Return(fixture.CreateAnonymous<string>())
                          .Repeat.Any();
            firstTableMock.Expect(m => m.RecordFilters)
                          .Return(new ReadOnlyObservableCollection<IFilter>(new ObservableCollection<IFilter>(new List<IFilter>(0))))
                          .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<ITable>(e => e.FromFactory(() => firstTableMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var secondTableMock = MockRepository.GenerateMock<ITable>();
            secondTableMock.Expect(m => m.NameTarget)
                           .Return(firstTableMock.NameTarget)
                           .Repeat.Any();
            secondTableMock.Expect(m => m.RecordFilters)
                           .Return(new ReadOnlyObservableCollection<IFilter>(new ObservableCollection<IFilter>(new List<IFilter>(0))))
                           .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => secondTableMock));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(new List<ITable> {firstTableMock, secondTableMock})))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var oracleClientMock = MockRepository.GenerateMock<IOracleClient>();
            oracleClientMock.Expect(m => m.GetData(Arg<ITable>.Is.NotNull, Arg<DeliveryEngineEventHandler<IHandleDataEventArgs>>.Is.NotNull))
                            .Throw(fixture.CreateAnonymous<Exception>())
                            .Repeat.Any();
            fixture.Customize<IOracleClient>(e => e.FromFactory(() => oracleClientMock));

            var oracleClientFactoryMock = MockRepository.GenerateMock<IOracleClientFactory>();
            oracleClientFactoryMock.Expect(m => m.Create())
                                   .Return(oracleClientMock)
                                   .Repeat.Any();
            fixture.Customize<IOracleClientFactory>(e => e.FromFactory(() => oracleClientFactoryMock));

            var dataManipulatorsMock = MockRepository.GenerateMock<IDataManipulators>();
            fixture.Customize<IDataManipulators>(e => e.FromFactory(() => dataManipulatorsMock));

            var dataRepository = new OracleDataRepository(fixture.CreateAnonymous<IOracleClientFactory>(), fixture.CreateAnonymous<IDataManipulators>());
            Assert.That(dataRepository, Is.Not.Null);

            Assert.Throws<DeliveryEngineRepositoryException>(() => dataRepository.DataGetForTargetTable(firstTableMock.NameTarget, fixture.CreateAnonymous<IDataSource>()));

            dataSourceMock.AssertWasCalled(m => m.Tables);

            oracleClientFactoryMock.AssertWasCalled(m => m.Create());
            oracleClientMock.AssertWasCalled(m => m.ValidateTable(Arg<ITable>.Is.NotNull));
            oracleClientMock.AssertWasCalled(m => m.GetData(Arg<ITable>.Is.NotNull, Arg<DeliveryEngineEventHandler<IHandleDataEventArgs>>.Is.NotNull));
            oracleClientMock.AssertWasCalled(m => m.Dispose());

            firstTableMock.AssertWasNotCalled(m => m.RecordFilters);
            secondTableMock.AssertWasNotCalled(m => m.RecordFilters);
            
            // ReSharper disable ImplicitlyCapturedClosure
            dataManipulatorsMock.AssertWasNotCalled(m => m.ManipulateData(Arg<ITable>.Is.Equal(firstTableMock), Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull, Arg<bool>.Is.Anything));
            // ReSharper restore ImplicitlyCapturedClosure
            dataManipulatorsMock.AssertWasNotCalled(m => m.ManipulateData(Arg<ITable>.Is.Equal(secondTableMock), Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull, Arg<bool>.Is.Anything));
        }

        /// <summary>
        /// Tests that DataGetForTargetTable throws an DeliveryEngineSystemException when this exception occurs.
        /// </summary>
        [Test]
        public void TestThatDataGetForTargetTableThrowsDeliveryEngineSystemExceptionWhenDeliveryEngineSystemExceptionOccurs()
        {
            var fixture = new Fixture();
            fixture.Customize<IEnumerable<IDataObjectBase>>(e => e.FromFactory(() => new List<IDataObjectBase>(0)));

            var firstTableMock = MockRepository.GenerateMock<ITable>();
            firstTableMock.Expect(m => m.NameTarget)
                          .Return(fixture.CreateAnonymous<string>())
                          .Repeat.Any();
            firstTableMock.Expect(m => m.RecordFilters)
                          .Return(new ReadOnlyObservableCollection<IFilter>(new ObservableCollection<IFilter>(new List<IFilter>(0))))
                          .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<ITable>(e => e.FromFactory(() => firstTableMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var secondTableMock = MockRepository.GenerateMock<ITable>();
            secondTableMock.Expect(m => m.NameTarget)
                           .Return(firstTableMock.NameTarget)
                           .Repeat.Any();
            secondTableMock.Expect(m => m.RecordFilters)
                           .Return(new ReadOnlyObservableCollection<IFilter>(new ObservableCollection<IFilter>(new List<IFilter>(0))))
                           .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => secondTableMock));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(new List<ITable> {firstTableMock, secondTableMock})))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var oracleClientMock = MockRepository.GenerateMock<IOracleClient>();
            // ReSharper disable ImplicitlyCapturedClosure
            oracleClientMock.Expect(m => m.GetData(Arg<ITable>.Is.NotNull, Arg<DeliveryEngineEventHandler<IHandleDataEventArgs>>.Is.NotNull))
                            .WhenCalled(e =>
                                {
                                    var eventHandler = (DeliveryEngineEventHandler<IHandleDataEventArgs>) e.Arguments[1];
                                    eventHandler.Invoke(this, new HandleOracleDataEventArgs(e.Arguments[0] as ITable, fixture.CreateMany<IEnumerable<IDataObjectBase>>(250).ToList(), true));
                                })
                            .Repeat.Any();
            // ReSharper restore ImplicitlyCapturedClosure
            fixture.Customize<IOracleClient>(e => e.FromFactory(() => oracleClientMock));

            var oracleClientFactoryMock = MockRepository.GenerateMock<IOracleClientFactory>();
            oracleClientFactoryMock.Expect(m => m.Create())
                                   .Return(oracleClientMock)
                                   .Repeat.Any();
            fixture.Customize<IOracleClientFactory>(e => e.FromFactory(() => oracleClientFactoryMock));

            var dataManipulatorsMock = MockRepository.GenerateMock<IDataManipulators>();
            dataManipulatorsMock.Expect(m => m.ManipulateData(Arg<ITable>.Is.NotNull, Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull, Arg<bool>.Is.Anything))
                                .WhenCalled(e => e.ReturnValue = e.Arguments[1])
                                .Repeat.Any();
            fixture.Customize<IDataManipulators>(e => e.FromFactory(() => dataManipulatorsMock));

            var dataRepository = new OracleDataRepository(fixture.CreateAnonymous<IOracleClientFactory>(), fixture.CreateAnonymous<IDataManipulators>());
            Assert.That(dataRepository, Is.Not.Null);

            // ReSharper disable ImplicitlyCapturedClosure
            dataRepository.OnHandleData += (s, e) =>
                {
                    Assert.That(s, Is.Not.Null);
                    Assert.That(e, Is.Not.Null);
                    Assert.That(e.Table, Is.Not.Null);
                    Assert.That(e.Data, Is.Not.Null);
                    Assert.That(e.Data, Is.Not.Empty);
                    Assert.That(e.EndOfData, Is.True);
                    throw fixture.CreateAnonymous<DeliveryEngineSystemException>();
                };
            // ReSharper restore ImplicitlyCapturedClosure
            Assert.Throws<DeliveryEngineSystemException>(() => dataRepository.DataGetForTargetTable(firstTableMock.NameTarget, fixture.CreateAnonymous<IDataSource>()));

            dataSourceMock.AssertWasCalled(m => m.Tables);

            oracleClientFactoryMock.AssertWasCalled(m => m.Create());
            oracleClientMock.AssertWasCalled(m => m.ValidateTable(Arg<ITable>.Is.NotNull));
            oracleClientMock.AssertWasCalled(m => m.GetData(Arg<ITable>.Is.NotNull, Arg<DeliveryEngineEventHandler<IHandleDataEventArgs>>.Is.NotNull));
            oracleClientMock.AssertWasCalled(m => m.Dispose());

            firstTableMock.AssertWasCalled(m => m.RecordFilters);
            secondTableMock.AssertWasNotCalled(m => m.RecordFilters);

            // ReSharper disable ImplicitlyCapturedClosure
            dataManipulatorsMock.AssertWasCalled(m => m.ManipulateData(Arg<ITable>.Is.Equal(firstTableMock), Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull, Arg<bool>.Is.Equal(true)));
            // ReSharper restore ImplicitlyCapturedClosure
            dataManipulatorsMock.AssertWasNotCalled(m => m.ManipulateData(Arg<ITable>.Is.Equal(secondTableMock), Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull, Arg<bool>.Is.Anything));
        }

        /// <summary>
        /// Tests that DataGetFromTable gets data for the table.
        /// </summary>
        [Test]
        public void TestThatDataGetFromTableGetsData()
        {
            var fixture = new Fixture();
            fixture.Customize<IEnumerable<IDataObjectBase>>(e => e.FromFactory(() => new List<IDataObjectBase>(0)));

            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.RecordFilters)
                     .Return(new ReadOnlyObservableCollection<IFilter>(new ObservableCollection<IFilter>(new List<IFilter>(0))))
                     .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => tableMock));

            var oracleClientMock = MockRepository.GenerateMock<IOracleClient>();
            oracleClientMock.Expect(m =>m.GetData(Arg<ITable>.Is.Equal(tableMock), Arg<DeliveryEngineEventHandler<IHandleDataEventArgs>>.Is.NotNull))
                            .WhenCalled(e =>
                                {
                                    var eventHandler = (DeliveryEngineEventHandler<IHandleDataEventArgs>) e.Arguments[1];
                                    eventHandler.Invoke(this, new HandleOracleDataEventArgs(e.Arguments[0] as ITable, fixture.CreateMany<IEnumerable<IDataObjectBase>>(250).ToList(), true));
                                })
                            .Repeat.Any();
            fixture.Customize<IOracleClient>(e => e.FromFactory(() => oracleClientMock));

            var oracleClientFactoryMock = MockRepository.GenerateMock<IOracleClientFactory>();
            oracleClientFactoryMock.Expect(m => m.Create())
                .Return(oracleClientMock)
                .Repeat.Any();
            fixture.Customize<IOracleClientFactory>(e => e.FromFactory(() => oracleClientFactoryMock));

            var dataManipulatorsMock = MockRepository.GenerateMock<IDataManipulators>();
            dataManipulatorsMock.Expect(m => m.ManipulateData(Arg<ITable>.Is.NotNull, Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull, Arg<bool>.Is.Anything))
                                .WhenCalled(e => e.ReturnValue = e.Arguments[1])
                                .Repeat.Any();
            fixture.Customize<IDataManipulators>(e => e.FromFactory(() => dataManipulatorsMock));

            var dataRepository = new OracleDataRepository(fixture.CreateAnonymous<IOracleClientFactory>(), fixture.CreateAnonymous<IDataManipulators>());
            Assert.That(dataRepository, Is.Not.Null);

            var data = new List<IEnumerable<IDataObjectBase>>();
            dataRepository.OnHandleData += (s, e) =>
                {
                    Assert.That(s, Is.Not.Null);
                    Assert.That(e, Is.Not.Null);
                    Assert.That(e.Table, Is.Not.Null);
                    Assert.That(e.Data, Is.Not.Null);
                    Assert.That(e.Data, Is.Not.Empty);
                    Assert.That(e.EndOfData, Is.True);
                    data.AddRange(e.Data);
                };
            dataRepository.DataGetFromTable(fixture.CreateAnonymous<ITable>());
            Assert.That(data, Is.Not.Null);
            Assert.That(data.Count(), Is.EqualTo(250));

            oracleClientFactoryMock.AssertWasCalled(m => m.Create());
            oracleClientMock.AssertWasCalled(m => m.ValidateTable(Arg<ITable>.Is.Equal(tableMock)));
            oracleClientMock.AssertWasCalled(m => m.GetData(Arg<ITable>.Is.Equal(tableMock), Arg<DeliveryEngineEventHandler<IHandleDataEventArgs>>.Is.NotNull));
            oracleClientMock.AssertWasCalled(m => m.Dispose());

            tableMock.AssertWasCalled(m => m.RecordFilters);

            dataManipulatorsMock.AssertWasCalled(m => m.ManipulateData(Arg<ITable>.Is.Equal(tableMock), Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull, Arg<bool>.Is.Equal(true)));
        }

        /// <summary>
        /// Tests that DataGetFromTable evaluates record filters.
        /// </summary>
        [Test]
        public void TestThatDataGetFromTableEvaluatesRecordFilters()
        {
            var fixture = new Fixture();
            fixture.Customize<IEnumerable<IDataObjectBase>>(e => e.FromFactory(() => new List<IDataObjectBase>(0)));

            fixture.Customize<IFilter>(e => e.FromFactory(() =>
                {
                    var filterMock = MockRepository.GenerateMock<IFilter>();
                    filterMock.Expect(m => m.Exclude(Arg<IEnumerable<IDataObjectBase>>.Is.NotNull))
                              .Return(true)
                              .Repeat.Any();
                    return filterMock;
                }));
            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.RecordFilters)
                     .Return(new ReadOnlyObservableCollection<IFilter>(new ObservableCollection<IFilter>(fixture.CreateMany<IFilter>(5).ToList())))
                     .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => tableMock));

            var oracleClientMock = MockRepository.GenerateMock<IOracleClient>();

            oracleClientMock.Expect(m => m.GetData(Arg<ITable>.Is.Equal(tableMock), Arg<DeliveryEngineEventHandler<IHandleDataEventArgs>>.Is.NotNull))
                            .WhenCalled(e =>
                                {
                                    var eventHandler = (DeliveryEngineEventHandler<IHandleDataEventArgs>) e.Arguments[1];
                                    eventHandler.Invoke(this, new HandleOracleDataEventArgs(e.Arguments[0] as ITable, fixture.CreateMany<IEnumerable<IDataObjectBase>>(250).ToList(), true));
                                })
                            .Repeat.Any();
            fixture.Customize<IOracleClient>(e => e.FromFactory(() => oracleClientMock));

            var oracleClientFactoryMock = MockRepository.GenerateMock<IOracleClientFactory>();
            oracleClientFactoryMock.Expect(m => m.Create())
                                   .Return(oracleClientMock)
                                   .Repeat.Any();
            fixture.Customize<IOracleClientFactory>(e => e.FromFactory(() => oracleClientFactoryMock));

            var dataManipulatorsMock = MockRepository.GenerateMock<IDataManipulators>();
            dataManipulatorsMock.Expect(m => m.ManipulateData(Arg<ITable>.Is.NotNull, Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull, Arg<bool>.Is.Anything))
                                .WhenCalled(e => e.ReturnValue = e.Arguments[1])
                                .Repeat.Any();
            fixture.Customize<IDataManipulators>(e => e.FromFactory(() => dataManipulatorsMock));

            var dataRepository = new OracleDataRepository(fixture.CreateAnonymous<IOracleClientFactory>(), fixture.CreateAnonymous<IDataManipulators>());
            Assert.That(dataRepository, Is.Not.Null);

            var data = new List<IEnumerable<IDataObjectBase>>();
            dataRepository.OnHandleData += (s, e) =>
                {
                    Assert.That(s, Is.Not.Null);
                    Assert.That(e, Is.Not.Null);
                    Assert.That(e.Table, Is.Not.Null);
                    Assert.That(e.Data, Is.Not.Null);
                    Assert.That(e.Data, Is.Empty);
                    Assert.That(e.EndOfData, Is.True);
                    data.AddRange(e.Data);
                };
            dataRepository.DataGetFromTable(fixture.CreateAnonymous<ITable>());
            Assert.That(data, Is.Not.Null);
            Assert.That(data.Count(), Is.EqualTo(0));

            oracleClientFactoryMock.AssertWasCalled(m => m.Create());
            oracleClientMock.AssertWasCalled(m => m.ValidateTable(Arg<ITable>.Is.Equal(tableMock)));
            oracleClientMock.AssertWasCalled(m => m.GetData(Arg<ITable>.Is.Equal(tableMock), Arg<DeliveryEngineEventHandler<IHandleDataEventArgs>>.Is.NotNull));
            oracleClientMock.AssertWasCalled(m => m.Dispose());

            tableMock.AssertWasCalled(m => m.RecordFilters);
            foreach (var filterMock in tableMock.RecordFilters)
            {
                filterMock.AssertWasCalled(m => m.Exclude(Arg<IEnumerable<IDataObjectBase>>.Is.NotNull), opt => opt.Repeat.Times(250));
            }

            dataManipulatorsMock.AssertWasCalled(m => m.ManipulateData(Arg<ITable>.Is.Equal(tableMock), Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull, Arg<bool>.Is.Equal(true)));
        }

        /// <summary>
        /// Tests that DataGetFromTable throws an DeliveryEngineMetadataException, if the oracle client throws an DeliveryEngineMetadataException.
        /// </summary>
        [Test]
        public void TestThatDataGetFromTableThrowsDeliveryEngineMetadataExceptionIfOracleClientThrowsDeliveryEngineMetadataException()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineMetadataExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineMetadataExceptionInfo>()));

            var tableMock = MockRepository.GenerateMock<ITable>();
            fixture.Customize<ITable>(e => e.FromFactory(() => tableMock));

            var oracleClientMock = MockRepository.GenerateMock<IOracleClient>();
            oracleClientMock.Expect(m => m.GetData(Arg<ITable>.Is.Equal(tableMock), Arg<DeliveryEngineEventHandler<IHandleDataEventArgs>>.Is.NotNull))
                            .Throw(fixture.CreateAnonymous<DeliveryEngineMetadataException>())
                            .Repeat.Any();
            fixture.Customize<IOracleClient>(e => e.FromFactory(() => oracleClientMock));

            var oracleClientFactoryMock = MockRepository.GenerateMock<IOracleClientFactory>();
            oracleClientFactoryMock.Expect(m => m.Create())
                                   .Return(oracleClientMock)
                                   .Repeat.Any();
            fixture.Customize<IOracleClientFactory>(e => e.FromFactory(() => oracleClientFactoryMock));

            var dataManipulatorsMock = MockRepository.GenerateMock<IDataManipulators>();
            fixture.Customize<IDataManipulators>(e => e.FromFactory(() => dataManipulatorsMock));

            var dataRepository = new OracleDataRepository(fixture.CreateAnonymous<IOracleClientFactory>(), fixture.CreateAnonymous<IDataManipulators>());
            Assert.That(dataRepository, Is.Not.Null);

            Assert.Throws<DeliveryEngineMetadataException>(() => dataRepository.DataGetFromTable(fixture.CreateAnonymous<ITable>()));

            oracleClientFactoryMock.AssertWasCalled(m => m.Create());
            oracleClientMock.AssertWasCalled(m => m.ValidateTable(Arg<ITable>.Is.Equal(tableMock)));
            oracleClientMock.AssertWasCalled(m => m.GetData(Arg<ITable>.Is.Equal(tableMock), Arg<DeliveryEngineEventHandler<IHandleDataEventArgs>>.Is.NotNull));
            oracleClientMock.AssertWasCalled(m => m.Dispose());

            tableMock.AssertWasNotCalled(m => m.RecordFilters);

            dataManipulatorsMock.AssertWasNotCalled(m => m.ManipulateData(Arg<ITable>.Is.Equal(tableMock), Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull, Arg<bool>.Is.Anything));
        }

        /// <summary>
        /// Tests that DataGetFromTable throws an DeliveryEngineRepositoryException, if the oracle client throws an DeliveryEngineRepositoryException.
        /// </summary>
        [Test]
        public void TestThatDataGetFromTableThrowsDeliveryEngineRepositoryExceptionIfOracleClientThrowsDeliveryEngineRepositoryException()
        {
            var fixture = new Fixture();

            var tableMock = MockRepository.GenerateMock<ITable>();
            fixture.Customize<ITable>(e => e.FromFactory(() => tableMock));

            var oracleClientMock = MockRepository.GenerateMock<IOracleClient>();
            oracleClientMock.Expect(m => m.GetData(Arg<ITable>.Is.Equal(tableMock), Arg<DeliveryEngineEventHandler<IHandleDataEventArgs>>.Is.NotNull))
                            .Throw(fixture.CreateAnonymous<DeliveryEngineRepositoryException>())
                            .Repeat.Any();
            fixture.Customize<IOracleClient>(e => e.FromFactory(() => oracleClientMock));

            var oracleClientFactoryMock = MockRepository.GenerateMock<IOracleClientFactory>();
            oracleClientFactoryMock.Expect(m => m.Create())
                                   .Return(oracleClientMock)
                                   .Repeat.Any();
            fixture.Customize<IOracleClientFactory>(e => e.FromFactory(() => oracleClientFactoryMock));

            var dataManipulatorsMock = MockRepository.GenerateMock<IDataManipulators>();
            fixture.Customize<IDataManipulators>(e => e.FromFactory(() => dataManipulatorsMock));

            var dataRepository = new OracleDataRepository(fixture.CreateAnonymous<IOracleClientFactory>(), fixture.CreateAnonymous<IDataManipulators>());
            Assert.That(dataRepository, Is.Not.Null);

            Assert.Throws<DeliveryEngineRepositoryException>(() => dataRepository.DataGetFromTable(fixture.CreateAnonymous<ITable>()));

            oracleClientFactoryMock.AssertWasCalled(m => m.Create());
            oracleClientMock.AssertWasCalled(m => m.ValidateTable(Arg<ITable>.Is.Equal(tableMock)));
            oracleClientMock.AssertWasCalled(m => m.GetData(Arg<ITable>.Is.Equal(tableMock), Arg<DeliveryEngineEventHandler<IHandleDataEventArgs>>.Is.NotNull));
            oracleClientMock.AssertWasCalled(m => m.Dispose());

            tableMock.AssertWasNotCalled(m => m.RecordFilters);

            dataManipulatorsMock.AssertWasNotCalled(m => m.ManipulateData(Arg<ITable>.Is.Equal(tableMock), Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull, Arg<bool>.Is.Anything));
        }

        /// <summary>
        /// Tests that DataGetFromTable throws an DeliveryEngineRepositoryException, if the oracle client throws an Exception.
        /// </summary>
        [Test]
        public void TestThatDataGetFromTableThrowsDeliveryEngineRepositoryExceptionIfOracleClientThrowsException()
        {
            var fixture = new Fixture();

            var tableMock = MockRepository.GenerateMock<ITable>();
            fixture.Customize<ITable>(e => e.FromFactory(() => tableMock));

            var oracleClientMock = MockRepository.GenerateMock<IOracleClient>();
            oracleClientMock.Expect(m => m.GetData(Arg<ITable>.Is.Equal(tableMock), Arg<DeliveryEngineEventHandler<IHandleDataEventArgs>>.Is.NotNull))
                            .Throw(fixture.CreateAnonymous<Exception>())
                            .Repeat.Any();
            fixture.Customize<IOracleClient>(e => e.FromFactory(() => oracleClientMock));

            var oracleClientFactoryMock = MockRepository.GenerateMock<IOracleClientFactory>();
            oracleClientFactoryMock.Expect(m => m.Create())
                                   .Return(oracleClientMock)
                                   .Repeat.Any();
            fixture.Customize<IOracleClientFactory>(e => e.FromFactory(() => oracleClientFactoryMock));

            var dataManipulatorsMock = MockRepository.GenerateMock<IDataManipulators>();
            fixture.Customize<IDataManipulators>(e => e.FromFactory(() => dataManipulatorsMock));

            var dataRepository = new OracleDataRepository(fixture.CreateAnonymous<IOracleClientFactory>(), fixture.CreateAnonymous<IDataManipulators>());
            Assert.That(dataRepository, Is.Not.Null);

            Assert.Throws<DeliveryEngineRepositoryException>(() => dataRepository.DataGetFromTable(fixture.CreateAnonymous<ITable>()));

            oracleClientFactoryMock.AssertWasCalled(m => m.Create());
            oracleClientMock.AssertWasCalled(m => m.ValidateTable(Arg<ITable>.Is.Equal(tableMock)));
            oracleClientMock.AssertWasCalled(m => m.GetData(Arg<ITable>.Is.Equal(tableMock), Arg<DeliveryEngineEventHandler<IHandleDataEventArgs>>.Is.NotNull));
            oracleClientMock.AssertWasCalled(m => m.Dispose());

            tableMock.AssertWasNotCalled(m => m.RecordFilters);

            dataManipulatorsMock.AssertWasNotCalled(m => m.ManipulateData(Arg<ITable>.Is.Equal(tableMock), Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull, Arg<bool>.Is.Anything));
        }

        /// <summary>
        /// Tests that DataGetFromTable throws an DeliveryEngineSystemException when this exception occurs.
        /// </summary>
        [Test]
        public void TestThatDataGetFromTableThrowsDeliveryEngineSystemExceptionWhenDeliveryEngineSystemExceptionOccurs()
        {
            var fixture = new Fixture();
            fixture.Customize<IEnumerable<IDataObjectBase>>(e => e.FromFactory(() => new List<IDataObjectBase>(0)));

            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.RecordFilters)
                     .Return(new ReadOnlyObservableCollection<IFilter>(new ObservableCollection<IFilter>(new List<IFilter>(0))))
                     .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => tableMock));

            var oracleClientMock = MockRepository.GenerateMock<IOracleClient>();
            oracleClientMock.Expect(m => m.GetData(Arg<ITable>.Is.Equal(tableMock), Arg<DeliveryEngineEventHandler<IHandleDataEventArgs>>.Is.NotNull))
                            .WhenCalled(e =>
                                {
                                    var eventHandler = (DeliveryEngineEventHandler<IHandleDataEventArgs>) e.Arguments[1];
                                    eventHandler.Invoke(this, new HandleOracleDataEventArgs(e.Arguments[0] as ITable, fixture.CreateMany<IEnumerable<IDataObjectBase>>(250).ToList(), true));
                                })
                            .Repeat.Any();
            fixture.Customize<IOracleClient>(e => e.FromFactory(() => oracleClientMock));

            var oracleClientFactoryMock = MockRepository.GenerateMock<IOracleClientFactory>();
            oracleClientFactoryMock.Expect(m => m.Create())
                .Return(oracleClientMock)
                .Repeat.Any();
            fixture.Customize<IOracleClientFactory>(e => e.FromFactory(() => oracleClientFactoryMock));

            var dataManipulatorsMock = MockRepository.GenerateMock<IDataManipulators>();
            dataManipulatorsMock.Expect(m => m.ManipulateData(Arg<ITable>.Is.NotNull, Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull, Arg<bool>.Is.Anything))
                                .WhenCalled(e => e.ReturnValue = e.Arguments[1])
                                .Repeat.Any();
            fixture.Customize<IDataManipulators>(e => e.FromFactory(() => dataManipulatorsMock));

            var dataRepository = new OracleDataRepository(fixture.CreateAnonymous<IOracleClientFactory>(), fixture.CreateAnonymous<IDataManipulators>());
            Assert.That(dataRepository, Is.Not.Null);

            // ReSharper disable ImplicitlyCapturedClosure
            dataRepository.OnHandleData += (s, e) =>
                {
                    Assert.That(s, Is.Not.Null);
                    Assert.That(e, Is.Not.Null);
                    Assert.That(e.Table, Is.Not.Null);
                    Assert.That(e.Data, Is.Not.Null);
                    Assert.That(e.Data, Is.Not.Empty);
                    Assert.That(e.EndOfData, Is.True);
                    throw fixture.CreateAnonymous<DeliveryEngineSystemException>();
                };
            // ReSharper restore ImplicitlyCapturedClosure
            Assert.Throws<DeliveryEngineSystemException>(() => dataRepository.DataGetFromTable(fixture.CreateAnonymous<ITable>()));

            oracleClientFactoryMock.AssertWasCalled(m => m.Create());
            oracleClientMock.AssertWasCalled(m => m.ValidateTable(Arg<ITable>.Is.Equal(tableMock)));
            oracleClientMock.AssertWasCalled(m => m.GetData(Arg<ITable>.Is.Equal(tableMock), Arg<DeliveryEngineEventHandler<IHandleDataEventArgs>>.Is.NotNull));
            oracleClientMock.AssertWasCalled(m => m.Dispose());

            tableMock.AssertWasCalled(m => m.RecordFilters);

            dataManipulatorsMock.AssertWasCalled(m => m.ManipulateData(Arg<ITable>.Is.Equal(tableMock), Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull, Arg<bool>.Is.Equal(true)));
        }

        /// <summary>
        /// Tests that GetDataQueryer gets the data queryer.
        /// </summary>
        [Test]
        public void TestThatGetDataQueryerGetDataQueryer()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataManipulators>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataManipulators>()));

            var dataQueryerMock = MockRepository.GenerateMock<IDataQueryer>();
            fixture.Customize<IDataQueryer>(e => e.FromFactory(() => dataQueryerMock));

            var oracleClientFactoryMock = MockRepository.GenerateMock<IOracleClientFactory>();
            oracleClientFactoryMock.Expect(m => m.CreateDataQueryer(Arg<IDataManipulators>.Is.NotNull))
                                   .Return(dataQueryerMock)
                                   .Repeat.Any();
            fixture.Customize<IOracleClientFactory>(e => e.FromFactory(() => oracleClientFactoryMock));

            var dataRepository = new OracleDataRepository(fixture.CreateAnonymous<IOracleClientFactory>(), fixture.CreateAnonymous<IDataManipulators>());
            Assert.That(dataRepository, Is.Not.Null);

            using (var dataQueryer = dataRepository.GetDataQueryer())
            {
                Assert.That(dataQueryer, Is.Not.Null);
                Assert.That(dataQueryer, Is.EqualTo(dataQueryerMock));
                
                dataQueryer.Dispose();
            }

            dataQueryerMock.AssertWasCalled(m => m.Dispose());
        }

        /// <summary>
        /// Tests that OnHandleOracleData throws an ArgumentNullException if the sender is null.
        /// </summary>
        [Test]
        public void TestThatOnHandleOracleDataThrowsArgumentNullExceptionIfSenderIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IEnumerable<IDataObjectBase>>(e => e.FromFactory(() => new List<IDataObjectBase>(0)));

            fixture.Customize<IFilter>(e => e.FromFactory(() =>
                {
                    var filterMock = MockRepository.GenerateMock<IFilter>();
                    filterMock.Expect(m => m.Exclude(Arg<IEnumerable<IDataObjectBase>>.Is.NotNull))
                              .Return(true)
                              .Repeat.Any();
                    return filterMock;
                }));
            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.RecordFilters)
                     .Return(new ReadOnlyObservableCollection<IFilter>(new ObservableCollection<IFilter>(fixture.CreateMany<IFilter>(5).ToList())))
                     .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => tableMock));

            var oracleClientMock = MockRepository.GenerateMock<IOracleClient>();

            oracleClientMock.Expect(m => m.GetData(Arg<ITable>.Is.Equal(tableMock), Arg<DeliveryEngineEventHandler<IHandleDataEventArgs>>.Is.NotNull))
                            .WhenCalled(e =>
                                {
                                    var eventHandler = (DeliveryEngineEventHandler<IHandleDataEventArgs>) e.Arguments[1];
                                    eventHandler.Invoke(null, new HandleOracleDataEventArgs(e.Arguments[0] as ITable, fixture.CreateMany<IEnumerable<IDataObjectBase>>(250).ToList(), true));
                                })
                            .Repeat.Any();
            fixture.Customize<IOracleClient>(e => e.FromFactory(() => oracleClientMock));

            var oracleClientFactoryMock = MockRepository.GenerateMock<IOracleClientFactory>();
            oracleClientFactoryMock.Expect(m => m.Create())
                                   .Return(oracleClientMock)
                                   .Repeat.Any();
            fixture.Customize<IOracleClientFactory>(e => e.FromFactory(() => oracleClientFactoryMock));

            var dataManipulatorsMock = MockRepository.GenerateMock<IDataManipulators>();
            dataManipulatorsMock.Expect(m => m.ManipulateData(Arg<ITable>.Is.NotNull, Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull, Arg<bool>.Is.Anything))
                                .WhenCalled(e => e.ReturnValue = e.Arguments[1])
                                .Repeat.Any();
            fixture.Customize<IDataManipulators>(e => e.FromFactory(() => dataManipulatorsMock));

            var dataRepository = new OracleDataRepository(fixture.CreateAnonymous<IOracleClientFactory>(), fixture.CreateAnonymous<IDataManipulators>());
            Assert.That(dataRepository, Is.Not.Null);

            var exeption =Assert.Throws<DeliveryEngineRepositoryException>(() => dataRepository.DataGetFromTable(fixture.CreateAnonymous<ITable>()));
            Assert.That(exeption, Is.Not.Null);
            Assert.That(exeption.InnerException, Is.Not.Null);
            Assert.That(exeption.InnerException, Is.TypeOf<ArgumentNullException>());

            oracleClientFactoryMock.AssertWasCalled(m => m.Create());
            oracleClientMock.AssertWasCalled(m => m.ValidateTable(Arg<ITable>.Is.Equal(tableMock)));
            oracleClientMock.AssertWasCalled(m => m.GetData(Arg<ITable>.Is.Equal(tableMock), Arg<DeliveryEngineEventHandler<IHandleDataEventArgs>>.Is.NotNull));
            oracleClientMock.AssertWasCalled(m => m.Dispose());

            tableMock.AssertWasNotCalled(m => m.RecordFilters);

            dataManipulatorsMock.AssertWasNotCalled(m => m.ManipulateData(Arg<ITable>.Is.Equal(tableMock), Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull, Arg<bool>.Is.Anything));
        }

        /// <summary>
        /// Tests that OnHandleOracleData throws an ArgumentNullException if the arguments to the event handler is null.
        /// </summary>
        [Test]
        public void TestThatOnHandleOracleDataThrowsArgumentNullExceptionIfEventArgsIsNull()
        {
            var fixture = new Fixture();

            fixture.Customize<IFilter>(e => e.FromFactory(() =>
                {
                    var filterMock = MockRepository.GenerateMock<IFilter>();
                    filterMock.Expect(m => m.Exclude(Arg<IEnumerable<IDataObjectBase>>.Is.NotNull))
                              .Return(true)
                              .Repeat.Any();
                    return filterMock;
                }));
            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.RecordFilters)
                     .Return(new ReadOnlyObservableCollection<IFilter>(new ObservableCollection<IFilter>(fixture.CreateMany<IFilter>(5).ToList())))
                     .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => tableMock));

            var oracleClientMock = MockRepository.GenerateMock<IOracleClient>();

            oracleClientMock.Expect(m => m.GetData(Arg<ITable>.Is.Equal(tableMock), Arg<DeliveryEngineEventHandler<IHandleDataEventArgs>>.Is.NotNull))
                            .WhenCalled(e =>
                                {
                                    var eventHandler = (DeliveryEngineEventHandler<IHandleDataEventArgs>) e.Arguments[1];
                                    eventHandler.Invoke(this, null);
                                })
                            .Repeat.Any();
            fixture.Customize<IOracleClient>(e => e.FromFactory(() => oracleClientMock));

            var oracleClientFactoryMock = MockRepository.GenerateMock<IOracleClientFactory>();
            oracleClientFactoryMock.Expect(m => m.Create())
                                   .Return(oracleClientMock)
                                   .Repeat.Any();
            fixture.Customize<IOracleClientFactory>(e => e.FromFactory(() => oracleClientFactoryMock));

            var dataManipulatorsMock = MockRepository.GenerateMock<IDataManipulators>();
            dataManipulatorsMock.Expect(m => m.ManipulateData(Arg<ITable>.Is.NotNull, Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull, Arg<bool>.Is.Anything))
                                .WhenCalled(e => e.ReturnValue = e.Arguments[1])
                                .Repeat.Any();
            fixture.Customize<IDataManipulators>(e => e.FromFactory(() => dataManipulatorsMock));

            var dataRepository = new OracleDataRepository(fixture.CreateAnonymous<IOracleClientFactory>(), fixture.CreateAnonymous<IDataManipulators>());
            Assert.That(dataRepository, Is.Not.Null);

            var exeption = Assert.Throws<DeliveryEngineRepositoryException>(() => dataRepository.DataGetFromTable(fixture.CreateAnonymous<ITable>()));
            Assert.That(exeption, Is.Not.Null);
            Assert.That(exeption.InnerException, Is.Not.Null);
            Assert.That(exeption.InnerException, Is.TypeOf<ArgumentNullException>());

            oracleClientFactoryMock.AssertWasCalled(m => m.Create());
            oracleClientMock.AssertWasCalled(m => m.ValidateTable(Arg<ITable>.Is.Equal(tableMock)));
            oracleClientMock.AssertWasCalled(m => m.GetData(Arg<ITable>.Is.Equal(tableMock), Arg<DeliveryEngineEventHandler<IHandleDataEventArgs>>.Is.NotNull));
            oracleClientMock.AssertWasCalled(m => m.Dispose());

            tableMock.AssertWasNotCalled(m => m.RecordFilters);

            dataManipulatorsMock.AssertWasNotCalled(m => m.ManipulateData(Arg<ITable>.Is.Equal(tableMock), Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull, Arg<bool>.Is.Anything));
        }

        /// <summary>
        /// Test that Clone clones data repository.
        /// </summary>
        [Test]
        public void TestThatCloneClonesDataRepository()
        {
            var fixture = new Fixture();
            fixture.Customize<IOracleClientFactory>(e => e.FromFactory(() => MockRepository.GenerateMock<IOracleClientFactory>()));
            fixture.Customize<IDataManipulators>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataManipulators>()));

            var dataRepository = new OracleDataRepository(fixture.CreateAnonymous<IOracleClientFactory>(), fixture.CreateAnonymous<IDataManipulators>());
            Assert.That(dataRepository, Is.Not.Null);

            var clonedRepository = dataRepository.Clone() as IDataRepository;
            Assert.That(clonedRepository, Is.Not.Null);
            Assert.That(clonedRepository, Is.Not.EqualTo(dataRepository));
            Assert.That(clonedRepository, Is.TypeOf<OracleDataRepository>());
        }

        /// <summary>
        /// Test that Clone raise the OnClone event.
        /// </summary>
        [Test]
        public void TestThatCloneRaiseOnCloneEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<IOracleClientFactory>(e => e.FromFactory(() => MockRepository.GenerateMock<IOracleClientFactory>()));
            fixture.Customize<IDataManipulators>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataManipulators>()));

            var dataRepository = new OracleDataRepository(fixture.CreateAnonymous<IOracleClientFactory>(), fixture.CreateAnonymous<IDataManipulators>());
            Assert.That(dataRepository, Is.Not.Null);

            var eventCalled = false;
            dataRepository.OnClone += (sender, eventArgs) =>
                {
                    Assert.That(sender, Is.Not.Null);
                    Assert.That(eventArgs, Is.Not.Null);
                    Assert.That(eventArgs.ClonedDataRepository, Is.Not.Null);
                    eventCalled = true;
                };

            Assert.That(eventCalled, Is.False);
            var clonedRepository = dataRepository.Clone() as IDataRepository;
            Assert.That(clonedRepository, Is.Not.Null);
            Assert.That(eventCalled, Is.True);
        }
    }
}
