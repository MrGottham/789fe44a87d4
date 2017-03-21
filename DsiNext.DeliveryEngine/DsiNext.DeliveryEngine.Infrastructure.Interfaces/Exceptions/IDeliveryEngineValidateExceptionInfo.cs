namespace DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions
{
    /// <summary>
    /// Interface describing information about the validate exception.
    /// </summary>
    public interface IDeliveryEngineValidateExceptionInfo : IDeliveryEngineExceptionInfo
    {
        /// <summary>
        /// The validate object.
        /// </summary>
        object ValidateObject
        {
            get;
        }

        /// <summary>
        /// Data for the validating object.
        /// </summary>
        object ValidateObjectData
        {
            get;
            set;
        }
    }
}
