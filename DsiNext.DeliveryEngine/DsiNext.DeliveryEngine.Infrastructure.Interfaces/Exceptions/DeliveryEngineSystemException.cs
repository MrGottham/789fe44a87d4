using System;
using System.Runtime.Serialization;

namespace DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions
{
    /// <summary>
    /// System exception for the delivery engine.
    /// </summary>
    [Serializable]
    public class DeliveryEngineSystemException : DeliveryEngineExceptionBase
    {
        #region Constructors

        /// <summary>
        /// Creates a system exception for the delivery engine.
        /// </summary>
        /// <param name="message">>Message.</param>
        public DeliveryEngineSystemException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a system exception for the delivery engine.
        /// </summary>
        /// <param name="message">>Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public DeliveryEngineSystemException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates a system exception for the delivery engine.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Streaming context.</param>
        protected DeliveryEngineSystemException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
