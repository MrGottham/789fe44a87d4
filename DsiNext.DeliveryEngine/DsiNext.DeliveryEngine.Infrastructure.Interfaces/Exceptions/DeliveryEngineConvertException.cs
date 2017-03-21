using System;
using System.Runtime.Serialization;

namespace DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions
{
    /// <summary>
    /// Convert exception for the delivery engine.
    /// </summary>
    [Serializable]
    public class DeliveryEngineConvertException : DeliveryEngineBusinessException
    {
        #region Private variables

        private readonly IDeliveryEngineConvertExceptionInfo _info;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a convert exception for the delivery engine.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="info">Convert exception information.</param>
        public DeliveryEngineConvertException(string message, IDeliveryEngineConvertExceptionInfo info)
            : base(message)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            _info = info;
        }

        /// <summary>
        /// Creates a convert exception for the delivery engine.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="info">Convert exception information.</param>
        /// <param name="innerException">Inner exception.</param>
        public DeliveryEngineConvertException(string message, IDeliveryEngineConvertExceptionInfo info, Exception innerException)
            : base(message, innerException)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            _info = info;
        }

        /// <summary>
        /// Creates a convert exception for the delivery engine.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Streaming context.</param>
        protected DeliveryEngineConvertException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Information about the convert exception.
        /// </summary>
        public virtual IDeliveryEngineConvertExceptionInfo Information
        {
            get
            {
                return _info;
            }
        }

        #endregion
    }
}
