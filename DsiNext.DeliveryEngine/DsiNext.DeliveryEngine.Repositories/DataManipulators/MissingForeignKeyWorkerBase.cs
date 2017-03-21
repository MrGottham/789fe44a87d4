using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Helpers;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Repositories.DataManipulators
{
    /// <summary>
    /// Basic worker to manipulate missing foreign key values.
    /// </summary>
    public abstract class MissingForeignKeyWorkerBase : IMissingForeignKeyWorker
    {
        #region Private constants

        protected const int MaxRowsInCache = 8192;

        #endregion

        #region Private variables

        private readonly ITable _foreignTable;
        private readonly Guid _workerId = Guid.NewGuid();
        protected static readonly IDictionary<string, string> IsCaching = new Dictionary<string, string>();
        protected static readonly IDictionary<string, Tuple<List<string>, List<string>>> DataCache = new Dictionary<string, Tuple<List<string>, List<string>>>();
        protected static readonly object SyncRoot = new object();

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a basic worker to manipulate missing foreign key values.
        /// </summary>
        /// <param name="targetTableName">Target name of the table to validate against.</param>
        /// <param name="metadataRepository">Metadata repository.</param>
        protected MissingForeignKeyWorkerBase(string targetTableName, IMetadataRepository metadataRepository)
        {
            if (string.IsNullOrEmpty(targetTableName))
            {
                throw new ArgumentNullException("targetTableName");
            }
            if (metadataRepository == null)
            {
                throw new ArgumentNullException("metadataRepository");
            }
            var dataSource = metadataRepository.DataSourceGet();
            _foreignTable = dataSource.Tables.SingleOrDefault(m => string.Compare(m.NameTarget, targetTableName, StringComparison.OrdinalIgnoreCase) == 0);
            if (_foreignTable == null)
            {
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.TableNotFound, targetTableName));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Foreign table to validate against.
        /// </summary>
        public virtual ITable ForeignKeyTable
        {
            get
            {
                return _foreignTable;
            }
        }

        /// <summary>
        /// Gets an unique identification for the worker.
        /// </summary>
        protected virtual string WorkerId
        {
            get
            {
                return string.Format("{0} @ {1}", _workerId.ToString("B"), Thread.CurrentThread.ManagedThreadId);
            }
        }

        #endregion

        #region IMissingForeignKeyWorker Members

        /// <summary>
        /// Manipulates missing foreign key values for a given table.
        /// </summary>
        /// <param name="table">Table to manipulate data for missing foreign key values.</param>
        /// <param name="dataRepository">Data repository.</param>
        /// <param name="data">Data for the table where to manipulate data for missing foreign key values.</param>
        /// <returns>Manipulated data for the table.</returns>
        public IEnumerable<IEnumerable<IDataObjectBase>> ManipulateData(ITable table, IDataRepository dataRepository, IEnumerable<IEnumerable<IDataObjectBase>> data)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            if (dataRepository == null)
            {
                throw new ArgumentNullException("dataRepository");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var dataAsList = data as IList<IEnumerable<IDataObjectBase>> ?? new List<IEnumerable<IDataObjectBase>>(data);
            return Manipulate(table, dataRepository, dataAsList);
        }

        /// <summary>
        /// Finalize missing foreign key values for a given table.
        /// </summary>
        /// <param name="table">Table to finalize data manipulation for missing foreign key values.</param>
        /// <param name="dataRepository">Data repository.</param>
        /// <param name="data">The last manipulated data which has been received.</param>
        /// <returns>Finalized and manipulated data for the table.</returns>
        public IEnumerable<IEnumerable<IDataObjectBase>> FinalizeDataManipulation(ITable table, IDataRepository dataRepository, IEnumerable<IEnumerable<IDataObjectBase>> data)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            if (dataRepository == null)
            {
                throw new ArgumentNullException("dataRepository");
            }
            if (data == null)
            {
                throw new ArgumentNullException("table");
            }
            var dataAsList = data as IList<IEnumerable<IDataObjectBase>> ?? new List<IEnumerable<IDataObjectBase>>(data);
            return Finalize(table, dataRepository, dataAsList);
        }

        /// <summary>
        /// Indicates whether the worker is manipulating a given field.
        /// </summary>
        /// <param name="fieldName">Name of the field on which to exam for use in the worker.</param>
        /// <param name="workOnTable">The table which the worker are allocated to.</param>
        /// <returns>True if the worker use the field otherwise false.</returns>
        public bool IsManipulatingField(string fieldName, ITable workOnTable)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException("fieldName");
            }
            if (workOnTable == null)
            {
                throw new ArgumentNullException("workOnTable");
            }
            return ManipulatingField(fieldName, workOnTable);
        }

        /// <summary>
        /// Manipulates missing foreign key values for a given table.
        /// </summary>
        /// <param name="table">Table to manipulate data for missing foreign key values.</param>
        /// <param name="dataRepository">Data repository.</param>
        /// <param name="dataToManipulate">Data for the table where to manipulate data for missing foreign key values.</param>
        /// <returns>Manipulated data for the table.</returns>
        protected abstract IEnumerable<IEnumerable<IDataObjectBase>> Manipulate(ITable table, IDataRepository dataRepository, IList<IEnumerable<IDataObjectBase>> dataToManipulate);

        /// <summary>
        /// Finalize data manipulations for missing foreign key values for a given table.
        /// </summary>
        /// <param name="table">Table to finalize data manipulation for missing foreign key values.</param>
        /// <param name="dataRepository">Data repository.</param>
        /// <param name="dataToManipulate">The last manipulated data which has been received.</param>
        /// <returns>Finalized and manipulated data for the table.</returns>
        protected virtual IEnumerable<IEnumerable<IDataObjectBase>> Finalize(ITable table, IDataRepository dataRepository, IList<IEnumerable<IDataObjectBase>> dataToManipulate)
        {
            return dataToManipulate;
        }

        /// <summary>
        /// Indicates whether the worker is manipulating a given field.
        /// </summary>
        /// <param name="fieldName">Name of the field on which to exam for use in the worker.</param>
        /// <param name="workOnTable">The table which the worker are allocated to.</param>
        /// <returns>True if the worker use the field otherwise false.</returns>
        protected virtual bool ManipulatingField(string fieldName, ITable workOnTable)
        {
            return false;
        }

        /// <summary>
        /// Get the dictionary name for a given key.
        /// </summary>
        /// <param name="key">Key.</param>
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
                        stringBuilder.Append(",");
                    }
                    stringBuilder.Append(field.NameTarget ?? "{null}");
                    stringBuilder.Append(":");
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
        /// Get key values for a given key.
        /// </summary>
        /// <param name="dataRepository">Data repository which can read data.</param>
        /// <param name="key">Key from which to get key values.</param>
        /// <param name="excludeNulls">Indicates whether to exclude key values where the last field value is null.</param>
        /// <returns>Key values for the key.</returns>
        protected virtual IEnumerable<string> GetKeyValues(IDataRepository dataRepository, IKey key, bool excludeNulls)
        {
            if (dataRepository == null)
            {
                throw new ArgumentNullException("dataRepository");
            }
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            var dictionaryName = GetDictionaryName(key);
            // Wait if a worker already is caching the dictionary name.
            bool isCaching;
            lock (SyncRoot)
            {
                isCaching = IsCaching.ContainsKey(dictionaryName);
            }
            while (isCaching)
            {
                Thread.Sleep(250);
                lock (SyncRoot)
                {
                    isCaching = IsCaching.ContainsKey(dictionaryName);
                }
            }
            // Try to read the key values from the data cache.
            lock (SyncRoot)
            {
                if (DataCache.ContainsKey(dictionaryName))
                {
                    if (DataCache[dictionaryName].Item1.Contains(WorkerId) == false)
                    {
                        DataCache[dictionaryName].Item1.Add(WorkerId);
                    }
                    return DataCache[dictionaryName].Item2;
                }
                IsCaching.Add(dictionaryName, WorkerId);
            }
            try
            {
                var keyValues = new List<string>(DataRepositoryHelper.GetKeyValues(dataRepository, key, excludeNulls));
                lock (SyncRoot)
                {
                    if (DataCache.ContainsKey(dictionaryName))
                    {
                        if (DataCache[dictionaryName].Item1.Contains(WorkerId) == false)
                        {
                            DataCache[dictionaryName].Item1.Add(WorkerId);
                        }
                        while (DataCache[dictionaryName].Item2.Count > 0)
                        {
                            DataCache[dictionaryName].Item2.Clear();
                        }
                        DataCache[dictionaryName].Item2.AddRange(keyValues);
                        return DataCache[dictionaryName].Item2;
                    }
                    DataCache.Add(dictionaryName, new Tuple<List<string>, List<string>>(new List<string> {WorkerId}, keyValues));
                    return DataCache[dictionaryName].Item2;
                }
            }
            finally
            {
                lock (SyncRoot)
                {
                    while (IsCaching.ContainsKey(dictionaryName))
                    {
                        IsCaching.Remove(dictionaryName);
                    }
                }
            }
        }

        /// <summary>
        /// Remove key value from the data cache for a given key.
        /// </summary>
        /// <param name="key">Key for which to remove key values.</param>
        protected void RemoveFromCache(IKey key)
        {
            var dictionaryName = GetDictionaryName(key);
            lock (SyncRoot)
            {
                if (IsCaching.ContainsKey(dictionaryName))
                {
                    return;
                }
                if (DataCache.ContainsKey(dictionaryName) == false)
                {
                    return;
                }
                while (DataCache[dictionaryName].Item1.Contains(WorkerId))
                {
                    DataCache[dictionaryName].Item1.Remove(WorkerId);
                }
                if (DataCache[dictionaryName].Item1.Count > 0)
                {
                    return;
                }
                var cleanUpWorker = new BackgroundWorker
                    {
                        WorkerReportsProgress = false,
                        WorkerSupportsCancellation = false
                    };
                cleanUpWorker.DoWork += (sender, eventArg) =>
                    {
                        Thread.Sleep(5000);
                        lock (SyncRoot)
                        {
                            var cacheName = eventArg.Argument as string;
                            if (DataCache.ContainsKey(cacheName) == false)
                            {
                                return;
                            }
                            if (DataCache[cacheName].Item1.Count > 0)
                            {
                                return;
                            }
                            try
                            {
                                while (DataCache[cacheName].Item2.Count > 0)
                                {
                                    DataCache[cacheName].Item2.Clear();
                                }
                                DataCache.Remove(cacheName);
                            }
                            finally
                            {
                                GC.Collect();
                            }
                        }
                    };
                cleanUpWorker.RunWorkerCompleted += (sender, eventArgs) => ((BackgroundWorker) sender).Dispose();
                cleanUpWorker.RunWorkerAsync(dictionaryName);
            }
        }

        #endregion
    }
}
