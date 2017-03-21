using System.Collections.Generic;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators
{
    /// <summary>
    /// Interface for a worker to manipulate missing foreign key values.
    /// </summary>
    public interface IMissingForeignKeyWorker
    {
        /// <summary>
        /// Foreign table to validate against.
        /// </summary>
        ITable ForeignKeyTable
        {
            get;
        }

        /// <summary>
        /// Manipulates missing foreign key values for a given table.
        /// </summary>
        /// <param name="table">Table to manipulate data for missing foreign key values.</param>
        /// <param name="dataRepository">Data repository.</param>
        /// <param name="data">Data for the table where to manipulate data for missing foreign key values.</param>
        /// <returns>Manipulated data for the table.</returns>
        IEnumerable<IEnumerable<IDataObjectBase>> ManipulateData(ITable table, IDataRepository dataRepository, IEnumerable<IEnumerable<IDataObjectBase>> data);

        /// <summary>
        /// Finalize missing foreign key values for a given table.
        /// </summary>
        /// <param name="table">Table to finalize data manipulation for missing foreign key values.</param>
        /// <param name="dataRepository">Data repository.</param>
        /// <param name="data">The last manipulated data which has been received.</param>
        /// <returns>Finalized and manipulated data for the table.</returns>
        IEnumerable<IEnumerable<IDataObjectBase>> FinalizeDataManipulation(ITable table, IDataRepository dataRepository, IEnumerable<IEnumerable<IDataObjectBase>> data);

        /// <summary>
        /// Indicates whether the worker is manipulating a given field.
        /// </summary>
        /// <param name="fieldName">Name of the field on which to exam for use in the worker.</param>
        /// <param name="workOnTable">The table which the worker are allocated to.</param>
        /// <returns>True if the worker use the field otherwise false.</returns>
        bool IsManipulatingField(string fieldName, ITable workOnTable);
    }
}
