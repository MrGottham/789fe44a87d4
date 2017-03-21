using System;
using System.Configuration;
using System.IO;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using DsiNext.DeliveryEngine.Infrastructure.ExceptionHandling;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.ExceptionHandling;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.IoC;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Infrastructure.IoC
{
    /// <summary>
    /// Configuration provider for exception logger.
    /// </summary>
    public class ExceptionLoggerConfigurationProvider : IExceptionLoggerConfigurationProvider
    {
        #region IConfigurationProvider Members

        /// <summary>
        /// Adding configuration to the container for Inversion of Control.
        /// </summary>
        /// <param name="container">Container for Inversion of Control.</param>
        public void AddConfiguration(IWindsorContainer container)
        {
            var logPath = ConfigurationManager.AppSettings["LogPath"];
            if (string.IsNullOrEmpty(logPath))
            {
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.ApplicationSettingMissing, "LogPath"));
            }

            var exceptionLogger = new ExceptionLogger(new DirectoryInfo(Environment.ExpandEnvironmentVariables(logPath)));
            container.Register(Component.For<IExceptionLogger>().Instance(exceptionLogger).LifeStyle.Singleton);
        }

        #endregion
    }
}
