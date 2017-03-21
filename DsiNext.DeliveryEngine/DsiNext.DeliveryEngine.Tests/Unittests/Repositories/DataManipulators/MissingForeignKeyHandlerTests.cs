using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Domstolene.JFS.CommonLibrary.IoC.Interfaces;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.DataManipulators;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Repositories.DataManipulators
{
    /// <summary>
    /// Tests the data manipulator to handle missing foreign keys.
    /// </summary>
    [TestFixture]
    public class MissingForeignKeyHandlerTests
    {
        /// <summary>
        /// Tests that the constructor initialize a data manipulator to handle missing foreign keys.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeMissingForeignKeyHandler()
        {
            var fixture = new Fixture();
            fixture.Customize<IMissingForeignKeyWorker>(e => e.FromFactory(() => MockRepository.GenerateMock<IMissingForeignKeyWorker>()));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));

            var containerMock = MockRepository.GenerateMock<IContainer>();
            containerMock.Expect(m => m.Resolve<IMetadataRepository>())
                         .Return(metadataRepositoryMock)
                         .Repeat.Any();
            containerMock.Expect(m => m.Resolve<IDataRepository>())
                         .Return(fixture.CreateAnonymous<IDataRepository>())
                         .Repeat.Any();
            fixture.Customize<IContainer>(e => e.FromFactory(() => containerMock));

            var tableName = fixture.CreateAnonymous<string>();
            var worker = fixture.CreateAnonymous<IMissingForeignKeyWorker>();
            var missingForeignKeyHandler = new MissingForeignKeyHandler(tableName, worker, fixture.CreateAnonymous<IContainer>());
            Assert.That(missingForeignKeyHandler, Is.Not.Null);
            Assert.That(missingForeignKeyHandler.TableName, Is.Not.Null);
            Assert.That(missingForeignKeyHandler.TableName, Is.Not.Empty);
            Assert.That(missingForeignKeyHandler.TableName, Is.EqualTo(tableName));
            Assert.That(missingForeignKeyHandler.DataRepository, Is.Not.Null);
            Assert.That(missingForeignKeyHandler.DataRepository, Is.EqualTo(dataRepositoryMock));
            Assert.That(missingForeignKeyHandler.MetadataRepository, Is.Not.Null);
            Assert.That(missingForeignKeyHandler.MetadataRepository, Is.EqualTo(metadataRepositoryMock));
            Assert.That(missingForeignKeyHandler.Worker, Is.Not.Null);
            Assert.That(missingForeignKeyHandler.Worker, Is.EqualTo(worker));

            containerMock.AssertWasCalled(m => m.Resolve<IDataRepository>());
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if name of the table is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfTableNameIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IMissingForeignKeyWorker>(e => e.FromFactory(() => MockRepository.GenerateMock<IMissingForeignKeyWorker>()));
            fixture.Customize<IContainer>(e => e.FromFactory(() => MockRepository.GenerateMock<IContainer>()));

            Assert.Throws<ArgumentNullException>(() => new MissingForeignKeyHandler(null, fixture.CreateAnonymous<IMissingForeignKeyWorker>(), fixture.CreateAnonymous<IContainer>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if name of the table is empty.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfTableNameIsEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize<IMissingForeignKeyWorker>(e => e.FromFactory(() => MockRepository.GenerateMock<IMissingForeignKeyWorker>()));
            fixture.Customize<IContainer>(e => e.FromFactory(() => MockRepository.GenerateMock<IContainer>()));

            Assert.Throws<ArgumentNullException>(() => new MissingForeignKeyHandler(string.Empty, fixture.CreateAnonymous<IMissingForeignKeyWorker>(), fixture.CreateAnonymous<IContainer>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the worker to manipulate missing foreign key values is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfWorkerIsEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize<IContainer>(e => e.FromFactory(() => MockRepository.GenerateMock<IContainer>()));

            Assert.Throws<ArgumentNullException>(() => new MissingForeignKeyHandler(fixture.CreateAnonymous<string>(), null, fixture.CreateAnonymous<IContainer>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the container for Inversion of Control is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfContainerIsEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize<IMissingForeignKeyWorker>(e => e.FromFactory(() => MockRepository.GenerateMock<IMissingForeignKeyWorker>()));

            Assert.Throws<ArgumentNullException>(() => new MissingForeignKeyHandler(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<IMissingForeignKeyWorker>(), null));
        }

        /// <summary>
        /// Tests that ManipulateData returns without manipulating data if source name of the table does not match table name in the constructor.
        /// </summary>
        [Test]
        public void TestThatManipulateDataReturnsWithoutManipulatingDataIfTableSourceNameDoesNotMatchTableNameInTheConstructor()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (int)));
            fixture.Customize<IContainer>(e => e.FromFactory(() => MockRepository.GenerateMock<IContainer>()));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return fieldMock;
                }));

            var workerMock = MockRepository.GenerateMock<IMissingForeignKeyWorker>();
            fixture.Customize<IMissingForeignKeyWorker>(e => e.FromFactory(() => workerMock));

            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            tableMock.Expect(m => m.Fields)
                     .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(fixture.CreateMany<IField>(5).ToList())))
                     .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => tableMock));

            var dataCollectionMock = new Collection<IEnumerable<IDataObjectBase>>();
            while (dataCollectionMock.Count < 250)
            {
                var dataObjects = new Collection<IDataObjectBase>();
                for (var i = 0; i < tableMock.Fields.Count; i++)
                {
                    dataObjects.Add(MockRepository.GenerateMock<IFieldData<int, int>>());
                }
                dataCollectionMock.Add(dataObjects);
            }

            var missingForeignKeyHandler = new MissingForeignKeyHandler(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<IMissingForeignKeyWorker>(), fixture.CreateAnonymous<IContainer>());
            Assert.That(missingForeignKeyHandler, Is.Not.Null);

            var manipulatedData = missingForeignKeyHandler.ManipulateData(fixture.CreateAnonymous<ITable>(), dataCollectionMock);
            Assert.That(manipulatedData, Is.Not.Null);
            Assert.That(manipulatedData, Is.Not.Empty);
            Assert.That(manipulatedData.Count(), Is.EqualTo(dataCollectionMock.Count));

            workerMock.AssertWasNotCalled(m => m.ManipulateData(Arg<ITable>.Is.Anything, Arg<IDataRepository>.Is.Anything, Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.Anything));
        }

        /// <summary>
        /// Tests that ManipulateData returns data from the worker which manipulates missing foreign key values.
        /// </summary>
        [Test]
        public void TestThatManipulateDataReturnsDataFromWorkerWhichManipulatesMissingForeignKeyValues()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (int)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return fieldMock;
                }));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));

            var containerMock = MockRepository.GenerateMock<IContainer>();
            containerMock.Expect(m => m.Resolve<IDataRepository>())
                         .Return(fixture.CreateAnonymous<IDataRepository>())
                         .Repeat.Any();
            fixture.Customize<IContainer>(e => e.FromFactory(() => containerMock));

            var workerMock = MockRepository.GenerateMock<IMissingForeignKeyWorker>();
            workerMock.Expect(m => m.ManipulateData(Arg<ITable>.Is.NotNull, Arg<IDataRepository>.Is.NotNull, Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull))
                      .Return(new Collection<IEnumerable<IDataObjectBase>>())
                      .Repeat.Any();
            fixture.Customize<IMissingForeignKeyWorker>(e => e.FromFactory(() => workerMock));

            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            tableMock.Expect(m => m.Fields)
                     .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(fixture.CreateMany<IField>(5).ToList())))
                     .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<ITable>(e => e.FromFactory(() => tableMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var dataCollectionMock = new Collection<IEnumerable<IDataObjectBase>>();
            while (dataCollectionMock.Count < 250)
            {
                var dataObjects = new Collection<IDataObjectBase>();
                for (var i = 0; i < tableMock.Fields.Count; i++)
                {
                    dataObjects.Add(MockRepository.GenerateMock<IFieldData<int, int>>());
                }
                dataCollectionMock.Add(dataObjects);
            }

            var missingForeignKeyHandler = new MissingForeignKeyHandler(tableMock.NameSource, fixture.CreateAnonymous<IMissingForeignKeyWorker>(), fixture.CreateAnonymous<IContainer>());
            Assert.That(missingForeignKeyHandler, Is.Not.Null);

            var manipulatedData = missingForeignKeyHandler.ManipulateData(fixture.CreateAnonymous<ITable>(), dataCollectionMock);
            Assert.That(manipulatedData, Is.Not.Null);
            Assert.That(manipulatedData, Is.Empty);

            workerMock.AssertWasCalled(m => m.ManipulateData(Arg<ITable>.Is.Equal(tableMock), Arg<IDataRepository>.Is.Equal(dataRepositoryMock), Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.Equal(dataCollectionMock)));
        }

        /// <summary>
        /// Tests that FinalizeDataManipulation returns without finalizing data manipulation if source name of the table does not match table name in the constructor.
        /// </summary>
        [Test]
        public void TestThatFinalizeDataManipulationReturnsWithoutFinalizingDataManipulationIfTableSourceNameDoesNotMatchTableNameInTheConstructor()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (int)));
            fixture.Customize<IContainer>(e => e.FromFactory(() => MockRepository.GenerateMock<IContainer>()));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return fieldMock;
                }));

            var workerMock = MockRepository.GenerateMock<IMissingForeignKeyWorker>();
            fixture.Customize<IMissingForeignKeyWorker>(e => e.FromFactory(() => workerMock));

            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            tableMock.Expect(m => m.Fields)
                     .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(fixture.CreateMany<IField>(5).ToList())))
                     .Repeat.Any();
            fixture.Customize<ITable>(e => e.FromFactory(() => tableMock));

            var dataCollectionMock = new Collection<IEnumerable<IDataObjectBase>>();
            while (dataCollectionMock.Count < 250)
            {
                var dataObjects = new Collection<IDataObjectBase>();
                for (var i = 0; i < tableMock.Fields.Count; i++)
                {
                    dataObjects.Add(MockRepository.GenerateMock<IFieldData<int, int>>());
                }
                dataCollectionMock.Add(dataObjects);
            }

            var missingForeignKeyHandler = new MissingForeignKeyHandler(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<IMissingForeignKeyWorker>(), fixture.CreateAnonymous<IContainer>());
            Assert.That(missingForeignKeyHandler, Is.Not.Null);

            var manipulatedData = missingForeignKeyHandler.FinalizeDataManipulation(fixture.CreateAnonymous<ITable>(), dataCollectionMock);
            Assert.That(manipulatedData, Is.Not.Null);
            Assert.That(manipulatedData, Is.Not.Empty);
            Assert.That(manipulatedData.Count(), Is.EqualTo(dataCollectionMock.Count));

            workerMock.AssertWasNotCalled(m => m.FinalizeDataManipulation(Arg<ITable>.Is.Anything, Arg<IDataRepository>.Is.Anything, Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.Anything));
        }

        /// <summary>
        /// Tests that FinalizeDataManipulation returns finalized data from the worker which manipulates missing foreign key values.
        /// </summary>
        [Test]
        public void TestThatFinalizeDataManipulationReturnsFinalizedDataFromWorkerWhichManipulatesMissingForeignKeyValues()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (int)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return fieldMock;
                }));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));

            var containerMock = MockRepository.GenerateMock<IContainer>();
            containerMock.Expect(m => m.Resolve<IDataRepository>())
                         .Return(fixture.CreateAnonymous<IDataRepository>())
                         .Repeat.Any();
            fixture.Customize<IContainer>(e => e.FromFactory(() => containerMock));

            var workerMock = MockRepository.GenerateMock<IMissingForeignKeyWorker>();
            workerMock.Expect(m => m.FinalizeDataManipulation(Arg<ITable>.Is.NotNull, Arg<IDataRepository>.Is.NotNull, Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.NotNull))
                      .Return(new Collection<IEnumerable<IDataObjectBase>>())
                      .Repeat.Any();
            fixture.Customize<IMissingForeignKeyWorker>(e => e.FromFactory(() => workerMock));

            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            tableMock.Expect(m => m.Fields)
                     .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(fixture.CreateMany<IField>(5).ToList())))
                     .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<ITable>(e => e.FromFactory(() => tableMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var dataCollectionMock = new Collection<IEnumerable<IDataObjectBase>>();
            while (dataCollectionMock.Count < 250)
            {
                var dataObjects = new Collection<IDataObjectBase>();
                for (var i = 0; i < tableMock.Fields.Count; i++)
                {
                    dataObjects.Add(MockRepository.GenerateMock<IFieldData<int, int>>());
                }
                dataCollectionMock.Add(dataObjects);
            }

            var missingForeignKeyHandler = new MissingForeignKeyHandler(tableMock.NameSource, fixture.CreateAnonymous<IMissingForeignKeyWorker>(), fixture.CreateAnonymous<IContainer>());
            Assert.That(missingForeignKeyHandler, Is.Not.Null);

            var manipulatedData = missingForeignKeyHandler.FinalizeDataManipulation(fixture.CreateAnonymous<ITable>(), dataCollectionMock);
            Assert.That(manipulatedData, Is.Not.Null);
            Assert.That(manipulatedData, Is.Empty);

            workerMock.AssertWasCalled(m => m.FinalizeDataManipulation(Arg<ITable>.Is.Equal(tableMock), Arg<IDataRepository>.Is.Equal(dataRepositoryMock), Arg<IEnumerable<IEnumerable<IDataObjectBase>>>.Is.Equal(dataCollectionMock)));
        }

        /// <summary>
        /// Test that IsManipulatingField calls and returns the result from IsManipulatingField on the worker.
        /// </summary>
        [Test]
        public void TestThatIsManipulatingFieldCallsAndReturnsResultFromIsManipulatingFieldOnWorker()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));
            fixture.Customize<IDataSource>(e => e.FromFactory(() =>
                {
                    var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
                    dataSourceMock.Expect(m => m.Tables)
                                  .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                                  .Repeat.Any();
                    return dataSourceMock;
                }));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var containerMock = MockRepository.GenerateMock<IContainer>();
            containerMock.Expect(m => m.Resolve<IMetadataRepository>())
                         .Return(metadataRepositoryMock)
                         .Repeat.Any();
            fixture.Customize<IContainer>(e => e.FromFactory(() => containerMock));

            var workerMock = MockRepository.GenerateMock<IMissingForeignKeyWorker>();
            workerMock.Expect(m => m.IsManipulatingField(Arg<string>.Is.NotNull, Arg<ITable>.Is.NotNull))
                      .Return(false)
                      .Repeat.Any();
            fixture.Customize<IMissingForeignKeyWorker>(e => e.FromFactory(() => workerMock));

            var missingForeignKeyHandler = new MissingForeignKeyHandler(metadataRepositoryMock.DataSourceGet().Tables.ElementAt(1).NameTarget, fixture.CreateAnonymous<IMissingForeignKeyWorker>(), fixture.CreateAnonymous<IContainer>());
            Assert.That(missingForeignKeyHandler, Is.Not.Null);
            
            // ReSharper disable PossibleNullReferenceException
            missingForeignKeyHandler.GetType().GetField("_currentDataSource", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static).SetValue(missingForeignKeyHandler, null);
            // ReSharper restore PossibleNullReferenceException
            Assert.That(missingForeignKeyHandler.IsManipulatingField(fixture.CreateAnonymous<string>()), Is.False);

            containerMock.AssertWasCalled(m => m.Resolve<IMetadataRepository>());
            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
            workerMock.AssertWasCalled(m => m.IsManipulatingField(Arg<string>.Is.NotNull, Arg<ITable>.Is.NotNull));
        }

        /// <summary>
        /// Test that IsManipulatingField throws an DeliveryEngineSystemException if the table is not found.
        /// </summary>
        [Test]
        public void TestThatIsManipulatingFieldThrowsDeliveryEngineSystemExceptionIfTableIfNotFound()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));
            fixture.Customize<IDataSource>(e => e.FromFactory(() =>
                {
                    var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
                    dataSourceMock.Expect(m => m.Tables)
                                  .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                                  .Repeat.Any();
                    return dataSourceMock;
                }));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var containerMock = MockRepository.GenerateMock<IContainer>();
            containerMock.Expect(m => m.Resolve<IMetadataRepository>())
                         .Return(metadataRepositoryMock)
                         .Repeat.Any();
            fixture.Customize<IContainer>(e => e.FromFactory(() => containerMock));

            var workerMock = MockRepository.GenerateMock<IMissingForeignKeyWorker>();
            workerMock.Expect(m => m.IsManipulatingField(Arg<string>.Is.NotNull, Arg<ITable>.Is.NotNull))
                      .Return(false)
                      .Repeat.Any();
            fixture.Customize<IMissingForeignKeyWorker>(e => e.FromFactory(() => workerMock));

            var missingForeignKeyHandler = new MissingForeignKeyHandler(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<IMissingForeignKeyWorker>(), fixture.CreateAnonymous<IContainer>());
            Assert.That(missingForeignKeyHandler, Is.Not.Null);

            // ReSharper disable PossibleNullReferenceException
            missingForeignKeyHandler.GetType().GetField("_currentDataSource", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static).SetValue(missingForeignKeyHandler, null);
            // ReSharper restore PossibleNullReferenceException
            Assert.Throws<DeliveryEngineSystemException>(() => missingForeignKeyHandler.IsManipulatingField(fixture.CreateAnonymous<string>()));

            containerMock.AssertWasCalled(m => m.Resolve<IMetadataRepository>());
            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
            workerMock.AssertWasNotCalled(m => m.IsManipulatingField(Arg<string>.Is.Anything, Arg<ITable>.Is.Anything));
        }
    }
}
