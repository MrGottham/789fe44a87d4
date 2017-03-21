using System.Collections.Generic;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators
{
    /// <summary>
    /// Interface for a collection of data manipulators.
    /// </summary>
    public interface IDataManipulators : IEnumerable<IDataManipulator>
    {
        /// <summary>
        /// Manipulates data for a given table.
        /// </summary>
        /// <param name="table">Table for which data should be manipulated.</param>
        /// <param name="data">Data which should be manipulated.</param>
        /// <param name="endOfData">Indicates whether this is the last data for the table.</param>
        /// <returns>Manipulated data for the table.</returns>
        IEnumerable<IEnumerable<IDataObjectBase>> ManipulateData(ITable table, IEnumerable<IEnumerable<IDataObjectBase>> data, bool endOfData);
    }
}
