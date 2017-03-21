using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using DsiNext.DeliveryEngine.Domain.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Events;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.Data.OldToNew.Events;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Events;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Repositories.Data.OldToNew
{
    /// <summary>
    /// Data repository for converting old delivery format to the new delivery format.
    /// </summary>
    public class OldToNewDataRepository : IDataRepository
    {
        #region Private variables

        private readonly IDictionary<string, IList<FileInfo>> _tableDictionary = new Dictionary<string, IList<FileInfo>>();

        #endregion

        #region Constructors

        /// <summary>
        /// Creates data repository for converting old delivery format to the new delivery format.
        /// </summary>
        /// <param name="path">Path containing the old delivery format.</param>
        public OldToNewDataRepository(DirectoryInfo path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (!path.Exists)
            {
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.DirectoryNotFound, path.FullName));
            }
            try
            {
                var fileMaps = path.GetFiles("FILMAP.TAB", SearchOption.AllDirectories);
                if (fileMaps.Length == 0)
                {
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.FileNotFound, "FILMAP.TAB"));
                }
                foreach (var fileMap in fileMaps)
                {
                    if (fileMap.Directory == null)
                    {
                        continue;
                    }
                    using (var fileStream = fileMap.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        using (var streamReader = new StreamReader(fileStream, Encoding.Default))
                        {
                            var buffer = new char[12 + 8];
                            while (!streamReader.EndOfStream)
                            {
                                var readedBytes = streamReader.Read(buffer, 0, 12 + 8);
                                var s = new string(buffer, 0, readedBytes);
                                if (string.IsNullOrEmpty(s))
                                {
                                    continue;
                                }
                                
                                // ReSharper disable PossibleNullReferenceException
                                var archiveNumber = fileMap.Directory.FullName.Substring(fileMap.Directory.FullName.LastIndexOf('\\') + 1);
                                // ReSharper restore PossibleNullReferenceException
                                var fileName = string.Format("{0}{1}{2}{3}{4}{5}{6}", path.FullName, Path.DirectorySeparatorChar, s.Substring(12, 8), Path.DirectorySeparatorChar, archiveNumber, Path.DirectorySeparatorChar, s.Substring(0, 12).Trim());
                                var fileInfo = new FileInfo(fileName);
                                if (!fileInfo.Exists || string.IsNullOrEmpty(fileInfo.Extension) || fileInfo.Extension.ToUpper() == ".XML" || fileInfo.Extension.ToUpper() == ".TAB")
                                {
                                    continue;
                                }

                                var tableName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                                if (string.IsNullOrEmpty(tableName))
                                {
                                    continue;
                                }

                                IList<FileInfo> tableFiles;
                                if (_tableDictionary.TryGetValue(tableName.ToUpper(), out tableFiles))
                                {
                                    tableFiles.Add(fileInfo);
                                    continue;
                                }
                                _tableDictionary.Add(tableName.ToUpper(), new List<FileInfo> {fileInfo});
                            }
                            streamReader.Close();
                        }
                        fileStream.Close();
                    }
                }
            }
            catch (DeliveryEngineRepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.RepositoryError, MethodBase.GetCurrentMethod().Name, ex.Message), ex);
            }
        }

        /// <summary>
        /// Creates data repository for converting old delivery format to the new delivery format.
        /// </summary>
        /// <param name="tableDictionary">Dictionary for table files.</param>
        private OldToNewDataRepository(IEnumerable<KeyValuePair<string, IList<FileInfo>>> tableDictionary)
        {
            if (tableDictionary == null)
            {
                throw new ArgumentNullException("tableDictionary");
            }
            foreach (var table in tableDictionary)
            {
                _tableDictionary.Add(table.Key, table.Value);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event raised when the data repository must handle data.
        /// </summary>
        public virtual event DeliveryEngineEventHandler<IHandleDataEventArgs> OnHandleData;

        /// <summary>
        /// Event raised when the data repository is cloned.
        /// </summary>
        public virtual event DeliveryEngineEventHandler<ICloneDataRepositoryEventArgs> OnClone;

        #endregion

        #region IDataRepository Members

        /// <summary>
        /// Gets data for a specific target table where data can be merged from one or more source tables.
        /// </summary>
        /// <param name="targetTableName">Name of the target table where data should be returned.</param>
        /// <param name="dataSource">Data source from where to get the data.</param>
        public virtual void DataGetForTargetTable(string targetTableName, IDataSource dataSource)
        {
            if (string.IsNullOrEmpty(targetTableName))
            {
                throw new ArgumentNullException("targetTableName");
            }
            if (dataSource == null)
            {
                throw new ArgumentNullException("dataSource");
            }
            try
            {
                dataSource.Tables.Where(m => String.Compare(targetTableName, m.NameTarget, StringComparison.Ordinal) == 0)
                          .ToList()
                          .ForEach(DataGetFromTable);
            }
            catch (DeliveryEngineSystemException)
            {
                throw;
            }
            catch (DeliveryEngineRepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.RepositoryError, MethodBase.GetCurrentMethod().Name, ex.Message), ex);
            }
        }

        /// <summary>
        /// Gets data from a table.
        /// </summary>
        /// <param name="table">Table from where data should be returned.</param>
        public virtual void DataGetFromTable(ITable table)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            try
            {
                IList<FileInfo> tableFiles;
                if (!_tableDictionary.TryGetValue(table.NameSource.ToUpper(), out tableFiles))
                {
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.TableNotFound, table.NameSource));
                }

                var records = new List<List<IDataObjectBase>>();
                try
                {
                    foreach (var tableFile in tableFiles)
                    {
                        using (var fileStream = tableFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            using (var streamReader = new StreamReader(fileStream, Encoding.Default))
                            {
                                while (!streamReader.EndOfStream)
                                {
                                    var record = ReadRecordFromTable(table, streamReader);
                                    records.Add(record);
                                    if (records.Count < 1024)
                                    {
                                        continue;
                                    }
                                    if (OnHandleData == null)
                                    {
                                        records.Clear();
                                        continue;
                                    }
                                    OnHandleData.Invoke(this, new HandleOldToNewDataEventArgs(table, records, false));
                                    while (records.Count > 0)
                                    {
                                        while (records.ElementAt(0).Count > 0)
                                        {
                                            records.ElementAt(0).Clear();
                                        }
                                        records.RemoveAt(0);
                                    }
                                }
                                streamReader.Close();
                            }
                            fileStream.Close();
                        }
                    }
                    if (OnHandleData != null)
                    {
                        OnHandleData.Invoke(this, new HandleOldToNewDataEventArgs(table, records, true));
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
                }
            }
            catch (DeliveryEngineSystemException)
            {
                throw;
            }
            catch (DeliveryEngineRepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.RepositoryError, MethodBase.GetCurrentMethod().Name, ex.Message), ex);
            }
        }

        /// <summary>
        /// Gets a data queryer for executing queries.
        /// </summary>
        /// <returns>Data queryer to execute queries.</returns>
        public virtual IDataQueryer GetDataQueryer()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Read a record from a table.
        /// </summary>
        /// <param name="table">Table.</param>
        /// <param name="streamReader">Stream reader from which to read the record.</param>
        /// <returns>Readed record.</returns>
        private static List<IDataObjectBase> ReadRecordFromTable(ITable table, TextReader streamReader)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            if (streamReader == null)
            {
                throw new ArgumentNullException("streamReader");
            }
            var columns = new List<IDataObjectBase>(table.Fields.Count);
            foreach (var field in table.Fields)
            {
                try
                {
                    var fieldDataGenericType = typeof (FieldData<,>);
                    if (field.DatatypeOfSource == typeof (string))
                    {
                        var fieldDataType = fieldDataGenericType.MakeGenericType(new[] {typeof (string), field.DatatypeOfTarget});
                        var buffer = new char[field.LengthOfSource];
                        var readed = streamReader.Read(buffer, 0, field.LengthOfSource);
                        var value = new string(buffer, 0, readed).Trim();
                        if (field.Nullable && string.IsNullOrEmpty(value) && fieldDataType.IsValueType == false)
                        {
                            columns.Add(CreateEmptyFieldData(fieldDataType, field));
                            continue;
                        }
                        var fieldData = Activator.CreateInstance(fieldDataType, new object[] {field, value}) as IDataObjectBase;
                        if (fieldData == null)
                        {
                            throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.UnableToCreateInstanceOfType, fieldDataType.Name));
                        }
                        columns.Add(fieldData);
                        continue;
                    }
                    if (field.DatatypeOfSource == typeof (int?))
                    {
                        var fieldDataType = fieldDataGenericType.MakeGenericType(new[] {typeof (int?), field.DatatypeOfTarget});
                        var buffer = new char[field.LengthOfSource];
                        var readed = streamReader.Read(buffer, 0, field.LengthOfSource);
                        var stringValue = new string(buffer, 0, readed).Trim();
                        if (field.Nullable && string.IsNullOrEmpty(stringValue) && fieldDataType.IsValueType == false)
                        {
                            columns.Add(CreateEmptyFieldData(fieldDataType, field));
                            continue;
                        }
                        var value = int.Parse(stringValue);
                        var fieldData = Activator.CreateInstance(fieldDataType, new object[] {field, value}) as IDataObjectBase;
                        if (fieldData == null)
                        {
                            throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.UnableToCreateInstanceOfType, fieldDataType.Name));
                        }
                        columns.Add(fieldData);
                        continue;
                    }
                    if (field.DatatypeOfSource == typeof (long?))
                    {
                        var fieldDataType = fieldDataGenericType.MakeGenericType(new[] {typeof (long?), field.DatatypeOfTarget});
                        var buffer = new char[field.LengthOfSource];
                        var readed = streamReader.Read(buffer, 0, field.LengthOfSource);
                        var stringValue = new string(buffer, 0, readed).Trim();
                        if (field.Nullable && string.IsNullOrEmpty(stringValue) && fieldDataType.IsValueType == false)
                        {
                            columns.Add(CreateEmptyFieldData(fieldDataType, field));
                            continue;
                        }
                        var value = long.Parse(stringValue);
                        var fieldData = Activator.CreateInstance(fieldDataType, new object[] {field, value}) as IDataObjectBase;
                        if (fieldData == null)
                        {
                            throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.UnableToCreateInstanceOfType, fieldDataType.Name));
                        }
                        columns.Add(fieldData);
                        continue;
                    }
                    if (field.DatatypeOfSource == typeof (decimal?))
                    {
                        var fieldDataType = fieldDataGenericType.MakeGenericType(new[] {typeof (decimal?), field.DatatypeOfTarget});
                        var buffer = new char[field.LengthOfSource];
                        var readed = streamReader.Read(buffer, 0, field.LengthOfSource);
                        var stringValue = new string(buffer, 0, readed).Trim();
                        if (field.Nullable && string.IsNullOrEmpty(stringValue) && fieldDataType.IsValueType == false)
                        {
                            columns.Add(CreateEmptyFieldData(fieldDataType, field));
                            continue;
                        }
                        decimal value;
                        if (!decimal.TryParse(stringValue, NumberStyles.Any, new CultureInfo("en-US"), out value))
                        {
                            value = decimal.Parse(stringValue, Thread.CurrentThread.CurrentUICulture);
                        }
                        var fieldData = Activator.CreateInstance(fieldDataType, new object[] {field, value}) as IDataObjectBase;
                        if (fieldData == null)
                        {
                            throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.UnableToCreateInstanceOfType, fieldDataType.Name));
                        }
                        columns.Add(fieldData);
                        continue;
                    }
                    if (field.DatatypeOfSource == typeof (DateTime?))
                    {
                        var fieldDataType = fieldDataGenericType.MakeGenericType(new[] {typeof (DateTime?), field.DatatypeOfTarget});
                        var buffer = new char[field.LengthOfSource];
                        var readed = streamReader.Read(buffer, 0, field.LengthOfSource);
                        var stringValue = new string(buffer, 0, readed).Trim();
                        if (field.Nullable && string.IsNullOrEmpty(stringValue) && fieldDataType.IsValueType == false)
                        {
                            columns.Add(CreateEmptyFieldData(fieldDataType, field));
                            continue;
                        }
                        var dateTimeValue = stringValue.IndexOf('T') > 0 ? new DateTime(int.Parse(stringValue.Substring(0, 4)), int.Parse(stringValue.Substring(4, 2)), int.Parse(stringValue.Substring(6, 2)), int.Parse(stringValue.Substring(9, 2)), int.Parse(stringValue.Substring(11, 2)), int.Parse(stringValue.Substring(13, 2)), int.Parse(stringValue.Substring(15, 2))) : new DateTime(int.Parse(stringValue.Substring(0, 4)), int.Parse(stringValue.Substring(4, 2)), int.Parse(stringValue.Substring(6, 2)));
                        var fieldData = Activator.CreateInstance(fieldDataType, new object[] {field, dateTimeValue}) as IDataObjectBase;
                        if (fieldData == null)
                        {
                            throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.UnableToCreateInstanceOfType, fieldDataType.Name));
                        }
                        columns.Add(fieldData);
                        continue;
                    }
                    if (field.DatatypeOfSource == typeof (TimeSpan?))
                    {
                        var fieldDataType = fieldDataGenericType.MakeGenericType(new[] {typeof (TimeSpan?), field.DatatypeOfTarget});
                        var buffer = new char[field.LengthOfSource];
                        var readed = streamReader.Read(buffer, 0, field.LengthOfSource);
                        var stringValue = new string(buffer, 0, readed).Trim();
                        if (field.Nullable && string.IsNullOrEmpty(stringValue))
                        {
                            columns.Add(CreateEmptyFieldData(fieldDataType, field));
                            continue;
                        }
                        var timeSpanValue = new TimeSpan(0, int.Parse(stringValue.Substring(0, 2)), int.Parse(stringValue.Substring(2, 2)), int.Parse(stringValue.Substring(4, 2)), int.Parse(stringValue.Substring(7)) == 0 ? 0 : 100/int.Parse(stringValue.Substring(7)));
                        var fieldData = Activator.CreateInstance(fieldDataType, new object[] {field, timeSpanValue}) as IDataObjectBase;
                        if (fieldData == null)
                        {
                            throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.UnableToCreateInstanceOfType, fieldDataType.Name));
                        }
                        columns.Add(fieldData);
                        continue;
                    }
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.IllegalValue, field.DatatypeOfSource, "DatatypeOfSource"));
                }
                catch (DeliveryEngineRepositoryException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.ErrorReadingFieldValue, field.NameSource, table.NameSource, ex.Message), ex);
                }
            }
            return columns;
        }

        /// <summary>
        /// Creates an empty data object for a field.
        /// </summary>
        /// <param name="fieldDataType">Type of data object.</param>
        /// <param name="field">Field.</param>
        /// <returns>Empty data object.</returns>
        private static IDataObjectBase CreateEmptyFieldData(Type fieldDataType, IField field)
        {
            if (fieldDataType == null)
            {
                throw new ArgumentNullException("fieldDataType");
            }
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }
            var emptyFieldData = Activator.CreateInstance(fieldDataType, new object[] {field, null}) as IDataObjectBase;
            if (emptyFieldData == null)
            {
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.UnableToCreateInstanceOfType, fieldDataType.Name));
            }
            return emptyFieldData;
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Clones the data repository.
        /// </summary>
        /// <returns>Cloned data repository.</returns>
        public virtual object Clone()
        {
            var clonedDataRepository = new OldToNewDataRepository(_tableDictionary);
            if (OnClone != null)
            {
                OnClone.Invoke(this, new CloneOldToNewDataRepositoryEventArgs(clonedDataRepository));
            }
            return clonedDataRepository;
        }

        #endregion
    }
}
