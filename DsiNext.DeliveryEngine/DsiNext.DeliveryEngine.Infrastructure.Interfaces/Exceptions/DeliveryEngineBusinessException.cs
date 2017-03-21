using System;
using System.Runtime.Serialization;

namespace DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions
{
    /// <summary>
    /// Business exception for the delivery engine.
    /// </summary>
    [Serializable]
    public class DeliveryEngineBusinessException : DeliveryEngineExceptionBase
    {
        #region Constructors

        /// <summary>
        ///  Creates a business exception for the delivery engine.
        /// </summary>
        /// <param name="message">Message.</param>
        public DeliveryEngineBusinessException(string message) : base(message)
        {
        }

        /// <summary>
        /// Creates a business exception for the delivery engine.
        /// </summary>
        /// <param name="message">Mesasge.</param>
        /// <param name="innerException">Inner exception.</param>
        public DeliveryEngineBusinessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates a business exception for the delivery engine.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Streaming context.</param>
        protected DeliveryEngineBusinessException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
