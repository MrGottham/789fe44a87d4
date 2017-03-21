namespace DsiNext.DeliveryEngine.Domain.Interfaces.Metadata
{
    /// <summary>
    /// Interface for a functionality to a metadata object.
    /// </summary>
    public interface IFunctionality : IMetadataObjectBase
    {
        /// <summary>
        /// Name of functionality.
        /// </summary>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Value for functionality.
        /// </summary>
        object Functionality
        {
            get;
            set;
        }

        /// <summary>
        /// The XML metadata value for the funtionality.
        /// </summary>
        string XmlValue
        {
            get;
        }
    }
}
