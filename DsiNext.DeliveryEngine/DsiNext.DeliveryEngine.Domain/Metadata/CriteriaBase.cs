using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Domain.Metadata
{
    /// <summary>
    /// Base class for criterias.
    /// </summary>
    public abstract class CriteriaBase : MetadataObjectBase, ICriteria
    {
        #region Methods

        /// <summary>
        /// String representation of the criteria.
        /// </summary>
        /// <returns>String representation of the criteria.</returns>
        public abstract string AsString();

        /// <summary>
        /// SQL representation of the criteria.
        /// </summary>
        /// <returns>SQL representation of the criteria.</returns>
        public abstract string AsSql();

        /// <summary>
        /// Indicates whether a value does not meet the criteria.
        /// </summary>
        /// <param name="value">Value to test against the criteria.</param>
        /// <returns>Indication of where the value does not meet the criteria.</returns>
        public abstract bool Exclude(object value);

        #endregion
    }
}
