using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using DsiNext.DeliveryEngine.Domain.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Domain.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Events;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.Data.Oracle.Events;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Comparers;
using DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Events;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Helpers;
using DsiNext.DeliveryEngine.Resources;
using Oracle.DataAccess.Client;

namespace DsiNext.DeliveryEngine.Repositories.Data.Oracle
{
    /// <summary>
    /// Oracle client used by the delivery engine.
    /// </summary>
    public class OracleClient : IOracleClient
    {
        #region Private variables

        private readonly OracleConnection _oracleConnection;
        private readonly IDataManipulators _dataManipulators;
        private bool _disposed;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an oracle client to be used by the delivery engine.
        /// </summary>
        public OracleClient()
            : this(null)
        {
        }

        /// <summary>
        /// Creates an oracle client to be used by the delivery engine.
        /// </summary>
        /// <param name="dataManipulators">Data manipulaters used to manipulate data is queried.</param>
        public OracleClient(IDataManipulators dataManipulators)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["OracleDataRepository"];
            if (connectionString == null)
            {
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.NamedConnectionStringMissing, "OracleDataRepository"));
            }
            if (string.IsNullOrEmpty(connectionString.ConnectionString))
            {
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.NamedConnectionStringMissing, "OracleDataRepository"));
            }
            _dataManipulators = dataManipulators;
            _oracleConnection = new OracleConnection(connectionString.ConnectionString);
            _oracleConnection.Open();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Destructor.
        /// </summary>
        ~OracleClient()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose the Oracle client used by the delivery engine.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the Oracle client used by the delivery engine.
        /// </summary>
        /// <param name="disposing">Indicates whether all managed resources should be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            // Dispose all unmanaged resources.
            _oracleConnection.Close();
            // Dispose all managed resources.
            if (disposing)
            {
                _oracleConnection.Dispose();
                GC.Collect();
            }
            _disposed = true;
        }

        #endregion

        #region IOracleClient Members

        /// <summary>
        /// Validates a table against the Oracle schema.
        /// </summary>
        /// <param name="table">Table to be validated.</param>
        public virtual void ValidateTable(ITable table)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }

            // Check whether the table exists.
            using (var command = _oracleConnection.CreateCommand())
            {
                command.CommandText = string.Format("SELECT COUNT(*) FROM USER_TABLES WHERE UPPER(TABLE_NAME)='{0}'", table.NameSource == null ? string.Empty : table.NameSource.ToUpper());
                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        throw new DeliveryEngineMetadataException(Resource.GetExceptionMessage(ExceptionMessage.TableNotFound, table.NameSource), table);
                    }
                    var tableCount = reader.GetDecimal(0);
                    if (tableCount != 1M)
                    {
                        throw new DeliveryEngineMetadataException(Resource.GetExceptionMessage(ExceptionMessage.TableNotFound, table.NameSource), table);
                    }
                    reader.Close();
                    reader.Dispose();
                }
                command.Dispose();
            }

            // Check whether the columns in the table exists.
            foreach(var field in table.Fields)
            {
                using (var command = _oracleConnection.CreateCommand())
                {
                    command.CommandText = string.Format("SELECT UPPER(DATA_TYPE) FROM USER_TAB_COLUMNS WHERE UPPER(TABLE_NAME)='{0}' AND UPPER(COLUMN_NAME)='{1}'", table.NameSource == null ? string.Empty : table.NameSource.ToUpper(), field.NameSource == null ? string.Empty : field.NameSource.ToUpper());
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            throw new DeliveryEngineMetadataException(Resource.GetExceptionMessage(ExceptionMessage.FieldNotFoundOnTable, field.NameSource, table.NameSource), field);
                        }
                        if (!reader.Read())
                        {
                            throw new DeliveryEngineMetadataException(Resource.GetExceptionMessage(ExceptionMessage.FieldNotFoundOnTable, field.NameSource, table.NameSource), field);
                        }
                        // Check the Oracle data type against the data type on the field.
                        var oracleDataType = reader.GetString(0);
                        switch (oracleDataType)
                        {
                            case "VARCHAR2":
                            case "NVARCHAR2":
                            case "VARCHAR":
                            case "CHAR":
                            case "NCHAR":
                            case "CLOB":
                                if (field.DatatypeOfSource == typeof (string))
                                {
                                    break;
                                }
                                if (field.DatatypeOfSource == typeof (DateTime?))
                                {
                                    break;
                                }
                                if (field.DatatypeOfSource == typeof (TimeSpan?))
                                {
                                    break;
                                }
                                throw new DeliveryEngineMetadataException(Resource.GetExceptionMessage(ExceptionMessage.UnableToMapDataTypeFromDatabase, oracleDataType, field.DatatypeOfSource, field.NameSource, table.NameSource), field);

                            case "NUMBER":
                                if (field.DatatypeOfSource == typeof (int?))
                                {
                                    break;
                                }
                                if (field.DatatypeOfSource == typeof (long?))
                                {
                                    break;
                                }
                                if (field.DatatypeOfSource == typeof (decimal?))
                                {
                                    break;
                                }
                                throw new DeliveryEngineMetadataException(Resource.GetExceptionMessage(ExceptionMessage.UnableToMapDataTypeFromDatabase, oracleDataType, field.DatatypeOfSource, field.NameSource, table.NameSource), field);

                            case "DATE":
                                if (field.DatatypeOfSource == typeof (DateTime?))
                                {
                                    break;
                                }
                                throw new DeliveryEngineMetadataException(Resource.GetExceptionMessage(ExceptionMessage.UnableToMapDataTypeFromDatabase, oracleDataType, field.DatatypeOfSource, field.NameSource, table.NameSource), field);

                            case "TIMESTAMP":
                                if (field.DatatypeOfSource == typeof (DateTime?))
                                {
                                    break;
                                }
                                throw new DeliveryEngineMetadataException(Resource.GetExceptionMessage(ExceptionMessage.UnableToMapDataTypeFromDatabase, oracleDataType, field.DatatypeOfSource, field.NameSource, table.NameSource), field);

                            default:
                                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.DataTypeNotSupported, oracleDataType));
                        }
                        reader.Dispose();
                    }
                    command.Dispose();
                }
            }
        }

        /// <summary>
        /// Gets data for a table.
        /// </summary>
        /// <param name="table">Tabel for which to get data.</param>
        /// <param name="onHandleOracleData">Event handler to handle data from the oracle data repository.</param>
        public virtual void GetData(ITable table, DeliveryEngineEventHandler<IHandleDataEventArgs> onHandleOracleData)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            if (onHandleOracleData == null)
            {
                throw new ArgumentNullException("onHandleOracleData");
            }

            var sqlFields = table.Fields;
            var sqlFilters = table.RecordFilters.Where(m => string.IsNullOrEmpty(m.AsSql()) == false).ToList();
            var sqlKey = table.PrimaryKey;
            
            var selectFields = new StringBuilder();
            foreach (var sqlField in sqlFields)
            {
                if (selectFields.Length > 0)
                {
                    selectFields.AppendFormat(",{0}", sqlField.NameSource);
                    continue;
                }
                selectFields.Append(sqlField.NameSource);
            }

            var whereClause = new StringBuilder();
            foreach (var sqlFilter in sqlFilters)
            {
                if (whereClause.Length > 0)
                {
                    whereClause.AppendFormat(" OR ({0})", sqlFilter.AsSql());
                    continue;
                }
                whereClause.Append(string.Format("({0})", sqlFilter.AsSql()));
            }


            var orderByFields = new StringBuilder();
            if (sqlKey != null)
            {
                foreach (var field in sqlKey.Fields.Where(keyField => keyField.Key != null).Select(keyField => keyField.Key))
                {
                    if (orderByFields.Length > 0)
                    {
                        orderByFields.Append(string.Format(",{0}", field.NameSource));
                        continue;
                    }
                    orderByFields.Append(field.NameSource);
                }
            }

            var records = new List<List<IDataObjectBase>>();
            try
            {
                using (var command = _oracleConnection.CreateCommand())
                {
                    var sqlCommand = new StringBuilder(string.Format("SELECT {0} FROM {1}", selectFields, table.NameSource));
                    if (whereClause.Length > 0)
                    {
                        sqlCommand.AppendFormat(" WHERE {0}", whereClause);
                    }
                    if (orderByFields.Length > 0)
                    {
                        sqlCommand.AppendFormat(" ORDER BY {0}", orderByFields);
                    }
                    command.CommandText = sqlCommand.ToString();
                    using (var reader = command.ExecuteReader())
                    {
                        var fieldDataGenericType = typeof(FieldData<,>);
                        while (reader.Read())
                        {
                            var recordDataObjects = new List<IDataObjectBase>(sqlFields.Count);
                            for (var i = 0; i < sqlFields.Count; i++)
                            {
                                if (sqlFields[i].DatatypeOfSource == typeof(string))
                                {
                                    var fieldDataType = fieldDataGenericType.MakeGenericType(new[]  {typeof (string), sqlFields[i].DatatypeOfTarget});
                                    var fieldData = Activator.CreateInstance(fieldDataType, new object[] {sqlFields[i], reader.IsDBNull(i) ? null : reader.GetString(i).TrimEnd()}) as IDataObjectBase;
                                    recordDataObjects.Add(fieldData);
                                    continue;
                                }
                                if (sqlFields[i].DatatypeOfSource == typeof(int?))
                                {
                                    var fieldDataType = fieldDataGenericType.MakeGenericType(new[] {typeof (int?), sqlFields[i].DatatypeOfTarget});
                                    var fieldData = Activator.CreateInstance(fieldDataType, new object[] {sqlFields[i], reader.IsDBNull(i) ? null : new int?(reader.GetInt32(i))}) as IDataObjectBase;
                                    recordDataObjects.Add(fieldData);
                                    continue;
                                }
                                if (sqlFields[i].DatatypeOfSource == typeof(long?))
                                {
                                    var fieldDataType = fieldDataGenericType.MakeGenericType(new[] {typeof (long?), sqlFields[i].DatatypeOfTarget});
                                    var fieldData = Activator.CreateInstance(fieldDataType, new object[] {sqlFields[i], reader.IsDBNull(i) ? null : new long?(reader.GetInt64(i))}) as IDataObjectBase;
                                    recordDataObjects.Add(fieldData);
                                    continue;
                                }
                                if (sqlFields[i].DatatypeOfSource == typeof(decimal?))
                                {
                                    var fieldDataType = fieldDataGenericType.MakeGenericType(new[] {typeof (decimal?), sqlFields[i].DatatypeOfTarget});
                                    var fieldData = Activator.CreateInstance(fieldDataType, new object[] {sqlFields[i], reader.IsDBNull(i) ? null : new decimal?(reader.GetDecimal(i))}) as IDataObjectBase;
                                    recordDataObjects.Add(fieldData);
                                    continue;
                                }
                                if (sqlFields[i].DatatypeOfSource == typeof(DateTime?))
                                {
                                    DateTime? value = null;
                                    switch (reader.GetDataTypeName(i).ToUpper())
                                    {
                                        case "VARCHAR2":
                                        case "NVARCHAR2":
                                        case "VARCHAR":
                                        case "CHAR":
                                        case "NCHAR":
                                            if (reader.IsDBNull(i))
                                            {
                                                break;
                                            }
                                            var asString = reader.GetString(i).TrimEnd();
                                            if (string.IsNullOrEmpty(asString))
                                            {
                                                break;
                                            }
                                            try
                                            {
                                                value = DateTime.Parse(asString, Thread.CurrentThread.CurrentUICulture);
                                            }
                                            catch (Exception ex)
                                            {
                                                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.ParseError, typeof(DateTime).Name, asString, ex.Message), ex);
                                            }
                                            break;

                                        default:
                                            if (reader.IsDBNull(i))
                                            {
                                                break;
                                            }
                                            value = reader.GetDateTime(i);
                                            break;
                                    }
                                    var fieldDataType = fieldDataGenericType.MakeGenericType(new[] {typeof (DateTime?), sqlFields[i].DatatypeOfTarget});
                                    var fieldData = Activator.CreateInstance(fieldDataType, new object[] {sqlFields[i], value}) as IDataObjectBase;
                                    recordDataObjects.Add(fieldData);
                                    continue;
                                }
                                if (sqlFields[i].DatatypeOfSource == typeof(TimeSpan?))
                                {
                                    TimeSpan? value = null;
                                    switch (reader.GetDataTypeName(i).ToUpper())
                                    {
                                        case "VARCHAR2":
                                        case "NVARCHAR2":
                                        case "VARCHAR":
                                        case "CHAR":
                                        case "NCHAR":
                                            if (reader.IsDBNull(i))
                                            {
                                                break;
                                            }
                                            var asString = reader.GetString(i).TrimEnd();
                                            if (string.IsNullOrEmpty(asString))
                                            {
                                                break;
                                            }
                                            try
                                            {
                                                if (Regex.IsMatch(asString, @"^\d{6}$"))
                                                {
                                                    value = new TimeSpan(0, int.Parse(asString.Substring(0, 2)), int.Parse(asString.Substring(2, 2)), int.Parse(asString.Substring(4, 2)));
                                                    break;
                                                }
                                                value = TimeSpan.Parse(asString, Thread.CurrentThread.CurrentUICulture);
                                            }
                                            catch (Exception ex)
                                            {
                                                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.ParseError, typeof(TimeSpan).Name, asString, ex.Message), ex);
                                            }
                                            break;

                                        default:
                                            if (reader.IsDBNull(i))
                                            {
                                                break;
                                            }
                                            value = reader.GetTimeSpan(i);
                                            break;
                                    }
                                    var fieldDataType = fieldDataGenericType.MakeGenericType(new[] {typeof (TimeSpan?), sqlFields[i].DatatypeOfTarget});
                                    var fieldData = Activator.CreateInstance(fieldDataType, new object[] {sqlFields[i], value}) as IDataObjectBase;
                                    recordDataObjects.Add(fieldData);
                                    continue;
                                }
                                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.IllegalValue, sqlFields[i].DatatypeOfSource, "DatatypeOfSource"));
                            }
                            records.Add(recordDataObjects);
                            if (records.Count < 1024)
                            {
                                continue;
                            }
                            onHandleOracleData.Invoke(this, new HandleOracleDataEventArgs(table, records, false));
                            while (records.Count > 0)
                            {
                                while (records.ElementAt(0).Count > 0)
                                {
                                    records.ElementAt(0).Clear();
                                }
                                records.RemoveAt(0);
                            }
                        }
                        onHandleOracleData.Invoke(this, new HandleOracleDataEventArgs(table, records, true));
                        reader.Close();
                        reader.Dispose();
                    }
                    command.Dispose();
                }
            }
            finally
            {
                while (records.Count > 0)
                {
                    while (records.ElementAt(0).Count > 0)
                    {
                        records.ElementAt(0).Clear();
                    }
                    records.RemoveAt(0);
                }
                records = null;
                Debug.Assert(records == null);
                GC.Collect();
            }
        }

        /// <summary>
        /// Gets the number of records in a given table.
        /// </summary>
        /// <param name="table">Tabel for which to select the number of records.</param>
        /// <returns>Number of records in the table.</returns>
        public int SelectCountForTable(ITable table)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }

            var sqlFilters = table.RecordFilters.Where(m => string.IsNullOrEmpty(m.AsSql()) == false).ToList();
            var whereClause = new StringBuilder();
            foreach (var sqlFilter in sqlFilters)
            {
                if (whereClause.Length > 0)
                {
                    whereClause.AppendFormat(" OR ({0})", sqlFilter.AsSql());
                    continue;
                }
                whereClause.Append(string.Format("({0})", sqlFilter.AsSql()));
            }

            var count = 0;
            using (var command = _oracleConnection.CreateCommand())
            {
                command.CommandText = whereClause.Length > 0 ? string.Format("SELECT COUNT(*) FROM {0} WHERE {1}", table.NameSource, whereClause) : string.Format("SELECT COUNT(*) FROM {0}", table.NameSource);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        count = Convert.ToInt32(reader.GetDecimal(0));
                    }
                    reader.Close();
                    reader.Close();
                }
                command.Dispose();
            }
            return count;
        }

        #endregion

        #region IDataQueryer Members

        /// <summary>
        /// Get the number of equal key values for a given key.
        /// </summary>
        /// <param name="key">Key on which to calculate equal number of key values.</param>
        /// <param name="extraCriterias">Extra criterias (field name and value) to put into the record filter when querying for number of equal key values.</param>
        /// <param name="matchingKeyValue">The key value on which to calculate the number of equal key values.</param>
        /// <returns>Number of equal key values for the key.</returns>
        public virtual int GetNumberOfEqualKeyValues(IKey key, IEnumerable<KeyValuePair<string, object>> extraCriterias, string matchingKeyValue)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (extraCriterias == null)
            {
                throw new ArgumentNullException("extraCriterias");
            }
            if (string.IsNullOrEmpty(matchingKeyValue))
            {
                throw new ArgumentNullException("matchingKeyValue");
            }

            var table = key.Table;
            if (table == null)
            {
                throw new DeliveryEngineMetadataException(Resource.GetExceptionMessage(ExceptionMessage.IllegalValue, table, "Table"), key);
            }

            var clonedTable = (ITable) table.Clone();
            var addCriterias = new List<KeyValuePair<string, object>>(extraCriterias.ToList());
            try
            {
                foreach (var extraCriteria in addCriterias)
                {
                    var field = DataRepositoryHelper.GetTableField(clonedTable, extraCriteria.Key);
                    if (clonedTable.RecordFilters.Count == 0)
                    {
                        clonedTable.AddRecordFilter(new Filter());
                    }
                    foreach (var recordFilter in clonedTable.RecordFilters)
                    {
                        recordFilter.AddCriteria(DataRepositoryHelper.BuildEqualCriteria(field, extraCriteria.Value));
                    }
                }

                var selectCountForTable = SelectCountForTable(clonedTable);
                if (selectCountForTable == 1)
                {
                    return selectCountForTable;
                }

                var numberOfEqualKeyValues = 0;
                GetData(clonedTable, (sender, eventArgs) =>
                    {
                        if (eventArgs == null)
                        {
                            throw new ArgumentNullException("eventArgs");
                        }
                        var keyFields = new List<IField>(key.Fields.Select(m => m.Key).Select(keyField => DataRepositoryHelper.GetTableField(eventArgs.Table, keyField.NameTarget ?? keyField.NameSource)));
                        IList<string> keyValues;
                        var keyValueComparer = new KeyValueComparer();
                        if (_dataManipulators == null)
                        {
                            keyValues = new List<string>(DataRepositoryHelper.GetKeyValues(keyFields, eventArgs.Data, false));
                            try
                            {
                                numberOfEqualKeyValues += keyValues.Count(keyValue => keyValueComparer.Equals(keyValue, matchingKeyValue));
                            }
                            finally
                            {
                                while (keyValues.Count > 0)
                                {
                                    keyValues.Clear();
                                }
                                keyValues = null;
                                Debug.Assert(keyValues == null);
                                while (keyFields.Count > 0)
                                {
                                    keyFields.Clear();
                                }
                                keyFields = null;
                                Debug.Assert(keyFields == null);
                            }
                            return;
                        }
                        var manipulatedData = eventArgs.Data;
                        var myDataManipulators = _dataManipulators.Where(m => (string.IsNullOrEmpty(eventArgs.Table.NameTarget) == false && m.IsManipulatingTable(eventArgs.Table.NameTarget)) || (string.IsNullOrEmpty(eventArgs.Table.NameSource) == false && m.IsManipulatingTable(eventArgs.Table.NameSource))).ToArray();
                        foreach (var dataManipulator in myDataManipulators)
                        {
                            if (keyFields.Any(keyField => (string.IsNullOrEmpty(keyField.NameTarget) == false && dataManipulator.IsManipulatingField(keyField.NameTarget)) || (string.IsNullOrEmpty(keyField.NameSource) == false && dataManipulator.IsManipulatingField(keyField.NameSource))) == false)
                            {
                                continue;
                            }
                            if (dataManipulator is IMissingForeignKeyHandler)
                            {
                                continue;
                            }
                            manipulatedData = dataManipulator.ManipulateData(eventArgs.Table, manipulatedData);
                            if (eventArgs.EndOfData)
                            {
                                manipulatedData = dataManipulator.FinalizeDataManipulation(eventArgs.Table, manipulatedData);
                            }
                        }
                        keyValues = new List<string>(DataRepositoryHelper.GetKeyValues(keyFields, manipulatedData, false));
                        try
                        {
                            numberOfEqualKeyValues += keyValues.Count(keyValue => keyValueComparer.Equals(keyValue, matchingKeyValue));
                            if (numberOfEqualKeyValues > 0 || eventArgs.EndOfData == false)
                            {
                                return;
                            }
                            // ReSharper disable LoopCanBeConvertedToQuery
                            foreach (var missingForeignKeyHandler in myDataManipulators.OfType<IMissingForeignKeyHandler>())
                            {
                                if (keyFields.Any(keyField => (string.IsNullOrEmpty(keyField.NameTarget) == false && missingForeignKeyHandler.IsManipulatingField(keyField.NameTarget)) || (string.IsNullOrEmpty(keyField.NameSource) == false && missingForeignKeyHandler.IsManipulatingField(keyField.NameSource))) == false)
                                {
                                    continue;
                                }
                                if (missingForeignKeyHandler.Worker is IPrimaryKeyAdder)
                                {
                                    numberOfEqualKeyValues += Convert.ToInt32(addCriterias.Any() == false);
                                }
                            }
                            // ReSharper restore LoopCanBeConvertedToQuery
                        }
                        finally
                        {
                            while (keyValues.Count > 0)
                            {
                                keyValues.Clear();
                            }
                            keyValues = null;
                            Debug.Assert(keyValues == null);
                            while (keyFields.Count > 0)
                            {
                                keyFields.Clear();
                            }
                            keyFields = null;
                            Debug.Assert(keyFields == null);
                        }
                    });
                if (numberOfEqualKeyValues == 1 || addCriterias.Any() == false)
                {
                    return numberOfEqualKeyValues;
                }

                var newCriterias = new List<KeyValuePair<string, object>>(addCriterias.GetRange(0, addCriterias.Count - 1));
                try
                {
                    if (_dataManipulators != null)
                    {
                        var tableNameTarget = key.Table.NameTarget;
                        var tableNameSource = key.Table.NameSource;
                        var retry = _dataManipulators.Any(dataManipulator =>
                            {
                                if (string.IsNullOrEmpty(tableNameTarget) == false && dataManipulator.IsManipulatingTable(tableNameTarget))
                                {
                                    return dataManipulator.IsManipulatingField(addCriterias.Last().Key) || newCriterias.Select(newCriteria => newCriteria.Key).Any(dataManipulator.IsManipulatingField);
                                }
                                if (string.IsNullOrEmpty(tableNameSource) == false && dataManipulator.IsManipulatingTable(tableNameSource))
                                {
                                    return dataManipulator.IsManipulatingField(addCriterias.Last().Key) || newCriterias.Select(newCriteria => newCriteria.Key).Any(dataManipulator.IsManipulatingField);
                                }
                                return false;
                            });
                        if (retry)
                        {
                            return GetNumberOfEqualKeyValues(key, newCriterias, matchingKeyValue);
                        }
                    }
                    if (newCriterias.Count == 0 && clonedTable.RecordFilters.Any(recordFilter => recordFilter.Criterias.Take(recordFilter.Criterias.Count - 1).OfType<IFieldCriteria>().Any(criteria => criteria.Field != null && (string.Compare(addCriterias.Last().Key, criteria.Field.NameSource, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(addCriterias.Last().Key, criteria.Field.NameTarget, StringComparison.OrdinalIgnoreCase) == 0))))
                    {
                        return numberOfEqualKeyValues;
                    }
                    return newCriterias.Count == key.Fields.Count - 1 ? GetNumberOfEqualKeyValues(key, newCriterias, matchingKeyValue) : numberOfEqualKeyValues;
                }
                finally
                {
                    while (newCriterias.Count > 0)
                    {
                        newCriterias.Clear();
                    }
                    newCriterias = null;
                    Debug.Assert(newCriterias == null);
                }
            }
            finally
            {
                while (addCriterias.Count > 0)
                {
                    addCriterias.Clear();
                }
                clonedTable = null;
                Debug.Assert(clonedTable == null);
            }
        }

        #endregion
    }
}
