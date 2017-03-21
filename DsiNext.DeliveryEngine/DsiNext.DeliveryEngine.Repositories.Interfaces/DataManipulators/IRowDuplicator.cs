using System;
using System.Collections.Generic;

namespace DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators
{
    /// <summary>
    /// Interface for a row duplicator.
    /// </summary>
    public interface IRowDuplicator : IDataManipulator, ICriteriaConfigurations
    {
        /// <summary>
        /// >Field names and the new value to set on the duplicated rows.
        /// </summary>
        IEnumerable<Tuple<string, object>> FieldUpdates
        {
            get;
        }
    }
}
