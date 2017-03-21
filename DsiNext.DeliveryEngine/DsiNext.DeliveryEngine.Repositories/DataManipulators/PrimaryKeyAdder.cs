using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Domain.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Log;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Comparers;
using DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Helpers;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Repositories.DataManipulators
{
    /// <summary>
    /// Worker to add missing primary keys.
    /// </summary>
    public class PrimaryKeyAdder : MissingForeignKeyWorkerBase, IPrimaryKeyAdder
    {
        #region Private variables

        private readonly IList<string> _foreignKeyFields;
        private readonly IDictionary<string, object> _setFieldValues;
        private readonly IInformationLogger _informationLogger;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a worker to add missing primary keys.
        /// </summary>
        /// <param name="targetTableName">Target name of the table to validate against.</param>
        /// <param name="foreignKeyFields">Name on fields in the foreign key to validate.</param>
        /// <param name="setFieldValues">Field values to be set on missing primary keys.</param>
        /// <param name="metadataRepository">Metadata repository.</param>
        /// <param name="informationLogger">Information logger.</param>
        public PrimaryKeyAdder(string targetTableName, IEnumerable<string> foreignKeyFields, IDictionary<string, object> setFieldValues, IMetadataRepository metadataRepository, IInformationLogger informationLogger)
            : base(targetTableName, metadataRepository)
        {
            if (foreignKeyFields == null)
            {
                throw new ArgumentNullException("foreignKeyFields");
            }
            if (setFieldValues == null)
            {
                throw new ArgumentNullException("setFieldValues");
            }
            if (informationLogger == null)
            {
                throw new ArgumentNullException("informationLogger");
            }
            _foreignKeyFields = foreignKeyFields.ToList();
            _setFieldValues = setFieldValues;
            _informationLogger = informationLogger;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Field values to be set on missing primary keys.
        /// </summary>
        public virtual IDictionary<string, object> SetFieldValues
        {
            get
            {
                return _setFieldValues;
            }
        }

        /// <summary>
        /// Information logger.
        /// </summary>
        public virtual IInformationLogger InformationLogger
        {
            get
            {
                return _informationLogger;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Manipulates missing foreign key values for a given table.
        /// </summary>
        /// <param name="table">Tabel to manipulate data for missing foreign key values.</param>
        /// <param name="dataRepository">Data repository.</param>
        /// <param name="dataToManipulate">Data for the table where to manipulate data for missing foreign key values.</param>
        /// <returns>Manipulates data for the table.</returns>
        protected override IEnumerable<IEnumerable<IDataObjectBase>> Manipulate(ITable table, IDataRepository dataRepository, IList<IEnumerable<IDataObjectBase>> dataToManipulate)
        {
            var dictionaryName = GetDictionaryName(table.PrimaryKey);
            var workerDictionaryName = string.Format("{0} - {1}", dictionaryName, WorkerId);
            // Store primary key values in the cache.
            lock (SyncRoot)
            {
                try
                { 
                    var keyValues = new List<string>(DataRepositoryHelper.GetKeyValues(table.PrimaryKey, dataToManipulate, false));
                    try
                    {
                        if (DataCache.ContainsKey(workerDictionaryName) == false)
                        {
                            DataCache.Add(workerDictionaryName, new Tuple<List<string>, List<string>>(new List<string>(keyValues), null));
                            return dataToManipulate;
                        }
                        DataCache[workerDictionaryName].Item1.AddRange(keyValues);
                        return dataToManipulate;
                    }
                    finally
                    {
                        while (keyValues.Count > 0)
                        {
                            keyValues.Clear();
                        }
                    }
                }
                catch
                {
                    while (DataCache.ContainsKey(workerDictionaryName))
                    {
                        while (DataCache[workerDictionaryName].Item1.Count > 0)
                        {
                            DataCache[workerDictionaryName].Item1.Clear();
                        }
                        DataCache.Remove(workerDictionaryName);
                    }
                    throw;
                }
            }
        }

        /// <summary>
        /// Finalize missing foreign key values for a given table.
        /// </summary>
        /// <param name="table">Table to finalize data manipulation for missing foreign key values.</param>
        /// <param name="dataRepository">Data repository.</param>
        /// <param name="dataToManipulate">The last manipulated data which has been received.</param>
        /// <returns>Finalized and manipulated data for the table.</returns>
        protected override IEnumerable<IEnumerable<IDataObjectBase>> Finalize(ITable table, IDataRepository dataRepository, IList<IEnumerable<IDataObjectBase>> dataToManipulate)
        {
            var dictionaryName = GetDictionaryName(table.PrimaryKey);
            var workerDictionaryName = string.Format("{0} - {1}", dictionaryName, WorkerId);
            var primaryKeyValues = new List<string>();
            try
            {
                // Restore primary key values from the cache.
                lock (SyncRoot)
                {
                    if (DataCache.ContainsKey(workerDictionaryName) == false)
                    {
                        return dataToManipulate;
                    }
                    primaryKeyValues.AddRange(DataCache[workerDictionaryName].Item1);
                    while (DataCache[workerDictionaryName].Item1.Count > 0)
                    {
                        DataCache[workerDictionaryName].Item1.Clear();
                    }
                    DataCache.Remove(workerDictionaryName);
                }
                // Add missing foreign key values.
                var clonedForeignKeyTable = GetForeignTableWithNotNullCriteria(ForeignKeyTable, _foreignKeyFields.Last());
                try
                {
                    var keyValueComparer = new KeyValueComparer();
                    var foreignKeyValues = new List<string>(DataRepositoryHelper.GetKeyValues(dataRepository, clonedForeignKeyTable, _foreignKeyFields, true).Distinct(keyValueComparer));
                    try
                    {
                        var keyValuesToCreate = new List<string>(foreignKeyValues.Where(m => primaryKeyValues.Count(n => keyValueComparer.Equals(m, n)) == 0));
                        try
                        {
                            AddMissingRows(table, dataToManipulate, keyValuesToCreate);
                        }
                        finally
                        {
                            while (keyValuesToCreate.Count > 0)
                            {
                                keyValuesToCreate.Clear();
                            }
                        }
                    }
                    finally
                    {
                        while (foreignKeyValues.Count > 0)
                        {
                            foreignKeyValues.Clear();
                        }
                    }
                    return dataToManipulate;
                }
                finally
                {
                    clonedForeignKeyTable = null;
                    Debug.Assert(clonedForeignKeyTable == null);
                    GC.Collect();
                }
            }
            finally
            {
                while (primaryKeyValues.Count > 0)
                {
                    primaryKeyValues.Clear();
                }
                // Remove primary key values from the cache.
                lock (SyncRoot)
                {
                    while (DataCache.ContainsKey(workerDictionaryName))
                    {
                        while (DataCache[workerDictionaryName].Item1.Count > 0)
                        {
                            DataCache[workerDictionaryName].Item1.Clear();
                        }
                        DataCache.Remove(workerDictionaryName);
                    }
                }
                RemoveFromCache(table.PrimaryKey);
            }
        }

        /// <summary>
        /// Indicates whether the worker is manipulating a given field.
        /// </summary>
        /// <param name="fieldName">Name of the field on which to exam for use in the worker.</param>
        /// <param name="workOnTable">The table which the worker are allocated to.</param>
        /// <returns>True if the worker use the field otherwise false.</returns>
        protected override bool ManipulatingField(string fieldName, ITable workOnTable)
        {
            return workOnTable.PrimaryKey.Fields.Select(m => m.Key).Any(field => string.Compare(field.NameSource, fieldName, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(field.NameTarget, fieldName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        /// <summary>
        /// Adds missing rows to the manipulated data.
        /// </summary>
        /// <param name="table">Table on which to add the missing rows.</param>
        /// <param name="dataToManipulate"></param>
        /// <param name="keyValuesToCreate">Key values for the rows, which should be created.</param>
        private void AddMissingRows(ITable table, ICollection<IEnumerable<IDataObjectBase>> dataToManipulate, IEnumerable<string> keyValuesToCreate)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            if (dataToManipulate == null)
            {
                throw new ArgumentNullException("dataToManipulate");
            }
            if (keyValuesToCreate == null)
            {
                throw new ArgumentNullException("keyValuesToCreate");
            }
            var missingRows = CreateMissingRows(table, keyValuesToCreate);
            try
            {
                if (missingRows.Count == 0)
                {
                    return;
                }
                missingRows.ForEach(dataToManipulate.Add);
                var foreignKeyInformationBuilder = new StringBuilder();
                foreignKeyInformationBuilder.AppendFormat("{0}(", ForeignKeyTable.NameTarget);
                for (var i = 0; i < _foreignKeyFields.Count; i++)
                {
                    if (i > 0)
                    {
                        foreignKeyInformationBuilder.AppendFormat(", {0}", _foreignKeyFields.ElementAt(i));
                        continue;
                    }
                    foreignKeyInformationBuilder.Append(_foreignKeyFields.ElementAt(i));
                }
                foreignKeyInformationBuilder.Append(')');
                InformationLogger.LogInformation(Resource.GetText(Text.PrimaryKeyAdderInformation, GetType().Name, missingRows.Count, table.NameTarget, foreignKeyInformationBuilder.ToString()));
            }
            finally
            {
                while (missingRows.Count > 0)
                {
                    missingRows.Clear();
                }
            }
        }

        /// <summary>
        /// Creates missing rows on a given table.
        /// </summary>
        /// <param name="table">Table on which to create the missing rows.</param>
        /// <param name="keyValuesToCreate">Key values for the rows, which should be created.</param>
        /// <returns>Created rows.</returns>
        private List<IEnumerable<IDataObjectBase>> CreateMissingRows(ITable table, IEnumerable<string> keyValuesToCreate)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            if (keyValuesToCreate == null)
            {
                throw new ArgumentNullException("keyValuesToCreate");
            }
            return new List<IEnumerable<IDataObjectBase>>(keyValuesToCreate.Select(keyValueToCreate => CreateMissingRow(table, keyValueToCreate.Split('|'))));
        }

        /// <summary>
        /// Creates a missing row on a given table.
        /// </summary>
        /// <param name="table">Table on which to create a missing row.</param>
        /// <param name="keyValues">Key values for the missing row.</param>
        /// <returns>Created rows.</returns>
        private IEnumerable<IDataObjectBase> CreateMissingRow(ITable table, ICollection<string> keyValues)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            if (keyValues == null)
            {
                throw new ArgumentNullException("keyValues");
            }
            var missingRow = new List<IDataObjectBase>(table.CreateRow());
            foreach (var setFieldValue in SetFieldValues)
            {
                DataRepositoryHelper.UpdateSourceValue(DataRepositoryHelper.GetDataObject(missingRow, setFieldValue.Key), setFieldValue.Value);
            }
            for (var i = 0; i < Math.Min(table.PrimaryKey.Fields.Count, keyValues.Count); i++)
            {
                IDataObjectBase dataObject;
                try
                {
                    dataObject = DataRepositoryHelper.GetDataObject(missingRow, table.PrimaryKey.Fields.ElementAt(i).Key);
                }
                catch (DeliveryEngineSystemException)
                {
                    dataObject = DataRepositoryHelper.GetDataObject(missingRow, table.PrimaryKey.Fields.ElementAt(i).Key.NameTarget);
                }
                DataRepositoryHelper.UpdateSourceValue(dataObject, keyValues.ElementAt(i));
            }
            return missingRow;
        }

        /// <summary>
        /// Gets the foreign table with a not null criteria in the filters.
        /// </summary>
        /// <param name="foreignTable">Foreign table to clone and insert a not null criteria in the filters.</param>
        /// <param name="fieldName">Name of the field on which to create the not null criteria.</param>
        /// <returns>Cloned foreign table with a not null criteria in the filters.</returns>
        private static ITable GetForeignTableWithNotNullCriteria(ICloneable foreignTable, string fieldName)
        {
            if (foreignTable == null)
            {
                throw new ArgumentNullException("foreignTable");
            }
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException("fieldName");
            }
            var clonedForeignTable = (ITable) foreignTable.Clone();
            var notNullField = DataRepositoryHelper.GetTableField(clonedForeignTable, fieldName);
            if (clonedForeignTable.RecordFilters.Count == 0)
            {
                clonedForeignTable.AddRecordFilter(new Filter());
            }
            foreach (var recordFilter in clonedForeignTable.RecordFilters)
            {
                recordFilter.AddCriteria(new NotNullCriteria(notNullField));
            }
            return clonedForeignTable;
        }

        #endregion
    }
}
