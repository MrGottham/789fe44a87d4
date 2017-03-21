using System;
using System.Runtime.Serialization;

namespace DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions
{
    /// <summary>
    /// Basic exception for the delivery engine.
    /// </summary>
    [Serializable]
    public abstract class DeliveryEngineExceptionBase : Exception
    {
        #region Constructors

        /// <summary>
        /// Creates a basic exception for the delivery engine.
        /// </summary>
        /// <param name="message">Message.</param>
        protected DeliveryEngineExceptionBase(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a basic exception for the delivery engine.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        protected DeliveryEngineExceptionBase(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates a basic exception for the delivery engine.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Streaming context.</param>
        protected DeliveryEngineExceptionBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
