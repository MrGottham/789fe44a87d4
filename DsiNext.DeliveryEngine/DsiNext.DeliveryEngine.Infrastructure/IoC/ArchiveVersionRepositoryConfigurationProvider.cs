using System;
using System.Configuration;
using System.IO;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.IoC;
using DsiNext.DeliveryEngine.Repositories;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Infrastructure.IoC
{
    /// <summary>
    /// Configuration provider to the repository for writing the delivery format.
    /// </summary>
    public class ArchiveVersionRepositoryConfigurationProvider : IArchiveVersionRepositoryConfigurationProvider
    {
        #region IConfigurationProvider Members

        /// <summary>
        /// Adding configuration to the container for Inversion of Control.
        /// </summary>
        /// <param name="container">Container for Inversion of Control.</param>
        public void AddConfiguration(IWindsorContainer container)
        {
            var archivePath = ConfigurationManager.AppSettings["ArchivePath"];
            if (string.IsNullOrEmpty(archivePath))
            {
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.ApplicationSettingMissing, "ArchivePath"));
            }

            var archiveVersionRepository = new ArchiveVersionRepository(new DirectoryInfo(Environment.ExpandEnvironmentVariables(archivePath)));
            container.Register(Component.For<IArchiveVersionRepository>().Instance(archiveVersionRepository).LifeStyle.PerThread);
        }

        #endregion
    }
}
