using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Events;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Events;

namespace DsiNext.DeliveryEngine.Repositories.Data.Oracle
{
    /// <summary>
    /// Interface for an Oracle client used by the delivery engine.
    /// </summary>
    public interface IOracleClient : IDataQueryer
    {
        /// <summary>
        /// Validates a table against the Oracle schema.
        /// </summary>
        /// <param name="table">Table to be validated.</param>
        void ValidateTable(ITable table);

        /// <summary>
        /// Gets data for a table.
        /// </summary>
        /// <param name="table">Tabel for which to get data.</param>
        /// <param name="onHandleOracleData">Event handler to handle data from the oracle data repository.</param>
        void GetData(ITable table, DeliveryEngineEventHandler<IHandleDataEventArgs> onHandleOracleData);

        /// <summary>
        /// Gets the number of records in a given table.
        /// </summary>
        /// <param name="table">Tabel for which to select the number of records.</param>
        /// <returns>Number of records in the table.</returns>
        int SelectCountForTable(ITable table);
    }
}
