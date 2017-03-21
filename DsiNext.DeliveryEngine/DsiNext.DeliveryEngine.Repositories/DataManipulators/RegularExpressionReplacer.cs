using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Enums;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Helpers;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Repositories.DataManipulators
{
    /// <summary>
    /// Regular expression replacer which can set, change or clear a field value.
    /// </summary>
    public class RegularExpressionReplacer : DataManipulatorBase, IRegularExpressionReplacer
    {
        #region Private variables

        private readonly Regex _regularExpression;
        private readonly RegularExpressionApplyOn _applyOn;
        private readonly string _fieldName;
        private readonly object _fieldValue;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a regular expression replacer which can set or change a field value.
        /// </summary>
        /// <param name="regularExpression">Regular expression for the replacer.</param>
        /// <param name="tableName">Table name for the table on which the data should be manipulated.</param>
        /// <param name="applyOn">Descripes when the regular expression should set, change or clear the field value.</param>
        /// <param name="fieldName">Name of the field on which to set or change data.</param>
        /// <param name="fieldValue">The new field value.</param>
        public RegularExpressionReplacer(string tableName, Regex regularExpression, RegularExpressionApplyOn applyOn, string fieldName, object fieldValue)
            : base(tableName)
        {
            if (regularExpression == null)
            {
                throw new ArgumentNullException("regularExpression");
            }
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException("fieldName");
            }
            _regularExpression = regularExpression;
            _applyOn = applyOn;
            _fieldName = fieldName;
            _fieldValue = fieldValue;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Regular expression for the replacer.
        /// </summary>
        public virtual Regex RegularExpression
        {
            get
            {
                return _regularExpression;
            }
        }

        /// <summary>
        /// Descripes when the regular expression should set, change or clear the field value.
        /// </summary>
        public virtual RegularExpressionApplyOn ApplyOn
        {
            get
            {
                return _applyOn;
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

        #region Method

        /// <summary>
        /// Manipulates data for the table used by the data manipulator.
        /// </summary>
        /// <param name="table">Table on which to manipulate data.</param>
        /// <param name="dataToManipulate">Data which sould be manipulated.</param>
        /// <returns>Manipulated data for the table.</returns>
        protected override IEnumerable<IEnumerable<IDataObjectBase>> Manipulate(ITable table, IList<IEnumerable<IDataObjectBase>> dataToManipulate)
        {
            for (var dataRowNo = 0; dataRowNo < dataToManipulate.Count; dataRowNo++)
            {
                var dataObject = DataRepositoryHelper.GetDataObject(dataToManipulate.ElementAt(dataRowNo).ToList(), FieldName);
                var sourceValue = DataRepositoryHelper.GetSourceValue(dataObject);
                if (Equals(sourceValue, null))
                {
                    continue;
                }
                var sourceValueAsString = sourceValue.ToString();
                switch (ApplyOn)
                {
                    case RegularExpressionApplyOn.ApplyOnMatch:
                        if (RegularExpression.IsMatch(sourceValueAsString))
                        {
                            DataRepositoryHelper.UpdateSourceValue(dataObject, FieldValue);
                        }
                        continue;

                    case RegularExpressionApplyOn.ApplyOnUnmatch:
                        if (RegularExpression.IsMatch(sourceValueAsString) == false)
                        {
                            DataRepositoryHelper.UpdateSourceValue(dataObject, FieldValue);
                        }
                        continue;

                    default:
                        throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.IllegalValue, ApplyOn, "ApplyOn"));
                }
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
