using Castle.Windsor;
using Castle.MicroKernel.Registration;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.IoC;
using DsiNext.DeliveryEngine.Repositories.Document.OldToNew;
using DsiNext.DeliveryEngine.Repositories.Interfaces;

namespace DsiNext.DeliveryEngine.Infrastructure.IoC
{
    /// <summary>
    /// Configuration provider for document repository to converting old delivery format to the new delivery format.
    /// </summary>
    public class OldToNewDocumentRepositoryConfigurationProvider : IOldToNewDocumentRepositoryConfigurationProvider
    {
        #region IConfigurationProvider Members

        /// <summary>
        /// Adding configuration to the container for Inversion of Control.
        /// </summary>
        /// <param name="container">Container for Inversion of Control.</param>
        public void AddConfiguration(IWindsorContainer container)
        {
            container.Register(Component.For<IDocumentRepository>().ImplementedBy<OldToNewDocumentRepository>().LifeStyle.PerThread);
        }

        #endregion
    }
}
