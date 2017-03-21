using System;
using System.Runtime.Serialization;

namespace DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions
{
    /// <summary>
    /// Repository exception for the delivery engine.
    /// </summary>
    [Serializable]
    public class DeliveryEngineRepositoryException : DeliveryEngineExceptionBase
    {
        #region Constructurs

        /// <summary>
        /// Creates a repository exception for the delivery engine.
        /// </summary>
        /// <param name="message">Message.</param>
        public DeliveryEngineRepositoryException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a repository exception for the delivery engine.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">Inner exception.</param>
        public DeliveryEngineRepositoryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates a repository exception for the delivery engine.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Streaming context.</param>
        protected DeliveryEngineRepositoryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
