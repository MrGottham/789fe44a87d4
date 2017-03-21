using System;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.ExceptionHandling;

namespace DsiNext.DeliveryEngine.Infrastructure.ExceptionHandling
{
    /// <summary>
    /// Arguments to the eventhandler which handles exceptions.
    /// </summary>
    public class HandleExceptionEventArgs : EventArgs, IHandleExceptionEventArgs
    {
        #region Private variables

        private readonly Exception _exception;
        private readonly object _exceptionObject;
        private readonly object _exceptionObjectData;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates arguments to the eventhandler which handles exceptions.
        /// </summary>
        /// <param name="exception">Exception to be handled by the eventhandler.</param>
        public HandleExceptionEventArgs(Exception exception)
            : this(exception, null)
        {
        }

        /// <summary>
        /// Creates arguments to the eventhandler which handles exceptions.
        /// </summary>
        /// <param name="exception">Exception to be handled by the eventhandler.</param>
        /// <param name="exceptionObject">Object which has been send with the exception.</param>
        public HandleExceptionEventArgs(Exception exception, object exceptionObject)
            : this(exception, exceptionObject, null)
        {
        }

        /// <summary>
        /// Creates arguments to the eventhandler which handles exceptions.
        /// </summary>
        /// <param name="exception">Exception to be handled by the eventhandler.</param>
        /// <param name="exceptionObject">Object which has been send with the exception.</param>
        /// <param name="exceptionObjectData">Data for the object which has been send with the exception.</param>
        public HandleExceptionEventArgs(Exception exception, object exceptionObject, object exceptionObjectData)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
            _exception = exception;
            _exceptionObject = exceptionObject;
            _exceptionObjectData = exceptionObjectData;
        }

        #endregion

        #region IHandleExceptionEventArgs Members

        /// <summary>
        /// Exception message send to the eventhandler.
        /// </summary>
        public virtual string Message
        {
            get
            {
                return _exception.Message;
            }
        }

        /// <summary>
        /// Exception to be handled by the eventhandler.
        /// </summary>
        public virtual Exception Exception
        {
            get
            {
                return _exception;
            }
        }

        /// <summary>
        /// Object which has been send with the exception.
        /// This can be an MetadataObject, MappingObject, ConvertionObject or ValidationObject.
        /// </summary>
        public virtual object ExceptionObject
        {
            get
            {
                return _exceptionObject;
            }
        }

        /// <summary>
        /// Data for the object which has been send with the exception.
        /// This can be a MappingObjectData, ConvertionObjectData or ValidationObjectData.
        /// </summary>
        public virtual object ExceptionObjectData
        {
            get
            {
                return _exceptionObjectData;
            }
        }

        /// <summary>
        /// Indicates whether the eventhandler handle the exception so that the delivery engine can continue the process.
        /// </summary>
        public virtual bool CanContinue
        {
            get;
            set;
        }

        #endregion
    }
}
