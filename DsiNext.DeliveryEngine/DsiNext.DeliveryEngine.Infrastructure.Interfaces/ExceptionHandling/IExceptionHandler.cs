using System;

namespace DsiNext.DeliveryEngine.Infrastructure.Interfaces.ExceptionHandling
{
    /// <summary>
    /// Delegate for an eventhandler which handles exceptions.
    /// </summary>
    /// <param name="sender">Object which raise the event.</param>
    /// <param name="eventArgs">Arguments to eventhandler.</param>
    public delegate void HandleExceptionEventHandler(object sender, IHandleExceptionEventArgs eventArgs);

    /// <summary>
    /// Interface for an exception handler used by the delivery engine.
    /// </summary>
    public interface IExceptionHandler
    {
        /// <summary>
        /// Event which is raised when an exception occurs.
        /// </summary>
        event HandleExceptionEventHandler OnException;

        /// <summary>
        /// Handle an exception for the delivery engine.
        /// </summary>
        /// <param name="exception">Exception to handle.</param>
        void HandleException(Exception exception);

        /// <summary>
        /// Handle an exception for the delivery engine.
        /// </summary>
        /// <param name="exception">Exception to handle.</param>
        /// <param name="canContinue">Indicates whether the delivery engine may continue the process.</param>
        void HandleException(Exception exception, out bool canContinue);
    }
}
