using System;
using System.Diagnostics;
using System.IO;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.ExceptionHandling;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Infrastructure.ExceptionHandling
{
    /// <summary>
    /// Exception logger.
    /// </summary>
    public class ExceptionLogger : IExceptionLogger, IDisposable
    {
        #region Private variables

        private bool _disposed;
        private readonly XmlWriterTraceListener _traceListener;
        
        #endregion

        #region Constructor

        /// <summary>
        /// Creates an exception logger.
        /// </summary>
        /// <param name="logPath">Path where the exception logger can write log files.</param>
        public ExceptionLogger(DirectoryInfo logPath)
        {
            if (logPath == null)
            {
                throw new ArgumentNullException("logPath");
            }
            if (!logPath.Exists)
            {
                throw new DeliveryEngineRepositoryException(
                    Resource.GetExceptionMessage(ExceptionMessage.DirectoryNotFound, logPath.FullName));
            }
            _traceListener = new XmlWriterTraceListener(string.Format("{0}{1}DeliveryEngine.Exceptions.{2}.svclog", logPath.FullName, Path.DirectorySeparatorChar, DateTime.Now.ToString("yyyyMMdd")), "DeliveryEngine")
                                 {
                                     TraceOutputOptions = TraceOptions.DateTime | TraceOptions.Timestamp | TraceOptions.Callstack | TraceOptions.ProcessId | TraceOptions.ThreadId
                                 };
            Trace.Listeners.Add(_traceListener);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Default destructor which release all internal resources.
        /// </summary>
        ~ExceptionLogger()
        {
            Dispose(false);
        }

        /// <summary>
        /// Release all internal resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Release all internal resources.
        /// </summary>
        /// <param name="disposing">Indicates whether all managed resources should be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            try
            {
                _traceListener.Close();
                if (disposing)
                {
                    _traceListener.Dispose();
                }
            }
            finally
            {
                Trace.Listeners.Remove(_traceListener);
                _disposed = true;
            }
        }

        #endregion

        #region IExceptionLogger Members

        /// <summary>
        /// Log a repository exception in the delivery engine.
        /// </summary>
        /// <param name="exception">Repository exception from the delivery engine.</param>
        public virtual void LogException(DeliveryEngineRepositoryException exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
            try
            {
                Trace.TraceError("{0}: {1}, StackTrace: {2}", exception.GetType().Name, exception.Message, exception.StackTrace);
                Trace.Flush();
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            {
            }
            // ReSharper restore EmptyGeneralCatchClause
        }

        /// <summary>
        /// Log a system exception in the delivery engine.
        /// </summary>
        /// <param name="exception">Repository exception from the delivery engine.</param>
        public virtual void LogException(DeliveryEngineSystemException exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
            try
            {
                Trace.TraceError("{0}: {1}, StackTrace: {2}", exception.GetType().Name, exception.Message, exception.StackTrace);
                Trace.Flush();
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            {
            }
            // ReSharper restore EmptyGeneralCatchClause
        }

        /// <summary>
        /// Log a business exception in the delivery engine.
        /// </summary>
        /// <param name="exception">Business exception from the delivery engine.</param>
        public virtual void LogException(DeliveryEngineBusinessException exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
            try
            {
                Trace.TraceError("{0}: {1}, StackTrace: {2}", exception.GetType().Name, exception.Message, exception.StackTrace);
                Trace.Flush();
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            {
            }
            // ReSharper restore EmptyGeneralCatchClause
        }

        /// <summary>
        /// Log a metadata exception in the delivery engine.
        /// </summary>
        /// <param name="exception">Metadata exception from the delivery engine.</param>
        public virtual void LogException(DeliveryEngineMetadataException exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
            try
            {
                Trace.TraceError("{0} ({1}): {2}, StackTrace: {3}", exception.GetType().Name, exception.Information.ExceptionInfo, exception.Message, exception.StackTrace);
                Trace.Flush();
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            {
            }
            // ReSharper restore EmptyGeneralCatchClause
        }

        /// <summary>
        /// Log a mapping exception in the delivery engine.
        /// </summary>
        /// <param name="exception">Mapping exception from the delivery engine.</param>
        public virtual void LogException(DeliveryEngineMappingException exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
            try
            {
                Trace.TraceError("{0} ({1}): {2}, StackTrace: {3}", exception.GetType().Name, exception.Information.ExceptionInfo, exception.Message, exception.StackTrace);
                Trace.Flush();
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            {
            }
            // ReSharper restore EmptyGeneralCatchClause
        }

        /// <summary>
        /// Log a convert exception in the delivery engine.
        /// </summary>
        /// <param name="exception">Convert exception from the delivery engine.</param>
        public virtual void LogException(DeliveryEngineConvertException exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
            try
            {
                Trace.TraceError("{0} ({1}): {2}, StackTrace: {3}", exception.GetType().Name, exception.Information.ExceptionInfo, exception.Message, exception.StackTrace);
                Trace.Flush();
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            {
            }
            // ReSharper restore EmptyGeneralCatchClause
        }

        /// <summary>
        /// Log a validation exception in the delivery engine.
        /// </summary>
        /// <param name="exception">Validation exception from the delivery engine.</param>
        public virtual void LogException(DeliveryEngineValidateException exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
            try
            {
                Trace.TraceError("{0} ({1}): {2}, StackTrace: {3}", exception.GetType().Name, exception.Information.ExceptionInfo, exception.Message, exception.StackTrace);
                Trace.Flush();
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            {
            }
            // ReSharper restore EmptyGeneralCatchClause
        }

        /// <summary>
        /// Log an exception in the delivery engine.
        /// </summary>
        /// <param name="exception">Exception from the delivery engine.</param>
        public virtual void LogException(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }
            try
            {
                Trace.TraceError("{0}: {1}, StackTrace: {2}", exception.GetType().Name, exception.Message, exception.StackTrace);
                Trace.Flush();
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            {
            }
            // ReSharper restore EmptyGeneralCatchClause
        }

        #endregion
    }
}
