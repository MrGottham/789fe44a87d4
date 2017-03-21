using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Commands;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.DataValidators;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Events;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Events;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.BusinessLogic.DataValidators
{
    /// <summary>
    /// Base class for a data validator.
    /// </summary>
    /// <typeparam name="TCommand">Type of command which to validate with.</typeparam>
    public abstract class DataValidatorBase<TCommand> : IDataValidator where TCommand : ICommand
    {
        #region Events

        /// <summary>
        /// Event raised for each validation.
        /// </summary>
        public virtual event DeliveryEngineEventHandler<IDataValidatorEventArgs> OnValidation;

        #endregion

        #region Methods

        /// <summary>
        /// Validating data for a target table.
        /// </summary>
        /// <param name="targetTable">Target table.</param>
        /// <param name="targetTableData">Data for the target table.</param>
        /// <param name="endOfData">Indicates whether this is the last data for the target table.</param>
        /// <param name="command">Command which to validate with.</param>
        protected abstract void ValidateData(ITable targetTable, IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>> targetTableData, bool endOfData, TCommand command);

        /// <summary>
        /// Raises the OnValidation event.
        /// </summary>
        /// <param name="sender">Object which raises the event.</param>
        /// <param name="eventArgs">Arguments for the event.</param>
        protected virtual void RaiseOnValidationEvent(object sender, IDataValidatorEventArgs eventArgs)
        {
            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }
            if (eventArgs == null)
            {
                throw new ArgumentNullException("eventArgs");
            }
            if (OnValidation != null)
            {
                OnValidation.Invoke(sender, eventArgs);
            }
        }

        /// <summary>
        /// Gets the dictionary name for a given key.
        /// </summary>
        /// <param name="key">Key for which to get dictionary name.</param>
        /// <returns>Dictionary name.</returns>
        protected virtual string GetDictionaryName(IKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            var stringBuilder = new StringBuilder();
            key.Fields.Select(m => m.Key).ToList().ForEach(field =>
                {
                    if (stringBuilder.Length > 0)
                    {
                        stringBuilder.Append(',');
                    }
                    stringBuilder.Append(field.NameTarget ?? "{null}");
                    stringBuilder.Append(':');
                    stringBuilder.Append(field.NameSource ?? "{null}");
                });
            return string.Format("{0}:{1}({2})", key.Table.NameTarget ?? "{null}", key.Table.NameSource ?? "{null}", stringBuilder);
        }

        /// <summary>
        /// Gets a data queryer for executing queries on a given data repository.
        /// </summary>
        /// <param name="dataRepository">Data repository on which to execute queries.</param>
        /// <returns>Data queryer for executing queries.</returns>
        protected virtual IDataQueryer GetDataQueryer(IDataRepository dataRepository)
        {
            if (dataRepository == null)
            {
                throw new ArgumentNullException("dataRepository");
            }
            try
            {
                return dataRepository.GetDataQueryer();
            }
            catch (NotSupportedException)
            {
                return null;
            }
            catch (DeliveryEngineExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.UnableToGetDataQueryer, dataRepository.GetType().Name, ex.Message), ex);
            }
        }

        /// <summary>
        /// Validating data for a target table.
        /// </summary>
        /// <param name="targetTable">Target table.</param>
        /// <param name="targetTableData">Data for the target table.</param>
        /// <param name="endOfData">Indicates whether this is the last data for the target table.</param>
        /// <param name="command">Command which to validate with.</param>
        public virtual void Validate(ITable targetTable, IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>> targetTableData, bool endOfData, ICommand command)
        {
            if (targetTable == null)
            {
                throw new ArgumentNullException("targetTable");
            }
            if (targetTableData == null)
            {
                throw new ArgumentNullException("targetTableData");
            }
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            if (command is TCommand == false)
            {
                return;
            }
            try
            {
                ValidateData(targetTable, targetTableData, endOfData, (TCommand) command);
            }
            finally
            {
                GC.Collect();
            }
        }

        #endregion
    }
}
