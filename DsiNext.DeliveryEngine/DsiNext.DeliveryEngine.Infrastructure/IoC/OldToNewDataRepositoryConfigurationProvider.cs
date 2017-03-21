using System;
using System.Configuration;
using System.IO;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.IoC;
using DsiNext.DeliveryEngine.Repositories.Data.OldToNew;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Infrastructure.IoC
{
    /// <summary>
    /// Configuration provider for data repository to converting old delivery format to the new delivery format.
    /// </summary>
    public class OldToNewDataRepositoryConfigurationProvider : IOldToNewDataRepositoryConfigurationProvider
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

            var dataRepository = new OldToNewDataRepository(new DirectoryInfo(Environment.ExpandEnvironmentVariables(sourcePath)));
            container.Register(Component.For<IDataRepository>().Instance(dataRepository).LifeStyle.PerThread);
        }

        #endregion
    }
}
