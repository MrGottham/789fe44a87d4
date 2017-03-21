using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Commands;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.DataValidators;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Events;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Events;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.ExceptionHandling;
using DsiNext.DeliveryEngine.Repositories.Interfaces;

namespace DsiNext.DeliveryEngine.BusinessLogic.Interfaces
{
    /// <summary>
    /// Interface describing the delivery engine.
    /// </summary>
    public interface IDeliveryEngine : IBusinessLogic
    {
        /// <summary>
        /// Repository containing configuration.
        /// </summary>
        IConfigurationRepository ConfigurationRepository
        {
            get;
        }

        /// <summary>
        /// Repository containing metadata to get data from the source and creating the delivery format.
        /// </summary>
        IMetadataRepository MetadataRepository
        {
            get;
        }

        /// <summary>
        /// Repository containing functionality to get data from the source.
        /// </summary>
        IDataRepository DataRepository
        {
            get;
        }

        /// <summary>
        /// Repository containing functionality to get documents for the delivery format.
        /// </summary>
        IDocumentRepository DocumentRepository
        {
            get;
        }

        /// <summary>
        /// Data validators for the delivery engine.
        /// </summary>
        IDataValidators DataValidators
        {
            get;
        }

        /// <summary>
        /// Repository containing functionality to write the delivery format.
        /// </summary>
        IArchiveVersionRepository ArchiveVersionRepository
        {
            get;
        }

        /// <summary>
        /// Exception handler for the delivery engine.
        /// </summary>
        IExceptionHandler ExceptionHandler
        {
            get;
        }

        /// <summary>
        /// Event raised before getting the data source.
        /// </summary>
        event DeliveryEngineEventHandler<IGetDataSourceEventArgs> BeforeGetDataSource;

        /// <summary>
        /// Event raised before archiving the metadata in the data source.
        /// </summary>
        event DeliveryEngineEventHandler<IArchiveMetadataEventArgs> BeforeArchiveMetadata;

        /// <summary>
        /// Event raised before getting data for a target table.
        /// </summary>
        event DeliveryEngineEventHandler<IGetDataForTargetTableEventArgs> BeforeGetDataForTargetTable;

        /// <summary>
        /// Event raised before validating data in a target table.
        /// </summary>
        event DeliveryEngineEventHandler<IValidateDataInTargetTableEventArgs> BeforeValidateDataInTargetTable;

        /// <summary>
        /// Event raised before archiving data for a target table.
        /// </summary>
        event DeliveryEngineEventHandler<IArchiveDataForTargetTableEventArgs> BeforeArchiveDataForTargetTable;

        /// <summary>
        /// Execute the delivery engine to create and write the delivery format.
        /// </summary>
        /// <param name="command">Command for executing the delivery engine.</param>
        void Execute(IDeliveryEngineExecuteCommand command);
    }
}
