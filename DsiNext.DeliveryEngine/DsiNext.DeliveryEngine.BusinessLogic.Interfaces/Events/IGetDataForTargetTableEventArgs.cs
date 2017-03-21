using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Events;

namespace DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Events
{
    /// <summary>
    /// Interface for arguments to the event raised before getting data for a target table.
    /// </summary>
    public interface IGetDataForTargetTableEventArgs : IDeliveryEngineEventArgs
    {
        /// <summary>
        /// Data source.
        /// </summary>
        IDataSource DataSource
        {
            get;
        }

        /// <summary>
        /// Target table.
        /// </summary>
        ITable TargetTable
        {
            get;
        }

        /// <summary>
        /// Data block number.
        /// </summary>
        int DataBlock
        {
            get;
        }
    }
}
