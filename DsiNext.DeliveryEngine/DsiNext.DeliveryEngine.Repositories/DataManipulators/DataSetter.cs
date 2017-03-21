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
    /// Data setter which can set or change a field value.
    /// </summary>
    public class DataSetter : DataManipulatorBase, IDataSetter
    {
        #region Private variables

        private readonly string _fieldName;
        private readonly object _fieldValue;
        private readonly IEnumerable<Tuple<Type, string, object>> _criteriaConfigurations;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a data setter which can set or change a field value.
        /// </summary>
        /// <param name="tableName">Table name for the table on which the data should be manipulated.</param>
        /// <param name="fieldName">Name of the field on which to set or change data.</param>
        /// <param name="value">The new field value.</param>
        public DataSetter(string tableName, string fieldName, object value)
            : this(tableName, fieldName, value, new Collection<Tuple<Type, string, object>>())
        {
        }

        /// <summary>
        /// Creates da ata setter which can set or change a field value.
        /// </summary>
        /// <param name="tableName">Table name for the table on which the data should be manipulated.</param>
        /// <param name="fieldName">Name of the field on which to set or change data.</param>
        /// <param name="value">The new field value.</param>
        /// <param name="criteriaConfigurations">Configuration for criterias which sort out the data to be set or changed.</param>
        public DataSetter(string tableName, string fieldName, object value, IEnumerable<Tuple<Type, string, object>> criteriaConfigurations)
            : base(tableName)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException("fieldName");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (criteriaConfigurations == null)
            {
                throw new ArgumentNullException("criteriaConfigurations");
            }
            _fieldName = fieldName;
            _fieldValue = value;
            _criteriaConfigurations = criteriaConfigurations;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Configuration for the criterias which sort out the data to be set or changed.
        /// </summary>
        public virtual IEnumerable<Tuple<Type, string, object>> CriteriaConfigurations
        {
            get
            {
                return _criteriaConfigurations;
            }
        }

        /// <summary>
        /// Name of the field on which to set or change data.
        /// </summary>
        public virtual string FieldName
        {
            get
            {
                return _fieldName;
            }
        }

        /// <summary>
        /// The new field value.
        /// </summary>
        public virtual object FieldValue
        {
            get
            {
                return _fieldValue;
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
            for (var i = 0; i < dataToManipulate.Count; i++)
            {
                if (filter.Exclude(dataToManipulate.ElementAt(i)))
                {
                    continue;
                }
                var dataObject = DataRepositoryHelper.GetDataObject(dataToManipulate.ElementAt(i).ToList(), FieldName);
                DataRepositoryHelper.UpdateSourceValue(dataObject, FieldValue);
            }
            return dataToManipulate;
        }

        /// <summary>
        /// Indicates whether the data manipulator is manipulating a given field.
        /// </summary>
        /// <param name="fieldName">Name of the field on which to exam for use in the data manipulator.</param>
        /// <returns>True if the data manipulator use the field otherwise false.</returns>
        protected override bool ManipulatingField(string fieldName)
        {
            return string.Compare(FieldName, fieldName, StringComparison.OrdinalIgnoreCase) == 0;
        }

        #endregion
    }
}
