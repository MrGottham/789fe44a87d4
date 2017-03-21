using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Domstolene.JFS.CommonLibrary.IoC.Interfaces;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators;

namespace DsiNext.DeliveryEngine.Repositories.DataManipulators
{
    /// <summary>
    /// Collection of data manipulators.
    /// </summary>
    public class DataManipulators : Collection<IDataManipulator>, IDataManipulators
    {
        #region Constructor

        /// <summary>
        /// Creates a collection of data manipulators.
        /// </summary>
        /// <param name="container">Container for Inversion of Control.</param>
        public DataManipulators(IContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            foreach (var dataManipulator in container.ResolveAll<IDataManipulator>())
            {
                Add(dataManipulator);
            }
        }

        #endregion

        #region IDataManipulators Members

        /// <summary>
        /// Manipulates data for a given table.
        /// </summary>
        /// <param name="table">Table for which data should be manipulated.</param>
        /// <param name="data">Data which should be manipulated.</param>
        /// <param name="endOfData">Indicates whether this is the last data for the table.</param>
        /// <returns>Manipulated data for the table.</returns>
        public virtual IEnumerable<IEnumerable<IDataObjectBase>> ManipulateData(ITable table, IEnumerable<IEnumerable<IDataObjectBase>> data, bool endOfData)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var manipulatedData = this.Aggregate(data, (current, dataManipulator) => dataManipulator.ManipulateData(table, current));
            if (endOfData == false)
            {
                return manipulatedData;
            }
            return this.Aggregate(manipulatedData, (current, dataManipulator) => dataManipulator.FinalizeDataManipulation(table, current));
        }

        #endregion
    }
}
