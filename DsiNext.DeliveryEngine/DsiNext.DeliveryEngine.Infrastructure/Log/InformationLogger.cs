using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Log;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Infrastructure.Log
{
    /// <summary>
    /// Information logger.
    /// </summary>
    public class InformationLogger : IInformationLogger, IDisposable
    {
        #region Private variables

        private bool _disposed;
        private readonly FileStream _logFileStream;
        private readonly TextWriter _textWriter;
        private static readonly object SyncRoot = new object();

        #endregion

        #region Constructor

        /// <summary>
        /// Creates an information logger.
        /// </summary>
        /// <param name="logPath">Path where the information logger can write log files.</param>
        public InformationLogger(DirectoryInfo logPath)
        {
            if (logPath == null)
            {
                throw new ArgumentNullException("logPath");
            }
            if (!logPath.Exists)
            {
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.DirectoryNotFound, logPath.FullName));
            }
            _logFileStream = new FileStream(string.Format("{0}{1}DeliveryEngine.Informations.{2}.txt", logPath.FullName, Path.DirectorySeparatorChar, DateTime.Now.ToString("yyyyMMdd")), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            try
            {
                _textWriter = new StreamWriter(_logFileStream, Encoding.UTF8);
            }
            catch
            {
                _logFileStream.Close();
                _logFileStream.Dispose();
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Default destructor which release all internal resources.
        /// </summary>
        ~InformationLogger()
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
            if (_logFileStream.CanWrite)
            {
                _textWriter.Close();
            }
            _logFileStream.Close();
            if (disposing)
            {
                _textWriter.Dispose();
                _logFileStream.Dispose();
            }
            _disposed = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Log information.
        /// </summary>
        /// <param name="information">Information.</param>
        public virtual void LogInformation(string information)
        {
            LogInformation(information, null);
        }

        /// <summary>
        /// Log information.
        /// </summary>
        /// <param name="information">Information.</param>
        /// <param name="arguments">Arguments to log with information.</param>
        public virtual void LogInformation(string information, params object[] arguments)
        {
            if (string.IsNullOrEmpty(information))
            {
                throw new ArgumentNullException("information");
            }
            var message = arguments == null ? information : string.Format(information, arguments);
            WriteToLog(Resource.GetText(Text.Information), message);
        }

        /// <summary>
        /// Log warning.
        /// </summary>
        /// <param name="warning">Warning.</param>
        public virtual void LogWarning(string warning)
        {
            LogWarning(warning, null);
        }

        /// <summary>
        /// Log warning.
        /// </summary>
        /// <param name="warning">Warning.</param>
        /// <param name="arguments">Arguments to log with warning.</param>
        public virtual void LogWarning(string warning, params object[] arguments)
        {
            if (string.IsNullOrEmpty(warning))
            {
                throw new ArgumentNullException("warning");
            }
            var message = arguments == null ? warning : string.Format(warning, arguments);
            WriteToLog(Resource.GetText(Text.Warning), message);
        }

        /// <summary>
        /// Write information to the log.
        /// </summary>
        /// <param name="type">Type of information.</param>
        /// <param name="message">Message to log.</param>
        protected virtual void WriteToLog(string type, string message)
        {
            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentNullException("type");
            }
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException("message");
            }
            lock (SyncRoot)
            {
                var currentTime = DateTime.Now;
                var messageBuilder = new StringBuilder(string.Format("**\t{0}", type));
                messageBuilder.AppendLine();
                messageBuilder.AppendFormat("\t\t{0} - {1}", currentTime.ToShortDateString(), currentTime.ToShortTimeString());
                messageBuilder.AppendLine();
                try
                {
                    var stackTrace = new StackTrace();
                    var frame = stackTrace.GetFrame(3);
                    var method = frame.GetMethod();
                    messageBuilder.AppendFormat("\t\t{0}:\t{1}", Resource.GetText(Text.Assembly), method.Module.Assembly.FullName);
                    messageBuilder.AppendLine();
                    if (method.DeclaringType != null)
                    {
                        messageBuilder.AppendFormat("\t\t{0}:\t\t{1}", Resource.GetText(Text.Class), method.DeclaringType.Name);
                        messageBuilder.AppendLine();
                    }
                    messageBuilder.AppendFormat("\t\t{0}:\t\t{1}", Resource.GetText(Text.Method), method.Name);
                    messageBuilder.AppendLine();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                if (Resource.GetText(Text.Thread).Length <= 4)
                {
                    messageBuilder.AppendFormat("\t\t{0}:\t\t\t{1}", Resource.GetText(Text.Thread), Thread.CurrentThread.ManagedThreadId);
                    messageBuilder.AppendLine();
                }
                else
                {
                    messageBuilder.AppendFormat("\t\t{0}:\t\t{1}", Resource.GetText(Text.Thread), Thread.CurrentThread.ManagedThreadId);
                    messageBuilder.AppendLine();
                }
                messageBuilder.AppendFormat("\t\tS1:\t\t\t\t{0}", message);
                messageBuilder.AppendLine();
                try
                {
                    _logFileStream.Seek(0, SeekOrigin.End);
                    _textWriter.WriteLine(messageBuilder.ToString());
                    _logFileStream.Flush();
                }
                finally
                {
                    _logFileStream.Seek(0, SeekOrigin.End);
                }
            }
        }

        #endregion
    }
}
