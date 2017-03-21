using System;
using System.Runtime.Serialization;

namespace DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions
{
    /// <summary>
    /// Mapping exception for the delivery engine.
    /// </summary>
    [Serializable]
    public class DeliveryEngineMappingException : DeliveryEngineBusinessException
    {
        #region Private variables

        private readonly IDeliveryEngineMappingExceptionInfo _info;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a mapping exception for the delivery engine.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="info">Mapping exception information.</param>
        public DeliveryEngineMappingException(string message, IDeliveryEngineMappingExceptionInfo info)
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
        /// <param name="message">Mesage.</param>
        /// <param name="info">Mapping exception information.</param>
        /// <param name="innerException">Inner exception.</param>
        public DeliveryEngineMappingException(string message, IDeliveryEngineMappingExceptionInfo info, Exception innerException)
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
        protected DeliveryEngineMappingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Information about th mapping exception.
        /// </summary>
        public virtual IDeliveryEngineMappingExceptionInfo Information
        {
            get
            {
                return _info;
            }
        }

        #endregion
    }
}
