using System.Collections.Generic;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Log;

namespace DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators
{
    /// <summary>
    /// Interface for a worker to add missing primary keys.
    /// </summary>
    public interface IPrimaryKeyAdder : IMissingForeignKeyWorker
    {
        /// <summary>
        /// Field values to be set on missing primary keys.
        /// </summary>
        IDictionary<string, object> SetFieldValues
        {
            get;
        }

        /// <summary>
        /// Information logger.
        /// </summary>
        IInformationLogger InformationLogger
        {
            get;
        }
    }
}
