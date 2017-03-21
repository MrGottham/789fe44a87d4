using Castle.MicroKernel.Registration;
using Castle.Windsor;
using DsiNext.DeliveryEngine.BusinessLogic.DataValidators;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.DataValidators;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.IoC;

namespace DsiNext.DeliveryEngine.Infrastructure.IoC
{
    /// <summary>
    /// Configuration provider to data validators for the delivery engine.
    /// </summary>
    public class DataValidatorsConfigurationProvider : IDataValidatorsConfigurationProvider
    {
        #region IConfigurationProvider Members

        /// <summary>
        ///  Adding configuration to the container for Inversion of Control.
        /// </summary>
        /// <param name="container">Container for Inversion of Control.</param>
        public void AddConfiguration(IWindsorContainer container)
        {
            container.Register(Component.For<IDataValidators>().ImplementedBy<DataValidators>().LifeStyle.Transient);
        }

        #endregion
    }
}
