using System;
using System.Collections.Generic;
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
            if (path == null) throw new ArgumentNullException("path");
            if (!path.Exists) throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.DirectoryNotFound, path.FullName));

            DestinationFolder = path;
            _currentMedia = 0;
            _tableCache.Clear();
        }

        #endregion

        public DirectoryInfo DestinationFolder { get; private set; }

        public IDataSource DataSource
        {
            get
            {
                if (_dataSource == null) throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.DataSourceNotSet));
                return _dataSource;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("DataSource" + "");
                if (_dataSource != null) throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.DataSourceAlreadySet));

                _dataSource = value;
            }
        }

        #region TableData

        public void ArchiveTableData(IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>> tableData)
        {
            if (tableData == null)
            {
                throw new ArgumentNullException("tableData");
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
                    if (_tableCache.TryGetValue(table, out tableParams) == false)
                    {
                        var tableNo = _tableCache.Count + 1;
                        // Create the table folder.
                        var tableFolder = String.Format(@"{0}\table{1}", TablesFolder, tableNo);
                        CreateDirectory(tableFolder);
                        // Create file information for table data file.
                        var tableXmlFileInfo = new FileInfo(String.Format(@"{0}\table{1}.xml", tableFolder, tableNo));
                        // Create the XML schema for the table.
                        var tableXsdFileInfo = new FileInfo(String.Format(@"{0}\table{1}.xsd", tableFolder, tableNo));
                        var tableXsd = ArchiveTableXsd(tableXsdFileInfo, table, tableNo);
                        // Append to cache.
                        _tableCache.Add(table, new Tuple<int, FileInfo, TableXsd>(tableNo, tableXmlFileInfo, tableXsd));
                        tableParams = _tableCache[table];
                    }

                    var rowCount = ArchiveTableXml(tableParams.Item2, tableParams.Item3, data.Value, tableParams.Item1);
                    _tableIndex.AddTable(table, String.Format("table{0}", tableParams.Item1), rowCount);
                }
                _tableIndex.Persist();
                _fileIndex.Persist();
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
                throw new ArgumentNullException("path");
            }
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            if (tableNo < 1)
            {
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.IllegalValue, tableNo, "tableNo"));
            }

            var columnId = 1;
            var xsd = new TableXsd(table, tableNo, path, _fileIndex);
            foreach (var field in table.Fields.Where(m => ExcludeField(m) == false))
            {
                field.ColumnId = string.Format("c{0}", columnId++);
                xsd.AddColumn(field.ColumnId, field.DatatypeOfTarget, field.Nullable);
            }

            xsd.Perist();
            return xsd;
        }

        private int ArchiveTableXml(FileInfo path, TableXsd tableXsd, IEnumerable<IEnumerable<IDataObjectBase>> dataRows, int tableNo)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (tableXsd == null)
            {
                throw new ArgumentNullException("tableXsd");
            }
            if (dataRows == null)
            {
                throw new ArgumentNullException("dataRows");
            }
            if (tableNo < 1)
            {
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.IllegalValue, tableNo, "tableNo"));
            }

            var tableXml = path.Exists ? TableXml.Load(tableXsd, path, _fileIndex, tableNo) : new TableXml(tableXsd, path, _fileIndex, tableNo);

            var rowCount = 0;
            foreach (var dataRow in dataRows)
            {
                tableXml.AddRow();
                foreach (var dataObject in dataRow.Where(m => ExcludeField(m.Field) == false))
                {
                    var genericMethodForValue = dataObject.GetType()
                                                          .GetMethod("GetTargetValue", new Type[] {})
                                                          .MakeGenericMethod(dataObject.Field.DatatypeOfTarget);
                    var value = genericMethodForValue.Invoke(dataObject, null);
                    tableXml.AddField(dataObject.Field.ColumnId, value);
                }
                rowCount++;
            }
            
            tableXml.Persist();
            return rowCount;
        }

        #endregion

        #region MetaData

        public void ArchiveMetaData()
        {
            try
            {
                foreach (var directory in DestinationFolder.GetDirectories(string.Format("{0}.*", DataSource.ArchiveInformationPackageId)))
                {
                    directory.Delete(true);
                }

                var fileIndexPath = new FileInfo(String.Format(@"{0}\{1}", IndicesFolder, "fileIndex.xml"));
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

            ArchiveEmbededResource("Schemas.standard.archiveIndex.xsd", new FileInfo(String.Format(@"{0}\{1}", SchemasStandardFolder, "archiveIndex.xsd")));
            ArchiveEmbededResource("Schemas.standard.contextDocumentationIndex.xsd", new FileInfo(String.Format(@"{0}\{1}", SchemasStandardFolder, "contextDocumentationIndex.xsd")));
            ArchiveEmbededResource("Schemas.standard.fileIndex.xsd", new FileInfo(String.Format(@"{0}\{1}", SchemasStandardFolder, "fileIndex.xsd")));
            ArchiveEmbededResource("Schemas.standard.tableIndex.xsd", new FileInfo(String.Format(@"{0}\{1}", SchemasStandardFolder, "tableIndex.xsd")));
            ArchiveEmbededResource("Schemas.standard.XMLSchema.xsd", new FileInfo(String.Format(@"{0}\{1}", SchemasStandardFolder, "XMLSchema.xsd")));
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

            var archiveIndexPath = new FileInfo(String.Format(@"{0}\{1}", IndicesFolder, "archiveIndex.xml"));
            var archiveIndex = new ArchiveIndex(DataSource, archiveIndexPath, _fileIndex);
            archiveIndex.Persist();

            var contextDocumentationIndexPath = new FileInfo(String.Format(@"{0}\{1}", IndicesFolder, "contextDocumentationIndex.xml"));
            var contextDocumentationIndex = new ContextDocumentationIndex(DataSource.ContextDocuments, contextDocumentationIndexPath, _fileIndex);
            contextDocumentationIndex.Persist();

            var tableIndexPath = new FileInfo(String.Format(@"{0}\{1}", IndicesFolder, "tableIndex.xml"));
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
                    CreateDirectory(String.Format(@"{0}\docCollection{1}", ContextDocumentationFolder, docCollectionCount));
                }

                CreateDirectory(String.Format(@"{0}\docCollection{1}\{2}", ContextDocumentationFolder, docCollectionCount, contextDocument.Id));

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

                    var destination = String.Format(@"{0}\docCollection{1}\{2}\{3}{4}", ContextDocumentationFolder, docCollectionCount, contextDocument.Id, fileCount[extension], extension);
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
            if (resourceName == null) throw new ArgumentNullException("resourceName");
            if (file == null) throw new ArgumentNullException("file");

            var assembly = GetType().Assembly;
            var assemblyName = assembly.GetName().Name;
            using (var resourceStream = assembly.GetManifestResourceStream(string.Format("{0}.{1}", assemblyName, resourceName)))
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

        public static bool ExcludeField(IField field)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }
            var table = field.Table;
            if (table == null)
            {
                return false;
            }
            return table.FieldFilters.Count > 0 && table.FieldFilters.Any(m => m.Exclude(field));
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
            if (directory == null) throw new ArgumentNullException("directory");

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

            return String.Format(@"{0}\{1}.{2}", DestinationFolder, DataSource.ArchiveInformationPackageId, mediaNumber);
        }

        private string IndicesFolder { get { return String.Format(@"{0}\{1}", MediaFolderPath(1), "Indices"); } }

        private string TablesFolder { get { return String.Format(@"{0}\{1}", MediaFolderPath(1), "Tables"); } }
        private string ContextDocumentationFolder { get { return String.Format(@"{0}\{1}", MediaFolderPath(1), "ContextDocumentation"); } }
        private string SchemasFolder { get { return String.Format(@"{0}\{1}", MediaFolderPath(1), "Schemas"); } }
        private string SchemasStandardFolder { get { return String.Format(@"{0}\{1}", SchemasFolder, "standard"); } }
        private string SchemasLocalSharedFolder { get { return String.Format(@"{0}\{1}", SchemasFolder, "localShared"); } }
        //private string DocumentsFolder { get { return String.Format(@"{0}\{1}", MediaFolderPath(1), "Documents"); } }
    }
}
