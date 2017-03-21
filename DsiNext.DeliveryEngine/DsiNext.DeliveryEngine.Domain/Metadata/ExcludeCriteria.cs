using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Domain.Metadata
{
    /// <summary>
    /// Exclusion criteria.
    /// </summary>
    public class ExcludeCriteria : CriteriaBase, IExcludeCriteria
    {
        /// <summary>
        /// String representation of the criteria.
        /// </summary>
        /// <returns>String representation of the criteria.</returns>
        public override string AsString()
        {
            return string.Empty;
        }

        /// <summary>
        /// SQL representation of the criteria.
        /// </summary>
        /// <returns>SQL representation of the criteria.</returns>
        public override string AsSql()
        {
            return string.Empty;
        }

        /// <summary>
        /// Indicates whether a value does not meet the criteria.
        /// </summary>
        /// <param name="value">Value to test against the criteria.</param>
        /// <returns>Indication of where the value does not meet the criteria.</returns>
        public override bool Exclude(object value)
        {
            return true;
        }
    }
}
