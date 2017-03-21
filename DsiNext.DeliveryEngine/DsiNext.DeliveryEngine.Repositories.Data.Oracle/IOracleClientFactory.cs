using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators;

namespace DsiNext.DeliveryEngine.Repositories.Data.Oracle
{
    /// <summary>
    /// Interface for a factory to create Oracle clients used by the delivery engine.
    /// </summary>
    public interface IOracleClientFactory
    {
        /// <summary>
        /// Creates an Oracle client to be used by the delivery engine.
        /// </summary>
        /// <returns>Oracle client for the delivery engine.</returns>
        IOracleClient Create();

        /// <summary>
        /// Creates a data queryer for executing queries on Oracle.
        /// </summary>
        /// <param name="dataManipulators">Data manipulaters used to manipulate data is queried.</param>
        /// <returns>Data queryer for executing queries on Oracle.</returns>
        IDataQueryer CreateDataQueryer(IDataManipulators dataManipulators);
    }
}
