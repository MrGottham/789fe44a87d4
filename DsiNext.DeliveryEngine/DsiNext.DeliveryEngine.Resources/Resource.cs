using System;
using System.Reflection;
using System.Resources;
using System.Threading;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;

namespace DsiNext.DeliveryEngine.Resources
{
    /// <summary>
    /// Access to resources for the delivery engine.
    /// </summary>
    public class Resource
    {
        #region Private variables

        private static readonly ResourceManager ExceptionMessages = new ResourceManager("DsiNext.DeliveryEngine.Resources.ExceptionMessages", Assembly.GetCallingAssembly());
        private static readonly ResourceManager Texts = new ResourceManager("DsiNext.DeliveryEngine.Resources.Texts", Assembly.GetCallingAssembly());

        #endregion

        #region Methods

        /// <summary>
        /// Gets an exception message.
        /// </summary>
        /// <param name="resourceName">Resource name for the exception message.</param>
        /// <returns>Exception message.</returns>
        public static string GetExceptionMessage(ExceptionMessage resourceName)
        {
            return GetExceptionMessage(resourceName, null);
        }

        /// <summary>
        /// Get an exception message.
        /// </summary>
        /// <param name="resourceName">Resource name for the exception message.</param>
        /// <param name="args">Arguments to the exception message.</param>
        /// <returns>Exception message.</returns>
        public static string GetExceptionMessage(ExceptionMessage resourceName, params object[] args)
        {
            try
            {
                var exceptionMessage = ExceptionMessages.GetString(resourceName.ToString());
                if (exceptionMessage == null)
                {
                    throw new DeliveryEngineSystemException("Null returned for ExceptionMessages.");
                }
                return args != null ? string.Format(exceptionMessage, args) : exceptionMessage;
            }
            catch (Exception ex)
            {
                throw new DeliveryEngineSystemException(string.Format("Couldn't get resource string '{0}' using culture '{1}'.", resourceName, Thread.CurrentThread.CurrentUICulture.Name), ex);
            }
        }

        /// <summary>
        /// Gets a text for the delivery engine.
        /// </summary>
        /// <param name="resourceName">Resource name for the text.</param>
        /// <returns>Text.</returns>
        public static string GetText(Text resourceName)
        {
            return GetText(resourceName, null);
        }

        /// <summary>
        /// Get a text for the delivery engine
        /// </summary>
        /// <param name="resourceName">Resource name for the text.</param>
        /// <param name="args">Arguments to the text.</param>
        /// <returns>Text.</returns>
        public static string GetText(Text resourceName, params object[] args)
        {
            try
            {
                var text = Texts.GetString(resourceName.ToString());
                if (text == null)
                {
                    throw new DeliveryEngineSystemException("Null returned for Texts.");
                }
                return args != null ? string.Format(text, args) : text;
            }
            catch (Exception ex)
            {
                throw new DeliveryEngineSystemException(string.Format("Couldn't get resource string '{0}' using culture '{1}'.", resourceName, Thread.CurrentThread.CurrentUICulture.Name), ex);
            }
        }

        #endregion
    }
}
