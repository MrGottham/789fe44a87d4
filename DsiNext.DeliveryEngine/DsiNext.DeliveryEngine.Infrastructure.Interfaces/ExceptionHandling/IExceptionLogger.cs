using System;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;

namespace DsiNext.DeliveryEngine.Infrastructure.Interfaces.ExceptionHandling
{
    /// <summary>
    /// Interface for an exception logger.
    /// </summary>
    public interface IExceptionLogger
    {
        /// <summary>
        /// Log a repository exception in the delivery engine.
        /// </summary>
        /// <param name="exception">Repository exception from the delivery engine.</param>
        void LogException(DeliveryEngineRepositoryException exception);

        /// <summary>
        /// Log a system exception in the delivery engine.
        /// </summary>
        /// <param name="exception">Repository exception from the delivery engine.</param>
        void LogException(DeliveryEngineSystemException exception);

        /// <summary>
        /// Log a business exception in the delivery engine.
        /// </summary>
        /// <param name="exception">Business exception from the delivery engine.</param>
        void LogException(DeliveryEngineBusinessException exception);

        /// <summary>
        /// Log a metadata exception in the delivery engine.
        /// </summary>
        /// <param name="exception">Metadata exception from the delivery engine.</param>
        void LogException(DeliveryEngineMetadataException exception);

        /// <summary>
        /// Log a mapping exception in the delivery engine.
        /// </summary>
        /// <param name="exception">Mapping exception from the delivery engine.</param>
        void LogException(DeliveryEngineMappingException exception);

        /// <summary>
        /// Log a convert exception in the delivery engine.
        /// </summary>
        /// <param name="exception">Convert exception from the delivery engine.</param>
        void LogException(DeliveryEngineConvertException exception);

        /// <summary>
        /// Log a validation exception in the delivery engine.
        /// </summary>
        /// <param name="exception">Validation exception from the delivery engine.</param>
        void LogException(DeliveryEngineValidateException exception);

        /// <summary>
        /// Log an exception in the delivery engine.
        /// </summary>
        /// <param name="exception">Exception from the delivery engine.</param>
        void LogException(Exception exception);
    }
}
