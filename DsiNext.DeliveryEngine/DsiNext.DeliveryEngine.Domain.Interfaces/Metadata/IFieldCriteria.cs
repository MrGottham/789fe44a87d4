namespace DsiNext.DeliveryEngine.Domain.Interfaces.Metadata
{
    /// <summary>
    /// Interface for a basic field criteria.
    /// </summary>
    public interface IFieldCriteria : ICriteria
    {
        /// <summary>
        /// Field for the criteria.
        /// </summary>
        IField Field
        {
            get;
        }
    }
}
