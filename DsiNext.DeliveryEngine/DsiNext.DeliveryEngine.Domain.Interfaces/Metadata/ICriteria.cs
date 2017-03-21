namespace DsiNext.DeliveryEngine.Domain.Interfaces.Metadata
{
    /// <summary>
    /// Interface for a basic criteria.
    /// </summary>
    public interface ICriteria : IMetadataObjectBase
    {
        /// <summary>
        /// String representation of the criteria.
        /// </summary>
        /// <returns>String representation of the criteria.</returns>
        string AsString();

        /// <summary>
        /// SQL representation of the criteria.
        /// </summary>
        /// <returns>SQL representation of the criteria.</returns>
        string AsSql();

        /// <summary>
        /// Indicates whether a value does not meet the criteria.
        /// </summary>
        /// <param name="value">Value to test against the criteria.</param>
        /// <returns>Indication of where the value does not meet the criteria.</returns>
        bool Exclude(object value);
    }
}
