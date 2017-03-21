using Castle.MicroKernel.Registration;
using Castle.Windsor;
using DsiNext.DeliveryEngine.BusinessLogic.DataValidators;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.DataValidators;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.IoC;

namespace DsiNext.DeliveryEngine.Infrastructure.IoC
{
    /// <summary>
    /// Configuration provider for a primary key data validator.
    /// </summary>
    public class PrimaryKeyDataValidatorConfigurationProvider : IPrimaryKeyDataValidatorConfigurationProvider
    {
        #region IConfigurationProvider Members

        /// <summary>
        /// Adding configuration to the container for Inversion of Control.
        /// </summary>
        /// <param name="container">Container for Inversion of Control.</param>
        public void AddConfiguration(IWindsorContainer container)
        {
            container.Register(Component.For<IPrimaryKeyDataValidator>().ImplementedBy<PrimaryKeyDataValidator>().LifeStyle.PerThread);
        }

        #endregion
    }
}
