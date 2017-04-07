using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Commands;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.DataValidators;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.ExceptionHandling;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Events;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.BusinessLogic
{
    /// <summary>
    /// Tests business logic for the delivery engine.
    /// </summary>
    [TestFixture]
    public class DeliveryEngineTests
    {
        /// <summary>
        /// Test that the constructor initialize the delivery engine.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeDeliveryEngine()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataSource>()));
            fixture.Customize<IConfigurationRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IConfigurationRepository>()));
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IMetadataRepository>()));
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));
            fixture.Customize<IDocumentRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDocumentRepository>()));
            fixture.Customize<IDataValidators>(e => e.FromFactory(() =>
                {
                    var mocker = new MockRepository();
                    var mock = mocker.DynamicMultiMock<IDataValidators>(new[] {typeof (IEnumerable)});
                    Expect.Call(((IEnumerable) mock).GetEnumerator())
                          .Return(new List<IDataValidator>(0).GetEnumerator())
                          .Repeat.Any();
                    mocker.ReplayAll();
                    return mock;
                }));
            fixture.Customize<IArchiveVersionRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IArchiveVersionRepository>()));
            fixture.Customize<IExceptionHandler>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionHandler>()));

            var configurationRepositoryMock = fixture.CreateAnonymous<IConfigurationRepository>();
            var metadataRepositoryMock = fixture.CreateAnonymous<IMetadataRepository>();
            var dataRepositoryMock = fixture.CreateAnonymous<IDataRepository>();
            var documentRepositoryMock = fixture.CreateAnonymous<IDocumentRepository>();
            var dataValidatorsMock = fixture.CreateAnonymous<IDataValidators>();
            var archiveRepositoryMock = fixture.CreateAnonymous<IArchiveVersionRepository>();
            var exceptionHandlerMock = fixture.CreateAnonymous<IExceptionHandler>();

            var deliveryEngine = new DeliveryEngine.BusinessLogic.DeliveryEngine(configurationRepositoryMock, metadataRepositoryMock, dataRepositoryMock, documentRepositoryMock, dataValidatorsMock, archiveRepositoryMock, exceptionHandlerMock);
            Assert.That(deliveryEngine, Is.Not.Null);
            Assert.That(deliveryEngine.ConfigurationRepository, Is.Not.Null);
            Assert.That(deliveryEngine.ConfigurationRepository, Is.EqualTo(configurationRepositoryMock));
            Assert.That(deliveryEngine.MetadataRepository, Is.Not.Null);
            Assert.That(deliveryEngine.MetadataRepository, Is.EqualTo(metadataRepositoryMock));
            Assert.That(deliveryEngine.DataRepository, Is.Not.Null);
            Assert.That(deliveryEngine.DataRepository, Is.EqualTo(dataRepositoryMock));
            Assert.That(deliveryEngine.DocumentRepository, Is.Not.Null);
            Assert.That(deliveryEngine.DocumentRepository, Is.EqualTo(documentRepositoryMock));
            Assert.That(deliveryEngine.DataValidators, Is.Not.Null);
            Assert.That(deliveryEngine.DataValidators, Is.EqualTo(dataValidatorsMock));
            Assert.That(deliveryEngine.ArchiveVersionRepository, Is.Not.Null);
            Assert.That(deliveryEngine.ArchiveVersionRepository, Is.EqualTo(archiveRepositoryMock));
            Assert.That(deliveryEngine.ExceptionHandler, Is.Not.Null);
            Assert.That(deliveryEngine.ExceptionHandler, Is.EqualTo(exceptionHandlerMock));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the configuration repository is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfConfigurationRepositoryIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataSource>()));
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IMetadataRepository>()));
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));
            fixture.Customize<IDocumentRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDocumentRepository>()));
            fixture.Customize<IDataValidators>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataValidators>()));
            fixture.Customize<IArchiveVersionRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IArchiveVersionRepository>()));
            fixture.Customize<IExceptionHandler>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionHandler>()));

            Assert.Throws<ArgumentNullException>(() => new DeliveryEngine.BusinessLogic.DeliveryEngine(null, fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IDataRepository>(), fixture.CreateAnonymous<IDocumentRepository>(), fixture.CreateAnonymous<IDataValidators>(), fixture.CreateAnonymous<IArchiveVersionRepository>(), fixture.CreateAnonymous<IExceptionHandler>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the metadata repository is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfMetadataRepositoryIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataSource>()));
            fixture.Customize<IConfigurationRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IConfigurationRepository>()));
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));
            fixture.Customize<IDocumentRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDocumentRepository>()));
            fixture.Customize<IDataValidators>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataValidators>()));
            fixture.Customize<IArchiveVersionRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IArchiveVersionRepository>()));
            fixture.Customize<IExceptionHandler>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionHandler>()));

            Assert.Throws<ArgumentNullException>(() => new DeliveryEngine.BusinessLogic.DeliveryEngine(fixture.CreateAnonymous<IConfigurationRepository>(), null, fixture.CreateAnonymous<IDataRepository>(), fixture.CreateAnonymous<IDocumentRepository>(), fixture.CreateAnonymous<IDataValidators>(), fixture.CreateAnonymous<IArchiveVersionRepository>(), fixture.CreateAnonymous<IExceptionHandler>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the data repository is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfDataRepositoryIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataSource>()));
            fixture.Customize<IConfigurationRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IConfigurationRepository>()));
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IMetadataRepository>()));
            fixture.Customize<IDocumentRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDocumentRepository>()));
            fixture.Customize<IDataValidators>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataValidators>()));
            fixture.Customize<IArchiveVersionRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IArchiveVersionRepository>()));
            fixture.Customize<IExceptionHandler>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionHandler>()));

            Assert.Throws<ArgumentNullException>(() => new DeliveryEngine.BusinessLogic.DeliveryEngine(fixture.CreateAnonymous<IConfigurationRepository>(), fixture.CreateAnonymous<IMetadataRepository>(), null, fixture.CreateAnonymous<IDocumentRepository>(), fixture.CreateAnonymous<IDataValidators>(), fixture.CreateAnonymous<IArchiveVersionRepository>(), fixture.CreateAnonymous<IExceptionHandler>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the document repository is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfDocumentRepositoryIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataSource>()));
            fixture.Customize<IConfigurationRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IConfigurationRepository>()));
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IMetadataRepository>()));
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));
            fixture.Customize<IDataValidators>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataValidators>()));
            fixture.Customize<IArchiveVersionRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IArchiveVersionRepository>()));
            fixture.Customize<IExceptionHandler>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionHandler>()));

            Assert.Throws<ArgumentNullException>(() => new DeliveryEngine.BusinessLogic.DeliveryEngine(fixture.CreateAnonymous<IConfigurationRepository>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IDataRepository>(), null, fixture.CreateAnonymous<IDataValidators>(), fixture.CreateAnonymous<IArchiveVersionRepository>(), fixture.CreateAnonymous<IExceptionHandler>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the collection of data validators is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfDataValidatorsIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataSource>()));
            fixture.Customize<IConfigurationRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IConfigurationRepository>()));
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IMetadataRepository>()));
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));
            fixture.Customize<IDocumentRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDocumentRepository>()));
            fixture.Customize<IArchiveVersionRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IArchiveVersionRepository>()));
            fixture.Customize<IExceptionHandler>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionHandler>()));

            Assert.Throws<ArgumentNullException>(() => new DeliveryEngine.BusinessLogic.DeliveryEngine(fixture.CreateAnonymous<IConfigurationRepository>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IDataRepository>(), fixture.CreateAnonymous<IDocumentRepository>(), null, fixture.CreateAnonymous<IArchiveVersionRepository>(), fixture.CreateAnonymous<IExceptionHandler>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the archive version repository is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfArchiveVersionRepositoryIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataSource>()));
            fixture.Customize<IConfigurationRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IConfigurationRepository>()));
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IMetadataRepository>()));
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));
            fixture.Customize<IDocumentRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDocumentRepository>()));
            fixture.Customize<IDataValidators>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataValidators>()));
            fixture.Customize<IExceptionHandler>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionHandler>()));

            Assert.Throws<ArgumentNullException>(() => new DeliveryEngine.BusinessLogic.DeliveryEngine(fixture.CreateAnonymous<IConfigurationRepository>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IDataRepository>(), fixture.CreateAnonymous<IDocumentRepository>(), fixture.CreateAnonymous<IDataValidators>(), null, fixture.CreateAnonymous<IExceptionHandler>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the exception handler is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfExceptionHandlerIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataSource>()));
            fixture.Customize<IConfigurationRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IConfigurationRepository>()));
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IMetadataRepository>()));
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));
            fixture.Customize<IDocumentRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDocumentRepository>()));
            fixture.Customize<IDataValidators>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataValidators>()));
            fixture.Customize<IArchiveVersionRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IArchiveVersionRepository>()));

            Assert.Throws<ArgumentNullException>(() => new DeliveryEngine.BusinessLogic.DeliveryEngine(fixture.CreateAnonymous<IConfigurationRepository>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IDataRepository>(), fixture.CreateAnonymous<IDocumentRepository>(), fixture.CreateAnonymous<IDataValidators>(), fixture.CreateAnonymous<IArchiveVersionRepository>(), null));
        }

        /// <summary>
        /// Test that Execute throws an ArgumentNullException if the exception the execute command is null.
        /// </summary>
        [Test]
        public void TestThatExecuteThrowsArgumentNullExceptionIfDeliveryEngineExecuteCommandIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataSource>()));
            fixture.Customize<IConfigurationRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IConfigurationRepository>()));
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IMetadataRepository>()));
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));
            fixture.Customize<IDocumentRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDocumentRepository>()));
            fixture.Customize<IDataValidators>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataValidators>()));
            fixture.Customize<IArchiveVersionRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IArchiveVersionRepository>()));
            fixture.Customize<IExceptionHandler>(e => e.FromFactory(() => MockRepository.GenerateMock<IExceptionHandler>()));

            var deliveryEngine = new DeliveryEngine.BusinessLogic.DeliveryEngine(fixture.CreateAnonymous<IConfigurationRepository>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IDataRepository>(), fixture.CreateAnonymous<IDocumentRepository>(), fixture.CreateAnonymous<IDataValidators>(), fixture.CreateAnonymous<IArchiveVersionRepository>(), fixture.CreateAnonymous<IExceptionHandler>());
            Assert.That(deliveryEngine, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => deliveryEngine.Execute(null));
        }

        /// <summary>
        /// Test that Execute gets a data source from the metadata repository.
        /// </summary>
        [Test]
        public void TestThatExecuteGetsDataSourceFromMetadataRepository()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IConfigurationRepository>()));
            fixture.Customize<IDocumentRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDocumentRepository>()));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>()))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));

            var mocker = new MockRepository();
            var dataValidatorsMock = mocker.DynamicMultiMock<IDataValidators>(new[] {typeof (IEnumerable<IDataValidator>)});
            Expect.Call(dataValidatorsMock.GetEnumerator())
                  .WhenCalled(e =>
                      {
                          var validators = new List<IDataValidator>(0);
                          e.ReturnValue = validators.GetEnumerator();
                      })
                  .Return(null)
                  .Repeat.Any();
            mocker.ReplayAll();
            fixture.Customize<IDataValidators>(e => e.FromFactory(() => dataValidatorsMock));

            var archiveVersionRepositoryMock = MockRepository.GenerateMock<IArchiveVersionRepository>();

            var exceptionHandlerMock = MockRepository.GenerateMock<IExceptionHandler>();
            fixture.Customize<IExceptionHandler>(e => e.FromFactory(() => exceptionHandlerMock));

            var deliveryEngine = new DeliveryEngine.BusinessLogic.DeliveryEngine(fixture.CreateAnonymous<IConfigurationRepository>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IDataRepository>(), fixture.CreateAnonymous<IDocumentRepository>(), fixture.CreateAnonymous<IDataValidators>(), archiveVersionRepositoryMock, fixture.CreateAnonymous<IExceptionHandler>());
            Assert.That(deliveryEngine, Is.Not.Null);

            var executeCommand = MockRepository.GenerateMock<IDeliveryEngineExecuteCommand>();
            executeCommand.Expect(m => m.OverrideArchiveInformationPackageId)
                          .Return(null)
                          .Repeat.Any();
            executeCommand.Expect(m => m.ValidationOnly)
                          .Return(false)
                          .Repeat.Any();
            fixture.Customize<IDeliveryEngineExecuteCommand>(e => e.FromFactory(() => executeCommand));

            deliveryEngine.Execute(fixture.CreateAnonymous<IDeliveryEngineExecuteCommand>());

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
            executeCommand.AssertWasCalled(m => m.OverrideArchiveInformationPackageId);
            archiveVersionRepositoryMock.AssertWasCalled(m => m.DataSource = Arg<IDataSource>.Is.Equal(dataSourceMock));

            executeCommand.AssertWasCalled(m => m.ValidationOnly);
            archiveVersionRepositoryMock.AssertWasCalled(m => m.ArchiveMetaData());

            dataSourceMock.AssertWasCalled(m => m.Tables);
            executeCommand.AssertWasNotCalled(m => m.Table);
            executeCommand.AssertWasNotCalled(m => m.TablesHandledSimultaneity);
            dataRepositoryMock.AssertWasNotCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Anything, Arg<IDataSource>.Is.Anything));
            dataValidatorsMock.AssertWasNotCalled(m => m.GetEnumerator());
            archiveVersionRepositoryMock.AssertWasNotCalled(m => m.ArchiveTableData(Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.Anything, Arg<object>.Is.Anything));

            exceptionHandlerMock.AssertWasNotCalled(m => m.HandleException(Arg<Exception>.Is.NotNull));
        }

        /// <summary>
        /// Test that Execute overrides the unique package ID for the archive in the data source.
        /// </summary>
        [Test]
        public void TestThatExecuteOverridesArchiveInformationPackageIdInDataSource()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IConfigurationRepository>()));
            fixture.Customize<IDocumentRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDocumentRepository>()));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>()))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));

            var mocker = new MockRepository();
            var dataValidatorsMock = mocker.DynamicMultiMock<IDataValidators>(new[] {typeof (IEnumerable<IDataValidator>)});
            Expect.Call(dataValidatorsMock.GetEnumerator())
                  .WhenCalled(e =>
                      {
                          var validators = new List<IDataValidator>(0);
                          e.ReturnValue = validators.GetEnumerator();
                      })
                  .Return(null)
                  .Repeat.Any();
            mocker.ReplayAll();
            fixture.Customize<IDataValidators>(e => e.FromFactory(() => dataValidatorsMock));

            var archiveVersionRepositoryMock = MockRepository.GenerateMock<IArchiveVersionRepository>();

            var exceptionHandlerMock = MockRepository.GenerateMock<IExceptionHandler>();
            fixture.Customize<IExceptionHandler>(e => e.FromFactory(() => exceptionHandlerMock));

            var deliveryEngine = new DeliveryEngine.BusinessLogic.DeliveryEngine(fixture.CreateAnonymous<IConfigurationRepository>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IDataRepository>(), fixture.CreateAnonymous<IDocumentRepository>(), fixture.CreateAnonymous<IDataValidators>(), archiveVersionRepositoryMock, fixture.CreateAnonymous<IExceptionHandler>());
            Assert.That(deliveryEngine, Is.Not.Null);

            var executeCommand = MockRepository.GenerateMock<IDeliveryEngineExecuteCommand>();
            executeCommand.Expect(m => m.OverrideArchiveInformationPackageId)
                          .Return(fixture.CreateAnonymous<string>())
                          .Repeat.Any();
            executeCommand.Expect(m => m.ValidationOnly)
                          .Return(false)
                          .Repeat.Any();
            fixture.Customize<IDeliveryEngineExecuteCommand>(e => e.FromFactory(() => executeCommand));

            deliveryEngine.Execute(fixture.CreateAnonymous<IDeliveryEngineExecuteCommand>());

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
            executeCommand.AssertWasCalled(m => m.OverrideArchiveInformationPackageId, opt => opt.Repeat.Times(2));
            dataSourceMock.AssertWasCalled(m => m.ArchiveInformationPackageId = Arg<string>.Is.Equal(executeCommand.OverrideArchiveInformationPackageId));
            archiveVersionRepositoryMock.AssertWasCalled(m => m.DataSource = Arg<IDataSource>.Is.Equal(dataSourceMock));

            executeCommand.AssertWasCalled(m => m.ValidationOnly);
            archiveVersionRepositoryMock.AssertWasCalled(m => m.ArchiveMetaData());

            dataSourceMock.AssertWasCalled(m => m.Tables);
            executeCommand.AssertWasNotCalled(m => m.Table);
            executeCommand.AssertWasNotCalled(m => m.TablesHandledSimultaneity);
            dataRepositoryMock.AssertWasNotCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Anything, Arg<IDataSource>.Is.Anything));
            dataValidatorsMock.AssertWasNotCalled(m => m.GetEnumerator());
            archiveVersionRepositoryMock.AssertWasNotCalled(m => m.ArchiveTableData(Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.Anything, Arg<object>.Is.Anything));

            exceptionHandlerMock.AssertWasNotCalled(m => m.HandleException(Arg<Exception>.Is.NotNull));
        }

        /// <summary>
        /// Test that Execute raises the event BeforeGetDataSource.
        /// </summary>
        [Test]
        public void TestThatExecuteRaisesBeforeGetDataSourceEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IConfigurationRepository>()));
            fixture.Customize<IDocumentRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDocumentRepository>()));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>()))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));

            var mocker = new MockRepository();
            var dataValidatorsMock = mocker.DynamicMultiMock<IDataValidators>(new[] {typeof (IEnumerable<IDataValidator>)});
            Expect.Call(dataValidatorsMock.GetEnumerator())
                  .WhenCalled(e =>
                      {
                          var validators = new List<IDataValidator>(0);
                          e.ReturnValue = validators.GetEnumerator();
                      })
                  .Return(null)
                  .Repeat.Any();
            mocker.ReplayAll();
            fixture.Customize<IDataValidators>(e => e.FromFactory(() => dataValidatorsMock));

            var archiveVersionRepositoryMock = MockRepository.GenerateMock<IArchiveVersionRepository>();

            var exceptionHandlerMock = MockRepository.GenerateMock<IExceptionHandler>();
            fixture.Customize<IExceptionHandler>(e => e.FromFactory(() => exceptionHandlerMock));

            var deliveryEngine = new DeliveryEngine.BusinessLogic.DeliveryEngine(fixture.CreateAnonymous<IConfigurationRepository>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IDataRepository>(), fixture.CreateAnonymous<IDocumentRepository>(), fixture.CreateAnonymous<IDataValidators>(), archiveVersionRepositoryMock, fixture.CreateAnonymous<IExceptionHandler>());
            Assert.That(deliveryEngine, Is.Not.Null);

            var eventCalled = false;
            deliveryEngine.BeforeGetDataSource += (s, e) =>
                {
                    Assert.That(s, Is.Not.Null);
                    Assert.That(e, Is.Not.Null);
                    eventCalled = true;
                };

            var executeCommand = MockRepository.GenerateMock<IDeliveryEngineExecuteCommand>();
            executeCommand.Expect(m => m.OverrideArchiveInformationPackageId)
                          .Return(null)
                          .Repeat.Any();
            executeCommand.Expect(m => m.ValidationOnly)
                          .Return(false)
                          .Repeat.Any();
            fixture.Customize<IDeliveryEngineExecuteCommand>(e => e.FromFactory(() => executeCommand));

            deliveryEngine.Execute(fixture.CreateAnonymous<IDeliveryEngineExecuteCommand>());
            Assert.That(eventCalled, Is.True);

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
            executeCommand.AssertWasCalled(m => m.OverrideArchiveInformationPackageId);
            archiveVersionRepositoryMock.AssertWasCalled(m => m.DataSource = Arg<IDataSource>.Is.Equal(dataSourceMock));

            executeCommand.AssertWasCalled(m => m.ValidationOnly);
            archiveVersionRepositoryMock.AssertWasCalled(m => m.ArchiveMetaData());

            dataSourceMock.AssertWasCalled(m => m.Tables);
            executeCommand.AssertWasNotCalled(m => m.Table);
            executeCommand.AssertWasNotCalled(m => m.TablesHandledSimultaneity);
            dataRepositoryMock.AssertWasNotCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Anything, Arg<IDataSource>.Is.Anything));
            dataValidatorsMock.AssertWasNotCalled(m => m.GetEnumerator());
            archiveVersionRepositoryMock.AssertWasNotCalled(m => m.ArchiveTableData(Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.Anything, Arg<object>.Is.Anything));

            exceptionHandlerMock.AssertWasNotCalled(m => m.HandleException(Arg<Exception>.Is.NotNull));
        }

        /// <summary>
        /// Test that Execute archives metadata in the data source.
        /// </summary>
        [Test]
        public void TestThatExecuteArchivesMetadataInDataSource()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IConfigurationRepository>()));
            fixture.Customize<IDocumentRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDocumentRepository>()));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>()))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));

            var mocker = new MockRepository();
            var dataValidatorsMock = mocker.DynamicMultiMock<IDataValidators>(new[] {typeof (IEnumerable<IDataValidator>)});
            Expect.Call(dataValidatorsMock.GetEnumerator())
                  .WhenCalled(e =>
                      {
                          var validators = new List<IDataValidator>(0);
                          e.ReturnValue = validators.GetEnumerator();
                      })
                  .Return(null)
                  .Repeat.Any();
            mocker.ReplayAll();
            fixture.Customize<IDataValidators>(e => e.FromFactory(() => dataValidatorsMock));

            var archiveVersionRepositoryMock = MockRepository.GenerateMock<IArchiveVersionRepository>();

            var exceptionHandlerMock = MockRepository.GenerateMock<IExceptionHandler>();
            fixture.Customize<IExceptionHandler>(e => e.FromFactory(() => exceptionHandlerMock));

            var deliveryEngine = new DeliveryEngine.BusinessLogic.DeliveryEngine(fixture.CreateAnonymous<IConfigurationRepository>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IDataRepository>(), fixture.CreateAnonymous<IDocumentRepository>(), fixture.CreateAnonymous<IDataValidators>(), archiveVersionRepositoryMock, fixture.CreateAnonymous<IExceptionHandler>());
            Assert.That(deliveryEngine, Is.Not.Null);

            var executeCommand = MockRepository.GenerateMock<IDeliveryEngineExecuteCommand>();
            executeCommand.Expect(m => m.OverrideArchiveInformationPackageId)
                          .Return(null)
                          .Repeat.Any();
            executeCommand.Expect(m => m.ValidationOnly)
                          .Return(false)
                          .Repeat.Any();
            fixture.Customize<IDeliveryEngineExecuteCommand>(e => e.FromFactory(() => executeCommand));

            deliveryEngine.Execute(fixture.CreateAnonymous<IDeliveryEngineExecuteCommand>());

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
            executeCommand.AssertWasCalled(m => m.OverrideArchiveInformationPackageId);
            archiveVersionRepositoryMock.AssertWasCalled(m => m.DataSource = Arg<IDataSource>.Is.Equal(dataSourceMock));

            executeCommand.AssertWasCalled(m => m.ValidationOnly);
            archiveVersionRepositoryMock.AssertWasCalled(m => m.ArchiveMetaData());

            dataSourceMock.AssertWasCalled(m => m.Tables);
            executeCommand.AssertWasNotCalled(m => m.Table);
            executeCommand.AssertWasNotCalled(m => m.TablesHandledSimultaneity);
            dataRepositoryMock.AssertWasNotCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Anything, Arg<IDataSource>.Is.Anything));
            dataValidatorsMock.AssertWasNotCalled(m => m.GetEnumerator());
            archiveVersionRepositoryMock.AssertWasNotCalled(m => m.ArchiveTableData(Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.Anything, Arg<object>.Is.Anything));

            exceptionHandlerMock.AssertWasNotCalled(m => m.HandleException(Arg<Exception>.Is.NotNull));
        }

        /// <summary>
        /// Test that Execute raises the event BeforeArchiveMetadata.
        /// </summary>
        [Test]
        public void TestThatExecuteRaisesBeforeArchiveMetadata()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IConfigurationRepository>()));
            fixture.Customize<IDocumentRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDocumentRepository>()));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>()))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));

            var mocker = new MockRepository();
            var dataValidatorsMock = mocker.DynamicMultiMock<IDataValidators>(new[] {typeof (IEnumerable<IDataValidator>)});
            Expect.Call(dataValidatorsMock.GetEnumerator())
                  .WhenCalled(e =>
                      {
                          var validators = new List<IDataValidator>(0);
                          e.ReturnValue = validators.GetEnumerator();
                      })
                  .Return(null)
                  .Repeat.Any();
            mocker.ReplayAll();
            fixture.Customize<IDataValidators>(e => e.FromFactory(() => dataValidatorsMock));

            var archiveVersionRepositoryMock = MockRepository.GenerateMock<IArchiveVersionRepository>();

            var exceptionHandlerMock = MockRepository.GenerateMock<IExceptionHandler>();
            fixture.Customize<IExceptionHandler>(e => e.FromFactory(() => exceptionHandlerMock));

            var deliveryEngine = new DeliveryEngine.BusinessLogic.DeliveryEngine(fixture.CreateAnonymous<IConfigurationRepository>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IDataRepository>(), fixture.CreateAnonymous<IDocumentRepository>(), fixture.CreateAnonymous<IDataValidators>(), archiveVersionRepositoryMock, fixture.CreateAnonymous<IExceptionHandler>());
            Assert.That(deliveryEngine, Is.Not.Null);

            var eventCalled = false;
            deliveryEngine.BeforeArchiveMetadata += (s, e) =>
                {
                    Assert.That(s, Is.Not.Null);
                    Assert.That(e, Is.Not.Null);
                    Assert.That(e.DataSource, Is.Not.Null);
                    Assert.That(e.DataSource, Is.EqualTo(dataSourceMock));
                    eventCalled = true;
                };

            var executeCommand = MockRepository.GenerateMock<IDeliveryEngineExecuteCommand>();
            executeCommand.Expect(m => m.OverrideArchiveInformationPackageId)
                          .Return(null)
                          .Repeat.Any();
            executeCommand.Expect(m => m.ValidationOnly)
                          .Return(false)
                          .Repeat.Any();
            fixture.Customize<IDeliveryEngineExecuteCommand>(e => e.FromFactory(() => executeCommand));

            deliveryEngine.Execute(fixture.CreateAnonymous<IDeliveryEngineExecuteCommand>());
            Assert.That(eventCalled, Is.True);

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
            executeCommand.AssertWasCalled(m => m.OverrideArchiveInformationPackageId);
            archiveVersionRepositoryMock.AssertWasCalled(m => m.DataSource = Arg<IDataSource>.Is.Equal(dataSourceMock));

            executeCommand.AssertWasCalled(m => m.ValidationOnly);
            archiveVersionRepositoryMock.AssertWasCalled(m => m.ArchiveMetaData());

            dataSourceMock.AssertWasCalled(m => m.Tables);
            executeCommand.AssertWasNotCalled(m => m.Table);
            executeCommand.AssertWasNotCalled(m => m.TablesHandledSimultaneity);
            dataRepositoryMock.AssertWasNotCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Anything, Arg<IDataSource>.Is.Anything));
            dataValidatorsMock.AssertWasNotCalled(m => m.GetEnumerator());
            archiveVersionRepositoryMock.AssertWasNotCalled(m => m.ArchiveTableData(Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.Anything, Arg<object>.Is.Anything));

            exceptionHandlerMock.AssertWasNotCalled(m => m.HandleException(Arg<Exception>.Is.NotNull));
        }

        /// <summary>
        /// Test that Execute gets data for each target table in the data source.
        /// </summary>
        [Test]
        public void TestThatExecuteGetsDataForEachTargetTableInDataSource()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IConfigurationRepository>()));
            fixture.Customize<IDocumentRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDocumentRepository>()));

            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.Expect(m => m.DataGetForTargetTable(Arg<string>.Is.NotNull, Arg<IDataSource>.Is.Equal(dataSourceMock)))
                              .WhenCalled(e =>
                                  {
                                      Thread.Sleep(2500);
                                      var eventArgsMock = MockRepository.GenerateMock<IHandleDataEventArgs>();
                                      eventArgsMock.Expect(n => n.Table)
                                                   .Return(dataSourceMock.Tables.Single(m => string.Compare(m.NameTarget, e.Arguments.ElementAt(0).ToString(), StringComparison.OrdinalIgnoreCase) == 0))
                                                   .Repeat.Any();
                                      eventArgsMock.Expect(n => n.Data)
                                                   .Return(new List<IEnumerable<IDataObjectBase>>(0))
                                                   .Repeat.Any();
                                      eventArgsMock.Expect(n => n.EndOfData)
                                                   .Return(true)
                                                   .Repeat.Any();
                                      dataRepositoryMock.Raise(n => n.OnHandleData += null, this, eventArgsMock);
                                  })
                              .Repeat.Any();
            // ReSharper restore ImplicitlyCapturedClosure
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var mocker = new MockRepository();
            var dataValidatorsMock = mocker.DynamicMultiMock<IDataValidators>(new[] {typeof (IEnumerable<IDataValidator>)});
            Expect.Call(dataValidatorsMock.GetEnumerator())
                  .WhenCalled(e =>
                      {
                          var validators = new List<IDataValidator>(0);
                          e.ReturnValue = validators.GetEnumerator();
                      })
                  .Return(null)
                  .Repeat.Any();
            mocker.ReplayAll();
            fixture.Customize<IDataValidators>(e => e.FromFactory(() => dataValidatorsMock));

            var archiveVersionRepositoryMock = MockRepository.GenerateMock<IArchiveVersionRepository>();

            var exceptionHandlerMock = MockRepository.GenerateMock<IExceptionHandler>();
            fixture.Customize<IExceptionHandler>(e => e.FromFactory(() => exceptionHandlerMock));

            var deliveryEngine = new DeliveryEngine.BusinessLogic.DeliveryEngine(fixture.CreateAnonymous<IConfigurationRepository>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IDataRepository>(), fixture.CreateAnonymous<IDocumentRepository>(), fixture.CreateAnonymous<IDataValidators>(), archiveVersionRepositoryMock, fixture.CreateAnonymous<IExceptionHandler>());
            Assert.That(deliveryEngine, Is.Not.Null);

            var executeCommand = MockRepository.GenerateMock<IDeliveryEngineExecuteCommand>();
            executeCommand.Expect(m => m.OverrideArchiveInformationPackageId)
                          .Return(null)
                          .Repeat.Any();
            executeCommand.Expect(m => m.ValidationOnly)
                          .Return(false)
                          .Repeat.Any();
            executeCommand.Expect(m => m.Table)
                          .Return(null)
                          .Repeat.Any();
            executeCommand.Expect(m => m.TablesHandledSimultaneity)
                          .Return(5)
                          .Repeat.Any();
            fixture.Customize<IDeliveryEngineExecuteCommand>(e => e.FromFactory(() => executeCommand));

            deliveryEngine.Execute(fixture.CreateAnonymous<IDeliveryEngineExecuteCommand>());

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
            executeCommand.AssertWasCalled(m => m.OverrideArchiveInformationPackageId);
            // ReSharper disable ImplicitlyCapturedClosure
            archiveVersionRepositoryMock.AssertWasCalled(m => m.DataSource = Arg<IDataSource>.Is.Equal(dataSourceMock));
            // ReSharper restore ImplicitlyCapturedClosure

            archiveVersionRepositoryMock.AssertWasCalled(m => m.ArchiveMetaData());

            dataSourceMock.AssertWasCalled(m => m.Tables);
            executeCommand.AssertWasCalled(m => m.Table);
            executeCommand.AssertWasCalled(m => m.TablesHandledSimultaneity);
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.AssertWasCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Equal(dataSourceMock.Tables[0].NameTarget), Arg<IDataSource>.Is.Equal(dataSourceMock)));
            dataRepositoryMock.AssertWasCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Equal(dataSourceMock.Tables[1].NameTarget), Arg<IDataSource>.Is.Equal(dataSourceMock)));
            dataRepositoryMock.AssertWasCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Equal(dataSourceMock.Tables[2].NameTarget), Arg<IDataSource>.Is.Equal(dataSourceMock)));
            dataRepositoryMock.AssertWasCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Equal(dataSourceMock.Tables[3].NameTarget), Arg<IDataSource>.Is.Equal(dataSourceMock)));
            dataRepositoryMock.AssertWasCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Equal(dataSourceMock.Tables[4].NameTarget), Arg<IDataSource>.Is.Equal(dataSourceMock)));
            // ReSharper restore ImplicitlyCapturedClosure
            dataValidatorsMock.AssertWasCalled(m => m.GetEnumerator(), opt => opt.Repeat.Times(5));
            archiveVersionRepositoryMock.AssertWasCalled(m => m.ArchiveTableData(Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.Anything, Arg<object>.Is.NotNull), opt => opt.Repeat.Times(5));

            executeCommand.AssertWasCalled(m => m.ValidationOnly, opt => opt.Repeat.Times(6));

            exceptionHandlerMock.AssertWasNotCalled(m => m.HandleException(Arg<Exception>.Is.NotNull));
        }

        /// <summary>
        /// Test that Execute raises the event BeforeGetDataForTargetTable.
        /// </summary>
        [Test]
        public void TestThatRaisesBeforeGetDataForTargetTableEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IConfigurationRepository>()));
            fixture.Customize<IDocumentRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDocumentRepository>()));

            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(1).ToList())))
                          .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.Expect(m => m.DataGetForTargetTable(Arg<string>.Is.NotNull, Arg<IDataSource>.Is.Equal(dataSourceMock)))
                              .WhenCalled(e =>
                                  {
                                      Thread.Sleep(2500);
                                      var eventArgsMock = MockRepository.GenerateMock<IHandleDataEventArgs>();
                                      eventArgsMock.Expect(n => n.Table)
                                                   .Return(dataSourceMock.Tables.Single(m => string.Compare(m.NameTarget, e.Arguments.ElementAt(0).ToString(), StringComparison.OrdinalIgnoreCase) == 0))
                                                   .Repeat.Any();
                                      eventArgsMock.Expect(n => n.Data)
                                                   .Return(new List<IEnumerable<IDataObjectBase>>(0))
                                                   .Repeat.Any();
                                      eventArgsMock.Expect(n => n.EndOfData)
                                                   .Return(true)
                                                   .Repeat.Any();
                                      dataRepositoryMock.Raise(n => n.OnHandleData += null, this, eventArgsMock);
                                  })
                              .Repeat.Any();
            // ReSharper restore ImplicitlyCapturedClosure
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var mocker = new MockRepository();
            var dataValidatorsMock = mocker.DynamicMultiMock<IDataValidators>(new[] {typeof (IEnumerable<IDataValidator>)});
            Expect.Call(dataValidatorsMock.GetEnumerator())
                  .WhenCalled(e =>
                      {
                          var validators = new List<IDataValidator>(0);
                          e.ReturnValue = validators.GetEnumerator();
                      })
                  .Return(null)
                  .Repeat.Any();
            mocker.ReplayAll();
            fixture.Customize<IDataValidators>(e => e.FromFactory(() => dataValidatorsMock));

            var archiveVersionRepositoryMock = MockRepository.GenerateMock<IArchiveVersionRepository>();

            var exceptionHandlerMock = MockRepository.GenerateMock<IExceptionHandler>();
            fixture.Customize<IExceptionHandler>(e => e.FromFactory(() => exceptionHandlerMock));

            var deliveryEngine = new DeliveryEngine.BusinessLogic.DeliveryEngine(fixture.CreateAnonymous<IConfigurationRepository>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IDataRepository>(), fixture.CreateAnonymous<IDocumentRepository>(), fixture.CreateAnonymous<IDataValidators>(), archiveVersionRepositoryMock, fixture.CreateAnonymous<IExceptionHandler>());
            Assert.That(deliveryEngine, Is.Not.Null);

            var eventCalled = false;
            // ReSharper disable ImplicitlyCapturedClosure
            deliveryEngine.BeforeGetDataForTargetTable += (s, e) =>
                {
                    Assert.That(s, Is.Not.Null);
                    Assert.That(e, Is.Not.Null);
                    Assert.That(e.DataSource, Is.Not.Null);
                    Assert.That(e.DataSource, Is.EqualTo(dataSourceMock));
                    Assert.That(e.TargetTable, Is.Not.Null);
                    Assert.That(e.TargetTable, Is.EqualTo(dataSourceMock.Tables[0]));
                    eventCalled = true;
                };
            // ReSharper restore ImplicitlyCapturedClosure

            var executeCommand = MockRepository.GenerateMock<IDeliveryEngineExecuteCommand>();
            executeCommand.Expect(m => m.OverrideArchiveInformationPackageId)
                          .Return(null)
                          .Repeat.Any();
            executeCommand.Expect(m => m.ValidationOnly)
                          .Return(false)
                          .Repeat.Any();
            executeCommand.Expect(m => m.Table)
                          .Return(null)
                          .Repeat.Any();
            executeCommand.Expect(m => m.TablesHandledSimultaneity)
                          .Return(5)
                          .Repeat.Any();
            fixture.Customize<IDeliveryEngineExecuteCommand>(e => e.FromFactory(() => executeCommand));

            deliveryEngine.Execute(fixture.CreateAnonymous<IDeliveryEngineExecuteCommand>());
            Assert.That(eventCalled, Is.True);

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
            executeCommand.AssertWasCalled(m => m.OverrideArchiveInformationPackageId);
            // ReSharper disable ImplicitlyCapturedClosure
            archiveVersionRepositoryMock.AssertWasCalled(m => m.DataSource = Arg<IDataSource>.Is.Equal(dataSourceMock));
            // ReSharper restore ImplicitlyCapturedClosure

            archiveVersionRepositoryMock.AssertWasCalled(m => m.ArchiveMetaData());

            dataSourceMock.AssertWasCalled(m => m.Tables);
            executeCommand.AssertWasCalled(m => m.Table);
            executeCommand.AssertWasCalled(m => m.TablesHandledSimultaneity);
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.AssertWasCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Equal(dataSourceMock.Tables[0].NameTarget), Arg<IDataSource>.Is.Equal(dataSourceMock)));
            // ReSharper restore ImplicitlyCapturedClosure
            dataValidatorsMock.AssertWasCalled(m => m.GetEnumerator());
            archiveVersionRepositoryMock.AssertWasCalled(m => m.ArchiveTableData(Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.Anything, Arg<object>.Is.NotNull)); 

            executeCommand.AssertWasCalled(m => m.ValidationOnly, opt => opt.Repeat.Times(2));

            exceptionHandlerMock.AssertWasNotCalled(m => m.HandleException(Arg<Exception>.Is.NotNull));
        }

        /// <summary>
        /// Test that Execute validates data for each target table.
        /// </summary>
        [Test]
        public void TestThatExecuteValidatesDataForEachTargetTable()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IConfigurationRepository>()));
            fixture.Customize<IDocumentRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDocumentRepository>()));

            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.Expect(m => m.DataGetForTargetTable(Arg<string>.Is.NotNull, Arg<IDataSource>.Is.Equal(dataSourceMock)))
                              .WhenCalled(e =>
                                  {
                                      Thread.Sleep(2500);
                                      var eventArgsMock = MockRepository.GenerateMock<IHandleDataEventArgs>();
                                      eventArgsMock.Expect(n => n.Table)
                                                   .Return(dataSourceMock.Tables.Single(m => string.Compare(m.NameTarget, e.Arguments.ElementAt(0).ToString(), StringComparison.OrdinalIgnoreCase) == 0))
                                                   .Repeat.Any();
                                      eventArgsMock.Expect(n => n.Data)
                                                   .Return(new List<IEnumerable<IDataObjectBase>>(0))
                                                   .Repeat.Any();
                                      eventArgsMock.Expect(n => n.EndOfData)
                                                   .Return(true)
                                                   .Repeat.Any();
                                      dataRepositoryMock.Raise(n => n.OnHandleData += null, this, eventArgsMock);
                                  })
                              .Repeat.Any();
            // ReSharper restore ImplicitlyCapturedClosure
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var primaryKeyDataValidatorMock = MockRepository.GenerateMock<IPrimaryKeyDataValidator>();
            primaryKeyDataValidatorMock.Expect(m => m.Validate(Arg<ITable>.Is.NotNull, Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.NotNull, Arg<bool>.Is.Equal(true), Arg<ICommand>.Is.NotNull))
                                       .WhenCalled(e => Thread.Sleep(2500))
                                       .Repeat.Any();
            var foreignKeyDataValidatorMock = MockRepository.GenerateMock<IForeignKeysDataValidator>();
            foreignKeyDataValidatorMock.Expect(m => m.Validate(Arg<ITable>.Is.NotNull, Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.NotNull, Arg<bool>.Is.Equal(true), Arg<ICommand>.Is.NotNull))
                                       .WhenCalled(e => Thread.Sleep(2500))
                                       .Repeat.Any();
            var mocker = new MockRepository();
            var dataValidatorsMock = mocker.DynamicMultiMock<IDataValidators>(new[] {typeof (IEnumerable<IDataValidator>)});
            Expect.Call(dataValidatorsMock.GetEnumerator())
                  .WhenCalled(e =>
                      {
                          var validators = new List<IDataValidator>(2)
                              {
                                  primaryKeyDataValidatorMock,
                                  foreignKeyDataValidatorMock
                              };
                          e.ReturnValue = validators.GetEnumerator();
                      })
                  .Return(null)
                  .Repeat.Any();
            mocker.ReplayAll();
            fixture.Customize<IDataValidators>(e => e.FromFactory(() => dataValidatorsMock));

            var archiveVersionRepositoryMock = MockRepository.GenerateMock<IArchiveVersionRepository>();

            var exceptionHandlerMock = MockRepository.GenerateMock<IExceptionHandler>();
            fixture.Customize<IExceptionHandler>(e => e.FromFactory(() => exceptionHandlerMock));

            var deliveryEngine = new DeliveryEngine.BusinessLogic.DeliveryEngine(fixture.CreateAnonymous<IConfigurationRepository>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IDataRepository>(), fixture.CreateAnonymous<IDocumentRepository>(), fixture.CreateAnonymous<IDataValidators>(), archiveVersionRepositoryMock, fixture.CreateAnonymous<IExceptionHandler>());
            Assert.That(deliveryEngine, Is.Not.Null);

            var executeCommand = MockRepository.GenerateMock<IDeliveryEngineExecuteCommand>();
            executeCommand.Expect(m => m.OverrideArchiveInformationPackageId)
                          .Return(null)
                          .Repeat.Any();
            executeCommand.Expect(m => m.ValidationOnly)
                          .Return(false)
                          .Repeat.Any();
            executeCommand.Expect(m => m.Table)
                          .Return(null)
                          .Repeat.Any();
            executeCommand.Expect(m => m.TablesHandledSimultaneity)
                          .Return(5)
                          .Repeat.Any();
            fixture.Customize<IDeliveryEngineExecuteCommand>(e => e.FromFactory(() => executeCommand));

            deliveryEngine.Execute(fixture.CreateAnonymous<IDeliveryEngineExecuteCommand>());

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
            executeCommand.AssertWasCalled(m => m.OverrideArchiveInformationPackageId);
            // ReSharper disable ImplicitlyCapturedClosure
            archiveVersionRepositoryMock.AssertWasCalled(m => m.DataSource = Arg<IDataSource>.Is.Equal(dataSourceMock));
            // ReSharper restore ImplicitlyCapturedClosure

            archiveVersionRepositoryMock.AssertWasCalled(m => m.ArchiveMetaData());

            dataSourceMock.AssertWasCalled(m => m.Tables);
            executeCommand.AssertWasCalled(m => m.Table);
            executeCommand.AssertWasCalled(m => m.TablesHandledSimultaneity);
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.AssertWasCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Equal(dataSourceMock.Tables[0].NameTarget), Arg<IDataSource>.Is.Equal(dataSourceMock)));
            dataRepositoryMock.AssertWasCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Equal(dataSourceMock.Tables[1].NameTarget), Arg<IDataSource>.Is.Equal(dataSourceMock)));
            dataRepositoryMock.AssertWasCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Equal(dataSourceMock.Tables[2].NameTarget), Arg<IDataSource>.Is.Equal(dataSourceMock)));
            dataRepositoryMock.AssertWasCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Equal(dataSourceMock.Tables[3].NameTarget), Arg<IDataSource>.Is.Equal(dataSourceMock)));
            dataRepositoryMock.AssertWasCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Equal(dataSourceMock.Tables[4].NameTarget), Arg<IDataSource>.Is.Equal(dataSourceMock)));
            // ReSharper restore ImplicitlyCapturedClosure
            dataValidatorsMock.AssertWasCalled(m => m.GetEnumerator(), opt => opt.Repeat.Times(10));
            // ReSharper disable ImplicitlyCapturedClosure
            primaryKeyDataValidatorMock.AssertWasCalled(m => m.Validate(Arg<ITable>.Is.Equal(dataSourceMock.Tables[0]), Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.NotNull, Arg<bool>.Is.Equal(true), Arg<ICommand>.Is.NotNull));
            primaryKeyDataValidatorMock.AssertWasCalled(m => m.Validate(Arg<ITable>.Is.Equal(dataSourceMock.Tables[1]), Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.NotNull, Arg<bool>.Is.Equal(true), Arg<ICommand>.Is.NotNull));
            primaryKeyDataValidatorMock.AssertWasCalled(m => m.Validate(Arg<ITable>.Is.Equal(dataSourceMock.Tables[2]), Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.NotNull, Arg<bool>.Is.Equal(true), Arg<ICommand>.Is.NotNull));
            primaryKeyDataValidatorMock.AssertWasCalled(m => m.Validate(Arg<ITable>.Is.Equal(dataSourceMock.Tables[3]), Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.NotNull, Arg<bool>.Is.Equal(true), Arg<ICommand>.Is.NotNull));
            primaryKeyDataValidatorMock.AssertWasCalled(m => m.Validate(Arg<ITable>.Is.Equal(dataSourceMock.Tables[4]), Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.NotNull, Arg<bool>.Is.Equal(true), Arg<ICommand>.Is.NotNull));
            foreignKeyDataValidatorMock.AssertWasCalled(m => m.Validate(Arg<ITable>.Is.Equal(dataSourceMock.Tables[0]), Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.NotNull, Arg<bool>.Is.Equal(true), Arg<ICommand>.Is.NotNull));
            foreignKeyDataValidatorMock.AssertWasCalled(m => m.Validate(Arg<ITable>.Is.Equal(dataSourceMock.Tables[1]), Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.NotNull, Arg<bool>.Is.Equal(true), Arg<ICommand>.Is.NotNull));
            foreignKeyDataValidatorMock.AssertWasCalled(m => m.Validate(Arg<ITable>.Is.Equal(dataSourceMock.Tables[2]), Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.NotNull, Arg<bool>.Is.Equal(true), Arg<ICommand>.Is.NotNull));
            foreignKeyDataValidatorMock.AssertWasCalled(m => m.Validate(Arg<ITable>.Is.Equal(dataSourceMock.Tables[3]), Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.NotNull, Arg<bool>.Is.Equal(true), Arg<ICommand>.Is.NotNull));
            foreignKeyDataValidatorMock.AssertWasCalled(m => m.Validate(Arg<ITable>.Is.Equal(dataSourceMock.Tables[4]), Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.NotNull, Arg<bool>.Is.Equal(true), Arg<ICommand>.Is.NotNull));
            // ReSharper restore ImplicitlyCapturedClosure
            archiveVersionRepositoryMock.AssertWasCalled(m => m.ArchiveTableData(Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.Anything, Arg<object>.Is.NotNull), opt => opt.Repeat.Times(5)); 
            executeCommand.AssertWasCalled(m => m.ValidationOnly, opt => opt.Repeat.Times(6));

            exceptionHandlerMock.AssertWasNotCalled(m => m.HandleException(Arg<Exception>.Is.NotNull, out Arg<bool>.Out(true).Dummy));
            exceptionHandlerMock.AssertWasNotCalled(m => m.HandleException(Arg<Exception>.Is.NotNull));
        }

        /// <summary>
        /// Test that Execute retry to validate data for a targettable if an DeliveryEngineConvertException occurs.
        /// </summary>
        [Test]
        public void TestThatExecuteRetryToValidateDataForTargetTableIfDeliveryEngineConvertExceptionOccurs()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IConfigurationRepository>()));
            fixture.Customize<IDocumentRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDocumentRepository>()));

            var exceptionInfoMock = MockRepository.GenerateMock<IDeliveryEngineConvertExceptionInfo>();
            exceptionInfoMock.Expect(m => m.ConvertObjectData)
                             .Return(fixture.CreateAnonymous<object>())
                             .Repeat.Any();
            fixture.Customize<IDeliveryEngineConvertExceptionInfo>(e => e.FromFactory(() => exceptionInfoMock));

            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(1).ToList())))
                          .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.Expect(m => m.DataGetForTargetTable(Arg<string>.Is.NotNull, Arg<IDataSource>.Is.Equal(dataSourceMock)))
                              .WhenCalled(e =>
                                  {
                                      Thread.Sleep(2500);
                                      var eventArgsMock = MockRepository.GenerateMock<IHandleDataEventArgs>();
                                      eventArgsMock.Expect(n => n.Table)
                                                   .Return(dataSourceMock.Tables.Single(m => string.Compare(m.NameTarget, e.Arguments.ElementAt(0).ToString(), StringComparison.OrdinalIgnoreCase) == 0))
                                                   .Repeat.Any();
                                      eventArgsMock.Expect(n => n.Data)
                                                   .Return(new List<IEnumerable<IDataObjectBase>>(0))
                                                   .Repeat.Any();
                                      eventArgsMock.Expect(n => n.EndOfData)
                                                   .Return(true)
                                                   .Repeat.Any();
                                      dataRepositoryMock.Raise(n => n.OnHandleData += null, this, eventArgsMock);
                                  })
                              .Repeat.Any();
            // ReSharper restore ImplicitlyCapturedClosure
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var primaryKeyDataValidatorMock = MockRepository.GenerateMock<IPrimaryKeyDataValidator>();
            // ReSharper disable ImplicitlyCapturedClosure
            primaryKeyDataValidatorMock.Expect(m => m.Validate(Arg<ITable>.Is.Equal(dataSourceMock.Tables[0]), Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.Anything, Arg<bool>.Is.Equal(true), Arg<ICommand>.Is.NotNull))
                                       .Throw(fixture.CreateAnonymous<DeliveryEngineConvertException>())
                                       .Repeat.Any();
            // ReSharper restore ImplicitlyCapturedClosure
            var mocker = new MockRepository();
            var dataValidatorsMock = mocker.DynamicMultiMock<IDataValidators>(new[] {typeof (IEnumerable<IDataValidator>)});
            Expect.Call(dataValidatorsMock.GetEnumerator())
                  .WhenCalled(e =>
                      {
                          var validators = new List<IDataValidator>(1) {primaryKeyDataValidatorMock};
                          e.ReturnValue = validators.GetEnumerator();
                      })
                  .Return(null)
                  .Repeat.Any();
            mocker.ReplayAll();
            fixture.Customize<IDataValidators>(e => e.FromFactory(() => dataValidatorsMock));

            var archiveVersionRepositoryMock = MockRepository.GenerateMock<IArchiveVersionRepository>();

            var retries = 0;
            var exceptionHandlerMock = MockRepository.GenerateMock<IExceptionHandler>();
            exceptionHandlerMock.Expect(m => m.HandleException(Arg<DeliveryEngineConvertException>.Is.NotNull, out Arg<bool>.Out(true).Dummy))
                                .WhenCalled(e =>
                                    {
                                        e.Arguments[1] = retries < 1;
                                        retries++;
                                    })
                                .Repeat.Any();
            fixture.Customize<IExceptionHandler>(e => e.FromFactory(() => exceptionHandlerMock));

            var deliveryEngine = new DeliveryEngine.BusinessLogic.DeliveryEngine(fixture.CreateAnonymous<IConfigurationRepository>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IDataRepository>(), fixture.CreateAnonymous<IDocumentRepository>(), fixture.CreateAnonymous<IDataValidators>(), archiveVersionRepositoryMock, fixture.CreateAnonymous<IExceptionHandler>());
            Assert.That(deliveryEngine, Is.Not.Null);

            var executeCommand = MockRepository.GenerateMock<IDeliveryEngineExecuteCommand>();
            executeCommand.Expect(m => m.OverrideArchiveInformationPackageId)
                          .Return(null)
                          .Repeat.Any();
            executeCommand.Expect(m => m.ValidationOnly)
                          .Return(true)
                          .Repeat.Any();
            executeCommand.Expect(m => m.Table)
                          .Return(null)
                          .Repeat.Any();
            executeCommand.Expect(m => m.TablesHandledSimultaneity)
                          .Return(5)
                          .Repeat.Any();
            fixture.Customize<IDeliveryEngineExecuteCommand>(e => e.FromFactory(() => executeCommand));

            deliveryEngine.Execute(fixture.CreateAnonymous<IDeliveryEngineExecuteCommand>());

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
            executeCommand.AssertWasCalled(m => m.OverrideArchiveInformationPackageId);
            executeCommand.AssertWasCalled(m => m.TablesHandledSimultaneity);
            // ReSharper disable ImplicitlyCapturedClosure
            archiveVersionRepositoryMock.AssertWasCalled(m => m.DataSource = Arg<IDataSource>.Is.Equal(dataSourceMock));
            // ReSharper restore ImplicitlyCapturedClosure

            executeCommand.AssertWasCalled(m => m.ValidationOnly);
            archiveVersionRepositoryMock.AssertWasNotCalled(m => m.ArchiveMetaData());

            dataSourceMock.AssertWasCalled(m => m.Tables);
            executeCommand.AssertWasCalled(m => m.Table);
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.AssertWasCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Equal(dataSourceMock.Tables[0].NameTarget), Arg<IDataSource>.Is.Equal(dataSourceMock)));
            // ReSharper restore ImplicitlyCapturedClosure
            dataValidatorsMock.AssertWasCalled(m => m.GetEnumerator(), opt => opt.Repeat.Times(2));
            // ReSharper disable ImplicitlyCapturedClosure
            primaryKeyDataValidatorMock.AssertWasCalled(m => m.Validate(Arg<ITable>.Is.Equal(dataSourceMock.Tables[0]), Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.NotNull, Arg<bool>.Is.Equal(true), Arg<ICommand>.Is.NotNull), opt => opt.Repeat.Times(2));
            // ReSharper restore ImplicitlyCapturedClosure
            archiveVersionRepositoryMock.AssertWasNotCalled(m => m.ArchiveTableData(Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.Anything, Arg<object>.Is.Anything));

            exceptionInfoMock.AssertWasCalled(m => m.ConvertObjectData);

            exceptionHandlerMock.AssertWasCalled(m => m.HandleException(Arg<DeliveryEngineConvertException>.Is.NotNull, out Arg<bool>.Out(true).Dummy), opt => opt.Repeat.Times(2));
            exceptionHandlerMock.AssertWasNotCalled(m => m.HandleException(Arg<Exception>.Is.NotNull));
        }

        /// <summary>
        /// Test that Execute retry to validate data for a targettable if an DeliveryEngineMappingException occurs.
        /// </summary>
        [Test]
        public void TestThatExecuteRetryToValidateDataForTargetTableIfDeliveryEngineMappingExceptionOccurs()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IConfigurationRepository>()));
            fixture.Customize<IDocumentRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDocumentRepository>()));

            var exceptionInfoMock = MockRepository.GenerateMock<IDeliveryEngineMappingExceptionInfo>();
            exceptionInfoMock.Expect(m => m.MappingObjectData)
                             .Return(fixture.CreateAnonymous<object>())
                             .Repeat.Any();
            fixture.Customize<IDeliveryEngineMappingExceptionInfo>(e => e.FromFactory(() => exceptionInfoMock));

            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(1).ToList())))
                          .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.Expect(m => m.DataGetForTargetTable(Arg<string>.Is.NotNull, Arg<IDataSource>.Is.Equal(dataSourceMock)))
                              .WhenCalled(e =>
                                  {
                                      Thread.Sleep(2500);
                                      var eventArgsMock = MockRepository.GenerateMock<IHandleDataEventArgs>();
                                      eventArgsMock.Expect(n => n.Table)
                                                   .Return(dataSourceMock.Tables.Single(m => string.Compare(m.NameTarget, e.Arguments.ElementAt(0).ToString(), StringComparison.OrdinalIgnoreCase) == 0))
                                                   .Repeat.Any();
                                      eventArgsMock.Expect(n => n.Data)
                                                   .Return(new List<IEnumerable<IDataObjectBase>>(0))
                                                   .Repeat.Any();
                                      eventArgsMock.Expect(n => n.EndOfData)
                                                   .Return(true)
                                                   .Repeat.Any();
                                      dataRepositoryMock.Raise(n => n.OnHandleData += null, this, eventArgsMock);
                                  })
                              .Repeat.Any();
            // ReSharper restore ImplicitlyCapturedClosure
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var primaryKeyDataValidatorMock = MockRepository.GenerateMock<IPrimaryKeyDataValidator>();
            // ReSharper disable ImplicitlyCapturedClosure
            primaryKeyDataValidatorMock.Expect(m => m.Validate(Arg<ITable>.Is.Equal(dataSourceMock.Tables[0]), Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.Anything, Arg<bool>.Is.Equal(true), Arg<ICommand>.Is.NotNull))
                                       .Throw(fixture.CreateAnonymous<DeliveryEngineMappingException>())
                                       .Repeat.Any();
            // ReSharper restore ImplicitlyCapturedClosure
            var mocker = new MockRepository();
            var dataValidatorsMock = mocker.DynamicMultiMock<IDataValidators>(new[] {typeof (IEnumerable<IDataValidator>)});
            Expect.Call(dataValidatorsMock.GetEnumerator())
                  .WhenCalled(e =>
                      {
                          var validators = new List<IDataValidator>(1) {primaryKeyDataValidatorMock};
                          e.ReturnValue = validators.GetEnumerator();
                      })
                  .Return(null)
                  .Repeat.Any();
            mocker.ReplayAll();
            fixture.Customize<IDataValidators>(e => e.FromFactory(() => dataValidatorsMock));

            var archiveVersionRepositoryMock = MockRepository.GenerateMock<IArchiveVersionRepository>();

            var retries = 0;
            var exceptionHandlerMock = MockRepository.GenerateMock<IExceptionHandler>();
            exceptionHandlerMock.Expect(m => m.HandleException(Arg<DeliveryEngineMappingException>.Is.NotNull, out Arg<bool>.Out(true).Dummy))
                                .WhenCalled(e =>
                                    {
                                        e.Arguments[1] = retries < 1;
                                        retries++;
                                    })
                                .Repeat.Any();
            fixture.Customize<IExceptionHandler>(e => e.FromFactory(() => exceptionHandlerMock));

            var deliveryEngine = new DeliveryEngine.BusinessLogic.DeliveryEngine(fixture.CreateAnonymous<IConfigurationRepository>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IDataRepository>(), fixture.CreateAnonymous<IDocumentRepository>(), fixture.CreateAnonymous<IDataValidators>(), archiveVersionRepositoryMock, fixture.CreateAnonymous<IExceptionHandler>());
            Assert.That(deliveryEngine, Is.Not.Null);

            var executeCommand = MockRepository.GenerateMock<IDeliveryEngineExecuteCommand>();
            executeCommand.Expect(m => m.OverrideArchiveInformationPackageId)
                          .Return(null)
                          .Repeat.Any();
            executeCommand.Expect(m => m.ValidationOnly)
                          .Return(true)
                          .Repeat.Any();
            executeCommand.Expect(m => m.Table)
                          .Return(null)
                          .Repeat.Any();
            executeCommand.Expect(m => m.TablesHandledSimultaneity)
                          .Return(5)
                          .Repeat.Any();
            fixture.Customize<IDeliveryEngineExecuteCommand>(e => e.FromFactory(() => executeCommand));

            deliveryEngine.Execute(fixture.CreateAnonymous<IDeliveryEngineExecuteCommand>());

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
            executeCommand.AssertWasCalled(m => m.OverrideArchiveInformationPackageId);
            // ReSharper disable ImplicitlyCapturedClosure
            archiveVersionRepositoryMock.AssertWasCalled(m => m.DataSource = Arg<IDataSource>.Is.Equal(dataSourceMock));
            // ReSharper restore ImplicitlyCapturedClosure

            executeCommand.AssertWasCalled(m => m.ValidationOnly);
            archiveVersionRepositoryMock.AssertWasNotCalled(m => m.ArchiveMetaData());

            dataSourceMock.AssertWasCalled(m => m.Tables);
            executeCommand.AssertWasCalled(m => m.Table);
            executeCommand.AssertWasCalled(m => m.TablesHandledSimultaneity);
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.AssertWasCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Equal(dataSourceMock.Tables[0].NameTarget), Arg<IDataSource>.Is.Equal(dataSourceMock)));
            // ReSharper restore ImplicitlyCapturedClosure
            dataValidatorsMock.AssertWasCalled(m => m.GetEnumerator(), opt => opt.Repeat.Times(2));
            // ReSharper disable ImplicitlyCapturedClosure
            primaryKeyDataValidatorMock.AssertWasCalled(m => m.Validate(Arg<ITable>.Is.Equal(dataSourceMock.Tables[0]), Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.NotNull, Arg<bool>.Is.Equal(true), Arg<ICommand>.Is.NotNull), opt => opt.Repeat.Times(2));
            // ReSharper restore ImplicitlyCapturedClosure
            archiveVersionRepositoryMock.AssertWasNotCalled(m => m.ArchiveTableData(Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.Anything, Arg<object>.Is.Anything));

            exceptionInfoMock.AssertWasCalled(m => m.MappingObjectData);

            exceptionHandlerMock.AssertWasCalled(m => m.HandleException(Arg<DeliveryEngineMappingException>.Is.NotNull, out Arg<bool>.Out(true).Dummy), opt => opt.Repeat.Times(2));
            exceptionHandlerMock.AssertWasNotCalled(m => m.HandleException(Arg<Exception>.Is.NotNull));
        }

        /// <summary>
        /// Test that Execute retry to validate data for a targettable if an DeliveryEngineValidateException occurs.
        /// </summary>
        [Test]
        public void TestThatExecuteRetryToValidateDataForTargetTableIfDeliveryEngineValidateExceptionOccurs()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IConfigurationRepository>()));
            fixture.Customize<IDocumentRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDocumentRepository>()));

            var exceptionInfoMock = MockRepository.GenerateMock<IDeliveryEngineValidateExceptionInfo>();
            exceptionInfoMock.Expect(m => m.ValidateObjectData)
                             .Return(fixture.CreateAnonymous<object>())
                             .Repeat.Any();
            fixture.Customize<IDeliveryEngineValidateExceptionInfo>(e => e.FromFactory(() => exceptionInfoMock));

            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(1).ToList())))
                          .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.Expect(m => m.DataGetForTargetTable(Arg<string>.Is.NotNull, Arg<IDataSource>.Is.Equal(dataSourceMock)))
                              .WhenCalled(e =>
                                  {
                                      Thread.Sleep(2500);
                                      var eventArgsMock = MockRepository.GenerateMock<IHandleDataEventArgs>();
                                      eventArgsMock.Expect(n => n.Table)
                                                   .Return(dataSourceMock.Tables.Single(m => string.Compare(m.NameTarget, e.Arguments.ElementAt(0).ToString(), StringComparison.OrdinalIgnoreCase) == 0))
                                                   .Repeat.Any();
                                      eventArgsMock.Expect(n => n.Data)
                                                   .Return(new List<IEnumerable<IDataObjectBase>>(0))
                                                   .Repeat.Any();
                                      eventArgsMock.Expect(n => n.EndOfData)
                                                   .Return(true)
                                                   .Repeat.Any();
                                      dataRepositoryMock.Raise(n => n.OnHandleData += null, this, eventArgsMock);
                                  })
                              .Repeat.Any();
            // ReSharper restore ImplicitlyCapturedClosure
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var primaryKeyDataValidatorMock = MockRepository.GenerateMock<IPrimaryKeyDataValidator>();
            // ReSharper disable ImplicitlyCapturedClosure
            primaryKeyDataValidatorMock.Expect(m => m.Validate(Arg<ITable>.Is.Equal(dataSourceMock.Tables[0]), Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.Anything, Arg<bool>.Is.Equal(true), Arg<ICommand>.Is.NotNull))
                                       .Throw(fixture.CreateAnonymous<DeliveryEngineValidateException>())
                                       .Repeat.Any();
            // ReSharper restore ImplicitlyCapturedClosure
            var mocker = new MockRepository();
            var dataValidatorsMock = mocker.DynamicMultiMock<IDataValidators>(new[] {typeof (IEnumerable<IDataValidator>)});
            Expect.Call(dataValidatorsMock.GetEnumerator())
                  .WhenCalled(e =>
                      {
                          var validators = new List<IDataValidator>(1) {primaryKeyDataValidatorMock};
                          e.ReturnValue = validators.GetEnumerator();
                      })
                  .Return(null)
                  .Repeat.Any();
            mocker.ReplayAll();
            fixture.Customize<IDataValidators>(e => e.FromFactory(() => dataValidatorsMock));

            var archiveVersionRepositoryMock = MockRepository.GenerateMock<IArchiveVersionRepository>();

            var retries = 0;
            var exceptionHandlerMock = MockRepository.GenerateMock<IExceptionHandler>();
            exceptionHandlerMock.Expect(m => m.HandleException(Arg<DeliveryEngineValidateException>.Is.NotNull, out Arg<bool>.Out(true).Dummy))
                                .WhenCalled(e =>
                                    {
                                        e.Arguments[1] = retries < 1;
                                        retries++;
                                    })
                                .Repeat.Any();
            fixture.Customize<IExceptionHandler>(e => e.FromFactory(() => exceptionHandlerMock));

            var deliveryEngine = new DeliveryEngine.BusinessLogic.DeliveryEngine(fixture.CreateAnonymous<IConfigurationRepository>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IDataRepository>(), fixture.CreateAnonymous<IDocumentRepository>(), fixture.CreateAnonymous<IDataValidators>(), archiveVersionRepositoryMock, fixture.CreateAnonymous<IExceptionHandler>());
            Assert.That(deliveryEngine, Is.Not.Null);

            var executeCommand = MockRepository.GenerateMock<IDeliveryEngineExecuteCommand>();
            executeCommand.Expect(m => m.OverrideArchiveInformationPackageId)
                          .Return(null)
                          .Repeat.Any();
            executeCommand.Expect(m => m.ValidationOnly)
                          .Return(true)
                          .Repeat.Any();
            executeCommand.Expect(m => m.Table)
                          .Return(null)
                          .Repeat.Any();
            executeCommand.Expect(m => m.TablesHandledSimultaneity)
                          .Return(5)
                          .Repeat.Any();
            fixture.Customize<IDeliveryEngineExecuteCommand>(e => e.FromFactory(() => executeCommand));

            deliveryEngine.Execute(fixture.CreateAnonymous<IDeliveryEngineExecuteCommand>());

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
            executeCommand.AssertWasCalled(m => m.OverrideArchiveInformationPackageId);
            // ReSharper disable ImplicitlyCapturedClosure
            archiveVersionRepositoryMock.AssertWasCalled(m => m.DataSource = Arg<IDataSource>.Is.Equal(dataSourceMock));
            // ReSharper restore ImplicitlyCapturedClosure

            executeCommand.AssertWasCalled(m => m.ValidationOnly);
            archiveVersionRepositoryMock.AssertWasNotCalled(m => m.ArchiveMetaData());

            dataSourceMock.AssertWasCalled(m => m.Tables);
            executeCommand.AssertWasCalled(m => m.Table);
            executeCommand.AssertWasCalled(m => m.TablesHandledSimultaneity);
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.AssertWasCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Equal(dataSourceMock.Tables[0].NameTarget), Arg<IDataSource>.Is.Equal(dataSourceMock)));
            // ReSharper restore ImplicitlyCapturedClosure
            dataValidatorsMock.AssertWasCalled(m => m.GetEnumerator(), opt => opt.Repeat.Times(2));
            // ReSharper disable ImplicitlyCapturedClosure
            primaryKeyDataValidatorMock.AssertWasCalled(m => m.Validate(Arg<ITable>.Is.Equal(dataSourceMock.Tables[0]), Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.NotNull, Arg<bool>.Is.Equal(true), Arg<ICommand>.Is.NotNull), opt => opt.Repeat.Times(2));
            // ReSharper restore ImplicitlyCapturedClosure
            archiveVersionRepositoryMock.AssertWasNotCalled(m => m.ArchiveTableData(Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.Anything, Arg<object>.Is.Anything));

            exceptionInfoMock.AssertWasCalled(m => m.ValidateObjectData);

            exceptionHandlerMock.AssertWasCalled(m => m.HandleException(Arg<DeliveryEngineValidateException>.Is.NotNull, out Arg<bool>.Out(true).Dummy), opt => opt.Repeat.Times(2));
            exceptionHandlerMock.AssertWasNotCalled(m => m.HandleException(Arg<Exception>.Is.NotNull));
        }

        /// <summary>
        /// Test that Execute retry to validate data for a targettable if an Exception occurs.
        /// </summary>
        [Test]
        public void TestThatExecuteRetryToValidateDataForTargetTableIfExceptionOccurs()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IConfigurationRepository>()));
            fixture.Customize<IDocumentRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDocumentRepository>()));

            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(1).ToList())))
                          .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.Expect(m => m.DataGetForTargetTable(Arg<string>.Is.NotNull, Arg<IDataSource>.Is.Equal(dataSourceMock)))
                              .WhenCalled(e =>
                                  {
                                      Thread.Sleep(2500);
                                      var eventArgsMock = MockRepository.GenerateMock<IHandleDataEventArgs>();
                                      eventArgsMock.Expect(n => n.Table)
                                                   .Return(dataSourceMock.Tables.Single(m => string.Compare(m.NameTarget, e.Arguments.ElementAt(0).ToString(), StringComparison.OrdinalIgnoreCase) == 0))
                                                   .Repeat.Any();
                                      eventArgsMock.Expect(n => n.Data)
                                                   .Return(new List<IEnumerable<IDataObjectBase>>(0))
                                                   .Repeat.Any();
                                      eventArgsMock.Expect(n => n.EndOfData)
                                                   .Return(true)
                                                   .Repeat.Any();
                                      dataRepositoryMock.Raise(n => n.OnHandleData += null, this, eventArgsMock);
                                  })
                              .Repeat.Any();
            // ReSharper restore ImplicitlyCapturedClosure
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var primaryKeyDataValidatorMock = MockRepository.GenerateMock<IPrimaryKeyDataValidator>();
            // ReSharper disable ImplicitlyCapturedClosure
            primaryKeyDataValidatorMock.Expect(m => m.Validate(Arg<ITable>.Is.Equal(dataSourceMock.Tables[0]), Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.Anything, Arg<bool>.Is.Equal(true), Arg<ICommand>.Is.NotNull))
                                       .Throw(fixture.CreateAnonymous<Exception>())
                                       .Repeat.Any();
            // ReSharper restore ImplicitlyCapturedClosure
            var mocker = new MockRepository();
            var dataValidatorsMock = mocker.DynamicMultiMock<IDataValidators>(new[] {typeof (IEnumerable<IDataValidator>)});
            Expect.Call(dataValidatorsMock.GetEnumerator())
                  .WhenCalled(e =>
                      {
                          var validators = new List<IDataValidator>(1) {primaryKeyDataValidatorMock};
                          e.ReturnValue = validators.GetEnumerator();
                      })
                  .Return(null)
                  .Repeat.Any();
            mocker.ReplayAll();
            fixture.Customize<IDataValidators>(e => e.FromFactory(() => dataValidatorsMock));

            var archiveVersionRepositoryMock = MockRepository.GenerateMock<IArchiveVersionRepository>();

            var retries = 0;
            var exceptionHandlerMock = MockRepository.GenerateMock<IExceptionHandler>();
            exceptionHandlerMock.Expect(m => m.HandleException(Arg<Exception>.Is.NotNull, out Arg<bool>.Out(true).Dummy))
                                .WhenCalled(e =>
                                    {
                                        e.Arguments[1] = retries < 1;
                                        retries++;
                                    })
                                .Repeat.Any();
            fixture.Customize<IExceptionHandler>(e => e.FromFactory(() => exceptionHandlerMock));

            var deliveryEngine = new DeliveryEngine.BusinessLogic.DeliveryEngine(fixture.CreateAnonymous<IConfigurationRepository>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IDataRepository>(), fixture.CreateAnonymous<IDocumentRepository>(), fixture.CreateAnonymous<IDataValidators>(), archiveVersionRepositoryMock, fixture.CreateAnonymous<IExceptionHandler>());
            Assert.That(deliveryEngine, Is.Not.Null);

            var executeCommand = MockRepository.GenerateMock<IDeliveryEngineExecuteCommand>();
            executeCommand.Expect(m => m.OverrideArchiveInformationPackageId)
                          .Return(null)
                          .Repeat.Any();
            executeCommand.Expect(m => m.ValidationOnly)
                          .Return(true)
                          .Repeat.Any();
            executeCommand.Expect(m => m.Table)
                          .Return(null)
                          .Repeat.Any();
            executeCommand.Expect(m => m.TablesHandledSimultaneity)
                          .Return(5)
                          .Repeat.Any();
            fixture.Customize<IDeliveryEngineExecuteCommand>(e => e.FromFactory(() => executeCommand));

            deliveryEngine.Execute(fixture.CreateAnonymous<IDeliveryEngineExecuteCommand>());

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
            executeCommand.AssertWasCalled(m => m.OverrideArchiveInformationPackageId);
            // ReSharper disable ImplicitlyCapturedClosure
            archiveVersionRepositoryMock.AssertWasCalled(m => m.DataSource = Arg<IDataSource>.Is.Equal(dataSourceMock));
            // ReSharper restore ImplicitlyCapturedClosure

            executeCommand.AssertWasCalled(m => m.ValidationOnly);
            archiveVersionRepositoryMock.AssertWasNotCalled(m => m.ArchiveMetaData());

            dataSourceMock.AssertWasCalled(m => m.Tables);
            executeCommand.AssertWasCalled(m => m.Table);
            executeCommand.AssertWasCalled(m => m.TablesHandledSimultaneity);
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.AssertWasCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Equal(dataSourceMock.Tables[0].NameTarget), Arg<IDataSource>.Is.Equal(dataSourceMock)));
            // ReSharper restore ImplicitlyCapturedClosure
            dataValidatorsMock.AssertWasCalled(m => m.GetEnumerator(), opt => opt.Repeat.Times(2));
            // ReSharper disable ImplicitlyCapturedClosure
            primaryKeyDataValidatorMock.AssertWasCalled(m => m.Validate(Arg<ITable>.Is.Equal(dataSourceMock.Tables[0]), Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.NotNull, Arg<bool>.Is.Equal(true), Arg<ICommand>.Is.NotNull), opt => opt.Repeat.Times(2));
            // ReSharper restore ImplicitlyCapturedClosure
            archiveVersionRepositoryMock.AssertWasNotCalled(m => m.ArchiveTableData(Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.Anything, Arg<object>.Is.Anything));

            exceptionHandlerMock.AssertWasCalled(m => m.HandleException(Arg<Exception>.Is.NotNull, out Arg<bool>.Out(true).Dummy), opt => opt.Repeat.Times(2));
            exceptionHandlerMock.AssertWasNotCalled(m => m.HandleException(Arg<Exception>.Is.NotNull));
        }

        /// <summary>
        /// Test that Execute raises the event BeforeValidateDataInTargetTable.
        /// </summary>
        [Test]
        public void TestThatRaisesBeforeValidateDataInTargetTableEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IConfigurationRepository>()));
            fixture.Customize<IDocumentRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDocumentRepository>()));

            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(1).ToList())))
                          .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.Expect(m => m.DataGetForTargetTable(Arg<string>.Is.NotNull, Arg<IDataSource>.Is.Equal(dataSourceMock)))
                              .WhenCalled(e =>
                                  {
                                      Thread.Sleep(2500);
                                      var eventArgsMock = MockRepository.GenerateMock<IHandleDataEventArgs>();
                                      eventArgsMock.Expect(n => n.Table)
                                                   .Return(dataSourceMock.Tables.Single(m => string.Compare(m.NameTarget, e.Arguments.ElementAt(0).ToString(), StringComparison.OrdinalIgnoreCase) == 0))
                                                   .Repeat.Any();
                                      eventArgsMock.Expect(n => n.Data)
                                                   .Return(new List<IEnumerable<IDataObjectBase>>(0))
                                                   .Repeat.Any();
                                      eventArgsMock.Expect(n => n.EndOfData)
                                                   .Return(true)
                                                   .Repeat.Any();
                                      dataRepositoryMock.Raise(n => n.OnHandleData += null, this, eventArgsMock);
                                  })
                              .Repeat.Any();
            // ReSharper restore ImplicitlyCapturedClosure
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var primaryKeyDataValidatorMock = MockRepository.GenerateMock<IPrimaryKeyDataValidator>();
            primaryKeyDataValidatorMock.Expect(m => m.Validate(Arg<ITable>.Is.NotNull, Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.NotNull, Arg<bool>.Is.Equal(true), Arg<ICommand>.Is.NotNull))
                                       .WhenCalled(e => Thread.Sleep(2500))
                                       .Repeat.Any();
            var foreignKeyDataValidatorMock = MockRepository.GenerateMock<IForeignKeysDataValidator>();
            foreignKeyDataValidatorMock.Expect(m => m.Validate(Arg<ITable>.Is.NotNull, Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.NotNull, Arg<bool>.Is.Equal(true), Arg<ICommand>.Is.NotNull))
                                       .WhenCalled(e => Thread.Sleep(2500))
                                       .Repeat.Any();
            var mocker = new MockRepository();
            var dataValidatorsMock = mocker.DynamicMultiMock<IDataValidators>(new[] {typeof (IEnumerable<IDataValidator>)});
            Expect.Call(dataValidatorsMock.GetEnumerator())
                  .WhenCalled(e =>
                      {
                          var validators = new List<IDataValidator>(2)
                              {
                                  primaryKeyDataValidatorMock,
                                  foreignKeyDataValidatorMock
                              };
                          e.ReturnValue = validators.GetEnumerator();
                      })
                  .Return(null)
                  .Repeat.Any();
            mocker.ReplayAll();
            fixture.Customize<IDataValidators>(e => e.FromFactory(() => dataValidatorsMock));

            var archiveVersionRepositoryMock = MockRepository.GenerateMock<IArchiveVersionRepository>();

            var exceptionHandlerMock = MockRepository.GenerateMock<IExceptionHandler>();
            fixture.Customize<IExceptionHandler>(e => e.FromFactory(() => exceptionHandlerMock));

            var deliveryEngine = new DeliveryEngine.BusinessLogic.DeliveryEngine(fixture.CreateAnonymous<IConfigurationRepository>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IDataRepository>(), fixture.CreateAnonymous<IDocumentRepository>(), fixture.CreateAnonymous<IDataValidators>(), archiveVersionRepositoryMock, fixture.CreateAnonymous<IExceptionHandler>());
            Assert.That(deliveryEngine, Is.Not.Null);

            var eventCalled = false;
            // ReSharper disable ImplicitlyCapturedClosure
            deliveryEngine.BeforeValidateDataInTargetTable += (s, e) =>
                {
                    Assert.That(s, Is.Not.Null);
                    Assert.That(e, Is.Not.Null);
                    Assert.That(e.DataSource, Is.Not.Null);
                    Assert.That(e.DataSource, Is.EqualTo(dataSourceMock));
                    Assert.That(e.TargetTable, Is.Not.Null);
                    Assert.That(e.TargetTable, Is.EqualTo(dataSourceMock.Tables[0]));
                    eventCalled = true;
                };
            // ReSharper restore ImplicitlyCapturedClosure

            var executeCommand = MockRepository.GenerateMock<IDeliveryEngineExecuteCommand>();
            executeCommand.Expect(m => m.OverrideArchiveInformationPackageId)
                          .Return(null)
                          .Repeat.Any();
            executeCommand.Expect(m => m.ValidationOnly)
                          .Return(false)
                          .Repeat.Any();
            executeCommand.Expect(m => m.Table)
                          .Return(null)
                          .Repeat.Any();
            executeCommand.Expect(m => m.TablesHandledSimultaneity)
                          .Return(5)
                          .Repeat.Any();
            fixture.Customize<IDeliveryEngineExecuteCommand>(e => e.FromFactory(() => executeCommand));

            deliveryEngine.Execute(fixture.CreateAnonymous<IDeliveryEngineExecuteCommand>());
            Assert.That(eventCalled, Is.True);

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
            executeCommand.AssertWasCalled(m => m.OverrideArchiveInformationPackageId);
            // ReSharper disable ImplicitlyCapturedClosure
            archiveVersionRepositoryMock.AssertWasCalled(m => m.DataSource = Arg<IDataSource>.Is.Equal(dataSourceMock));
            // ReSharper restore ImplicitlyCapturedClosure

            archiveVersionRepositoryMock.AssertWasCalled(m => m.ArchiveMetaData());

            dataSourceMock.AssertWasCalled(m => m.Tables);
            executeCommand.AssertWasCalled(m => m.Table);
            executeCommand.AssertWasCalled(m => m.TablesHandledSimultaneity);
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.AssertWasCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Equal(dataSourceMock.Tables[0].NameTarget), Arg<IDataSource>.Is.Equal(dataSourceMock)));
            // ReSharper restore ImplicitlyCapturedClosure
            dataValidatorsMock.AssertWasCalled(m => m.GetEnumerator(), opt => opt.Repeat.Times(2));
            // ReSharper disable ImplicitlyCapturedClosure
            primaryKeyDataValidatorMock.AssertWasCalled(m => m.Validate(Arg<ITable>.Is.Equal(dataSourceMock.Tables[0]), Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.NotNull, Arg<bool>.Is.Equal(true), Arg<ICommand>.Is.NotNull));
            foreignKeyDataValidatorMock.AssertWasCalled(m => m.Validate(Arg<ITable>.Is.Equal(dataSourceMock.Tables[0]), Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.NotNull, Arg<bool>.Is.Equal(true), Arg<ICommand>.Is.NotNull));
            // ReSharper restore ImplicitlyCapturedClosure
            archiveVersionRepositoryMock.AssertWasCalled(m => m.ArchiveTableData(Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.Anything, Arg<object>.Is.NotNull));

            executeCommand.AssertWasCalled(m => m.ValidationOnly, opt => opt.Repeat.Times(2));

            exceptionHandlerMock.AssertWasNotCalled(m => m.HandleException(Arg<Exception>.Is.NotNull, out Arg<bool>.Out(true).Dummy));
            exceptionHandlerMock.AssertWasNotCalled(m => m.HandleException(Arg<Exception>.Is.NotNull));
        }

        /// <summary>
        /// Test that Execute archive data for each target table in the data source.
        /// </summary>
        [Test]
        public void TestThatExecuteArchiveDataForEachTargetTableInDataSource()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IConfigurationRepository>()));
            fixture.Customize<IDocumentRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDocumentRepository>()));

            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.Expect(m => m.DataGetForTargetTable(Arg<string>.Is.NotNull, Arg<IDataSource>.Is.Equal(dataSourceMock)))
                              .WhenCalled(e =>
                                  {
                                      Thread.Sleep(2500);
                                      var eventArgsMock = MockRepository.GenerateMock<IHandleDataEventArgs>();
                                      eventArgsMock.Expect(n => n.Table)
                                                   .Return(dataSourceMock.Tables.Single(m => string.Compare(m.NameTarget, e.Arguments.ElementAt(0).ToString(), StringComparison.OrdinalIgnoreCase) == 0))
                                                   .Repeat.Any();
                                      eventArgsMock.Expect(n => n.Data)
                                                   .Return(new List<IEnumerable<IDataObjectBase>>(0))
                                                   .Repeat.Any();
                                      eventArgsMock.Expect(n => n.EndOfData)
                                                   .Return(true)
                                                   .Repeat.Any();
                                      dataRepositoryMock.Raise(n => n.OnHandleData += null, this, eventArgsMock);
                                  })
                              .Repeat.Any();
            // ReSharper restore ImplicitlyCapturedClosure
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var mocker = new MockRepository();
            var dataValidatorsMock = mocker.DynamicMultiMock<IDataValidators>(new[] {typeof (IEnumerable<IDataValidator>)});
            Expect.Call(dataValidatorsMock.GetEnumerator())
                  .WhenCalled(e =>
                      {
                          var validators = new List<IDataValidator>(0);
                          e.ReturnValue = validators.GetEnumerator();
                      })
                  .Return(null)
                  .Repeat.Any();
            mocker.ReplayAll();
            fixture.Customize<IDataValidators>(e => e.FromFactory(() => dataValidatorsMock));

            var archiveVersionRepositoryMock = MockRepository.GenerateMock<IArchiveVersionRepository>();

            var exceptionHandlerMock = MockRepository.GenerateMock<IExceptionHandler>();
            fixture.Customize<IExceptionHandler>(e => e.FromFactory(() => exceptionHandlerMock));

            var deliveryEngine = new DeliveryEngine.BusinessLogic.DeliveryEngine(fixture.CreateAnonymous<IConfigurationRepository>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IDataRepository>(), fixture.CreateAnonymous<IDocumentRepository>(), fixture.CreateAnonymous<IDataValidators>(), archiveVersionRepositoryMock, fixture.CreateAnonymous<IExceptionHandler>());
            Assert.That(deliveryEngine, Is.Not.Null);

            var executeCommand = MockRepository.GenerateMock<IDeliveryEngineExecuteCommand>();
            executeCommand.Expect(m => m.OverrideArchiveInformationPackageId)
                          .Return(null)
                          .Repeat.Any();
            executeCommand.Expect(m => m.ValidationOnly)
                          .Return(false)
                          .Repeat.Any();
            executeCommand.Expect(m => m.Table)
                          .Return(null)
                          .Repeat.Any();
            executeCommand.Expect(m => m.TablesHandledSimultaneity)
                          .Return(5)
                          .Repeat.Any();
            fixture.Customize<IDeliveryEngineExecuteCommand>(e => e.FromFactory(() => executeCommand));

            deliveryEngine.Execute(fixture.CreateAnonymous<IDeliveryEngineExecuteCommand>());

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
            executeCommand.AssertWasCalled(m => m.OverrideArchiveInformationPackageId);
            // ReSharper disable ImplicitlyCapturedClosure
            archiveVersionRepositoryMock.AssertWasCalled(m => m.DataSource = Arg<IDataSource>.Is.Equal(dataSourceMock));
            // ReSharper restore ImplicitlyCapturedClosure

            archiveVersionRepositoryMock.AssertWasCalled(m => m.ArchiveMetaData());

            dataSourceMock.AssertWasCalled(m => m.Tables);
            executeCommand.AssertWasCalled(m => m.Table);
            executeCommand.AssertWasCalled(m => m.TablesHandledSimultaneity);
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.AssertWasCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Equal(dataSourceMock.Tables[0].NameTarget), Arg<IDataSource>.Is.Equal(dataSourceMock)));
            dataRepositoryMock.AssertWasCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Equal(dataSourceMock.Tables[1].NameTarget), Arg<IDataSource>.Is.Equal(dataSourceMock)));
            dataRepositoryMock.AssertWasCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Equal(dataSourceMock.Tables[2].NameTarget), Arg<IDataSource>.Is.Equal(dataSourceMock)));
            dataRepositoryMock.AssertWasCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Equal(dataSourceMock.Tables[3].NameTarget), Arg<IDataSource>.Is.Equal(dataSourceMock)));
            dataRepositoryMock.AssertWasCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Equal(dataSourceMock.Tables[4].NameTarget), Arg<IDataSource>.Is.Equal(dataSourceMock)));
            // ReSharper restore ImplicitlyCapturedClosure
            dataValidatorsMock.AssertWasCalled(m => m.GetEnumerator(), opt => opt.Repeat.Times(5));
            archiveVersionRepositoryMock.AssertWasCalled(m => m.ArchiveTableData(Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.NotNull, Arg<object>.Is.NotNull), opt => opt.Repeat.Times(5));

            executeCommand.AssertWasCalled(m => m.ValidationOnly, opt => opt.Repeat.Times(6));

            exceptionHandlerMock.AssertWasNotCalled(m => m.HandleException(Arg<Exception>.Is.NotNull));
        }

        /// <summary>
        /// Test that Execute raises the event BeforeArchiveDataForTargetTable.
        /// </summary>
        [Test]
        public void TestThatRaisesBeforeArchiveDataForTargetTableEvent()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IConfigurationRepository>()));
            fixture.Customize<IDocumentRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDocumentRepository>()));

            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(1).ToList())))
                          .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.Expect(m => m.DataGetForTargetTable(Arg<string>.Is.NotNull, Arg<IDataSource>.Is.Equal(dataSourceMock)))
                              .WhenCalled(e =>
                                  {
                                      Thread.Sleep(2500);
                                      var eventArgsMock = MockRepository.GenerateMock<IHandleDataEventArgs>();
                                      eventArgsMock.Expect(n => n.Table)
                                                   .Return(dataSourceMock.Tables.Single(m => string.Compare(m.NameTarget, e.Arguments.ElementAt(0).ToString(), StringComparison.OrdinalIgnoreCase) == 0))
                                                   .Repeat.Any();
                                      eventArgsMock.Expect(n => n.Data)
                                                   .Return(new List<IEnumerable<IDataObjectBase>>(0))
                                                   .Repeat.Any();
                                      eventArgsMock.Expect(n => n.EndOfData)
                                                   .Return(true)
                                                   .Repeat.Any();
                                      dataRepositoryMock.Raise(n => n.OnHandleData += null, this, eventArgsMock);
                                  })
                              .Repeat.Any();
            // ReSharper restore ImplicitlyCapturedClosure
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var mocker = new MockRepository();
            var dataValidatorsMock =
                mocker.DynamicMultiMock<IDataValidators>(new[] {typeof (IEnumerable<IDataValidator>)});
            Expect.Call(dataValidatorsMock.GetEnumerator())
                  .WhenCalled(e =>
                      {
                          var validators = new List<IDataValidator>(0);
                          e.ReturnValue = validators.GetEnumerator();
                      })
                  .Return(null)
                  .Repeat.Any();
            mocker.ReplayAll();
            fixture.Customize<IDataValidators>(e => e.FromFactory(() => dataValidatorsMock));

            var archiveVersionRepositoryMock = MockRepository.GenerateMock<IArchiveVersionRepository>();

            var exceptionHandlerMock = MockRepository.GenerateMock<IExceptionHandler>();
            fixture.Customize<IExceptionHandler>(e => e.FromFactory(() => exceptionHandlerMock));

            var deliveryEngine = new DeliveryEngine.BusinessLogic.DeliveryEngine(fixture.CreateAnonymous<IConfigurationRepository>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IDataRepository>(), fixture.CreateAnonymous<IDocumentRepository>(), fixture.CreateAnonymous<IDataValidators>(), archiveVersionRepositoryMock, fixture.CreateAnonymous<IExceptionHandler>());
            Assert.That(deliveryEngine, Is.Not.Null);

            var eventCalled = false;
            // ReSharper disable ImplicitlyCapturedClosure
            deliveryEngine.BeforeArchiveDataForTargetTable += (s, e) =>
                {
                    Assert.That(s, Is.Not.Null);
                    Assert.That(e, Is.Not.Null);
                    Assert.That(e.DataSource, Is.Not.Null);
                    Assert.That(e.DataSource, Is.EqualTo(dataSourceMock));
                    Assert.That(e.TargetTable, Is.Not.Null);
                    Assert.That(e.TargetTable, Is.EqualTo(dataSourceMock.Tables[0]));
                    eventCalled = true;
                };
            // ReSharper restore ImplicitlyCapturedClosure

            var executeCommand = MockRepository.GenerateMock<IDeliveryEngineExecuteCommand>();
            executeCommand.Expect(m => m.OverrideArchiveInformationPackageId)
                          .Return(null)
                          .Repeat.Any();
            executeCommand.Expect(m => m.ValidationOnly)
                          .Return(false)
                          .Repeat.Any();
            executeCommand.Expect(m => m.Table)
                          .Return(null)
                          .Repeat.Any();
            executeCommand.Expect(m => m.TablesHandledSimultaneity)
                          .Return(5)
                          .Repeat.Any();
            fixture.Customize<IDeliveryEngineExecuteCommand>(e => e.FromFactory(() => executeCommand));

            deliveryEngine.Execute(fixture.CreateAnonymous<IDeliveryEngineExecuteCommand>());
            Assert.That(eventCalled, Is.True);

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
            executeCommand.AssertWasCalled(m => m.OverrideArchiveInformationPackageId);
            // ReSharper disable ImplicitlyCapturedClosure
            archiveVersionRepositoryMock.AssertWasCalled(m => m.DataSource = Arg<IDataSource>.Is.Equal(dataSourceMock));
            // ReSharper restore ImplicitlyCapturedClosure

            archiveVersionRepositoryMock.AssertWasCalled(m => m.ArchiveMetaData());

            dataSourceMock.AssertWasCalled(m => m.Tables);
            executeCommand.AssertWasCalled(m => m.Table);
            executeCommand.AssertWasCalled(m => m.TablesHandledSimultaneity);
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.AssertWasCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Equal(dataSourceMock.Tables[0].NameTarget), Arg<IDataSource>.Is.Equal(dataSourceMock)));
            // ReSharper restore ImplicitlyCapturedClosure
            dataValidatorsMock.AssertWasCalled(m => m.GetEnumerator(), opt => opt.Repeat.Times(1));
            archiveVersionRepositoryMock.AssertWasCalled(m => m.ArchiveTableData(Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.NotNull, Arg<object>.Is.NotNull));

            executeCommand.AssertWasCalled(m => m.ValidationOnly, opt => opt.Repeat.Times(2));

            exceptionHandlerMock.AssertWasNotCalled(m => m.HandleException(Arg<Exception>.Is.NotNull));
        }

        /// <summary>
        /// Test that Execute don't archive data íf ValidationOnly is true in the command.
        /// </summary>
        [Test]
        public void TestThatExecuteDontArchiveDataIfValidationOnlyIsTrueInCommand()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IConfigurationRepository>()));
            fixture.Customize<IDocumentRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDocumentRepository>()));

            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.Expect(m => m.DataGetForTargetTable(Arg<string>.Is.NotNull, Arg<IDataSource>.Is.Equal(dataSourceMock)))
                              .WhenCalled(e =>
                                  {
                                      Thread.Sleep(2500);
                                      var eventArgsMock = MockRepository.GenerateMock<IHandleDataEventArgs>();
                                      eventArgsMock.Expect(n => n.Table)
                                                   .Return(dataSourceMock.Tables.Single(m => string.Compare(m.NameTarget, e.Arguments.ElementAt(0).ToString(), StringComparison.OrdinalIgnoreCase) == 0))
                                                   .Repeat.Any();
                                      eventArgsMock.Expect(n => n.Data)
                                                   .Return(new List<IEnumerable<IDataObjectBase>>(0))
                                                   .Repeat.Any();
                                      eventArgsMock.Expect(n => n.EndOfData)
                                                   .Return(true)
                                                   .Repeat.Any();
                                      dataRepositoryMock.Raise(n => n.OnHandleData += null, this, eventArgsMock);
                                  })
                              .Repeat.Any();
            // ReSharper restore ImplicitlyCapturedClosure
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var mocker = new MockRepository();
            var dataValidatorsMock = mocker.DynamicMultiMock<IDataValidators>(new[] {typeof (IEnumerable<IDataValidator>)});
            Expect.Call(dataValidatorsMock.GetEnumerator())
                  .WhenCalled(e =>
                      {
                          var validators = new List<IDataValidator>(0);
                          e.ReturnValue = validators.GetEnumerator();
                      })
                  .Return(null)
                  .Repeat.Any();
            mocker.ReplayAll();
            fixture.Customize<IDataValidators>(e => e.FromFactory(() => dataValidatorsMock));

            var archiveVersionRepositoryMock = MockRepository.GenerateMock<IArchiveVersionRepository>();

            var exceptionHandlerMock = MockRepository.GenerateMock<IExceptionHandler>();
            fixture.Customize<IExceptionHandler>(e => e.FromFactory(() => exceptionHandlerMock));

            var deliveryEngine = new DeliveryEngine.BusinessLogic.DeliveryEngine(fixture.CreateAnonymous<IConfigurationRepository>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IDataRepository>(), fixture.CreateAnonymous<IDocumentRepository>(), fixture.CreateAnonymous<IDataValidators>(), archiveVersionRepositoryMock, fixture.CreateAnonymous<IExceptionHandler>());
            Assert.That(deliveryEngine, Is.Not.Null);

            var executeCommand = MockRepository.GenerateMock<IDeliveryEngineExecuteCommand>();
            executeCommand.Expect(m => m.OverrideArchiveInformationPackageId)
                          .Return(null)
                          .Repeat.Any();
            executeCommand.Expect(m => m.ValidationOnly)
                          .Return(true)
                          .Repeat.Any();
            executeCommand.Expect(m => m.Table)
                          .Return(null)
                          .Repeat.Any();
            executeCommand.Expect(m => m.TablesHandledSimultaneity)
                          .Return(5)
                          .Repeat.Any();
            fixture.Customize<IDeliveryEngineExecuteCommand>(e => e.FromFactory(() => executeCommand));

            deliveryEngine.Execute(fixture.CreateAnonymous<IDeliveryEngineExecuteCommand>());

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
            executeCommand.AssertWasCalled(m => m.OverrideArchiveInformationPackageId);
            // ReSharper disable ImplicitlyCapturedClosure
            archiveVersionRepositoryMock.AssertWasCalled(m => m.DataSource = Arg<IDataSource>.Is.Equal(dataSourceMock));
            // ReSharper restore ImplicitlyCapturedClosure

            archiveVersionRepositoryMock.AssertWasNotCalled(m => m.ArchiveMetaData());

            dataSourceMock.AssertWasCalled(m => m.Tables);
            executeCommand.AssertWasCalled(m => m.Table);
            executeCommand.AssertWasCalled(m => m.TablesHandledSimultaneity);
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.AssertWasCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Equal(dataSourceMock.Tables[0].NameTarget), Arg<IDataSource>.Is.Equal(dataSourceMock)));
            dataRepositoryMock.AssertWasCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Equal(dataSourceMock.Tables[1].NameTarget), Arg<IDataSource>.Is.Equal(dataSourceMock)));
            dataRepositoryMock.AssertWasCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Equal(dataSourceMock.Tables[2].NameTarget), Arg<IDataSource>.Is.Equal(dataSourceMock)));
            dataRepositoryMock.AssertWasCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Equal(dataSourceMock.Tables[3].NameTarget), Arg<IDataSource>.Is.Equal(dataSourceMock)));
            dataRepositoryMock.AssertWasCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Equal(dataSourceMock.Tables[4].NameTarget), Arg<IDataSource>.Is.Equal(dataSourceMock)));
            // ReSharper restore ImplicitlyCapturedClosure
            dataValidatorsMock.AssertWasCalled(m => m.GetEnumerator(), opt => opt.Repeat.Times(5));
            archiveVersionRepositoryMock.AssertWasNotCalled(m => m.ArchiveTableData(Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.Anything, Arg<object>.Is.Anything));

            executeCommand.AssertWasCalled(m => m.ValidationOnly, opt => opt.Repeat.Times(6));

            exceptionHandlerMock.AssertWasNotCalled(m => m.HandleException(Arg<Exception>.Is.NotNull));
        }

        /// <summary>
        /// Test that Execute calls the exception handler when an exception occurs.
        /// </summary>
        [Test]
        public void TestThatExecuteCallsExceptionHandlerWhenExceptionOccurs()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataSource>()));
            fixture.Customize<IConfigurationRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IConfigurationRepository>()));
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));
            fixture.Customize<IDocumentRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDocumentRepository>()));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Throw(fixture.CreateAnonymous<DeliveryEngineRepositoryException>());
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));

            var mocker = new MockRepository();
            var dataValidatorsMock =
                mocker.DynamicMultiMock<IDataValidators>(new[] {typeof (IEnumerable<IDataValidator>)});
            Expect.Call(dataValidatorsMock.GetEnumerator())
                  .WhenCalled(e =>
                      {
                          var validators = new List<IDataValidator>(0);
                          e.ReturnValue = validators.GetEnumerator();
                      })
                  .Return(null)
                  .Repeat.Any();
            mocker.ReplayAll();
            fixture.Customize<IDataValidators>(e => e.FromFactory(() => dataValidatorsMock));

            var archiveVersionRepositoryMock = MockRepository.GenerateMock<IArchiveVersionRepository>();

            var exceptionHandlerMock = MockRepository.GenerateMock<IExceptionHandler>();
            fixture.Customize<IExceptionHandler>(e => e.FromFactory(() => exceptionHandlerMock));

            var deliveryEngine = new DeliveryEngine.BusinessLogic.DeliveryEngine(fixture.CreateAnonymous<IConfigurationRepository>(), fixture.CreateAnonymous<IMetadataRepository>(), fixture.CreateAnonymous<IDataRepository>(), fixture.CreateAnonymous<IDocumentRepository>(), fixture.CreateAnonymous<IDataValidators>(), archiveVersionRepositoryMock, fixture.CreateAnonymous<IExceptionHandler>());
            Assert.That(deliveryEngine, Is.Not.Null);

            var executeCommand = MockRepository.GenerateMock<IDeliveryEngineExecuteCommand>();
            fixture.Customize<IDeliveryEngineExecuteCommand>(e => e.FromFactory(() => executeCommand));

            deliveryEngine.Execute(fixture.CreateAnonymous<IDeliveryEngineExecuteCommand>());

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
            executeCommand.AssertWasNotCalled(m => m.OverrideArchiveInformationPackageId);
            archiveVersionRepositoryMock.AssertWasNotCalled(m => m.DataSource = Arg<IDataSource>.Is.Anything);

            executeCommand.AssertWasNotCalled(m => m.ValidationOnly);
            archiveVersionRepositoryMock.AssertWasNotCalled(m => m.ArchiveMetaData());

            dataRepositoryMock.AssertWasNotCalled(m => m.DataGetForTargetTable(Arg<string>.Is.Anything, Arg<IDataSource>.Is.Anything));
            dataValidatorsMock.AssertWasNotCalled(m => m.GetEnumerator(), opt => opt.Repeat.Times(1));
            archiveVersionRepositoryMock.AssertWasNotCalled(m => m.ArchiveTableData(Arg<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>.Is.Anything, Arg<object>.Is.Anything));

            exceptionHandlerMock.AssertWasCalled(m => m.HandleException(Arg<Exception>.Is.NotNull));
        }
    }
}
