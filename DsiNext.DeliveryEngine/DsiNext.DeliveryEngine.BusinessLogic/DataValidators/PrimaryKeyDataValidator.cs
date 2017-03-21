using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Validator for primary key on data for a target table.
    /// </summary>
    public class PrimaryKeyDataValidator : DataValidatorBase<ICommand>, IPrimaryKeyDataValidator
    {
        #region Private variables

        private bool _disposed;
        private readonly IDataRepository _dataRepository;
        private readonly IDictionary<string, List<string>> _dataCache = new Dictionary<string, List<string>>();
        private readonly object _syncRoot = new object();

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a validator for primary key on data for a target table.
        /// </summary>
        /// <param name="dataRepository">Repository containing functionality to get data from the source.</param>
        public PrimaryKeyDataValidator(IDataRepository dataRepository)
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
        ~PrimaryKeyDataValidator()
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
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            // Dispose all unmanaged resources.
            lock (_syncRoot)
            {
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
        /// Validates primary key on data for a target table.
        /// </summary>
        /// <param name="targetTable">Target table.</param>
        /// <param name="targetTableData">Data for the target table.</param>
        /// <param name="endOfData">Indicates whether this is the last data for the target table.</param>
        /// <param name="command">Command which to validate with.</param>
        protected override void ValidateData(ITable targetTable, IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>> targetTableData, bool endOfData, ICommand command)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            foreach (var dataTable in targetTableData.Keys)
            {
                if (dataTable.CandidateKeys == null || dataTable.CandidateKeys.Count == 0)
                {
                    throw new DeliveryEngineMetadataException(Resource.GetExceptionMessage(ExceptionMessage.MissingCandidateKeysOnTable, dataTable.NameSource), dataTable);
                }
                foreach (var candidateKey in dataTable.CandidateKeys)
                {
                    if (candidateKey.Fields == null || candidateKey.Fields.Count == 0)
                    {
                        throw new DeliveryEngineMetadataException(Resource.GetExceptionMessage(ExceptionMessage.MissingFieldsOnCandidateKey, candidateKey.NameSource, dataTable.NameSource), candidateKey);
                    }
                    var keyValueComparer = new KeyValueComparer();
                    List<string> primaryKeyValues;
                    // Try to validate using a data queryer.
                    var dataQueryer = GetDataQueryer(DataRepository);
                    if (dataQueryer != null)
                    {
                        using (dataQueryer)
                        {
                            primaryKeyValues = new List<string>(DataRepositoryHelper.GetKeyValues(candidateKey, targetTableData[dataTable], false));
                            try
                            {
                                for (var primaryKeyValueNo = 0; primaryKeyValueNo < primaryKeyValues.Count; primaryKeyValueNo++)
                                {
                                    var dataRow = targetTableData[dataTable].ElementAt(primaryKeyValueNo);
                                    RaiseOnValidationEvent(this, new DataValidatorEventArgs(dataRow));
                                    if (endOfData)
                                    {
                                        if (primaryKeyValues.Count(m => keyValueComparer.Equals(m, primaryKeyValues.ElementAt(primaryKeyValueNo))) == 1)
                                        {
                                            continue;
                                        }
                                        candidateKey.ValidateObjectData = dataRow;
                                        throw new DeliveryEngineValidateException(Resource.GetExceptionMessage(ExceptionMessage.UniqueConstraintViolationOnCandidateKey, candidateKey.NameSource), candidateKey);
                                    }
                                    var extraCriterias = GetExtraCriterias(candidateKey, new List<IDataObjectBase>(dataRow));
                                    try
                                    {
                                        if (dataQueryer.GetNumberOfEqualKeyValues(candidateKey, extraCriterias, primaryKeyValues.ElementAt(primaryKeyValueNo)) == 1)
                                        {
                                            continue;
                                        }
                                        candidateKey.ValidateObjectData = dataRow;
                                        throw new DeliveryEngineValidateException(Resource.GetExceptionMessage(ExceptionMessage.UniqueConstraintViolationOnCandidateKey, candidateKey.NameSource), candidateKey);
                                    }
                                    finally
                                    {
                                        while (extraCriterias.Count > 0)
                                        {
                                            extraCriterias.Clear();
                                        }
                                    }
                                }
                            }
                            finally
                            {
                                while (primaryKeyValues.Count > 0)
                                {
                                    primaryKeyValues.Clear();
                                }
                            }
                            dataQueryer.Dispose();
                        }
                        continue;
                    }
                    // Validate without a data queryer.
                    var dictionaryName = GetDictionaryName(candidateKey);
                    lock (_syncRoot)
                    {
                        if (_dataCache.TryGetValue(dictionaryName, out primaryKeyValues) == false)
                        {
                            primaryKeyValues = new List<string>();
                            _dataCache.Add(dictionaryName, primaryKeyValues);
                        }
                    }
                    var keyValues = new List<string>(DataRepositoryHelper.GetKeyValues(candidateKey, targetTableData[dataTable], false));
                    try
                    {
                        for (var keyValueNo = 0; keyValueNo < keyValues.Count; keyValueNo++)
                        {
                            RaiseOnValidationEvent(this, new DataValidatorEventArgs(targetTableData[dataTable].ElementAt(keyValueNo)));
                            if (primaryKeyValues.Count(m => keyValueComparer.Equals(keyValues.ElementAt(keyValueNo), m)) + keyValues.Count(m => keyValueComparer.Equals(keyValues.ElementAt(keyValueNo), m)) == 1)
                            {
                                continue;
                            }
                            candidateKey.ValidateObjectData = targetTableData[dataTable].ElementAt(keyValueNo);
                            throw new DeliveryEngineValidateException(Resource.GetExceptionMessage(ExceptionMessage.UniqueConstraintViolationOnCandidateKey, candidateKey.NameSource), candidateKey);
                        }
                        lock (_syncRoot)
                        {
                            _dataCache[dictionaryName].AddRange(keyValues);
                            if (!endOfData)
                            {
                                continue;
                            }
                            while (_dataCache[dictionaryName].Count > 0)
                            {
                                _dataCache[dictionaryName].Clear();
                            }
                            _dataCache.Remove(dictionaryName);
                        }
                    }
                    finally
                    {
                        while (keyValues.Count > 0)
                        {
                            keyValues.Clear();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the extra criterias to the data queryer.
        /// </summary>
        /// <param name="key">Key on which to build extra criterias.</param>
        /// <param name="dataObjects">Data objects used to build the criteria.</param>
        /// <returns>Extra criterias.</returns>
        private static IList<KeyValuePair<string, object>> GetExtraCriterias(IKey key, IList<IDataObjectBase> dataObjects)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            try
            {
                var tableFields = key.Table.Fields;
                var extraCriterias = key.Fields
                                        .Select(keyField => keyField.Key)
                                        .Select(keyField =>
                                            {
                                                var dataObject = DataRepositoryHelper.GetDataObject(dataObjects, keyField);
                                                var keyFieldName = tableFields.Count(m => String.Compare(m.NameSource, keyField.NameSource, StringComparison.OrdinalIgnoreCase) == 0) > 1 ? keyField.NameTarget : keyField.NameSource;
                                                return new Tuple<string, object>(keyFieldName, DataRepositoryHelper.GetSourceValue(dataObject));
                                            })
                                        .Where(extraCriteria => Equals(extraCriteria.Item2, null) == false)
                                        .Select(extraCriteria => new KeyValuePair<string, object>(extraCriteria.Item1, extraCriteria.Item2));
                return new List<KeyValuePair<string, object>>(extraCriterias);
            }
            finally
            {
                while (dataObjects.Count > 0)
                {
                    dataObjects.Clear();
                }
            }
        }

        #endregion
    }
}
