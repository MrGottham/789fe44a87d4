using System;
using System.Runtime.Serialization;

namespace DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions
{
    /// <summary>
    /// Metadata exception for the delivery engine.
    /// </summary>
    [Serializable]
    public class DeliveryEngineMetadataException : DeliveryEngineBusinessException
    {
        #region Private variables

        private readonly IDeliveryEngineMetadataExceptionInfo _info;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a metadata exception for the delivery engine.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="info">Metadata exception information.</param>
        public DeliveryEngineMetadataException(string message, IDeliveryEngineMetadataExceptionInfo info)
            : base(message)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            _info = info;
        }

        /// <summary>
        /// Creates a metadata exception for the delivery engine.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="info">Metadata exception information.</param>
        /// <param name="innerException">Inner exception.</param>
        public DeliveryEngineMetadataException(string message, IDeliveryEngineMetadataExceptionInfo info, Exception innerException)
            : base(message, innerException)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            _info = info;
        }

        /// <summary>
        /// Creates a metadata exception for the delivery engine.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Streaming context.</param>
        protected DeliveryEngineMetadataException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Information about the metadata exception.
        /// </summary>
        public virtual IDeliveryEngineMetadataExceptionInfo Information
        {
            get
            {
                return _info;
            }
        }

        #endregion
    }
}
