using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Domstolene.JFS.CommonLibrary.IoC.Interfaces;
using DsiNext.DeliveryEngine.BusinessLogic.Commands;
using DsiNext.DeliveryEngine.BusinessLogic.DataValidators;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.DataValidators;
using DsiNext.DeliveryEngine.Infrastructure.ExceptionHandling;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.ExceptionHandling;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Log;
using DsiNext.DeliveryEngine.Infrastructure.IoC;
using DsiNext.DeliveryEngine.Repositories;
using DsiNext.DeliveryEngine.Repositories.Data.Oracle;
using DsiNext.DeliveryEngine.Repositories.DataManipulators;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators;
using DsiNext.DeliveryEngine.Repositories.Metadata.OldToNew;
using DsiNext.DeliveryEngine.Tests.Unittests.Repositories;
using NUnit.Framework;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Integrationtests.BusinessLogic
{

    /// <summary>
    /// Integration tests of business logic in the delivery engine with an oracle data repository.
    /// </summary>
    [TestFixture]
    [Category("IntegrationTests")]
    public class DeliveryEngineWithOracleDataRepositoryTests
    {
        /// <summary>
        /// Test validation in the delivery engine.
        /// </summary>
        [Test]
        public void TestValidation()
        {
            var containerMock = MockRepository.GenerateMock<IContainer>();
            var informationLoggerMock = MockRepository.GenerateMock<IInformationLogger>();
            informationLoggerMock.Expect(m => m.LogInformation(Arg<string>.Is.NotNull))
                                 .WhenCalled(e => Debug.WriteLine(e.Arguments.ElementAt(0)))
                                 .Repeat.Any();
            informationLoggerMock.Expect(m => m.LogWarning(Arg<string>.Is.NotNull))
                                 .WhenCalled(e => Debug.WriteLine(e.Arguments.ElementAt(0)))
                                 .Repeat.Any();

            var warnings = 0;
            var exceptionHandlerMock = MockRepository.GenerateMock<IExceptionHandler>();
            exceptionHandlerMock.OnException += (sender, eventArgs) =>
                {
                    Assert.That(sender, Is.Not.Null);
                    Assert.That(eventArgs, Is.Not.Null);
                    Assert.That(eventArgs.Exception, Is.Not.Null);
                    throw eventArgs.Exception;
                };
            exceptionHandlerMock.Expect(m => m.HandleException(Arg<Exception>.Is.NotNull, out Arg<bool>.Out(true).Dummy))
                                .WhenCalled(e =>
                                    {
                                        var validationException = e.Arguments.ElementAt(0) as DeliveryEngineValidateException;
                                        if (validationException != null)
                                        {
                                            Debug.WriteLine("{0}: {1}", "WARNING", validationException.Message);
                                            warnings++;
                                            e.Arguments[1] = warnings < 25;
                                            return;
                                        }
                                        var mappingException = e.Arguments.ElementAt(0) as DeliveryEngineMappingException;
                                        if (mappingException != null)
                                        {
                                            Debug.WriteLine("{0}: {1}", "WARNING", mappingException.Message);
                                            warnings++;
                                            e.Arguments[1] = warnings < 25;
                                            return;
                                        }
                                        var exception = (Exception) e.Arguments.ElementAt(0);
                                        Debug.WriteLine(exception);
                                        e.Arguments[1] = false;
                                        exceptionHandlerMock.Raise(f => f.OnException += null, exceptionHandlerMock, new HandleExceptionEventArgs(exception));
                                    })
                                .Repeat.Any();
            exceptionHandlerMock.Expect(m => m.HandleException(Arg<Exception>.Is.NotNull))
                                .WhenCalled(e =>
                                    {
                                        var exception = (Exception) e.Arguments.ElementAt(0);
                                        Debug.WriteLine(exception.InnerException.Message);
                                        Debug.WriteLine(exception.InnerException.StackTrace);
                                        exceptionHandlerMock.Raise(f => f.OnException += null, exceptionHandlerMock, new HandleExceptionEventArgs(exception));
                                    })
                                .Repeat.Any();

            var configurationRepositoryMock = MockRepository.GenerateMock<IConfigurationRepository>();
            var metadataRepository = new OldToNewMetadataRepository(RepositoryTestHelper.GetSourcePathForOracleTest(), new ConfigurationValues());
            containerMock.Expect(m => m.Resolve<IMetadataRepository>())
                         .Return(metadataRepository)
                         .Repeat.Any();
            ICollection<IDataManipulator> dataMainpulatorCollection;
            using (var windsorContainer = new WindsorContainer())
            {
                windsorContainer.Register(Component.For<IContainer>().Instance(containerMock).LifeStyle.Transient);
                windsorContainer.Register(Component.For<IInformationLogger>().Instance(informationLoggerMock).LifeStyle.Transient);
                windsorContainer.Register(Component.For<IMetadataRepository>().Instance(metadataRepository).LifeStyle.Transient);
                var dataManipulatorsConfigurationProvider = new DataManipulatorsConfigurationProvider();
                dataManipulatorsConfigurationProvider.AddConfiguration(windsorContainer);
                dataMainpulatorCollection = windsorContainer.ResolveAll<IDataManipulator>();
                windsorContainer.Dispose();
            }
            containerMock.Expect(m => m.ResolveAll<IDataManipulator>())
                         .Return(dataMainpulatorCollection.ToArray())
                         .Repeat.Any();
            var dataRepository = new OracleDataRepository(new OracleClientFactory(), new DataManipulators(containerMock));
            containerMock.Expect(m => m.Resolve<IDataRepository>())
                         .Return(dataRepository)
                         .Repeat.Any();
            var documentRepositoryMock = MockRepository.GenerateMock<IDocumentRepository>();
            var archiveRepository = new ArchiveVersionRepository(new DirectoryInfo(ConfigurationManager.AppSettings["ArchivePath"]));

            containerMock.Expect(m => m.ResolveAll<IDataValidator>())
                         .Return(new Collection<IDataValidator> {new PrimaryKeyDataValidator(dataRepository), new ForeignKeysDataValidator(dataRepository), new MappingDataValidator()}.ToArray())
                         .Repeat.Any();
            var dataValidators = new DataValidators(containerMock);

            var deliveryEngine = new DeliveryEngine.BusinessLogic.DeliveryEngine(configurationRepositoryMock, metadataRepository, dataRepository, documentRepositoryMock, dataValidators, archiveRepository, exceptionHandlerMock);
            Assert.That(deliveryEngine, Is.Not.Null);

            deliveryEngine.BeforeGetDataSource += (sender, eventArgs) =>
                {
                    Assert.That(sender, Is.Not.Null);
                    Assert.That(eventArgs, Is.Not.Null);
                    Debug.WriteLine("Getting the data source. Please wait...");
                };
            deliveryEngine.BeforeGetDataForTargetTable += (sender, eventArgs) =>
                {
                    Assert.That(sender, Is.Not.Null);
                    Assert.That(eventArgs, Is.Not.Null);
                    Assert.That(eventArgs.DataSource, Is.Not.Null);
                    Assert.That(eventArgs.TargetTable, Is.Not.Null);
                    Debug.WriteLine("Getting data for the table named '{0}' (reading data from block {1}). Please wait...", eventArgs.TargetTable.NameTarget, eventArgs.DataBlock);
                };
            deliveryEngine.BeforeValidateDataInTargetTable += (sender, eventArgs) =>
                {
                    Assert.That(sender, Is.Not.Null);
                    Assert.That(eventArgs, Is.Not.Null);
                    Assert.That(eventArgs.DataSource, Is.Not.Null);
                    Assert.That(eventArgs.TargetTable, Is.Not.Null);
                    Debug.WriteLine("Validating data from table named '{0}' (validating data from block {1}). Please wait...", eventArgs.TargetTable.NameTarget, eventArgs.DataBlock);
                };

            var command = new DeliveryEngineExecuteCommand
                {
                    OverrideArchiveInformationPackageId = "AVID.SA.12549",
                    ValidationOnly = true,
                    TablesHandledSimultaneity = 3
                };
            deliveryEngine.Execute(command);
        }

        /// <summary>
        /// Test archivation in the delivery engine.
        /// </summary>
        [Test]
        public void TestArchivation()
        {
            var containerMock = MockRepository.GenerateMock<IContainer>();
            var informationLoggerMock = MockRepository.GenerateMock<IInformationLogger>();
            informationLoggerMock.Expect(m => m.LogInformation(Arg<string>.Is.NotNull))
                                 .WhenCalled(e => Debug.WriteLine(e.Arguments.ElementAt(0)))
                                 .Repeat.Any();
            informationLoggerMock.Expect(m => m.LogWarning(Arg<string>.Is.NotNull))
                                 .WhenCalled(e => Debug.WriteLine(e.Arguments.ElementAt(0)))
                                 .Repeat.Any();
            var exceptionHandlerMock = MockRepository.GenerateMock<IExceptionHandler>();
            exceptionHandlerMock.OnException += (sender, eventArgs) =>
                {
                    Assert.That(sender, Is.Not.Null);
                    Assert.That(eventArgs, Is.Not.Null);
                    Assert.That(eventArgs.Exception, Is.Not.Null);
                    throw eventArgs.Exception;
                };
            exceptionHandlerMock.Expect(m => m.HandleException(Arg<Exception>.Is.NotNull, out Arg<bool>.Out(true).Dummy))
                                .WhenCalled(e =>
                                    {
                                        var exception = (Exception) e.Arguments.ElementAt(0);
                                        Debug.WriteLine(exception);
                                        e.Arguments[1] = false;
                                        exceptionHandlerMock.Raise(f => f.OnException += null, exceptionHandlerMock, new HandleExceptionEventArgs(exception));
                                    })
                                .Repeat.Any();
            exceptionHandlerMock.Expect(m => m.HandleException(Arg<Exception>.Is.NotNull))
                                .WhenCalled(e =>
                                    {
                                        var exception = (Exception) e.Arguments.ElementAt(0);
                                        Debug.WriteLine(exception);
                                        exceptionHandlerMock.Raise(f => f.OnException += null, exceptionHandlerMock, new HandleExceptionEventArgs(exception));
                                    })
                                .Repeat.Any();

            var configurationRepositoryMock = MockRepository.GenerateMock<IConfigurationRepository>();
            var metadataRepository = new OldToNewMetadataRepository(RepositoryTestHelper.GetSourcePathForOracleTest(), new ConfigurationValues());
            containerMock.Expect(m => m.Resolve<IMetadataRepository>())
                         .Return(metadataRepository)
                         .Repeat.Any();
            ICollection<IDataManipulator> dataMainpulatorCollection;
            using (var windsorContainer = new WindsorContainer())
            {
                windsorContainer.Register(Component.For<IContainer>().Instance(containerMock).LifeStyle.Transient);
                windsorContainer.Register(Component.For<IInformationLogger>().Instance(informationLoggerMock).LifeStyle.Transient);
                windsorContainer.Register(Component.For<IMetadataRepository>().Instance(metadataRepository).LifeStyle.Transient);
                var dataManipulatorsConfigurationProvider = new DataManipulatorsConfigurationProvider();
                dataManipulatorsConfigurationProvider.AddConfiguration(windsorContainer);
                dataMainpulatorCollection = windsorContainer.ResolveAll<IDataManipulator>();
                windsorContainer.Dispose();
            }
            containerMock.Expect(m => m.ResolveAll<IDataManipulator>())
                         .Return(dataMainpulatorCollection.ToArray())
                         .Repeat.Any();
            var dataRepository = new OracleDataRepository(new OracleClientFactory(), new DataManipulators(containerMock));
            containerMock.Expect(m => m.Resolve<IDataRepository>())
                         .Return(dataRepository)
                         .Repeat.Any();
            var documentRepositoryMock = MockRepository.GenerateMock<IDocumentRepository>();
            var archiveRepository = new ArchiveVersionRepository(new DirectoryInfo(ConfigurationManager.AppSettings["ArchivePath"]));

            containerMock.Expect(m => m.ResolveAll<IDataValidator>())
                         .Return(new Collection<IDataValidator>().ToArray())
                         .Repeat.Any();
            var dataValidators = new DataValidators(containerMock);

            var deliveryEngine = new DeliveryEngine.BusinessLogic.DeliveryEngine(configurationRepositoryMock, metadataRepository, dataRepository, documentRepositoryMock, dataValidators, archiveRepository, exceptionHandlerMock);
            Assert.That(deliveryEngine, Is.Not.Null);

            deliveryEngine.BeforeGetDataSource += (sender, eventArgs) =>
                {
                    Assert.That(sender, Is.Not.Null);
                    Assert.That(eventArgs, Is.Not.Null);
                    Debug.WriteLine("Getting the data source. Please wait...");
                };
            deliveryEngine.BeforeArchiveMetadata += (sender, eventArgs) =>
                {
                    Assert.That(sender, Is.Not.Null);
                    Assert.That(eventArgs, Is.Not.Null);
                    Assert.That(eventArgs.DataSource, Is.Not.Null);
                    Debug.WriteLine("Archive metadata for '{0}{1}'. Please wait...", eventArgs.DataSource.NameTarget, string.Empty);
                };
            deliveryEngine.BeforeGetDataForTargetTable += (sender, eventArgs) =>
                {
                    Assert.That(sender, Is.Not.Null);
                    Assert.That(eventArgs, Is.Not.Null);
                    Assert.That(eventArgs.DataSource, Is.Not.Null);
                    Assert.That(eventArgs.TargetTable, Is.Not.Null);
                    Debug.WriteLine("Getting data for the table named '{0}' (reading data from block {1}). Please wait...", eventArgs.TargetTable.NameTarget, eventArgs.DataBlock);
                };
            deliveryEngine.BeforeValidateDataInTargetTable += (sender, eventArgs) =>
                {
                    Assert.That(sender, Is.Not.Null);
                    Assert.That(eventArgs, Is.Not.Null);
                    Assert.That(eventArgs.DataSource, Is.Not.Null);
                    Assert.That(eventArgs.TargetTable, Is.Not.Null);
                    Debug.WriteLine("Validating data from table named '{0}' (validating data from block {1}). Please wait...", eventArgs.TargetTable.NameTarget, eventArgs.DataBlock);
                };
            deliveryEngine.BeforeArchiveDataForTargetTable += (sender, eventArgs) =>
                {
                    Assert.That(sender, Is.Not.Null);
                    Assert.That(eventArgs, Is.Not.Null);
                    Assert.That(eventArgs.DataSource, Is.Not.Null);
                    Assert.That(eventArgs.TargetTable, Is.Not.Null);
                    Debug.WriteLine("Writing data from the table named '{0}' (writing data from block {1}). Please wait...", eventArgs.TargetTable.NameTarget, eventArgs.DataBlock);
                };

            var command = new DeliveryEngineExecuteCommand
                {
                    OverrideArchiveInformationPackageId = "AVID.SA.12549",
                    ValidationOnly = false,
                    TablesHandledSimultaneity = 3
                };
            deliveryEngine.Execute(command);
        }

        /// <summary>
        /// Test.
        /// </summary>
        [Test]
        public void Test()
        {
            var metadataRepository = new OldToNewMetadataRepository(RepositoryTestHelper.GetSourcePathForOracleTest(), new ConfigurationValues());
            var dataSource = metadataRepository.DataSourceGet();
            foreach (var contextDocument in dataSource.ContextDocuments)
            {
                Debug.WriteLine(contextDocument.NameTarget);
            }
            Debug.WriteLine(string.Empty);
            Debug.WriteLine(string.Empty);
            foreach (var view in dataSource.Views)
            {
                Debug.WriteLine(view.Description);
                Debug.WriteLine(view.SqlQuery);
                Debug.WriteLine(string.Empty);
            }
        }
    }
}
