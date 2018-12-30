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
using Domstolene.JFS.CommonLibrary.IoC.Interfaces.Windsor;
using DsiNext.DeliveryEngine.BusinessLogic.Commands;
using DsiNext.DeliveryEngine.BusinessLogic.DataValidators;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Commands;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.DataValidators;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.ExceptionHandling;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.ExceptionHandling;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Log;
using DsiNext.DeliveryEngine.Infrastructure.IoC;
using DsiNext.DeliveryEngine.Repositories;
using DsiNext.DeliveryEngine.Repositories.Configuration;
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
            int warnings = 0;
            IExceptionHandler exceptionHandlerMock = MockRepository.GenerateMock<IExceptionHandler>();
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
                    if (e.Arguments.ElementAt(0) is DeliveryEngineValidateException validationException)
                    {
                        Debug.WriteLine("{0}: {1}", "WARNING", validationException.Message);
                        warnings++;
                        e.Arguments[1] = warnings < 25;
                        return;
                    }

                    if (e.Arguments.ElementAt(0) is DeliveryEngineMappingException mappingException)
                    {
                        Debug.WriteLine("{0}: {1}", "WARNING", mappingException.Message);
                        warnings++;
                        e.Arguments[1] = warnings < 25;
                        return;
                    }

                    Exception exception = (Exception) e.Arguments.ElementAt(0);
                    Debug.WriteLine(exception);
                    e.Arguments[1] = false;
                    exceptionHandlerMock.Raise(f => f.OnException += null, exceptionHandlerMock, new HandleExceptionEventArgs(exception));
                })
                .Repeat.Any();
            exceptionHandlerMock.Expect(m => m.HandleException(Arg<Exception>.Is.NotNull))
                .WhenCalled(e =>
                {
                    Exception exception = (Exception) e.Arguments.ElementAt(0);
                    Debug.WriteLine(exception.Message);
                    Debug.WriteLine(exception.StackTrace);
                    exceptionHandlerMock.Raise(f => f.OnException += null, exceptionHandlerMock, new HandleExceptionEventArgs(exception));
                })
                .Repeat.Any();

            IDeliveryEngine deliveryEngine = CreateSut(true, exceptionHandlerMock);
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

            IConfigurationRepository configurationRepository = new ConfigurationRepository(ConfigurationManager.AppSettings);
            IDeliveryEngineExecuteCommand command = new DeliveryEngineExecuteCommand
            {
                OverrideArchiveInformationPackageId = "AVID.SA.12549",
                ValidationOnly = true,
                TablesHandledSimultaneity = 3,
                IncludeEmptyTables = configurationRepository.IncludeEmptyTables
            };
            deliveryEngine.Execute(command);
        }

        /// <summary>
        /// Test archivation in the delivery engine.
        /// </summary>
        [Test]
        public void TestArchivation()
        {
            IExceptionHandler exceptionHandlerMock = MockRepository.GenerateMock<IExceptionHandler>();
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
                    Exception exception = (Exception) e.Arguments.ElementAt(0);
                    Debug.WriteLine(exception);
                    e.Arguments[1] = false;
                    exceptionHandlerMock.Raise(f => f.OnException += null, exceptionHandlerMock, new HandleExceptionEventArgs(exception));
                })
                .Repeat.Any();
            exceptionHandlerMock.Expect(m => m.HandleException(Arg<Exception>.Is.NotNull))
                .WhenCalled(e =>
                {
                    Exception exception = (Exception) e.Arguments.ElementAt(0);
                    Debug.WriteLine(exception);
                    exceptionHandlerMock.Raise(f => f.OnException += null, exceptionHandlerMock,
                        new HandleExceptionEventArgs(exception));
                })
                .Repeat.Any();

            IDeliveryEngine deliveryEngine = CreateSut(false, exceptionHandlerMock);
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

            IConfigurationRepository configurationRepository = new ConfigurationRepository(ConfigurationManager.AppSettings);
            IDeliveryEngineExecuteCommand command = new DeliveryEngineExecuteCommand
            {
                OverrideArchiveInformationPackageId = "AVID.SA.12549",
                ValidationOnly = false,
                TablesHandledSimultaneity = 3,
                IncludeEmptyTables = configurationRepository.IncludeEmptyTables
            };
            deliveryEngine.Execute(command);
        }

        /// <summary>
        /// Test.
        /// </summary>
        [Test]
        public void Test()
        {
            IMetadataRepository metadataRepository = new OldToNewMetadataRepository(RepositoryTestHelper.GetSourcePathForOracleTest(), new ConfigurationValues());
            IDataSource dataSource = metadataRepository.DataSourceGet();
            foreach (IContextDocument contextDocument in dataSource.ContextDocuments)
            {
                Debug.WriteLine(contextDocument.NameTarget);
            }
            Debug.WriteLine(string.Empty);
            Debug.WriteLine(string.Empty);
            foreach (IView view in dataSource.Views)
            {
                Debug.WriteLine(view.Description);
                Debug.WriteLine(view.SqlQuery);
                Debug.WriteLine(string.Empty);
            }
        }

        private IDeliveryEngine CreateSut(bool useDataValidators, IExceptionHandler exceptionHandler)
        {
            if (exceptionHandler == null)
            {
                throw new ArgumentNullException(nameof(exceptionHandler));
            }

            IContainer containerMock = MockRepository.GenerateMock<IContainer>();

            IInformationLogger informationLoggerMock = MockRepository.GenerateMock<IInformationLogger>();
            informationLoggerMock.Expect(m => m.LogInformation(Arg<string>.Is.NotNull))
                .WhenCalled(e => Debug.WriteLine(e.Arguments.ElementAt(0)))
                .Repeat.Any();
            informationLoggerMock.Expect(m => m.LogWarning(Arg<string>.Is.NotNull))
                .WhenCalled(e => Debug.WriteLine(e.Arguments.ElementAt(0)))
                .Repeat.Any();

            IConfigurationRepository configurationRepositoryMock = MockRepository.GenerateMock<IConfigurationRepository>();
            IMetadataRepository metadataRepository = new OldToNewMetadataRepository(RepositoryTestHelper.GetSourcePathForOracleTest(), new ConfigurationValues());
            containerMock.Expect(m => m.Resolve<IMetadataRepository>())
                .Return(metadataRepository)
                .Repeat.Any();

            ICollection<IDataManipulator> dataManipulatorCollection;
            using (var windsorContainer = new WindsorContainer())
            {
                windsorContainer.Register(Component.For<IContainer>().Instance(containerMock).LifeStyle.Transient);
                windsorContainer.Register(Component.For<IInformationLogger>().Instance(informationLoggerMock).LifeStyle.Transient);
                windsorContainer.Register(Component.For<IMetadataRepository>().Instance(metadataRepository).LifeStyle.Transient);

                IConfigurationProvider dataManipulatorsConfigurationProvider = new DataManipulatorsConfigurationProvider();
                dataManipulatorsConfigurationProvider.AddConfiguration(windsorContainer);

                dataManipulatorCollection = windsorContainer.ResolveAll<IDataManipulator>();
                windsorContainer.Dispose();
            }
            containerMock.Expect(m => m.ResolveAll<IDataManipulator>())
                .Return(dataManipulatorCollection.ToArray())
                .Repeat.Any();
            IDataRepository dataRepository = new OracleDataRepository(new OracleClientFactory(), new DataManipulators(containerMock));
            containerMock.Expect(m => m.Resolve<IDataRepository>())
                .Return(dataRepository)
                .Repeat.Any();
            IDocumentRepository documentRepositoryMock = MockRepository.GenerateMock<IDocumentRepository>();
            IArchiveVersionRepository archiveRepository = new ArchiveVersionRepository(new DirectoryInfo(ConfigurationManager.AppSettings["ArchivePath"]));

            ICollection<IDataValidator> dataValidatorCollection = useDataValidators ? new Collection<IDataValidator> {new PrimaryKeyDataValidator(dataRepository), new ForeignKeysDataValidator(dataRepository), new MappingDataValidator()} : new Collection<IDataValidator>();
            containerMock.Expect(m => m.ResolveAll<IDataValidator>())
                .Return(dataValidatorCollection.ToArray())
                .Repeat.Any();
            IDataValidators dataValidators = new DataValidators(containerMock);

            return new DeliveryEngine.BusinessLogic.DeliveryEngine(configurationRepositoryMock, metadataRepository, dataRepository, documentRepositoryMock, dataValidators, archiveRepository, exceptionHandler);
        }
    }
}
