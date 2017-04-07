using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.Indices;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Repositories.Tables;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Repositories
{
    public class ArchiveVersionRepository : IArchiveVersionRepository
    {
        #region Member Variables

        private IDataSource _dataSource;
        private int _currentMedia;
        private FileIndex _fileIndex;
        private TableIndex _tableIndex;
        private readonly IDictionary<ITable, Tuple<int, FileInfo, TableXsd>> _tableCache = new Dictionary<ITable, Tuple<int, FileInfo, TableXsd>>();

        #endregion

        #region Constructors

        public ArchiveVersionRepository(DirectoryInfo path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (!path.Exists) throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.DirectoryNotFound, path.FullName));

            DestinationFolder = path;
            _currentMedia = 0;
            _tableCache.Clear();
        }

        #endregion

        public DirectoryInfo DestinationFolder { get; }

        public IDataSource DataSource
        {
            get
            {
                if (_dataSource == null) throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.DataSourceNotSet));
                return _dataSource;
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (_dataSource != null) throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.DataSourceAlreadySet));

                _dataSource = value;
            }
        }

        #region TableData

        public void ArchiveTableData(IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>> tableData, object syncRoot)
        {
            if (tableData == null)
            {
                throw new ArgumentNullException(nameof(tableData));
            }
            if (syncRoot == null)
            {
                throw new ArgumentNullException(nameof(syncRoot));
            }

            try
            {
                CreateDirectory(TablesFolder);
                foreach (var data in tableData)
                {
                    if (data.Value.Any() == false)
                    {
                        continue;
                    }

                    var table = data.Key;
                    Tuple<int, FileInfo, TableXsd> tableParams;
                    lock (syncRoot)
                    {
                        if (_tableCache.TryGetValue(table, out tableParams) == false)
                        {
                            var tableNo = _tableCache.Count + 1;
                            // Create the table folder.
                            var tableFolder = $@"{TablesFolder}\table{tableNo}";
                            CreateDirectory(tableFolder);
                            // Create file information for table data file.
                            var tableXmlFileInfo = new FileInfo($@"{tableFolder}\table{tableNo}.xml");
                            // Create the XML schema for the table.
                            var tableXsdFileInfo = new FileInfo($@"{tableFolder}\table{tableNo}.xsd");
                            var tableXsd = ArchiveTableXsd(tableXsdFileInfo, table, tableNo);
                            // Append to cache.
                            _tableCache.Add(table, new Tuple<int, FileInfo, TableXsd>(tableNo, tableXmlFileInfo, tableXsd));
                            tableParams = _tableCache[table];
                        }
                    }

                    var rowCount = ArchiveTableXml(tableParams.Item2, tableParams.Item3, table, data.Value, tableParams.Item1, syncRoot);
                    lock (syncRoot)
                    {
                        _tableIndex.AddTable(table, $"table{tableParams.Item1}", rowCount);
                    }
                }
                lock (syncRoot)
                {
                    _tableIndex.Persist();
                    _fileIndex.Persist();
                }
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException == null)
                {
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.RepositoryError, GetType().Name, ex.Message), ex);
                }
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.RepositoryError, GetType().Name, ex.InnerException.Message), ex.InnerException);
            }
            catch (DeliveryEngineRepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.RepositoryError, GetType().Name, ex.Message), ex);
            }
        }

        private TableXsd ArchiveTableXsd(FileInfo path, ITable table, int tableNo)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }
            if (tableNo < 1)
            {
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.IllegalValue, tableNo, "tableNo"));
            }

            ReadOnlyObservableCollection<IFilter> fieldFilters = table.FieldFilters;

            var columnId = 1;
            var xsd = new TableXsd(table, tableNo, path, _fileIndex);
            foreach (var field in table.Fields.Where(m => ExcludeField(m, fieldFilters) == false))
            {
                field.ColumnId = $"c{columnId++}";
                xsd.AddColumn(field.ColumnId, field.DatatypeOfTarget, field.Nullable);
            }

            xsd.Perist();
            return xsd;
        }

        private int ArchiveTableXml(FileInfo path, TableXsd tableXsd, ITable table, IEnumerable<IEnumerable<IDataObjectBase>> dataRows, int tableNo, object syncRoot)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (tableXsd == null)
            {
                throw new ArgumentNullException(nameof(tableXsd));
            }
            if (dataRows == null)
            {
                throw new ArgumentNullException(nameof(dataRows));
            }
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }
            if (tableNo < 1)
            {
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.IllegalValue, tableNo, "tableNo"));
            }
            if (syncRoot == null)
            {
                throw new ArgumentNullException(nameof(syncRoot));
            }

            ReadOnlyObservableCollection<IFilter> fieldFilters = table.FieldFilters;
            using (var tableXml = new TableXml(tableXsd, path, _fileIndex, tableNo))
            {
                var rowCount = 0;
                foreach (var dataRow in dataRows)
                {
                    tableXml.AddRow();
                    foreach (var dataObject in dataRow.Where(m => ExcludeField(m.Field, fieldFilters) == false))
                    {
                        var genericMethodForValue = dataObject.GetType()
                            .GetMethod("GetTargetValue", new Type[] { })
                            .MakeGenericMethod(dataObject.Field.DatatypeOfTarget);
                        var value = genericMethodForValue.Invoke(dataObject, null);
                        tableXml.AddField(dataObject.Field.ColumnId, value);
                    }
                    tableXml.CloseRow();
                    rowCount++;
                }

                tableXml.Persist(syncRoot);

                return rowCount;
            }
        }

        #endregion

        #region MetaData

        public void ArchiveMetaData()
        {
            try
            {
                foreach (var directory in DestinationFolder.GetDirectories($"{DataSource.ArchiveInformationPackageId}.*"))
                {
                    directory.Delete(true);
                }

                var fileIndexPath = new FileInfo($@"{IndicesFolder}\{"fileIndex.xml"}");
                _fileIndex = new FileIndex(fileIndexPath, DestinationFolder);

                CreateNewMedia();
                ArchiveStandardSchemas();
                ArchiveLocalSchemas();
                ArchiveIndices();
                ArchiveContextDocumentation();

                _fileIndex.Persist();
            }
            catch (DeliveryEngineRepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.RepositoryError, GetType().Name, ex.Message), ex);
            }
        }

        private void ArchiveStandardSchemas()
        {
            Directory.CreateDirectory(SchemasFolder);
            Directory.CreateDirectory(SchemasStandardFolder);

            ArchiveEmbededResource("Schemas.standard.archiveIndex.xsd", new FileInfo($@"{SchemasStandardFolder}\{"archiveIndex.xsd"}"));
            ArchiveEmbededResource("Schemas.standard.contextDocumentationIndex.xsd", new FileInfo($@"{SchemasStandardFolder}\{"contextDocumentationIndex.xsd"}"));
            ArchiveEmbededResource("Schemas.standard.fileIndex.xsd", new FileInfo($@"{SchemasStandardFolder}\{"fileIndex.xsd"}"));
            ArchiveEmbededResource("Schemas.standard.tableIndex.xsd", new FileInfo($@"{SchemasStandardFolder}\{"tableIndex.xsd"}"));
            ArchiveEmbededResource("Schemas.standard.XMLSchema.xsd", new FileInfo($@"{SchemasStandardFolder}\{"XMLSchema.xsd"}"));
        }

        private void ArchiveLocalSchemas()
        {
            Directory.CreateDirectory(SchemasFolder);
            Directory.CreateDirectory(SchemasLocalSharedFolder);
            // TODO NiceToHave - add evt. gml skemaer
        }

        private void ArchiveIndices()
        {
            CreateDirectory(IndicesFolder);

            var archiveIndexPath = new FileInfo($@"{IndicesFolder}\{"archiveIndex.xml"}");
            var archiveIndex = new ArchiveIndex(DataSource, archiveIndexPath, _fileIndex);
            archiveIndex.Persist();

            var contextDocumentationIndexPath = new FileInfo($@"{IndicesFolder}\{"contextDocumentationIndex.xml"}");
            var contextDocumentationIndex = new ContextDocumentationIndex(DataSource.ContextDocuments, contextDocumentationIndexPath, _fileIndex);
            contextDocumentationIndex.Persist();

            var tableIndexPath = new FileInfo($@"{IndicesFolder}\{"tableIndex.xml"}");
            _tableIndex = new TableIndex(DataSource, tableIndexPath, _fileIndex);
            _tableIndex.Persist();

            // TODO NiceToHave - Add and build docIndex if present
            //ArchiveEmbededResource("Schemas.standard.docIndex.xsd", new FileInfo(String.Format(@"{0}\{1}", SchemasStandardFolder, "docIndex.xsd")));

            _fileIndex.Persist();
        }

        private void ArchiveContextDocumentation()
        {
            CreateDirectory(ContextDocumentationFolder);

            var contextDocumentCount = 0;
            var docCollectionCount = 0;
            foreach (var contextDocument in DataSource.ContextDocuments)
            {
                contextDocumentCount++;
                if (contextDocumentCount % 10000 == 1)
                {
                    docCollectionCount++;
                    CreateDirectory($@"{ContextDocumentationFolder}\docCollection{docCollectionCount}");
                }

                CreateDirectory($@"{ContextDocumentationFolder}\docCollection{docCollectionCount}\{contextDocument.Id}");

                var path = contextDocument.Reference;

                var directory = Path.GetDirectoryName(path);
                var extension = Path.GetExtension(path);
                var searchPattern = Path.GetFileName(path);

                if (directory == null || extension == null || searchPattern == null)
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.FileNotFound, path));

                var files = Directory.GetFiles(directory, searchPattern);

                var fileCount = new Dictionary<string, int>();
                foreach (var file in files)
                {
                    if (fileCount.ContainsKey(extension) == false)
                        fileCount.Add(extension, 0);
                    fileCount[extension]++;

                    var destination = $@"{ContextDocumentationFolder}\docCollection{docCollectionCount}\{contextDocument.Id}\{fileCount[extension]}{extension}";
                    try
                    {
                        File.Copy(file, destination, true);
                    }
                    catch (Exception ex)
                    {
                        throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.FileCopyError, file, destination, ex));
                    }

                    _fileIndex.AddFile(new FileInfo(destination));
                }
            }
        }

        private void ArchiveEmbededResource(string resourceName, FileInfo file)
        {
            if (resourceName == null) throw new ArgumentNullException(nameof(resourceName));
            if (file == null) throw new ArgumentNullException(nameof(file));

            var assembly = GetType().Assembly;
            var assemblyName = assembly.GetName().Name;
            using (var resourceStream = assembly.GetManifestResourceStream($"{assemblyName}.{resourceName}"))
            {
                if (resourceStream == null) throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.ResourceNotFound, resourceName));

                try
                {
                    using (var fileStream = new FileStream(file.FullName, FileMode.Create, FileAccess.Write))
                    {
                        var buffer = new byte[1024];
                        var bytesRead = resourceStream.Read(buffer, 0, buffer.Length);
                        while (bytesRead > 0)
                        {
                            fileStream.Write(buffer, 0, bytesRead);
                            bytesRead = resourceStream.Read(buffer, 0, buffer.Length);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.FileWriteError, file), ex);
                }
            }

            _fileIndex.AddFile(file);
        }

        #endregion

        public static bool ExcludeField(IField field, ReadOnlyObservableCollection<IFilter> fieldFilters)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }
            if (fieldFilters == null)
            {
                throw new ArgumentNullException(nameof(fieldFilters));
            }
            return fieldFilters.Count > 0 && fieldFilters.Any(m => m.Exclude(field));
        }

        private void CreateNewMedia()
        {
            _currentMedia += 1;
            CreateDirectory(MediaFolderPath(_currentMedia));

            if (_currentMedia == 1)
            {
                CreateDirectory(IndicesFolder);
            }
        }

        private static void CreateDirectory(string directory)
        {
            if (directory == null) throw new ArgumentNullException(nameof(directory));

            try
            {
                Directory.CreateDirectory(directory);
            }
            catch (Exception ex)
            {
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.DirectoryCreateError, directory), ex);
            }
        }

        private string MediaFolderPath(int mediaNumber)
        {
            if (mediaNumber < 1) throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.IllegalValue, mediaNumber, "mediaNumber"));

            return $@"{DestinationFolder}\{DataSource.ArchiveInformationPackageId}.{mediaNumber}";
        }

        private string IndicesFolder => $@"{MediaFolderPath(1)}\{"Indices"}";

        private string TablesFolder => $@"{MediaFolderPath(1)}\{"Tables"}";
        private string ContextDocumentationFolder => $@"{MediaFolderPath(1)}\{"ContextDocumentation"}";
        private string SchemasFolder => $@"{MediaFolderPath(1)}\{"Schemas"}";
        private string SchemasStandardFolder => $@"{SchemasFolder}\{"standard"}";
        private string SchemasLocalSharedFolder => $@"{SchemasFolder}\{"localShared"}";
        //private string DocumentsFolder { get { return String.Format(@"{0}\{1}", MediaFolderPath(1), "Documents"); } }
    }
}
