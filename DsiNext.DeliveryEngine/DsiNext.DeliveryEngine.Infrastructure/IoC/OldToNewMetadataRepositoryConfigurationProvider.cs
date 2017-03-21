using System;
using System.Configuration;
using System.IO;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.IoC;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Repositories.Metadata.OldToNew;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Infrastructure.IoC
{
    /// <summary>
    /// Configuration provider for metadata repository to converting old delivery format to the new delivery format.
    /// </summary>
    public class OldToNewMetadataRepositoryConfigurationProvider : IOldToNewMetadataRepositoryConfigurationProvider
    {
        #region IConfigurationProvider Members

        /// <summary>
        /// Adding configuration to the container for Inversion of Control.
        /// </summary>
        /// <param name="container">Container for Inversion of Control.</param>
        public void AddConfiguration(IWindsorContainer container)
        {
            var sourcePath = ConfigurationManager.AppSettings["SourcePath"];
            if (string.IsNullOrEmpty(sourcePath))
            {
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.ApplicationSettingMissing, "SourcePath"));
            }

            var metadataRepository = new OldToNewMetadataRepository(new DirectoryInfo(Environment.ExpandEnvironmentVariables(sourcePath)), new ConfigurationValues());
            container.Register(Component.For<IMetadataRepository>().Instance(metadataRepository).LifeStyle.PerThread);
        }

        #endregion
    }
}
