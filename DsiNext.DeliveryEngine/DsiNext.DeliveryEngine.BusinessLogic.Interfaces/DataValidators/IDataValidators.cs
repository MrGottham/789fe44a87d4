using System.Collections.Generic;

namespace DsiNext.DeliveryEngine.BusinessLogic.Interfaces.DataValidators
{
    /// <summary>
    /// Interface for a collection of data validators for the delivery engine.
    /// </summary>
    public interface IDataValidators : IEnumerable<IDataValidator>
    {
    }
}
