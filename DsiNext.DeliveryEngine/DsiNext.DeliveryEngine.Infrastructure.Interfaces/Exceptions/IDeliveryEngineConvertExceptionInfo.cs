namespace DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions
{
    /// <summary>
    /// Interface describing information about the convert exception.
    /// </summary>
    public interface IDeliveryEngineConvertExceptionInfo : IDeliveryEngineExceptionInfo
    {
        /// <summary>
        /// The convert object.
        /// </summary>
        object ConvertObject
        {
            get;
        }

        /// <summary>
        /// Data for hte converting object.
        /// </summary>
        object ConvertObjectData
        {
            get;
            set;
        }
    }
}
