using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DsiNext.DeliveryEngine.BusinessLogic.Events;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Commands;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.DataValidators;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.BusinessLogic.DataValidators
{
    /// <summary>
    /// Mapping validator for on data for a target table.
    /// </summary>
    public class MappingDataValidator : DataValidatorBase<ICommand>, IMappingDataValidator
    {
        #region Methods

        /// <summary>
        /// Validates mappers for each data object on the data for a given target table.
        /// </summary>
        /// <param name="targetTable">Target table.</param>
        /// <param name="targetTableData">Data for the target table.</param>
        /// <param name="endOfData">Indicates whether this is the last data for the target table.</param>
        /// <param name="command">Command which to validate with.</param>
        protected override void ValidateData(ITable targetTable, IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>> targetTableData, bool endOfData, ICommand command)
        {
            foreach (var dataTable in targetTableData.Keys)
            {
                foreach (var dataRow in targetTableData[dataTable])
                {
                    RaiseOnValidationEvent(this, new DataValidatorEventArgs(dataRow));
                    foreach (var mappedDataObject in dataRow.Where(m => m.Field != null && m.Field.Map != null))
                    {
                        var sourceValueType = mappedDataObject.Field.DatatypeOfSource;
                        var targetValueType = mappedDataObject.Field.DatatypeOfTarget;
                        var getSourceValueMethod = mappedDataObject.GetType()
                                                                   .GetMethod("GetSourceValue")
                                                                   .MakeGenericMethod(new[] {sourceValueType});
                        var mapper = mappedDataObject.Field.Map;
                        var mapMethod = mapper.GetType()
                                              .GetMethods()
                                              .Single(m => m.Name.Equals("MapValue") && m.IsGenericMethod && m.GetGenericArguments().Count() == 2)
                                              .MakeGenericMethod(new[] {sourceValueType, targetValueType});
                        var sourceValue = getSourceValueMethod.Invoke(mappedDataObject, null);
                        try
                        {
                            mapMethod.Invoke(mapper, new[] {sourceValue});
                        }
                        catch (TargetInvocationException ex)
                        {
                            mapper.MappingObjectData = dataRow;
                            if (ex.InnerException == null)
                            {
                                throw new DeliveryEngineMappingException(Resource.GetExceptionMessage(ExceptionMessage.UnableToMapValueForField, sourceValue, mappedDataObject.Field.NameTarget, mappedDataObject.Field.Table.NameTarget, ex.Message), mapper, ex);
                            }
                            throw new DeliveryEngineMappingException(Resource.GetExceptionMessage(ExceptionMessage.UnableToMapValueForField, sourceValue, mappedDataObject.Field.NameTarget, mappedDataObject.Field.Table.NameTarget, ex.InnerException.Message), mapper, ex.InnerException);
                        }
                        catch (Exception ex)
                        {
                            mapper.MappingObjectData = dataRow;
                            throw new DeliveryEngineMappingException(Resource.GetExceptionMessage(ExceptionMessage.UnableToMapValueForField, sourceValue, mappedDataObject.Field.NameTarget, mappedDataObject.Field.Table.NameTarget, ex.Message), mapper, ex);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
