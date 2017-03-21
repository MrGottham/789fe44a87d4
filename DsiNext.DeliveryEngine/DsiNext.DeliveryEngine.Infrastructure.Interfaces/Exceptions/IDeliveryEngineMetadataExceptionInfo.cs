namespace DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions
{
    /// <summary>
    /// Interface describing information about the metadata exception.
    /// </summary>
    public interface IDeliveryEngineMetadataExceptionInfo : IDeliveryEngineExceptionInfo
    {
        /// <summary>
        /// The metadata object.
        /// </summary>
        object MetadataObject
        {
            get;
        }
    }
}
