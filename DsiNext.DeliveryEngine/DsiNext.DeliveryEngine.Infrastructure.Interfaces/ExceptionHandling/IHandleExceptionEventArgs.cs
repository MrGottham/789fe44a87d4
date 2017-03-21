using System;

namespace DsiNext.DeliveryEngine.Infrastructure.Interfaces.ExceptionHandling
{
    /// <summary>
    /// Interface for arguments to the eventhandler which handles exceptions.
    /// </summary>
    public interface IHandleExceptionEventArgs
    {
        /// <summary>
        /// Exception message send to the eventhandler.
        /// </summary>
        string Message
        {
            get;
        }

        /// <summary>
        /// Exception to be handled by the eventhandler.
        /// </summary>
        Exception Exception
        {
            get;
        }

        /// <summary>
        /// Object which has been send with the exception.
        /// This can be an MetadataObject, MappingObject, ConvertionObject or ValidationObject.
        /// </summary>
        object ExceptionObject
        {
            get;
        }

        /// <summary>
        /// Data for the object which has been send with the exception.
        /// This can be a MappingObjectData, ConvertionObjectData or ValidationObjectData.
        /// </summary>
        object ExceptionObjectData
        {
            get;
        }

        /// <summary>
        /// Indicates whether the eventhandler handle the exception so that the delivery engine can continue the process.
        /// </summary>
        bool CanContinue
        {
            get;
            set;
        }
    }
}
