using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Domstolene.JFS.CommonLibrary.IoC;
using Domstolene.JFS.CommonLibrary.IoC.Interfaces;
using DsiNext.DeliveryEngine.BusinessLogic.Commands;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Commands;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Events;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.ExceptionHandling;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Log;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Gui.ArchiveMaker
{
    /// <summary>
    /// Console program class.
    /// </summary>
    public class Program
    {
        #region Properties

        /// <summary>
        /// Indicates whether the delivery engine is only in the validation mode.
        /// </summary>
        private static bool ValidationOnly { get; set; }

        /// <summary>
        /// Number of warnings to be accepted before the delivery engine stops.
        /// </summary>
        private static int AcceptWarnings { get; set; }

        /// <summary>
        /// Information logger which can log information and warnings.
        /// </summary>
        private static IInformationLogger InformationLogger { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Entry point.
        /// </summary>
        /// <param name="args">Arguments</param>
        static void Main(string[] args)
        {
            Console.WriteLine(Resource.GetText(Text.ArchiveMaker));
            Console.WriteLine();

            ValidationOnly = false;
            AcceptWarnings = 1024;
            string archiveInformationPackageId = null;
            int tablesHandledSimultaneity = 5;
            bool removeMissingRelationshipsOnForeignKeys = false;
            int numberOfForeignTablesToCache = 10;
            bool? includeEmptyTables = null;
            try
            {
                if (args != null && args.Length > 0)
                {
                    archiveInformationPackageId = args.Single(m => string.IsNullOrEmpty(m) == false && m.Length > 29 && m.Substring(0, 29).Equals("/ArchiveInformationPackageID:")).Substring(29);

                    string optionalArg = args.SingleOrDefault(m => string.IsNullOrEmpty(m) == false && m.Length > 16 && m.Substring(0, 16).Equals("/ValidationOnly:"));
                    if (optionalArg != null)
                    {
                        ValidationOnly = bool.Parse(optionalArg.Substring(16));
                    }

                    optionalArg = args.SingleOrDefault(m => string.IsNullOrEmpty(m) == false && m.Length > 28 && m.Substring(0, 30).Equals("/TablesHandledSimultaneity:"));
                    if (optionalArg != null)
                    {
                        tablesHandledSimultaneity = int.Parse(optionalArg.Substring(28));
                    }

                    optionalArg = args.SingleOrDefault(m => string.IsNullOrEmpty(m) == false && m.Length > 16 && m.Substring(0, 16).Equals("/AcceptWarnings:"));
                    if (optionalArg != null)
                    {
                        AcceptWarnings = int.Parse(optionalArg.Substring(16));
                    }

                    optionalArg = args.SingleOrDefault(m => string.IsNullOrEmpty(m) == false && m.Length > 41 && m.Substring(0, 41).Equals("/RemoveMissingRelationshipsOnForeignKeys:"));
                    if (optionalArg != null)
                    {
                        removeMissingRelationshipsOnForeignKeys = bool.Parse(optionalArg.Substring(41));
                    }

                    optionalArg = args.SingleOrDefault(m => string.IsNullOrEmpty(m) == false && m.Length > 30 && m.Substring(0, 30).Equals("/NumberOfForeignTablesToCache:"));
                    if (optionalArg != null)
                    {
                        numberOfForeignTablesToCache = int.Parse(optionalArg.Substring(30));
                    }

                    optionalArg = args.SingleOrDefault(m => string.IsNullOrEmpty(m) == false && m.Length > 20 && m.Substring(0, 20).Equals("/IncludeEmptyTables:"));
                    if (optionalArg != null)
                    {
                        includeEmptyTables = Convert.ToBoolean(optionalArg.Substring(20));
                    }
                }

                if (string.IsNullOrEmpty(archiveInformationPackageId))
                {
                    Console.WriteLine(Resource.GetText(Text.UsageArchiveMaker, new FileInfo(Process.GetCurrentProcess().MainModule.FileName).Name));
                    Console.WriteLine();
                    return;
                }
            }
            catch
            {
                Console.WriteLine(Resource.GetText(Text.UsageArchiveMaker, new FileInfo(Process.GetCurrentProcess().MainModule.FileName).Name));
                Console.WriteLine();
                return;
            }

            try
            {
                IContainer container = ContainerFactory.Create();

                InformationLogger = container.Resolve<IInformationLogger>();

                IExceptionHandler exceptionHandler = container.Resolve<IExceptionHandler>();
                exceptionHandler.OnException += ExceptionEventHandler;

                IDeliveryEngine deliveryEngine = container.Resolve<IDeliveryEngine>();
                deliveryEngine.BeforeGetDataSource += BeforeGetDataSourceEventHandler;
                deliveryEngine.BeforeArchiveMetadata += BeforeArchiveMetadataEventHandler;
                deliveryEngine.BeforeGetDataForTargetTable += BeforeGetDataForTargetTableEventHandler;
                deliveryEngine.BeforeValidateDataInTargetTable += BeforeValidatingDataInTargetTableEventHandler;
                deliveryEngine.BeforeArchiveDataForTargetTable += BeforeArchiveDataForTargetTableEventHandler;

                IConfigurationRepository configurationRepository = container.Resolve<IConfigurationRepository>();
                IDeliveryEngineExecuteCommand command = new DeliveryEngineExecuteCommand
                {
                    OverrideArchiveInformationPackageId = archiveInformationPackageId,
                    ValidationOnly = ValidationOnly,
                    TablesHandledSimultaneity = tablesHandledSimultaneity,
                    RemoveMissingRelationshipsOnForeignKeys = removeMissingRelationshipsOnForeignKeys,
                    NumberOfForeignTablesToCache = numberOfForeignTablesToCache,
                    IncludeEmptyTables = includeEmptyTables ?? configurationRepository.IncludeEmptyTables
                };
                deliveryEngine.Execute(command);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                Console.WriteLine(Resource.GetText(Text.ErrorMessage, ex.Message));
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Event handler raised when an exception occurs;
        /// </summary>
        /// <param name="sender">Object which raise the event.</param>
        /// <param name="eventArgs">Arguments to the event.</param>
        private static void ExceptionEventHandler(object sender, IHandleExceptionEventArgs eventArgs)
        {
            if (eventArgs == null)
            {
                throw new ArgumentNullException(nameof(eventArgs));
            }

            if (eventArgs.Exception is DeliveryEngineValidateException)
            {
                Console.WriteLine("{0}: {1}", Resource.GetText(Text.Warning), eventArgs.Message);
                InformationLogger.LogWarning(eventArgs.Message);
                AcceptWarnings -= 1;
                eventArgs.CanContinue = ValidationOnly && (AcceptWarnings > 0);
                return;
            }

            if (eventArgs.Exception is DeliveryEngineMappingException)
            {
                Console.WriteLine("{0}: {1}", Resource.GetText(Text.Warning), eventArgs.Message);
                InformationLogger.LogWarning(eventArgs.Message);
                AcceptWarnings -= 1;
                eventArgs.CanContinue = ValidationOnly && (AcceptWarnings > 0);
                return;
            }

            Console.WriteLine(eventArgs.Exception.Message);
            Console.WriteLine(eventArgs.Exception.StackTrace);
            Console.WriteLine();
            eventArgs.CanContinue = false;
        }

        /// <summary>
        /// Event handler raised before the delivery engine try to get the data source.
        /// </summary>
        /// <param name="sender">Object which raise the event.</param>
        /// <param name="eventArgs">Arguments to the event.</param>
        private static void BeforeGetDataSourceEventHandler(object sender, IGetDataSourceEventArgs eventArgs)
        {
            if (eventArgs == null)
            {
                throw new ArgumentNullException(nameof(eventArgs));
            }

            Console.WriteLine(Resource.GetText(Text.BeforeGetDataSourceInformation));
        }

        /// <summary>
        /// Event handler raised before the delivery engine try to get archive the metadata.
        /// </summary>
        /// <param name="sender">Object which raise the event.</param>
        /// <param name="eventArgs">Arguments to the event.</param>
        private static void BeforeArchiveMetadataEventHandler(object sender, IArchiveMetadataEventArgs eventArgs)
        {
            if (eventArgs == null)
            {
                throw new ArgumentNullException(nameof(eventArgs));
            }

            Console.WriteLine(Resource.GetText(Text.BeforeArchiveMetadataInformation, eventArgs.DataSource.ArchiveInformationPackageId));
        }

        /// <summary>
        /// Event handler raised before getting data for a target table.
        /// </summary>
        /// <param name="sender">Object which raise the event.</param>
        /// <param name="eventArgs">Arguments to the event.</param>
        private static void BeforeGetDataForTargetTableEventHandler(object sender, IGetDataForTargetTableEventArgs eventArgs)
        {
            if (eventArgs == null)
            {
                throw new ArgumentNullException(nameof(eventArgs));
            }

            Console.WriteLine(Resource.GetText(Text.BeforeGetDataForTargetTableInformation, eventArgs.DataBlock, eventArgs.TargetTable.NameTarget));
        }

        /// <summary>
        /// Event handler raised before validating data in a target table.
        /// </summary>
        /// <param name="sender">Object which raise the event.</param>
        /// <param name="eventArgs">Arguments to the event.</param>
        private static void BeforeValidatingDataInTargetTableEventHandler(object sender, IValidateDataInTargetTableEventArgs eventArgs)
        {
            if (eventArgs == null)
            {
                throw new ArgumentNullException(nameof(eventArgs));
            }

            Console.WriteLine(Resource.GetText(Text.BeforeValidateDataInTargetTableInformation, eventArgs.DataBlock, eventArgs.RowsInDataBlock, eventArgs.TargetTable.NameTarget));
        }

        /// <summary>
        /// Event handler raised before archiving data for a target table.
        /// </summary>
        /// <param name="sender">Object which raise the event.</param>
        /// <param name="eventArgs">Arguments to the event.</param>
        private static void BeforeArchiveDataForTargetTableEventHandler(object sender, IArchiveDataForTargetTableEventArgs eventArgs)
        {
            if (eventArgs == null)
            {
                throw new ArgumentNullException(nameof(eventArgs));
            }

            Console.WriteLine(Resource.GetText(Text.BeforeArchiveDataForTargetTableInformation, eventArgs.DataBlock, eventArgs.RowsInDataBlock, eventArgs.TargetTable.NameTarget));
        }

        #endregion
    }
}
