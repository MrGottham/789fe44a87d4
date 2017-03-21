using System;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Events;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Events;

namespace DsiNext.DeliveryEngine.Repositories.Interfaces
{
    /// <summary>
    /// Interface to the data repository used by the delivery engine.
    /// </summary>
    public interface IDataRepository : IRepository, ICloneable
    {
        /// <summary>
        /// Event raised when the data repository must handle data.
        /// </summary>
        event DeliveryEngineEventHandler<IHandleDataEventArgs> OnHandleData;

        /// <summary>
        /// Event raised when the data repository is cloned.
        /// </summary>
        event DeliveryEngineEventHandler<ICloneDataRepositoryEventArgs> OnClone;

        /// <summary>
        /// Gets data for a specific target table where data can be merged from one or more source tables.
        /// </summary>
        /// <param name="targetTableName">Name of the target table where data should be returned.</param>
        /// <param name="dataSource">Data source from where to get the data.</param>
        void DataGetForTargetTable(string targetTableName, IDataSource dataSource);

        /// <summary>
        /// Gets data from a table.
        /// </summary>
        /// <param name="table">Table from where data should be returned.</param>
        void DataGetFromTable(ITable table);

        /// <summary>
        /// Gets a data queryer for executing queries.
        /// </summary>
        /// <returns>Data queryer to execute queries.</returns>
        IDataQueryer GetDataQueryer();
    }
}
