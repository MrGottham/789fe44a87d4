using Castle.Windsor;
using Castle.MicroKernel.Registration;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.IoC;

namespace DsiNext.DeliveryEngine.Infrastructure.IoC
{
    /// <summary>
    /// Configuration provider to business logic in the delivery engine.
    /// </summary>
    public class BusinessLogicConfigurationProvider : IBusinessLogicConfigurationProvider
    {
        #region IConfigurationProvider Members

        /// <summary>
        /// Adding configuration to the container for Inversion of Control.
        /// </summary>
        /// <param name="container">Container for Inversion of Control.</param>
        public void AddConfiguration(IWindsorContainer container)
        {
            container.Register(Component.For<IDeliveryEngine>().ImplementedBy<BusinessLogic.DeliveryEngine>().LifeStyle.PerThread);
        }

        #endregion
    }
}
