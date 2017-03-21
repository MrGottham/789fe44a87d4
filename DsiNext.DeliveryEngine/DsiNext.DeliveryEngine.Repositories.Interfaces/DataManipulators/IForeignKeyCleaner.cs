using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Log;

namespace DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators
{
    /// <summary>
    /// Interface for a worker to clean missing foreign key valus.
    /// </summary>
    public interface IForeignKeyCleaner : IMissingForeignKeyWorker
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
