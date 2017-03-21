using System;
using System.Collections.Generic;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Repositories.Interfaces
{
    /// <summary>
    /// Interface for a data queryer used to executing queries.
    /// </summary>
    public interface IDataQueryer : IDisposable
    {
        /// <summary>
        /// Get the number of equal key values for a given key.
        /// </summary>
        /// <param name="key">Key on which to calculate equal number of key values.</param>
        /// <param name="extraCriterias">Extra criterias (field name and value) to put into the record filter when querying for number of equal key values.</param>
        /// <param name="matchingKeyValue">The key value on which to calculate the number of equal key values.</param>
        /// <returns>Number of equal key values for the key.</returns>
        int GetNumberOfEqualKeyValues(IKey key, IEnumerable<KeyValuePair<string, object>> extraCriterias, string matchingKeyValue);
    }
}
