using Castle.Windsor;
using Castle.MicroKernel.Registration;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.IoC;
using DsiNext.DeliveryEngine.Repositories.Data.Oracle;
using DsiNext.DeliveryEngine.Repositories.Interfaces;

namespace DsiNext.DeliveryEngine.Infrastructure.IoC
{
    /// <summary>
    /// Configuration provider for a data repository which read data from an Oracle database.
    /// </summary>
    public class OracleDataRepositoryConfigurationProvider : IOracleDataRepositoryConfigurationProvider
    {
        #region IConfigurationProvider Members

        /// <summary>
        /// Adding configuration to the container for Inversion of Control.
        /// </summary>
        /// <param name="container">Container for Inversion of Control.</param>
        public void AddConfiguration(IWindsorContainer container)
        {
            container.Register(Component.For<IOracleClientFactory>().ImplementedBy<OracleClientFactory>().LifeStyle.PerThread);
            container.Register(Component.For<IDataRepository>().ImplementedBy<OracleDataRepository>().LifeStyle.PerThread);
        }

        #endregion
    }
}
