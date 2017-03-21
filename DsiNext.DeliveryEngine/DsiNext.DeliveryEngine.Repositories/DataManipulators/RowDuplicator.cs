using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Helpers;

namespace DsiNext.DeliveryEngine.Repositories.DataManipulators
{
    /// <summary>
    /// Row duplicator which duplicates rows and updates some values on the duplicated rows.
    /// </summary>
    public class RowDuplicator : DataManipulatorBase, IRowDuplicator
    {
        #region Private variables

        private readonly IEnumerable<Tuple<string, object>> _fieldUpdates;
        private readonly IEnumerable<Tuple<Type, string, object>> _criteriaConfiugrations;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a row duplicator which duplicates rows and updates some values on the duplicated rows.
        /// </summary>
        /// <param name="tableName">Table name for the table on which the data should be manipulated.</param>
        /// <param name="fieldUpdates">Field names and the new value to set on the duplicated rows.</param>
        public RowDuplicator(string tableName, IEnumerable<Tuple<string, object>> fieldUpdates)
            : this(tableName, fieldUpdates, new Collection<Tuple<Type, string, object>>())
        {
        }

        /// <summary>
        /// Creates a row duplicator which duplicates rows and updates some values on the duplicated rows.
        /// </summary>
        /// <param name="tableName">Table name for the table on which the data should be manipulated.</param>
        /// <param name="fieldUpdates">Field names and the new value to set on the duplicated rows.</param>
        /// <param name="criteriaConfigurations">Configuration for the criterias used to duplicate rows.</param>
        public RowDuplicator(string tableName, IEnumerable<Tuple<string, object>> fieldUpdates, IEnumerable<Tuple<Type, string, object>> criteriaConfigurations)
            : base(tableName)
        {
            if (fieldUpdates == null)
            {
                throw new ArgumentNullException("fieldUpdates");
            }
            if (criteriaConfigurations == null)
            {
                throw new ArgumentNullException("criteriaConfigurations");
            }
            _fieldUpdates = fieldUpdates;
            _criteriaConfiugrations = criteriaConfigurations;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Configuration for the criterias used to duplicate rows.
        /// </summary>
        public virtual IEnumerable<Tuple<Type, string, object>> CriteriaConfigurations
        {
            get
            {
                return _criteriaConfiugrations;
            }
        }

        /// <summary>
        /// >Field names and the new value to set on the duplicated rows.
        /// </summary>
        public virtual IEnumerable<Tuple<string, object>> FieldUpdates
        {
            get
            {
                return _fieldUpdates;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Manipulates data for the table used by the data manipulator.
        /// </summary>
        /// <param name="table">Table on which to manipulate data.</param>
        /// <param name="dataToManipulate">Data which sould be manipulated.</param>
        /// <returns>Manipulated data for the table.</returns>
        protected override IEnumerable<IEnumerable<IDataObjectBase>> Manipulate(ITable table, IList<IEnumerable<IDataObjectBase>> dataToManipulate)
        {
            var filter = GenerateFilter(table, CriteriaConfigurations);
            var duplicatedRows = new List<IEnumerable<IDataObjectBase>>(dataToManipulate.Count);
            try
            {
                for (var dataRowNo = 0; dataRowNo < dataToManipulate.Count; dataRowNo++)
                {
                    if (filter.Exclude(dataToManipulate.ElementAt(dataRowNo)))
                    {
                        continue;
                    }
                    var duplicatedRow = new List<IDataObjectBase>(dataToManipulate.ElementAt(dataRowNo).Count());
                    duplicatedRow.AddRange(new List<IDataObjectBase>(dataToManipulate.ElementAt(dataRowNo).Select(m => (IDataObjectBase) m.Clone())));
                    duplicatedRows.Add(duplicatedRow);
                }
                for (var duplicatedRowNo = 0; duplicatedRowNo < duplicatedRows.Count; duplicatedRowNo++)
                {
                    foreach (var fieldUpdate in FieldUpdates)
                    {
                        var dataObject = DataRepositoryHelper.GetDataObject(duplicatedRows.ElementAt(duplicatedRowNo).ToList(), fieldUpdate.Item1);
                        DataRepositoryHelper.UpdateSourceValue(dataObject, fieldUpdate.Item2);
                    }
                }
                duplicatedRows.ForEach(dataToManipulate.Add);
                return dataToManipulate;
            }
            finally
            {
                while (duplicatedRows.Count > 0)
                {
                    duplicatedRows.Clear();
                }
            }
        }

        /// <summary>
        /// Indicates whether the data manipulator is manipulating a given field.
        /// </summary>
        /// <param name="fieldName">Name of the field on which to exam for use in the data manipulator.</param>
        /// <returns>True if the data manipulator use the field otherwise false.</returns>
        protected override bool ManipulatingField(string fieldName)
        {
            return FieldUpdates.Any(fieldUpdate => string.Compare(fieldUpdate.Item1, fieldName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        #endregion
    }
}
