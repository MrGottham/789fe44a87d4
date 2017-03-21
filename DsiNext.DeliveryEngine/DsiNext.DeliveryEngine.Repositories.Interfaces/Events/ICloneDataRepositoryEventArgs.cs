using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Events;

namespace DsiNext.DeliveryEngine.Repositories.Interfaces.Events
{
    /// <summary>
    /// Interface for arguments to the event raised when a data repository is cloned.
    /// </summary>
    public interface ICloneDataRepositoryEventArgs : IDeliveryEngineEventArgs
    {
        /// <summary>
        /// Cloned data repository.
        /// </summary>
        IDataRepository ClonedDataRepository
        {
            get;
        }
    }
}
