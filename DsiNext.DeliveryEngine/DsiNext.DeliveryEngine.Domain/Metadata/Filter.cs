using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Domain.Metadata
{
    /// <summary>
    /// A filter containing criterias.
    /// </summary>
    public class Filter : MetadataObjectBase, IFilter
    {
        #region Private variables

        private readonly ObservableCollection<ICriteria> _criterias = new ObservableCollection<ICriteria>();

        #endregion

        #region Properties

        /// <summary>
        /// Criteria for the filter.
        /// </summary>
        public virtual ReadOnlyObservableCollection<ICriteria> Criterias
        {
            get
            {
                return new ReadOnlyObservableCollection<ICriteria>(_criterias);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// String representation of the criterias in the filter.
        /// </summary>
        /// <returns>String representation of the criterias in the filter.</returns>
        public virtual string AsString()
        {
            var stringBuilder = new StringBuilder();
            foreach (var criteria in Criterias)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.AppendLine();
                }
                stringBuilder.Append(criteria.AsString());
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// SQL representation of the criterias in the filter.
        /// </summary>
        /// <returns>SQL representation of the criterias in the filter.</returns>
        public virtual string AsSql()
        {
            var stringBuilder = new StringBuilder();
            foreach (var criteria in Criterias)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(" AND ");
                }
                stringBuilder.AppendFormat("({0})", criteria.AsSql());
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Indicates whether a record does not meet the criteria.
        /// </summary>
        /// <param name="recordDataObjects">Data objects for the record.</param>
        /// <returns>Indication of whether the record does not meet the criteria.</returns>
        public virtual bool Exclude(IEnumerable<IDataObjectBase> recordDataObjects)
        {
            if (recordDataObjects == null)
            {
                throw new ArgumentNullException("recordDataObjects");
            }
            
            var dataObjects = recordDataObjects.ToList();
            // ReSharper disable LoopCanBeConvertedToQuery
            foreach (var fieldCriteria in Criterias.OfType<IFieldCriteria>())
            {
                var fieldName = fieldCriteria.Field.NameSource;
                var dataObject = dataObjects.SingleOrDefault(m => m.Field != null && String.IsNullOrEmpty(m.Field.NameSource) == false && String.Compare(m.Field.NameSource, fieldName, StringComparison.InvariantCulture) == 0);
                if (dataObject == null)
                {
                    continue;
                }
                var dataObjectType = dataObject.Field.DatatypeOfSource;
                var genericMethodForValue = dataObject.GetType()
                    .GetMethod("GetSourceValue")
                    .MakeGenericMethod(new[] {dataObjectType});
                if (fieldCriteria.Exclude(genericMethodForValue.Invoke(dataObject, null)))
                {
                    return true;
                }
            }
            // ReSharper restore LoopCanBeConvertedToQuery
            return false;
        }

        /// <summary>
        /// Indication whether a field does not meet the criteria.
        /// </summary>
        /// <param name="field">Field.</param>
        /// <returns>Indication of whether the field does not meet the criteria.</returns>
        public virtual bool Exclude(IField field)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }

            return Criterias.OfType<IFieldCriteria>()
                .Where(fieldCriteria => String.Compare(field.NameSource, fieldCriteria.Field.NameSource, StringComparison.InvariantCulture) == 0)
                .Any(fieldCriteria => fieldCriteria.Exclude(field));
        }

        /// <summary>
        /// Adds a criteria to the filter.
        /// </summary>
        /// <param name="criteria">Criteria.</param>
        public virtual void AddCriteria(ICriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException("criteria");
            }
            _criterias.Add(criteria);
        }

        #endregion
    }
}
