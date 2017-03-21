using System.Collections.Generic;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators
{
    /// <summary>
    /// Interface for a data manipulator.
    /// </summary>
    public interface IDataManipulator
    {
        /// <summary>
        /// Table name for the table on which the data should be manipulated.
        /// </summary>
        string TableName
        {
            get;
        }

        /// <summary>
        /// Manipulates data for a given table.
        /// </summary>
        /// <param name="table">Table for which data should be manipulated.</param>
        /// <param name="data">Data which should be manipulated.</param>
        /// <returns>Manipulated data for the table.</returns>
        IEnumerable<IEnumerable<IDataObjectBase>> ManipulateData(ITable table, IEnumerable<IEnumerable<IDataObjectBase>> data);

        /// <summary>
        /// Finalize the data manipulation for a given table when the last data has been received.
        /// </summary>
        /// <param name="table">Table for which to finalize the data manipulation.</param>
        /// <param name="data">The last manipulated data which has been received.</param>
        /// <returns>Finalized and manipulated data for the table.</returns>
        IEnumerable<IEnumerable<IDataObjectBase>> FinalizeDataManipulation(ITable table, IEnumerable<IEnumerable<IDataObjectBase>>data);

        /// <summary>
        /// Indicates whether the data manipulator is manipulating a given table.
        /// </summary>
        /// <param name="tableName">Name of the table on which to exam for use in the data manipulator.</param>
        /// <returns>True if the data manipulator use the table otherwise false.</returns>
        bool IsManipulatingTable(string tableName);

        /// <summary>
        /// Indicates whether the data manipulator is manipulating a given field.
        /// </summary>
        /// <param name="fieldName">Name of the field on which to exam for use in the data manipulator.</param>
        /// <returns>True if the data manipulator use the field otherwise false.</returns>
        bool IsManipulatingField(string fieldName);
    }
}
