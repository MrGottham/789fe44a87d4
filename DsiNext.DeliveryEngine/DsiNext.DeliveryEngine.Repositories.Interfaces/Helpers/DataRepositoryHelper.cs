using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Domain.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Comparers;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Repositories.Interfaces.Helpers
{
    /// <summary>
    /// Helper for a data repository.
    /// </summary>
    public static class DataRepositoryHelper
    {
        /// <summary>
        /// Get key values for a given key.
        /// </summary>
        /// <param name="keyFields">Fields in the key.</param>
        /// <param name="data">Data from which to get key values.</param>
        /// <param name="excludeNulls">>Indicates whether to exclude key values where the last field value is null.</param>
        /// <param name="keyValueBuilder">String builder which can build key values.</param>
        /// <returns>Key values for the key.</returns>
        public static IEnumerable<string> GetKeyValues(IEnumerable<IField> keyFields, IEnumerable<IEnumerable<IDataObjectBase>> data, bool excludeNulls, StringBuilder keyValueBuilder = null)
        {
            if (keyFields == null)
            {
                throw new ArgumentNullException("keyFields");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (keyValueBuilder == null)
            {
                keyValueBuilder = new StringBuilder();
            }
            var getFields = new List<IField>(keyFields);
            try
            {
                var keyValues = new Collection<string>();
                foreach (var dataRow in data)
                {
                    var dataObjects = new List<IDataObjectBase>(dataRow);
                    try
                    {
                        keyValueBuilder.Clear();
                        foreach (var field in getFields)
                        {
                            var dataObject = GetDataObject(dataObjects, field);
                            var dataObjectType = dataObject.Field.DatatypeOfTarget;
                            IMap mapper = null;
                            if (excludeNulls && field.Equals(getFields.Last()))
                            {
                                if (dataObjectType == typeof (string))
                                {
                                    mapper = new StaticMap<string, string>();
                                    ((IStaticMap<string, string>) mapper).AddRule(string.Empty, null);
                                }
                                if (dataObjectType.IsGenericType && dataObjectType.GetGenericTypeDefinition() == typeof (Nullable<>))
                                {
                                    mapper = (IMap) Activator.CreateInstance(typeof (StaticMap<,>).MakeGenericType(new[] {dataObjectType, dataObjectType}));
                                    var addRuleMethod = mapper.GetType().GetMethod("AddRule", new[] {dataObjectType, dataObjectType});
                                    if (dataObjectType == typeof (long?))
                                    {
                                        const long defaultValue = 0;
                                        addRuleMethod.Invoke(mapper, new object[] {defaultValue, null});
                                    }
                                    else if (dataObjectType == typeof (int?))
                                    {
                                        const int defaultValue = 0;
                                        addRuleMethod.Invoke(mapper, new object[] {defaultValue, null});
                                    }
                                    else
                                    {
                                        throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.DataTypeNotSupported, dataObjectType.Name));
                                    }
                                }
                            }
                            var getValueMethod = dataObject.GetType()
                                                           .GetMethod("GetTargetValue", new[] {typeof (IMap)})
                                                           .MakeGenericMethod(new[] {dataObjectType});
                            if (keyValueBuilder.Length > 0)
                            {
                                keyValueBuilder.Append('|');
                            }
                            try
                            {
                                var value = getValueMethod.Invoke(dataObject, new object[] {mapper});
                                if (Equals(value, null))
                                {
                                    keyValueBuilder.Append("{null}");
                                    continue;
                                }
                                keyValueBuilder.Append(value);
                            }
                            catch (TargetInvocationException ex)
                            {
                                var deliveryEngineException = ex.InnerException as DeliveryEngineExceptionBase;
                                if (deliveryEngineException != null)
                                {
                                    throw ex.InnerException;
                                }
                                 throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.UnableToGetValueForField, dataObject.Field.NameTarget, dataObject.Field.Table.NameTarget, ex.InnerException.Message), ex.InnerException);
                            }
                        }
                        var keyValue = (string) keyValueBuilder.ToString().Clone();
                        if (excludeNulls == false || keyValue.EndsWith("{null}") == false)
                        {
                            keyValues.Add(keyValue);
                        }
                    }
                    finally
                    {
                        while (dataObjects.Count > 0)
                        {
                            dataObjects.Clear();
                        }
                    }
                }
                return keyValues;
            }
            finally
            {
                while (getFields.Count > 0)
                {
                    getFields.Clear();
                }
            }
        }

        /// <summary>
        /// Get key values for a given key.
        /// </summary>
        /// <param name="key">Key for which to get key values.</param>
        /// <param name="data">Data from which to get key values.</param>
        /// <param name="excludeNulls">Indicates whether to exclude key values where the last field value is null.</param>
        /// <param name="keyValueBuilder">String builder which can build key values.</param>
        /// <returns>Key values for the key.</returns>
        public static IEnumerable<string> GetKeyValues(IKey key, IEnumerable<IEnumerable<IDataObjectBase>> data, bool excludeNulls, StringBuilder keyValueBuilder = null)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            return GetKeyValues(key.Fields.Select(m => m.Key).ToArray(), data, excludeNulls, keyValueBuilder ?? new StringBuilder());
        }

        /// <summary>
        /// Get key values for a given key in a given table.
        /// </summary>
        /// <param name="dataRepository">Data repository which can read data.</param>
        /// <param name="table">Table from where to get the key values.</param>
        /// <param name="keyFieldNames">Field names for the fields in the key.</param>
        /// <param name="excludeNulls">Indicates whether to exclude key values where the last field value is null.</param>
        /// <returns>Key values for the key.</returns>
        public static IEnumerable<string> GetKeyValues(IDataRepository dataRepository, ITable table, IEnumerable<string> keyFieldNames, bool excludeNulls)
        {
            if (dataRepository == null)
            {
                throw new ArgumentNullException("dataRepository");
            }
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            if (keyFieldNames == null)
            {
                throw new ArgumentNullException("keyFieldNames");
            }
            var keyFields = keyFieldNames.Select(keyFieldName => GetTableField(table, keyFieldName)).ToArray();
            var keyValueBuilder = new StringBuilder();
            var keyValues = new List<string>();
            var clonedDataRepository = (IDataRepository) dataRepository.Clone();
            clonedDataRepository.OnHandleData += (sender, eventArgs) =>
                {
                    if (sender == null)
                    {
                        throw new ArgumentNullException("sender");
                    }
                    if (eventArgs == null)
                    {
                        throw new ArgumentNullException("eventArgs");
                    }
                    keyValues.AddRange(GetKeyValues(keyFields, eventArgs.Data, excludeNulls, keyValueBuilder));
                };
            clonedDataRepository.DataGetFromTable(table);
            return keyValues;
        }

        /// <summary>
        /// Get key values for a given key.
        /// </summary>
        /// <param name="dataRepository">Data repository which can read data.</param>
        /// <param name="key">Key from which to get key values.</param>
        /// <param name="excludeNulls">Indicates whether to exclude key values where the last field value is null.</param>
        /// <returns>Key values for the key.</returns>
        public static IEnumerable<string> GetKeyValues(IDataRepository dataRepository, IKey key, bool excludeNulls)
        {
            if (dataRepository == null)
            {
                throw new ArgumentNullException("dataRepository");
            }
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            var keyValueBuilder = new StringBuilder();
            var keyValues = new List<string>();
            var clonedDataRepository = (IDataRepository)dataRepository.Clone();
            clonedDataRepository.OnHandleData += (sender, eventArgs) =>
                {
                    if (sender == null)
                    {
                        throw new ArgumentNullException("sender");
                    }
                    if (eventArgs == null)
                    {
                        throw new ArgumentNullException("eventArgs");
                    }
                    keyValues.AddRange(GetKeyValues(key.Fields.Select(m => m.Key).ToArray(), eventArgs.Data, excludeNulls, keyValueBuilder));
                };
            clonedDataRepository.DataGetFromTable(key.Table);
            return keyValues;
        }

        /// <summary>
        /// Gets all data rows which match a given key value.
        /// </summary>
        /// <param name="keyFields">Fields in the key on which the data rows should match.</param>
        /// <param name="data">Data from which to get data rows which match the key value.</param>
        /// <param name="matchingKeyValue">The key value which the data rows should match.</param>
        /// <returns>Data rows which match the key value.</returns>
        public static IEnumerable<IEnumerable<IDataObjectBase>> GetDataRowsForKeyValue(IEnumerable<IField> keyFields, IEnumerable<IEnumerable<IDataObjectBase>> data, string matchingKeyValue)
        {
            if (keyFields == null)
            {
                throw new ArgumentNullException("keyFields");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (string.IsNullOrEmpty(matchingKeyValue))
            {
                throw new ArgumentNullException("matchingKeyValue");
            }
            var dataRows = data as IList<IEnumerable<IDataObjectBase>> ?? new List<IEnumerable<IDataObjectBase>>(data);
            var keyValues = new List<string>(GetKeyValues(keyFields, dataRows, false));
            try
            {
                var keyValueComparer = new KeyValueComparer();
                var matchingDataRows = new List<IEnumerable<IDataObjectBase>>();
                for (var keyValueNo = 0; keyValueNo < keyValues.Count; keyValueNo++)
                {
                    if (keyValueComparer.Equals(keyValues.ElementAt(keyValueNo), matchingKeyValue))
                    {
                        matchingDataRows.Add(dataRows.ElementAt(keyValueNo));
                    }
                }
                return matchingDataRows;
            }
            finally
            {
                while (keyValues.Count > 0)
                {
                    keyValues.Clear();
                }
            }
        }

        /// <summary>
        /// Gets all data rows which match a given key value.
        /// </summary>
        /// <param name="key">Key on which the data rows should match.</param>
        /// <param name="data">Data from which to get data rows which match the key value.</param>
        /// <param name="matchingKeyValue">The key value which the data rows should match.</param>
        /// <returns>Data rows which match the key value.</returns>
        public static IEnumerable<IEnumerable<IDataObjectBase>> GetDataRowsForKeyValue(IKey key, IEnumerable<IEnumerable<IDataObjectBase>> data, string matchingKeyValue)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (string.IsNullOrEmpty(matchingKeyValue))
            {
                throw new ArgumentNullException("matchingKeyValue");
            }
            return GetDataRowsForKeyValue(key.Fields.Select(m => m.Key).ToArray(), data, matchingKeyValue);
        }

        /// <summary>
        /// Get a given field in a given table.
        /// </summary>
        /// <param name="table">Table from where to get the field.</param>
        /// <param name="fieldName">Name of the field to get.</param>
        /// <returns>Field.</returns>
        public static IField GetTableField(ITable table, string fieldName)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException("fieldName");
            }
            IField field;
            try
            {
                field = table.Fields.Single(m => string.Compare(m.NameSource, fieldName, StringComparison.OrdinalIgnoreCase) == 0);
            }
            catch (InvalidOperationException)
            {
                field = table.Fields.SingleOrDefault(m => string.Compare(m.NameTarget, fieldName, StringComparison.OrdinalIgnoreCase) == 0);
            }
            if (field == null)
            {
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.FieldNotFoundOnTable, fieldName, table.NameTarget));
            }
            return field;
        }

        /// <summary>
        /// Gets the data object for a given field in a given data row.
        /// </summary>
        /// <param name="dataObjects">Data objects in the data row.</param>
        /// <param name="fieldName">Name of the field for which to get the data object.</param>
        /// <returns>Data object for the field.</returns>
        public static IDataObjectBase GetDataObject(IList<IDataObjectBase> dataObjects, string fieldName)
        {
            if (dataObjects == null)
            {
                throw new ArgumentNullException("dataObjects");
            }
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException("fieldName");
            }
            IDataObjectBase dataObject;
            try
            {
                dataObject = dataObjects.Single(m => m.Field != null && string.Compare(m.Field.NameSource, fieldName, StringComparison.OrdinalIgnoreCase) == 0);
            }
            catch (InvalidOperationException)
            {
                dataObject = dataObjects.SingleOrDefault(m => m.Field != null && string.Compare(m.Field.NameTarget, fieldName, StringComparison.OrdinalIgnoreCase) == 0);
            }
            if (dataObject == null)
            {
               throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.FieldNotFound, fieldName));
            }
            return dataObject;
        }

        /// <summary>
        /// Gets the data object for a given field in a given data row.
        /// </summary>
        /// <param name="dataObjects">Data objects in the data row.</param>
        /// <param name="field">Field for which to get the data object.</param>
        /// <returns>Data object for the field.</returns>
        public static IDataObjectBase GetDataObject(IList<IDataObjectBase> dataObjects, IField field)
        {
            if (dataObjects == null)
            {
                throw new ArgumentNullException("dataObjects");
            }
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }
            var dataObject = dataObjects.SingleOrDefault(m => m.Field != null && (m.Field.Equals(field) || (string.IsNullOrEmpty(field.NameSource) == false && string.Compare(field.NameSource, m.Field.NameSource, StringComparison.OrdinalIgnoreCase) == 0) || (string.IsNullOrEmpty(field.NameTarget) == false && string.Compare(field.NameTarget, m.Field.NameTarget, StringComparison.OrdinalIgnoreCase) == 0)));
            if (dataObject == null)
            {
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.FieldNotFound, field.NameTarget));
            }
            return dataObject;
        }

        /// <summary>
        /// Gets the source value for a given data object.
        /// </summary>
        /// <param name="dataObject">Data object on which to get the source value.</param>
        /// <returns>Source value.</returns>
        public static object GetSourceValue(IDataObjectBase dataObject)
        {
            if (dataObject == null)
            {
                throw new ArgumentNullException("dataObject");
            }
            var sourceValueType = dataObject.Field.DatatypeOfSource;
            try
            {
                return dataObject.GetType()
                                 .GetMethod("GetSourceValue")
                                 .MakeGenericMethod(new[] {sourceValueType})
                                 .Invoke(dataObject, null);
            }
            catch (TargetInvocationException ex)
            {
                var deliveryEngineException = ex.InnerException as DeliveryEngineExceptionBase;
                if (deliveryEngineException != null)
                {
                    throw ex.InnerException;
                }
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.UnableToGetValueForField, dataObject.Field.NameTarget, dataObject.Field.Table.NameTarget, ex.InnerException.Message), ex.InnerException);
            }
        }

        /// <summary>
        /// Updates the source value on a given data object.
        /// </summary>
        /// <param name="dataObject">Data object on which to update the source value.</param>
        /// <param name="newSourceValue">New source value.</param>
        public static void UpdateSourceValue(IDataObjectBase dataObject, object newSourceValue)
        {
            if (dataObject == null)
            {
                throw new ArgumentNullException("dataObject");
            }
            var valueType = newSourceValue == null ? typeof (object) : newSourceValue.GetType();
            var updateSourceValueMethod = dataObject.GetType()
                                                    .GetMethod("UpdateSourceValue")
                                                    .MakeGenericMethod(new[] {valueType});
            try
            {
                updateSourceValueMethod.Invoke(dataObject, new[] {newSourceValue});
            }
            catch (TargetInvocationException ex)
            {
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.InvokeError, dataObject.GetType().Name, updateSourceValueMethod.Name, ex.InnerException.Message), ex);
            }
        }

        /// <summary>
        /// Builds an equal criteria on a given field.
        /// </summary>
        /// <param name="field">Field on which to build the criteria.</param>
        /// <param name="eqaulTo">Value for the criteria.</param>
        /// <returns>Equal criteria to the field.</returns>
        public static IEqualCriteria BuildEqualCriteria(IField field, object eqaulTo)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }
            if (Equals(eqaulTo, null))
            {
                throw new ArgumentNullException("eqaulTo");
            }
            var sourceValueType = field.DatatypeOfSource;
            try
            {
                if (sourceValueType.IsGenericType && sourceValueType.GetGenericTypeDefinition() == typeof (Nullable<>))
                {
                    var genericType = sourceValueType.GetGenericArguments().ElementAt(0);
                    if (genericType.IsValueType || genericType == typeof (string))
                    {
                        return Activator.CreateInstance(typeof (EqualCriteria<>).MakeGenericType(genericType), new[] {field, ConverTo(genericType, eqaulTo)}) as IEqualCriteria;
                    }
                    throw new NotSupportedException(genericType.Name);
                }
                if (sourceValueType.IsValueType || sourceValueType == typeof (string))
                {
                    return Activator.CreateInstance(typeof (EqualCriteria<>).MakeGenericType(sourceValueType), new[] {field, ConverTo(sourceValueType, eqaulTo)}) as IEqualCriteria;
                }
                throw new NotSupportedException(sourceValueType.Name);
            }
            catch (TargetInvocationException ex)
            {
                var deliveryEngineException = ex.InnerException as DeliveryEngineExceptionBase;
                if (deliveryEngineException != null)
                {
                    throw ex.InnerException;
                }
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.UnableToCreateCriteria, typeof (EqualCriteria<>).MakeGenericType(new[] {sourceValueType}), ex.InnerException.Message), ex.InnerException);
            }
            catch (NotSupportedException ex)
            {
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.UnableToCreateCriteria, typeof (EqualCriteria<>).MakeGenericType(new[] {sourceValueType}), ex.Message), ex);
            }
        }

        /// <summary>
        /// Converts a given value to a given type.
        /// </summary>
        /// <param name="type">Type which the value should be converted to.</param>
        /// <param name="value">Value to be converted.</param>
        /// <returns>Converted value.</returns>
        public static object ConverTo(Type type, object value)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (value == null || value.GetType() == type)
            {
                return value;
            }
            if (type == typeof (string))
            {
                var toStringMethod = value.GetType().GetMethod("ToString", new Type[] {});
                if (toStringMethod == null)
                {
                    throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.MethodNotFoundOnType, "ToString", value.GetType()));
                }
                try
                {
                    return toStringMethod.Invoke(value, null);
                }
                catch (TargetInvocationException ex)
                {
                    throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.InvokeError, value.GetType().Name, toStringMethod.Name, ex.InnerException.Message), ex);
                }
            }
            var parseMethod = type.GetMethod("Parse", new[] {value.GetType()});
            if (parseMethod == null)
            {
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.MethodNotFoundOnType, "Parse", type.Name));
            }
            try
            {
                return parseMethod.Invoke(type, new[] {value});
            }
            catch (TargetInvocationException ex)
            {
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.ParseError, type.Name, value, ex.InnerException.Message), ex);
            }
        }
    }
}
