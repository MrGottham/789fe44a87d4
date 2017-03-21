using System;
using System.Runtime.Serialization;

namespace DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions
{
    /// <summary>
    /// Exception that has been handled by the exception handler in the delivery engine.
    /// </summary>
    [Serializable]
    public class DeliveryEngineAlreadyHandledException : DeliveryEngineExceptionBase
    {
        #region Constructors

        /// <summary>
        /// Creates a exception that has been handled by the exception handler in the delivery engine.
        /// </summary>
        /// <param name="message">Message.</param>
        public DeliveryEngineAlreadyHandledException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a exception that has been handled by the exception handler in the delivery engine.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public DeliveryEngineAlreadyHandledException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates a exception that has been handled by the exception handler in the delivery engine.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Streaming context.</param>
        protected DeliveryEngineAlreadyHandledException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
