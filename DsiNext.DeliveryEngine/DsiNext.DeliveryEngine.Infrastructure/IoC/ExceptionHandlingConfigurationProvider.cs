using Castle.Windsor;
using Castle.MicroKernel.Registration;
using DsiNext.DeliveryEngine.Infrastructure.ExceptionHandling;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.ExceptionHandling;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.IoC;

namespace DsiNext.DeliveryEngine.Infrastructure.IoC
{
    /// <summary>
    /// Configuration provider for exception handling.
    /// </summary>
    public class ExceptionHandlingConfigurationProvider : IExceptionHandlingConfigurationProvider
    {
        #region IConfigurationProvider Members

        /// <summary>
        /// Adding configuration to the container for Inversion of Control.
        /// </summary>
        /// <param name="container">Container for Inversion of Control.</param>
        public void AddConfiguration(IWindsorContainer container)
        {
            container.Register(Component.For<IExceptionHandler>().ImplementedBy<ExceptionHandler>().LifeStyle.PerThread);
        }

        #endregion
    }
}
