using System;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators;

namespace DsiNext.DeliveryEngine.Repositories.Data.Oracle
{
    /// <summary>
    /// Factory to create Oracle clients used by the delivery engine.
    /// </summary>
    public class OracleClientFactory : IOracleClientFactory
    {
        /// <summary>
        /// Creates an Oracle client to be used by the delivery engine.
        /// </summary>
        /// <returns>Oracle client for the delivery engine.</returns>
        public virtual IOracleClient Create()
        {
            return new OracleClient();
        }

        /// <summary>
        /// Creates a data queryer for executing queries on Oracle.
        /// </summary>
        /// <param name="dataManipulators">Data manipulaters used to manipulate data is queried.</param>
        /// <returns>Data queryer for executing queries on Oracle.</returns>
        public virtual IDataQueryer CreateDataQueryer(IDataManipulators dataManipulators)
        {
            if (dataManipulators == null)
            {
                throw new ArgumentNullException("dataManipulators");
            }
            return new OracleClient(dataManipulators);
        }
    }
}
