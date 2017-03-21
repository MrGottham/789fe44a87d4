using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Events;

namespace DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Events
{
    /// <summary>
    /// Interface for arguments to the event raised before archiving the metadata.
    /// </summary>
    public interface IArchiveMetadataEventArgs : IDeliveryEngineEventArgs
    {
        /// <summary>
        /// Data source for the metadata to archive.
        /// </summary>
        IDataSource DataSource
        {
            get;
        }
    }
}
