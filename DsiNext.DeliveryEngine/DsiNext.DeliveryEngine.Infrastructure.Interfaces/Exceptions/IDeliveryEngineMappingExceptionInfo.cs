namespace DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions
{
    /// <summary>
    /// Interface describing information about the mapping exception.
    /// </summary>
    public interface IDeliveryEngineMappingExceptionInfo : IDeliveryEngineExceptionInfo
    {
        /// <summary>
        /// The mapping object.
        /// </summary>
        object MappingObject
        {
            get;
        }

        /// <summary>
        /// Data for the mapping object.
        /// </summary>
        object MappingObjectData
        {
            get;
            set;
        }
    }
}
