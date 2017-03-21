using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Log;

namespace DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators
{
    /// <summary>
    /// Interface for a worker to delete rows with missing foreign keys.
    /// </summary>
    public interface IForeignKeyDeleter : IMissingForeignKeyWorker
    {
        /// <summary>
        /// Information logger.
        /// </summary>
        IInformationLogger InformationLogger
        {
            get;
        }
    }
}
