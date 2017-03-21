using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Events;

namespace DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Events
{
    /// <summary>
    /// Interface for arguments to events raised by data validators.
    /// </summary>
    public interface IDataValidatorEventArgs : IDeliveryEngineEventArgs
    {
        /// <summary>
        /// Data to the data validator event.
        /// </summary>
        object Data
        {
            get;
        }
    }
}
