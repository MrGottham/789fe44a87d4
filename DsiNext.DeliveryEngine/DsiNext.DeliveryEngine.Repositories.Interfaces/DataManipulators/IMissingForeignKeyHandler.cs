namespace DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators
{
    /// <summary>
    /// Interface for a data manipulator to handle missing foreign keys.
    /// </summary>
    public interface IMissingForeignKeyHandler : IDataManipulator
    {
        /// <summary>
        /// Repository for getting data.
        /// </summary>
        IDataRepository DataRepository
        {
            get;
        }

        /// <summary>
        /// Repository for getting metadata.
        /// </summary>
        IMetadataRepository MetadataRepository
        {
            get;
        }

        /// <summary>
        /// Worker which manipulates data for missing foreign key values.
        /// </summary>
        IMissingForeignKeyWorker Worker
        {
            get;
        }
    }
}
