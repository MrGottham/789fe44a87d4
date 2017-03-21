namespace DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions
{
    /// <summary>
    /// Interface describing information for an delivery engine exception.
    /// </summary>
    public interface IDeliveryEngineExceptionInfo
    {
        /// <summary>
        /// Merged information about the object for the exception information.
        /// </summary>
        string ExceptionInfo
        {
            get;
        }
    }
}
