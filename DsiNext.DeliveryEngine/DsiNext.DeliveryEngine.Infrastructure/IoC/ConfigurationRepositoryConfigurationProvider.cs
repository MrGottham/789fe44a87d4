using System.Configuration;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.IoC;
using DsiNext.DeliveryEngine.Repositories.Configuration;
using DsiNext.DeliveryEngine.Repositories.Interfaces;

namespace DsiNext.DeliveryEngine.Infrastructure.IoC
{
    /// <summary>
    /// Configuration provider for configuration repository.
    /// </summary>
    public class ConfigurationRepositoryConfigurationProvider : IConfigurationRepositoryConfigurationProvider
    {
        #region IConfigurationProvider Members

        /// <summary>
        /// Adding configuration to the container for Inversion of Control.
        /// </summary>
        /// <param name="container">Container for Inversion of Control.</param>
        public void AddConfiguration(IWindsorContainer container)
        {
            IConfigurationRepository configurationRepository = new ConfigurationRepository(ConfigurationManager.AppSettings);

            container.Register(Component.For<IConfigurationRepository>().Instance(configurationRepository).LifeStyle.Singleton);
        }

        #endregion
    }
}
