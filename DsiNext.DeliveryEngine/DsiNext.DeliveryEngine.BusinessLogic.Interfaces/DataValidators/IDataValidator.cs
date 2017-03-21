using System.Collections.Generic;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Commands;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Events;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Events;

namespace DsiNext.DeliveryEngine.BusinessLogic.Interfaces.DataValidators
{
    /// <summary>
    /// Interface for a data validator.
    /// </summary>
    public interface IDataValidator : IBusinessLogic
    {
        /// <summary>
        /// Event raised for each validation.
        /// </summary>
        event DeliveryEngineEventHandler<IDataValidatorEventArgs> OnValidation;

        /// <summary>
        /// Validating data for a target table.
        /// </summary>
        /// <param name="targetTable">Target table.</param>
        /// <param name="targetTableData">Data for the target table.</param>
        /// <param name="endOfData">Indicates whether this is the last data for the target table.</param>
        /// <param name="command">Command which to validate with.</param>
        void Validate(ITable targetTable, IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>> targetTableData, bool endOfData, ICommand command);
    }
}
