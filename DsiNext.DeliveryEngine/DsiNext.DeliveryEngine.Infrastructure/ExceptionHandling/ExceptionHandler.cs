using System;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.ExceptionHandling;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Infrastructure.ExceptionHandling
{
    /// <summary>
    /// Exception handler used by the delivery engine.
    /// </summary>
    public class ExceptionHandler : IExceptionHandler
    {
        #region Private variables

        private readonly IExceptionLogger _exceptionLogger;

        #endregion 

        #region Constructor

        /// <summary>
        /// Createsx an exception handler used by the delivery engine.
        /// </summary>
        /// <param name="exceptionLogger">Implementation of an exception logger.</param>
        public ExceptionHandler(IExceptionLogger exceptionLogger)
        {
            if (exceptionLogger == null)
            {
                throw new ArgumentNullException("exceptionLogger");
            }
            _exceptionLogger = exceptionLogger;
        }

        #endregion

        #region IExceptionHandler Members

        /// <summary>
        /// Event which is raised when an exception occurs.
        /// </summary>
        public virtual event HandleExceptionEventHandler OnException;

        /// <summary>
        /// Handle an exception for the delivery engine.
        /// </summary>
        /// <param name="exception">Exception to handle.</param>
        public virtual void HandleException(Exception exception)
        {
            bool canContinue;
            HandleException(exception, out canContinue);
        }

        /// <summary>
        /// Handle an exception for the delivery engine.
        /// </summary>
        /// <param name="exception">Exception to handle.</param>
        /// <param name="canContinue">Indicates whether the delivery engine may continue the process.</param>
        public virtual void HandleException(Exception exception, out bool canContinue)
        {
            try
            {
                canContinue = false;
                HandleExceptionEventArgs handleExceptionEventArgs;
                // Handle exceptions based on DeliveryEngineBusinessException.
                if (exception is DeliveryEngineMetadataException)
                {
                    var metadataException = (DeliveryEngineMetadataException) exception;
                    _exceptionLogger.LogException(metadataException);
                    if (OnException == null)
                    {
                        return;
                    }
                    handleExceptionEventArgs = new HandleExceptionEventArgs(metadataException, metadataException.Information == null ? null : metadataException.Information.MetadataObject);
                    OnException.Invoke(this, handleExceptionEventArgs);
                    canContinue = handleExceptionEventArgs.CanContinue;
                    return;
                }
                if (exception is DeliveryEngineMappingException)
                {
                    var mappingException = (DeliveryEngineMappingException) exception;
                    _exceptionLogger.LogException(mappingException);
                    if (OnException == null)
                    {
                        return;
                    }
                    handleExceptionEventArgs = new HandleExceptionEventArgs(mappingException, mappingException.Information == null ? null : mappingException.Information.MappingObject, mappingException.Information == null ? null : mappingException.Information.MappingObjectData);
                    OnException.Invoke(this, handleExceptionEventArgs);
                    canContinue = handleExceptionEventArgs.CanContinue;
                    return;
                }
                if (exception is DeliveryEngineConvertException)
                {
                    var convertException = (DeliveryEngineConvertException) exception;
                    _exceptionLogger.LogException(convertException);
                    if (OnException == null)
                    {
                        return;
                    }
                    handleExceptionEventArgs = new HandleExceptionEventArgs(convertException, convertException.Information == null ? null : convertException.Information.ConvertObject, convertException.Information == null ? null : convertException.Information.ConvertObjectData);
                    OnException.Invoke(this, handleExceptionEventArgs);
                    canContinue = handleExceptionEventArgs.CanContinue;
                    return;
                }
                if (exception is DeliveryEngineValidateException)
                {
                    var validateException = (DeliveryEngineValidateException) exception;
                    _exceptionLogger.LogException(validateException);
                    if (OnException == null)
                    {
                        return;
                    }
                    handleExceptionEventArgs = new HandleExceptionEventArgs(validateException, validateException.Information == null ? null : validateException.Information.ValidateObject, validateException.Information == null ? null : validateException.Information.ValidateObjectData);
                    OnException.Invoke(this, handleExceptionEventArgs);
                    canContinue = handleExceptionEventArgs.CanContinue;
                    return;
                }
                // Handle exceptions based on DeliveryEngineExceptionBase.
                if (exception is DeliveryEngineRepositoryException)
                {
                    _exceptionLogger.LogException(exception as DeliveryEngineRepositoryException);
                    if (OnException == null)
                    {
                        return;
                    }
                    handleExceptionEventArgs = new HandleExceptionEventArgs(exception);
                    OnException.Invoke(this, handleExceptionEventArgs);
                    canContinue = handleExceptionEventArgs.CanContinue;
                    return;
                }
                if (exception is DeliveryEngineSystemException)
                {
                    _exceptionLogger.LogException(exception as DeliveryEngineSystemException);
                    if (OnException == null)
                    {
                        return;
                    }
                    handleExceptionEventArgs = new HandleExceptionEventArgs(exception);
                    OnException.Invoke(this, handleExceptionEventArgs);
                    canContinue = handleExceptionEventArgs.CanContinue;
                    return;
                }
                if (exception is DeliveryEngineBusinessException)
                {
                    _exceptionLogger.LogException(exception as DeliveryEngineBusinessException);
                    if (OnException == null)
                    {
                        return;
                    }
                    handleExceptionEventArgs = new HandleExceptionEventArgs(exception);
                    OnException.Invoke(this, handleExceptionEventArgs);
                    canContinue = handleExceptionEventArgs.CanContinue;
                    return;
                }
                // Handle exceptions based on Exception.
                _exceptionLogger.LogException(exception);
                if (OnException == null)
                {
                    return;
                }
                handleExceptionEventArgs = new HandleExceptionEventArgs(exception);
                OnException.Invoke(this, handleExceptionEventArgs);
                canContinue = handleExceptionEventArgs.CanContinue;
            }
            catch (Exception ex)
            {
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.ErrorHandleException, ex.Message));
            }
        }

        #endregion
    }
}
