using System;
using System.Collections.Generic;

namespace DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators
{
    /// <summary>
    /// Interface for criteria configurations to be used by a data manipulator.
    /// </summary>
    public interface ICriteriaConfigurations
    {
        /// <summary>
        /// Configuration for the criterias used by the data manipulator.
        /// </summary>
        IEnumerable<Tuple<Type, string, object>> CriteriaConfigurations
        {
            get;
        }
    }
}
