﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Log;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Comparers;
using DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Helpers;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Repositories.DataManipulators
{
    /// <summary>
    /// Worker to clean missing foreign key valus.
    /// </summary>
    public class ForeignKeyCleaner : MissingForeignKeyWorkerBase, IForeignKeyCleaner
    {
        #region Private variables

        private readonly IList<string> _foreignKeyFields;
        private readonly IInformationLogger _informationLogger;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a worker to clean missing foreign key valus.
        /// </summary>
        /// <param name="targetTableName">Target name of the table to validate against.</param>
        /// <param name="foreignKeyFields">Name on fields in the foreign key to validate.</param>
        /// <param name="metadataRepository">Metadata repository.</param>
        /// <param name="informationLogger">Information logger.</param>
        public ForeignKeyCleaner(string targetTableName, IEnumerable<string> foreignKeyFields, IMetadataRepository metadataRepository, IInformationLogger informationLogger)
            : base(targetTableName, metadataRepository)
        {
            if (foreignKeyFields == null)
            {
                throw new ArgumentNullException("foreignKeyFields");
            }
            if (informationLogger == null)
            {
                throw new ArgumentNullException("informationLogger");
            }
            _foreignKeyFields = foreignKeyFields.ToList();
            _informationLogger = informationLogger;
        }

        #endregion

        #region Properties

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
            var keyValueComparer = new KeyValueComparer();
            var foreignKeyFields = _foreignKeyFields.Select(m => DataRepositoryHelper.GetTableField(table, m)).ToArray();
            var foreignKeyValues = new List<string>(DataRepositoryHelper.GetKeyValues(foreignKeyFields, dataToManipulate, true).Distinct(keyValueComparer));
            try
            {
                if (foreignKeyValues.Count == 0)
                {
                    return dataToManipulate;
                }
                // Try to manipulate using a data queryer.
                var dataQueryer = GetDataQueryer(dataRepository);
                if (dataQueryer != null)
                {
                    var dictionaryName = GetDictionaryName(ForeignKeyTable.PrimaryKey);
                    var keyValueFromCache = new List<string>();
                    var keyValuesToClear = new List<string>();
                    try
                    {
                        using (dataQueryer)
                        {
                            lock (SyncRoot)
                            {
                                if (DataCache.ContainsKey(dictionaryName) == false)
                                {
                                    DataCache.Add(dictionaryName, new Tuple<List<string>, List<string>>(new List<string>(), new List<string>()));
                                }
                                if (DataCache[dictionaryName].Item1.Contains(WorkerId) == false)
                                {
                                    DataCache[dictionaryName].Item1.Add(WorkerId);
                                }
                                keyValueFromCache.AddRange(DataCache[dictionaryName].Item2);
                            }
                            while (foreignKeyValues.Count > 0)
                            {
                                if (keyValueFromCache.Any(m => keyValueComparer.Equals(m, foreignKeyValues.ElementAt(0))))
                                {
                                    foreignKeyValues.RemoveAt(0);
                                    continue;
                                }
                                var dataObjects = new List<IDataObjectBase>(DataRepositoryHelper.GetDataRowsForKeyValue(foreignKeyFields, dataToManipulate, foreignKeyValues.ElementAt(0)).First());
                                var extraCriterias = new List<KeyValuePair<string, object>>();
                                try
                                {
                                    for (var keyFieldNo = 0; keyFieldNo < Math.Min(foreignKeyFields.Count(), ForeignKeyTable.PrimaryKey.Fields.Count); keyFieldNo++)
                                    {
                                        var dataObject = DataRepositoryHelper.GetDataObject(dataObjects, foreignKeyFields.ElementAt(keyFieldNo));
                                        var sourceValue = DataRepositoryHelper.GetSourceValue(dataObject);
                                        if (Equals(sourceValue, null))
                                        {
                                            continue;
                                        }
                                        var nameSource = ForeignKeyTable.PrimaryKey.Fields.Select(m => m.Key).ElementAt(keyFieldNo).NameSource;
                                        var nameTarget = ForeignKeyTable.PrimaryKey.Fields.Select(m => m.Key).ElementAt(keyFieldNo).NameTarget;
                                        var foreignKeyFieldName = ForeignKeyTable.Fields.Count(m => string.Compare(m.NameSource, nameSource, StringComparison.OrdinalIgnoreCase) == 0) > 1 ? nameTarget : nameSource;
                                        extraCriterias.Add(new KeyValuePair<string, object>(foreignKeyFieldName, sourceValue));
                                    }
                                    if (dataQueryer.GetNumberOfEqualKeyValues(ForeignKeyTable.PrimaryKey, extraCriterias, foreignKeyValues.ElementAt(0)) == 0)
                                    {
                                        keyValuesToClear.Add(foreignKeyValues.ElementAt(0));
                                        foreignKeyValues.RemoveAt(0);
                                        continue;
                                    }
                                    lock (SyncRoot)
                                    {
                                        if (DataCache.ContainsKey(dictionaryName) == false)
                                        {
                                            foreignKeyValues.RemoveAt(0);
                                            continue;
                                        }
                                        var dataInCache = DataCache.Select(m => m.Value.Item2).Where(m => m != null).ToArray();
                                        while (dataInCache.Sum(m => m.Count) >= MaxRowsInCache)
                                        {
                                            dataInCache.OrderByDescending(m => m.Count).First().RemoveAt(0);
                                        }
                                        DataCache[dictionaryName].Item2.Add(foreignKeyValues.ElementAt(0));
                                    }
                                }
                                finally
                                {
                                    while (extraCriterias.Count > 0)
                                    {
                                        extraCriterias.Clear();
                                    }
                                    while (dataObjects.Count > 0)
                                    {
                                        dataObjects.Clear();
                                    }
                                }
                                foreignKeyValues.RemoveAt(0);
                            }
                            dataQueryer.Dispose();
                        }
                        return keyValuesToClear.Count == 0 ? dataToManipulate : ClearDataRows(table, dataToManipulate, keyValuesToClear, foreignKeyFields, keyValueComparer);
                    }
                    catch
                    {
                        RemoveFromCache(ForeignKeyTable.PrimaryKey);
                        throw;
                    }
                    finally
                    {
                        while (keyValuesToClear.Count > 0)
                        {
                            keyValuesToClear.Clear();
                        }
                        while (keyValueFromCache.Count > 0)
                        {
                            keyValueFromCache.Clear();
                        }
                    }
                }
                // Manipulated without a data queryer.
                try
                {
                    var primaryKeyValues = new List<string>(GetKeyValues(dataRepository, ForeignKeyTable.PrimaryKey, false));
                    try
                    {
                        var keyValuesToClear = new List<string>();
                        try
                        {
                            while (foreignKeyValues.Count > 0)
                            {
                                if (primaryKeyValues.Any(m => keyValueComparer.Equals(m, foreignKeyValues.ElementAt(0))) == false)
                                {
                                    keyValuesToClear.Add(foreignKeyValues.ElementAt(0));
                                }
                                foreignKeyValues.RemoveAt(0);
                            }
                            return keyValuesToClear.Count == 0 ? dataToManipulate : ClearDataRows(table, dataToManipulate, keyValuesToClear, foreignKeyFields, keyValueComparer);
                        }
                        finally
                        {
                            while (keyValuesToClear.Count > 0)
                            {
                                keyValuesToClear.Clear();
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
                }
                catch
                {
                    RemoveFromCache(ForeignKeyTable.PrimaryKey);
                    throw;
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

        /// <summary>
        /// Finalize missing foreign key values for a given table.
        /// </summary>
        /// <param name="table">Table to finalize data manipulation for missing foreign key values.</param>
        /// <param name="dataRepository">Data repository.</param>
        /// <param name="dataToManipulate">The last manipulated data which has been received.</param>
        /// <returns>Finalized and manipulated data for the table.</returns>
        protected override IEnumerable<IEnumerable<IDataObjectBase>> Finalize(ITable table, IDataRepository dataRepository, IList<IEnumerable<IDataObjectBase>> dataToManipulate)
        {
            RemoveFromCache(ForeignKeyTable.PrimaryKey);
            return base.Finalize(table, dataRepository, dataToManipulate);
        }

        /// <summary>
        /// Clear data rows where the foreign key is missing.
        /// </summary>
        /// <param name="table">Table for which to manipulate data.</param>
        /// <param name="dataToManipulate">Data rows which should be manipulated.</param>
        /// <param name="keyValuesToClear">Key values for the rows, where the value should be cleared.</param>
        /// <param name="foreignKeyFields">Field in the foreign key.</param>
        /// <param name="keyValueComparer">Functionality to compare key values.</param>
        /// <returns>Data rows where value for the foreign key missing is cleaned.</returns>
        private IEnumerable<IEnumerable<IDataObjectBase>> ClearDataRows(INamedObject table, ICollection<IEnumerable<IDataObjectBase>> dataToManipulate, IList<string> keyValuesToClear, IList<IField> foreignKeyFields, IEqualityComparer<string> keyValueComparer)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            if (dataToManipulate == null)
            {
                throw new ArgumentNullException("dataToManipulate");
            }
            if (keyValuesToClear == null)
            {
                throw new ArgumentNullException("keyValuesToClear");
            }
            if (foreignKeyFields == null)
            {
                throw new ArgumentNullException("foreignKeyFields");
            }
            if (keyValueComparer == null)
            {
                throw new ArgumentNullException("keyValueComparer");
            }
            var dataRowsToClear = new List<IEnumerable<IDataObjectBase>>();
            try
            {
                while (keyValuesToClear.Count > 0)
                {
                    dataRowsToClear.AddRange(DataRepositoryHelper.GetDataRowsForKeyValue(foreignKeyFields, dataToManipulate, keyValuesToClear.ElementAt(0)));
                    keyValuesToClear.RemoveAt(0);
                }
                dataRowsToClear.ForEach(dataRow =>
                    {
                        var dataObject = dataRow.Single(m => m.Field != null && m.Field.Equals(foreignKeyFields.Last()));
                        dataObject.UpdateSourceValue<object>(null);
                    });
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
                InformationLogger.LogInformation(Resource.GetText(Text.ForeignKeyCleanerInformation, GetType().Name, dataRowsToClear.Count, table.NameTarget, foreignKeyInformationBuilder.ToString()));
                return dataToManipulate;
            }
            finally
            {
                while (dataRowsToClear.Count > 0)
                {
                    dataRowsToClear.Clear();
                }
            }
        }

        #endregion
    }
}
