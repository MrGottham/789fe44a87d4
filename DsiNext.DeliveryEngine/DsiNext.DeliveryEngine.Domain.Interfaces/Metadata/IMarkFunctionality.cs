namespace DsiNext.DeliveryEngine.Domain.Interfaces.Metadata
{
    /// <summary>
    /// Interface for a marker functionality to a metadata object.
    /// </summary>
    public interface IMarkFunctionality : IFunctionality
    {
        /// <summary>
        /// Marker value for the functionality.
        /// </summary>
        new string Functionality
        {
            get;
            set;
        }
    }
}
