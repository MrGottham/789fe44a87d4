using System.Collections.Generic;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Events;

namespace DsiNext.DeliveryEngine.Repositories.Interfaces.Events
{
    /// <summary>
    /// Interface for arguments to the event raised when to handle data from a data repository.
    /// </summary>
    public interface IHandleDataEventArgs : IDeliveryEngineEventArgs
    {
        /// <summary>
        /// Table for which to handle the data.
        /// </summary>
        ITable Table
        {
            get;
        }

        /// <summary>
        /// Data to be handled.
        /// </summary>
        IEnumerable<IEnumerable<IDataObjectBase>> Data
        {
            get;
        }

        /// <summary>
        /// Indicates whether all data has been readed.
        /// </summary>
        bool EndOfData
        {
            get;
        }
    }
}
