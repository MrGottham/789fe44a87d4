using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Repositories.Interfaces
{
    /// <summary>
    /// Interface to the metadata repository used by the delivery engine.
    /// </summary>
    public interface IMetadataRepository : IRepository
    {
        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <returns>Data source.</returns>
        IDataSource DataSourceGet();
    }
}
