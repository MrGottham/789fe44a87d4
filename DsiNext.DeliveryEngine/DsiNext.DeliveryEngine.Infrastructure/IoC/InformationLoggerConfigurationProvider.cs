using System;
using System.Configuration;
using System.IO;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.IoC;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Log;
using DsiNext.DeliveryEngine.Infrastructure.Log;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Infrastructure.IoC
{
    /// <summary>
    /// Configuration provider for information logger.
    /// </summary>
    public class InformationLoggerConfigurationProvider : IInformationLoggerConfigurationProvider
    {
        #region IConfigurationProvider Members

        /// <summary>
        ///  Adding configuration to the container for Inversion of Control.
        /// </summary>
        /// <param name="container">Container for Inversion of Control.</param>
        public void AddConfiguration(IWindsorContainer container)
        {
            var logPath = ConfigurationManager.AppSettings["LogPath"];
            if (string.IsNullOrEmpty(logPath))
            {
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.ApplicationSettingMissing, "LogPath"));
            }

            var informationLogger = new InformationLogger(new DirectoryInfo(Environment.ExpandEnvironmentVariables(logPath)));
            container.Register(Component.For<IInformationLogger>().Instance(informationLogger).LifeStyle.Singleton);
        }

        #endregion
    }
}
