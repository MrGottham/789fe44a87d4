using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Events;

namespace DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Events
{
    /// <summary>
    /// Interface for arguments to the event raised before validating data in a target table.
    /// </summary>
    public interface IValidateDataInTargetTableEventArgs : IDeliveryEngineEventArgs
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

        /// <summary>
        /// Rows in the data block.
        /// </summary>
        int RowsInDataBlock
        {
            get;
        }
    }
}
