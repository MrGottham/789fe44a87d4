using System.Collections.Generic;
using System.Collections.ObjectModel;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;

namespace DsiNext.DeliveryEngine.Domain.Interfaces.Metadata
{
    /// <summary>
    /// Interface for a filter containing criterias.
    /// </summary>
    public interface IFilter : IMetadataObjectBase
    {
        /// <summary>
        /// Criteria for the filter.
        /// </summary>
        ReadOnlyObservableCollection<ICriteria> Criterias
        {
            get;
        }

        /// <summary>
        /// String representation of the criterias in the filter.
        /// </summary>
        /// <returns>String representation of the criterias in the filter.</returns>
        string AsString();

        /// <summary>
        /// SQL representation of the criterias in the filter.
        /// </summary>
        /// <returns>SQL representation of the criterias in the filter.</returns>
        string AsSql();

        /// <summary>
        /// Indicates whether a record does not meet the criteria.
        /// </summary>
        /// <param name="recordDataObjects">Data objects for the record.</param>
        /// <returns>Indication of whether the record does not meet the criteria.</returns>
        bool Exclude(IEnumerable<IDataObjectBase> recordDataObjects);

        /// <summary>
        /// Indication whether a field does not meet the criteria.
        /// </summary>
        /// <param name="field">Field.</param>
        /// <returns>Indication of whether the field does not meet the criteria.</returns>
        bool Exclude(IField field);

        /// <summary>
        /// Adds a criteria to the filter.
        /// </summary>
        /// <param name="criteria">Criteria.</param>
        void AddCriteria(ICriteria criteria);
    }
}
