using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DsiNext.DeliveryEngine.BusinessLogic.Events;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Commands;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.DataValidators;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Comparers;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Helpers;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.BusinessLogic.DataValidators
{
    /// <summary>
    /// Validator for foreign keys on data for a target table.
    /// </summary>
    public class ForeignKeysDataValidator : DataValidatorBase<IForeignKeysValidationCommand>, IForeignKeysDataValidator
    {
        #region Private variables

        private bool _disposed;
        private readonly IDataRepository _dataRepository;
        private readonly IList<string> _isCaching = new List<string>();
        private readonly IDictionary<string, List<string>> _dataCache = new Dictionary<string, List<string>>();
        private readonly object _syncRoot = new object();

        #endregion

        #region Constructor

        /// <summary>
        /// Creates validator for foreign keys on data for a target table.
        /// </summary>
        /// <param name="dataRepository">Repository containing functionality to get data from the source.</param>
        public ForeignKeysDataValidator(IDataRepository dataRepository)
        {
            if (dataRepository == null)
            {
                throw new ArgumentNullException("dataRepository");
            }
            _dataRepository = dataRepository;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Repository containing functionality to get data from the source.
        /// </summary>
        public virtual IDataRepository DataRepository
        {
            get
            {
                return _dataRepository;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Destructor.
        /// </summary>
        ~ForeignKeysDataValidator()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose managed and unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose managed and unmanaged resources.
        /// </summary>
        /// <param name="disposing">Indicates whether all managed resources should be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            // Dispose all unmanaged resources.
            lock (_syncRoot)
            {
                while (_isCaching.Count > 0)
                {
                    _isCaching.Clear();
                }
                while (_dataCache.Count > 0)
                {
                    while (_dataCache.ElementAt(0).Value.Count > 0)
                    {
                        _dataCache.ElementAt(0).Value.Clear();
                    }
                    _dataCache.Remove(_dataCache.ElementAt(0).Key);
                }
            }
            // Dispose all managed resources.
            if (disposing)
            {
            }
            _disposed = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Validates foreign keys on data for a target table.
        /// </summary>
        /// <param name="targetTable">Target table.</param>
        /// <param name="targetTableData">Data for the target table.</param>
        /// <param name="endOfData">Indicates whether this is the last data for the target table.</param>
        /// <param name="command">Command which to validate with.</param>
        protected override void ValidateData(ITable targetTable, IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>> targetTableData, bool endOfData, IForeignKeysValidationCommand command)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            foreach (var dataTable in targetTableData.Select(m => m.Key).Where(m => m.ForeignKeys != null && m.ForeignKeys.Count > 0))
            {
                foreach (var foreignKey in dataTable.ForeignKeys)
                {
                    if (foreignKey.CandidateKey == null)
                    {
                        throw new DeliveryEngineMetadataException(Resource.GetExceptionMessage(ExceptionMessage.MissingCandidateKeyOnForeignKey, foreignKey.NameSource, dataTable.NameSource), foreignKey);
                    }
                    if (foreignKey.CandidateKey.Fields == null || foreignKey.Fields == null || foreignKey.CandidateKey.Fields.Count != foreignKey.Fields.Count)
                    {
                        throw new DeliveryEngineMetadataException(Resource.GetExceptionMessage(ExceptionMessage.UnableToMatchFieldsOnForeignKey, foreignKey.CandidateKey.NameSource, foreignKey.NameSource, dataTable.NameSource), foreignKey);
                    }
                    var keyValueComparer = new KeyValueComparer();
                    var foreignKeyValues = new List<string>(DataRepositoryHelper.GetKeyValues(foreignKey, targetTableData[dataTable], true).Distinct(keyValueComparer));
                    if (foreignKeyValues.Count == 0)
                    {
                        continue;
                    }
                    try
                    {
                        var onRemove = new Action<ITable, List<IEnumerable<IDataObjectBase>>>((table, dataRowsToRemove) =>
                            {
                                var removeMethod = targetTableData[table].GetType().GetMethod("Remove");
                                if (removeMethod == null)
                                {
                                    throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.MethodNotFoundOnType, "Remove", targetTableData[table].GetType().Name));
                                }
                                dataRowsToRemove.ForEach(dataRow => removeMethod.Invoke(targetTableData[table], new object[] {dataRow}));
                            });
                        // Try to validate using a data queryer.
                        var dataQueryer = GetDataQueryer(DataRepository);
                        if (dataQueryer != null)
                        {
                            using (dataQueryer)
                            {
                                var keyValuesFromCache = new List<string>();
                                try
                                {
                                    var dictionaryName = GetDictionaryName(foreignKey);
                                    lock (_syncRoot)
                                    {
                                        List<string> dataInCache;
                                        if (_dataCache.TryGetValue(dictionaryName, out dataInCache))
                                        {
                                            while (_dataCache.ContainsKey(dictionaryName))
                                            {
                                                _dataCache.Remove(dictionaryName);
                                            }
                                            _dataCache.Add(dictionaryName, dataInCache);
                                        }
                                        else
                                        {
                                            _dataCache.Add(dictionaryName, new List<string>());
                                        }
                                        while (_dataCache.Count > 0 && _dataCache.Count >= command.NumberOfForeignTablesToCache && Equals(_dataCache.ElementAt(0).Key, dictionaryName) == false)
                                        {
                                            var keyName = _dataCache.ElementAt(0).Key;
                                            while (_dataCache[keyName].Count > 0)
                                            {
                                                _dataCache[keyName].Clear();
                                            }
                                            _dataCache.Remove(keyName);
                                        }
                                        keyValuesFromCache.AddRange(_dataCache[dictionaryName]);
                                    }
                                    while (foreignKeyValues.Count > 0)
                                    {
                                        var dataRows = new List<IEnumerable<IDataObjectBase>>(DataRepositoryHelper.GetDataRowsForKeyValue(foreignKey, targetTableData[dataTable], foreignKeyValues.ElementAt(0)));
                                        try
                                        {
                                            dataRows.ForEach(dataRow => RaiseOnValidationEvent(this, new DataValidatorEventArgs(dataRow)));
                                            if (keyValuesFromCache.Any(m => keyValueComparer.Equals(m, foreignKeyValues.ElementAt(0))))
                                            {
                                                foreignKeyValues.RemoveAt(0);
                                                continue;
                                            }
                                            var extraCriterias = new List<KeyValuePair<string, object>>();
                                            try
                                            {
                                                for (var keyFieldNo = 0; keyFieldNo < Math.Min(foreignKey.Fields.Count, foreignKey.CandidateKey.Fields.Count); keyFieldNo++)
                                                {
                                                    var dataObject = DataRepositoryHelper.GetDataObject(dataRows[0].ToList(), foreignKey.Fields.ElementAt(keyFieldNo).Key);
                                                    var sourceValue = DataRepositoryHelper.GetSourceValue(dataObject);
                                                    if (Equals(sourceValue, null))
                                                    {
                                                        continue;
                                                    }
                                                    var sourceName = foreignKey.CandidateKey.Fields.ElementAt(keyFieldNo).Key.NameSource;
                                                    var targetName = foreignKey.CandidateKey.Fields.ElementAt(keyFieldNo).Key.NameTarget;
                                                    var foreignKeyFieldName = foreignKey.CandidateKey.Table.Fields.Count(m => string.Compare(m.NameSource, sourceName, StringComparison.OrdinalIgnoreCase) == 0) > 1 ? targetName : sourceName;
                                                    extraCriterias.Add(new KeyValuePair<string, object>(foreignKeyFieldName, sourceValue));
                                                }
                                                var equalPrimaryKeys = dataQueryer.GetNumberOfEqualKeyValues(foreignKey.CandidateKey, extraCriterias, foreignKeyValues.ElementAt(0));
                                                HandleEqualPrimaryKeysResult(equalPrimaryKeys, dataTable, foreignKey, dataRows, command, onRemove);
                                                if (equalPrimaryKeys == 1)
                                                {
                                                    lock (_syncRoot)
                                                    {
                                                        if (_dataCache.ContainsKey(dictionaryName) == false)
                                                        {
                                                            foreignKeyValues.RemoveAt(0);
                                                            continue;
                                                        }
                                                        while (_dataCache.Select(m => m.Value).Sum(m => m.Count) >= 8192)
                                                        {
                                                            _dataCache.Select(m => m.Value).Where(m => m.Count > 0).OrderByDescending(m => m.Count).First().RemoveAt(0);
                                                        }
                                                        _dataCache[dictionaryName].Add(foreignKeyValues.ElementAt(0));
                                                    }
                                                }
                                            }
                                            finally
                                            {
                                                while (extraCriterias.Count > 0)
                                                {
                                                    extraCriterias.Clear();
                                                }
                                            }
                                        }
                                        finally
                                        {
                                            while (dataRows.Count > 0)
                                            {
                                                dataRows.Clear();
                                            }
                                        }
                                        foreignKeyValues.RemoveAt(0);
                                    }
                                }
                                finally
                                {
                                    while (keyValuesFromCache.Count > 0)
                                    {
                                        keyValuesFromCache.Clear();
                                    }
                                }
                                dataQueryer.Dispose();
                            }
                            continue;
                        }
                        // Validate without a data queryer.
                        var primaryKeyValues = GetForeignKeyValues(foreignKey.CandidateKey, command.NumberOfForeignTablesToCache);
                        while (foreignKeyValues.Count > 0)
                        {
                            var dataRows = new List<IEnumerable<IDataObjectBase>>(DataRepositoryHelper.GetDataRowsForKeyValue(foreignKey, targetTableData[dataTable], foreignKeyValues.ElementAt(0)));
                            try
                            {
                                dataRows.ForEach(dataRow => RaiseOnValidationEvent(this, new DataValidatorEventArgs(dataRow)));
                                var equalPrimaryKeys = primaryKeyValues.Count(m => keyValueComparer.Equals(foreignKeyValues.ElementAt(0), m));
                                HandleEqualPrimaryKeysResult(equalPrimaryKeys, dataTable, foreignKey, dataRows, command, onRemove);
                            }
                            finally
                            {
                                while (dataRows.Count > 0)
                                {
                                    dataRows.Clear();
                                }
                            }
                            foreignKeyValues.RemoveAt(0);
                        }
                    }
                    finally
                    {
                        while (foreignKeyValues.Count > 0)
                        {
                            foreignKeyValues.Clear();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets key values for a given foreign key.
        /// </summary>
        /// <param name="foreignKey">Foreign key which to get key values for.</param>
        /// <param name="numberOfForeignKeysToCache">Number of foreign keys to be cached.</param>
        /// <returns>Key values for the given foreign key.</returns>
        private List<string> GetForeignKeyValues(IKey foreignKey, int numberOfForeignKeysToCache)
        {
            if (foreignKey == null)
            {
                throw new ArgumentNullException("foreignKey");
            }
            List<string> keyValues;
            var dictonaryName = GetDictionaryName(foreignKey);
            // Wait if the foreign key validator already is caching the dictionary name.
            bool isCaching;
            lock (_syncRoot)
            {
                isCaching = _isCaching.Contains(dictonaryName);
            }
            while (isCaching)
            {
                Thread.Sleep(250);
                lock (_syncRoot)
                {
                    isCaching = _isCaching.Contains(dictonaryName);
                }
            }
            // Try to read the key values from the cache.
            lock (_syncRoot)
            {
                if (_dataCache.TryGetValue(dictonaryName, out keyValues))
                {
                    while (_dataCache.ContainsKey(dictonaryName))
                    {
                        _dataCache.Remove(dictonaryName);
                    }
                    _dataCache.Add(dictonaryName, keyValues);
                    return _dataCache[dictonaryName];
                }
                while (_dataCache.Count > 0 && _dataCache.Count >= numberOfForeignKeysToCache)
                {
                    var removeDictonaryName = _dataCache.ElementAt(0).Key;
                    while (_dataCache[removeDictonaryName].Count > 0)
                    {
                        _dataCache[removeDictonaryName].Clear();
                    }
                    _dataCache.Remove(removeDictonaryName);
                }
                _isCaching.Add(dictonaryName);
            }
            // Read and cache the key values.
            try
            {
                keyValues = new List<string>(DataRepositoryHelper.GetKeyValues(DataRepository, foreignKey, false));
                lock (_syncRoot)
                {
                    while (_dataCache.ContainsKey(dictonaryName))
                    {
                        while (_dataCache[dictonaryName].Count > 0)
                        {
                            _dataCache[dictonaryName].Clear();
                        }
                        _dataCache.Remove(dictonaryName);
                    }
                    _dataCache.Add(dictonaryName, keyValues);
                    return _dataCache[dictonaryName];
                }
            }
            finally
            {
                lock (_syncRoot)
                {
                    while (_isCaching.Contains(dictonaryName))
                    {
                        _isCaching.Remove(dictonaryName);
                    }
                }
            }
        }

        /// <summary>
        /// Handle the result for en equal number of primary keys.
        /// </summary>
        /// <param name="equalPrimaryKeys">The number of equal primary keys.</param>
        /// <param name="dataTable">Data table on which the data rows has been validated.</param>
        /// <param name="foreignKey">Foreign key on which the data rows has been validated.</param>
        /// <param name="dataRows">Data rows for the result.</param>
        /// <param name="command">Command which to validate with.</param>
        /// <param name="onRemove">Callback metode which can remove the data rows from the archive.</param>
        private static void HandleEqualPrimaryKeysResult(int equalPrimaryKeys, ITable dataTable, IKey foreignKey, List<IEnumerable<IDataObjectBase>> dataRows, IForeignKeysValidationCommand command, Action<ITable, List<IEnumerable<IDataObjectBase>>> onRemove)
        {
            if (dataTable == null)
            {
                throw new ArgumentNullException("dataTable");
            }
            if (foreignKey == null)
            {
                throw new ArgumentNullException("foreignKey");
            }
            if (dataRows == null)
            {
                throw new ArgumentNullException("dataRows");
            }
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            if (onRemove == null)
            {
                throw new ArgumentNullException("onRemove");
            }
            if (equalPrimaryKeys == 0)
            {
                if (command.RemoveMissingRelationshipsOnForeignKeys)
                {
                    onRemove(dataTable, dataRows);
                    return;
                }
                foreignKey.ValidateObjectData = new List<IEnumerable<IDataObjectBase>>(dataRows);
                throw new DeliveryEngineValidateException(Resource.GetExceptionMessage(ExceptionMessage.UnableToFindForeignKeyRelationship, GetForeignKeyName(foreignKey), dataTable.NameSource, GetKeyValues(dataTable.PrimaryKey, foreignKey, dataRows.ElementAt(0))), foreignKey);
            }
            if (equalPrimaryKeys == 1)
            {
                return;
            }
            foreignKey.ValidateObjectData = new List<IEnumerable<IDataObjectBase>>(dataRows);
            throw new DeliveryEngineValidateException(Resource.GetExceptionMessage(ExceptionMessage.TooManyForeignKeyRelationships, GetForeignKeyName(foreignKey), dataTable.NameSource, GetKeyValues(dataTable.PrimaryKey, foreignKey, dataRows.ElementAt(0))), foreignKey);
        }

        /// <summary>
        /// Gets the name information for a given foreign key.
        /// </summary>
        /// <param name="foreignKey">Foreign key.</param>
        /// <returns>Name information for the foreign key.</returns>
        private static string GetForeignKeyName(IKey foreignKey)
        {
            if (foreignKey == null)
            {
                throw new ArgumentNullException("foreignKey");
            }
            var stringBuilder = new StringBuilder(string.Format("{0}(", foreignKey.NameSource));
            var foreignKeyFields = foreignKey.Fields.Select(m => m.Key).ToList();
            foreignKeyFields.Aggregate(stringBuilder, (current, foreignKeyField) =>
                {
                    if (foreignKeyField.Equals(foreignKeyFields.Last()))
                    {
                        return current.Append(foreignKeyField.NameSource);
                    }
                    return current.Append(string.Format("{0}, ", foreignKeyField.NameSource));
                });
            stringBuilder.Append(')');
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Gets the values for data objects for a given foreign key.
        /// </summary>
        /// <param name="primaryKey">Primary key for the table.</param>
        /// <param name="foreignKey">Foreign key.</param>
        /// <param name="dataObjects">Data objects.</param>
        /// <returns>String representation of data objects in the foreign key.</returns>
        private static string GetKeyValues(IKey primaryKey, IKey foreignKey, IEnumerable<IDataObjectBase> dataObjects)
        {
            if (primaryKey == null)
            {
                throw new ArgumentNullException("primaryKey");
            }
            if (foreignKey == null)
            {
                throw new ArgumentNullException("foreignKey");
            }
            if (dataObjects == null)
            {
                throw new ArgumentNullException("dataObjects");
            }
            var stringBuilder = new StringBuilder();
            var keyFields = new List<KeyValuePair<IField, IMap>>(primaryKey.Fields);
            foreach (var foreignKeyField in foreignKey.Fields)
            {
                if (keyFields.Contains(foreignKeyField))
                {
                    continue;
                }
                keyFields.Add(foreignKeyField);
            }
            keyFields.Aggregate(stringBuilder, (current, keyField) =>
                {
                    var dataObject = DataRepositoryHelper.GetDataObject(dataObjects.ToList(), keyField.Key);
                    var targetValueType = dataObject.Field.DatatypeOfTarget;
                    var targetValueMethod = dataObject.GetType().GetMethod("GetTargetValue", new[] {typeof (IMap)}).MakeGenericMethod(new[] {targetValueType});
                    var targetValue = targetValueMethod.Invoke(dataObject, new object[] {keyField.Value});
                    if (keyField.Equals(keyFields.Last()))
                    {
                        return current.Append(string.Format("{0}={1}", dataObject.Field.NameSource, Equals(targetValue, null) ? "{null}" : targetValue));
                    }
                    return current.Append(string.Format("{0}={1}, ", dataObject.Field.NameSource,Equals(targetValue, null) ? "{null}" : targetValue));
                });
            return stringBuilder.ToString();
        }

        #endregion
    }
}
