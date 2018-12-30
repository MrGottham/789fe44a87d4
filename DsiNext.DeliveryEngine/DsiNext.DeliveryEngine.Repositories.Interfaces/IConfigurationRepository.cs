namespace DsiNext.DeliveryEngine.Repositories.Interfaces
{
    /// <summary>
    /// Interface to the configuration repository used by the delivery engine.
    /// </summary>
    public interface IConfigurationRepository : IRepository
    {
        /// <summary>
        /// Gets whether empty tables should be included in the delivery.
        /// </summary>
        bool IncludeEmptyTables { get; }
    }
}
