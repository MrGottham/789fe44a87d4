using System;
using System.Runtime.Serialization;

namespace DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions
{
    /// <summary>
    /// Validate exception for the delivery engine.
    /// </summary>
    [Serializable]
    public class DeliveryEngineValidateException : DeliveryEngineBusinessException
    {
        #region Private variables

        private readonly IDeliveryEngineValidateExceptionInfo _info;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a validate exception for the delivery engine.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="info">Validate exception information.</param>
        public DeliveryEngineValidateException(string message, IDeliveryEngineValidateExceptionInfo info)
            : base(message)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            _info = info;
        }

        /// <summary>
        /// Creates a validate exception for the delivery engine.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="info">Validate exception information.</param>
        /// <param name="innerException">Inner exception.</param>
        public DeliveryEngineValidateException(string message, IDeliveryEngineValidateExceptionInfo info, Exception innerException)
            : base(message, innerException)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            _info = info;
        }

        /// <summary>
        /// Creates a validate exception for the delivery engine.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Streaming context.</param>
        protected DeliveryEngineValidateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Information about the validate exception.
        /// </summary>
        public virtual IDeliveryEngineValidateExceptionInfo Information
        {
            get
            {
                return _info;
            }
        }

        #endregion
    }
}
