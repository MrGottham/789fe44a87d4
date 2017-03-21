using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Domain.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Repositories.Metadata.OldToNew
{
    /// <summary>
    /// Metadata repository for converting old delivery format to the new delivery format.
    /// </summary>
    public class OldToNewMetadataRepository : IMetadataRepository
    {
        #region Private constants

        private const string MetadataExtensionNamespace = "urn:dsinext:deliveryengine:metadata:1.0.0";

        #endregion

        #region Private variables

        private readonly DirectoryInfo _path;
        private readonly IConfigurationValues _configuration;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates metadata repository for converting old delivery format to the new delivery format.
        /// </summary>
        /// <param name="path">Path containing the old delivery format.</param>
        /// <param name="configuration">Configuration values to the metadata repository for converting old delivery format to the new delivery format.</param>
        public OldToNewMetadataRepository(DirectoryInfo path, IConfigurationValues configuration)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (!path.Exists)
            {
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.DirectoryNotFound, path.FullName));
            }
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            _path = path;
            _configuration = configuration;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <returns>Data source.</returns>
        public virtual IDataSource DataSourceGet()
        {
            try
            {
                var files = _path.GetFiles("ARKVER.TAB", SearchOption.AllDirectories);
                if (files.Length == 0)
                {
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.FileNotFound, "ARKVER.TAB"));
                }

                var archiveNumber = 0;
                var archiveType = '\0';
                var previousArchiveNumber = 0;
                string systemName = null;
                var fromSystemDate = DateTime.MinValue;
                var toSystemDate = DateTime.MaxValue;
                using (var fileStream = files[0].Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    using (var streamReader = new StreamReader(fileStream, Encoding.Default))
                    {
                        while (!streamReader.EndOfStream)
                        {
                            var streamLine = streamReader.ReadLine();
                            if (string.IsNullOrEmpty(streamLine))
                            {
                                continue;
                            }
                            archiveNumber = int.Parse(streamLine.Substring(0, 8));
                            archiveType = char.Parse(streamLine.Substring(8, 1));
                            if (!string.IsNullOrEmpty(streamLine.Substring(17, 8).Trim()))
                            {
                                previousArchiveNumber = int.Parse(streamLine.Substring(17, 8).Trim());
                            }
                            systemName = streamLine.Substring(25, 256).Trim();
                            fromSystemDate = ToDateTime(streamLine.Substring(281, 8));
                            toSystemDate = ToDateTime(streamLine.Substring(289, 8));
                            break;
                        }
                        streamReader.Close();
                    }
                    fileStream.Close();
                }

                var dataSource = ReadAndCreateMetadata(archiveNumber, archiveType, previousArchiveNumber, systemName, fromSystemDate, toSystemDate);
                return dataSource;
            }
            catch (DeliveryEngineRepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.RepositoryError, MethodBase.GetCurrentMethod().Name, ex.Message));
            }
        }

        /// <summary>
        /// Read and create metadata from the old delivery format.
        /// </summary>
        /// <param name="archiveNumber">Archive number for the old delivery format.</param>
        /// <param name="archiveType">Type of the archive in the old delivery format.</param>
        /// <param name="previousArchiveNumber">Archive number for an previous delivery.</param>
        /// <param name="systemName">System name.</param>
        /// <param name="fromSystemDate">Start date for system delivery period.</param>
        /// <param name="toSystemDate">End date for system delivery period.</param>
        /// <returns>Datasource based on the old delivery format.</returns>
        private IDataSource ReadAndCreateMetadata(int archiveNumber, char archiveType, int previousArchiveNumber, string systemName, DateTime fromSystemDate, DateTime toSystemDate)
        {
            if (archiveType == '\0')
            {
                throw new ArgumentNullException("archiveType");
            }
            if (string.IsNullOrEmpty(systemName))
            {
                throw new ArgumentNullException("systemName");
            }

            IDataSource dataSource = new DataSource(archiveNumber.ToString(CultureInfo.InvariantCulture), systemName, systemName, systemName, fromSystemDate, toSystemDate)
                                         {
                                             ArchiveInformationPackageIdPrevious = previousArchiveNumber,
                                             ArchiveInformationPacketType = _configuration.ArchiveInformationPacketType,
                                             ArchiveType = (archiveType == 'A' || archiveType == '1' || archiveType == '2'),
                                             SystemPurpose = _configuration.SystemPurpose,
                                             SystemContent = _configuration.SystemContent,
                                             RegionNum = _configuration.RegionNum,
                                             KomNum = _configuration.KomNum,
                                             CprNum = _configuration.CprNum,
                                             CvrNum = _configuration.CvrNum,
                                             MatrikNum = _configuration.MatrikNum,
                                             BbrNum = _configuration.BbrNum,
                                             WhoSygKod = _configuration.WhoSygKod,
                                             FormVersion = _configuration.FormVersion,
                                             ContainsDigitalDocuments = _configuration.ContainsDigitalDocuments,
                                             SearchRelatedOtherRecords = _configuration.SearchRelatedOtherRecords,
                                             SystemFileConcept = _configuration.SystemFileConcept,
                                             MultipleDataCollection = _configuration.MultipleDataCollection,
                                             PersonalDataRestrictedInfo = _configuration.PersonalDataRestrictedInfo,
                                             OtherAccessTypeRestrictions = _configuration.OtherAccessTypeRestrictions,
                                             ArchiveApproval = _configuration.ArchiveApproval,
                                             ArchiveRestrictions = string.IsNullOrEmpty(_configuration.ArchiveRestrictions) ? null : _configuration.ArchiveRestrictions
                                         };
            foreach (var alternativeSystemName in _configuration.AlternativeSystemNames)
            {
                dataSource.AddAlternativeSystemName(alternativeSystemName);
            }
            foreach (var sourceName in _configuration.SourceNames)
            {
                dataSource.AddSourceName(sourceName);
            }
            foreach (var userName in _configuration.UserNames)
            {
                dataSource.AddUserName(userName);
            }
            foreach (var predecessorName in _configuration.PredecessorNames)
            {
                dataSource.AddPredecessorName(predecessorName);
            }
            foreach (var formClass in _configuration.FormClasses)
            {
                dataSource.AddFormClass(formClass);
            }
            foreach (var relatedRecordName in _configuration.RelatedRecordsNames)
            {
                dataSource.AddRelatedRecordsName(relatedRecordName);
            }

            using (var fileStream = new FileStream(string.Format("{0}{1}{2:00000}001{3}{4:00000000}{5}{6:00000000}.xml", _path.FullName, Path.DirectorySeparatorChar, archiveNumber, Path.DirectorySeparatorChar, archiveNumber, Path.DirectorySeparatorChar, archiveNumber), FileMode.Open, FileAccess.Read, FileShare.None))
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(fileStream);
                var namespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
                if (xmlDocument.DocumentElement != null)
                {
                    namespaceManager.AddNamespace("ns", xmlDocument.DocumentElement.NamespaceURI);
                    namespaceManager.AddNamespace("metadataExtension", MetadataExtensionNamespace);
                }
                foreach (var table in GetTables(xmlDocument.SelectNodes("/ns:arkiveringsversion/ns:tabel", namespaceManager), namespaceManager))
                {
                    dataSource.AddTable(table);
                }
                foreach (var table in dataSource.Tables)
                {
                    foreach (var foreignKey in GetForeignKeys(xmlDocument.SelectNodes(string.Format("/ns:arkiveringsversion/ns:tabel[ns:titel='{0}']/ns:fn", table.NameSource), namespaceManager), namespaceManager, dataSource, table))
                    {
                        table.AddForeignKey(foreignKey);
                    }
                }
                foreach(var view in GetViews(xmlDocument.SelectNodes("/ns:arkiveringsversion/ns:saq", namespaceManager), namespaceManager))
                {
                    dataSource.AddView(view);
                }
                AppendMetadataExtensions(xmlDocument.DocumentElement, namespaceManager, dataSource);
                fileStream.Close();
            }

            FileInfo fileInfo;
            var fileDictionary = new Dictionary<string, FileInfo>();
            using (var fileStream = new FileStream(string.Format("{0}{1}{2:00000}001{3}{4:00000000}{5}Filmap.tab", _path.FullName, Path.DirectorySeparatorChar, archiveNumber, Path.DirectorySeparatorChar, archiveNumber, Path.DirectorySeparatorChar), FileMode.Open, FileAccess.Read, FileShare.None))
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
                        fileInfo = new FileInfo(string.Format("{0}{1}{2}{3}{4:00000000}{5}{6}", _path.FullName, Path.DirectorySeparatorChar, s.Substring(12, 8), Path.DirectorySeparatorChar, archiveNumber, Path.DirectorySeparatorChar, s.Substring(0, 12).Trim()));
                        if (fileInfo.Exists)
                        {
                            fileDictionary.Add(fileInfo.Name.ToUpper(), fileInfo);
                        }
                    }
                    streamReader.Close();
                }
                fileStream.Close();
            }

            if (!fileDictionary.TryGetValue("SKABER.TAB", out fileInfo))
            {
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.FileNotFound, "SKABER.TAB"));
            }
            using (var fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.None))
            {
                using (var streamReader = new StreamReader(fileStream, Encoding.Default))
                {
                    var buffer = new char[256 + 8 + 8];
                    while (!streamReader.EndOfStream)
                    {
                        var readedBytes = streamReader.Read(buffer, 0, 256 + 8 + 8);
                        var s = new string(buffer, 0, readedBytes);
                        if (string.IsNullOrEmpty(s))
                        {
                            continue;
                        }
                        var creatorName = s.Substring(0, 256).Trim();
                        var periodStart = ToDateTime(s.Substring(256, 8).Trim());
                        var periodEnd = ToDateTime(s.Substring(264, 8).Trim());
                        dataSource.AddCreator(new Creator(creatorName, creatorName, periodStart, periodEnd));
                    }
                    streamReader.Close();
                }
                fileStream.Close();
            }

            if (!fileDictionary.TryGetValue("GENINFO.TAB", out fileInfo))
            {
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.FileNotFound, "GENINFO.TAB"));
            }
            using (var fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.None))
            {
                using (var streamReader = new StreamReader(fileStream, Encoding.Default))
                {
                    var buffer = new char[8 + 8 + 256];
                    while (!streamReader.EndOfStream)
                    {
                        var readedBytes = streamReader.Read(buffer, 0, 8 + 8 + 256);
                        var s = new string(buffer, 0, readedBytes);
                        if (string.IsNullOrEmpty(s))
                        {
                            continue;
                        }
                        var contextDocumentId = dataSource.ContextDocuments.Count + 1;
                        var contextDocumentName = s.Substring(16, 256).Trim();
                        var contextDocumentReference = string.Format("{0}{1}{2}{3}{4:00000000}{5}{6}{7}*.TIF", _path.FullName, Path.DirectorySeparatorChar, s.Substring(0, 8), Path.DirectorySeparatorChar, archiveNumber, Path.DirectorySeparatorChar, s.Substring(8, 8).Trim(), Path.DirectorySeparatorChar);
                        var contextDocumentDate = _configuration.ContextDocumentDates
                            .Where(m => m.Key.Equals(s.Substring(8, 8).Trim().ToUpper()))
                            .Select(m => m.Value)
                            .FirstOrDefault();
                        var contextDocumentAuthors = new List<IDocumentAuthor>(0);
                        var authorValues = _configuration.ContextDocumentAuthors
                            .Where(m => m.Key.Equals(s.Substring(8, 8).Trim().ToUpper()) && m.Value != null && m.Value.Any())
                            .Select(m => m.Value)
                            .FirstOrDefault();
                        if (authorValues != null)
                        {
                            contextDocumentAuthors = authorValues.ToList();
                        }
                        var contextDocumentCategories = new List<ContextDocumentCategories> {ContextDocumentCategories.InformationOther};
                        var categoriesValues = _configuration.ContextDocumentCategories
                            .Where(m => m.Key.Equals(s.Substring(8, 8).Trim().ToUpper()) && m.Value != null && m.Value.Any())
                            .Select(m => m.Value)
                            .FirstOrDefault();
                        if (categoriesValues != null)
                        {
                            contextDocumentCategories = categoriesValues.ToList();
                        }
                        var contextDocument = new ContextDocument(contextDocumentId, contextDocumentName, contextDocumentName, contextDocumentReference, contextDocumentCategories.First());
                        if (contextDocumentDate.Value != DateTime.MinValue)
                        {
                            contextDocument.DocumentDatePresicion = contextDocumentDate.Key;
                            contextDocument.DocumentDate = contextDocumentDate.Value;
                        }
                        foreach (var contextDocumentAuthor in contextDocumentAuthors)
                        {
                            contextDocument.AddDocumentAuthor(contextDocumentAuthor);
                        }
                        foreach (var contextDocumentCatagory in contextDocumentCategories.Where(m => contextDocument.Categories.Contains(m) == false))
                        {
                            contextDocument.AddCategory(contextDocumentCatagory);
                        }
                        dataSource.AddContextDocument(contextDocument);
                    }
                    streamReader.Close();
                }
                fileStream.Close();
            }

            return dataSource;
        }

        /// <summary>
        /// Gets a list of tables from the node list of tables.
        /// </summary>
        /// <param name="tableNodeList">Node list of tables.</param>
        /// <param name="namespaceManager">Namespace manager.</param>
        /// <returns>List of tables.</returns>
        private static IEnumerable<ITable> GetTables(XmlNodeList tableNodeList, XmlNamespaceManager namespaceManager)
        {
            if (tableNodeList == null)
            {
                throw new ArgumentNullException("tableNodeList");
            }
            if (namespaceManager == null)
            {
                throw new ArgumentNullException("namespaceManager");
            }
            var tables = new List<ITable>(tableNodeList.OfType<XmlElement>().Count());
            foreach (var tableNode in tableNodeList.OfType<XmlElement>())
            {
                var tableName = GetNodeValue(tableNode.SelectSingleNode("ns:titel", namespaceManager));
                var tableDescription = GetNodeValue(tableNode.SelectSingleNode("ns:tabelinfo", namespaceManager));
                var table = new Table(tableName, tableName, tableDescription);

                foreach (var field in GetFields(tableNode.SelectNodes("ns:feltdef", namespaceManager), namespaceManager, table))
                {
                    table.AddField(field);
                }

                var primaryKey = GetPrimaryKey(tableNode.SelectSingleNode("ns:pn", namespaceManager), namespaceManager, table);
                table.AddCandidateKey(primaryKey);

                ConvertCodedValuesToMaps(tableNode.SelectNodes("ns:kodedef", namespaceManager), namespaceManager, table);

                tables.Add(table);
            }
            return tables;
        }

        /// <summary>
        /// Gets a list of fields from the node list of fields.
        /// </summary>
        /// <param name="fieldNodeList">Node list of fields.</param>
        /// <param name="namespaceManager">Namespace manager.</param>
        /// <param name="table">Table for the fields.</param>
        /// <returns>List of fields.</returns>
        private static IEnumerable<IField> GetFields(XmlNodeList fieldNodeList, XmlNamespaceManager namespaceManager, ITable table)
        {
            if (fieldNodeList == null)
            {
                throw new ArgumentNullException("fieldNodeList");
            }
            if (namespaceManager == null)
            {
                throw new ArgumentNullException("namespaceManager");
            }
            var fields = new List<IField>(fieldNodeList.OfType<XmlElement>().Count());
            foreach (var fieldNode in fieldNodeList.OfType<XmlElement>())
            {
                var fieldName = GetNodeValue(fieldNode.SelectSingleNode("ns:titel", namespaceManager));
                var targetFieldName = fieldName;
                if (Regex.IsMatch(targetFieldName, @"(\p{L}(_|\w)*)|(&quot;.*&quot;)") == false)
                {
                    targetFieldName = string.Format("RenamedField{0}", fields.Count(m => m.NameTarget.Contains("RenamedField")) + 1);
                }
                var fieldLength = int.Parse(GetNodeValue(fieldNode.SelectSingleNode("ns:bredde", namespaceManager)));
                Type fieldType;
                string originalFieldType;
                switch (GetNodeValue(fieldNode.SelectSingleNode("ns:datatype", namespaceManager)).ToLower())
                {
                    case "string":
                        fieldType = typeof (string);
                        originalFieldType = "Tekst";
                        break;

                    case "num":
                        fieldType = typeof (int?);
                        if (fieldLength > 9)
                        {
                            fieldType = typeof (long?);
                        }
                        originalFieldType = "Heltal";
                        break;

                    case "real":
                    case "exp":
                        fieldType = typeof (decimal?);
                        originalFieldType = "Decimaltal";
                        break;

                    case "date":
                        fieldType = typeof (DateTime?);
                        originalFieldType = "Dato";
                        break;

                    case "timestamp":
                        fieldType = typeof (DateTime?);
                        originalFieldType = "Tidsstempel";
                        break;

                    case "time":
                        fieldType = typeof (TimeSpan?);
                        originalFieldType = "Tidspunkt";
                        break;

                    default:
                        throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.IllegalValue, GetNodeValue(fieldNode.SelectSingleNode("ns:datatype", namespaceManager)), "datatype"));
                }
                var fieldDescription = GetNodeValue(fieldNode.SelectSingleNode("ns:feltinfo", namespaceManager));
                var field = new Field(fieldName, targetFieldName, fieldLength, fieldLength, fieldType, fieldType, table)
                    {
                        Description = fieldDescription,
                        ColumnId = string.Format("c{0}", fields.Count + 1),
                        OriginalDatatype = originalFieldType,
                        Nullable = true
                    };
                if (fieldNode.SelectSingleNode("ns:feltfunk", namespaceManager) != null)
                {
                    var fieldFunctionality = GetNodeValue(fieldNode.SelectSingleNode("ns:feltfunk", namespaceManager));
                    if (string.IsNullOrEmpty(fieldFunctionality))
                    {
                        continue;
                    }
                    fieldFunctionality = string.Format("{0}{1}", fieldFunctionality.Substring(0, 1).ToUpper(), fieldFunctionality.Substring(1)).Replace('/', '_');
                    switch (fieldFunctionality)
                    {
                        case "Form":
                            fieldFunctionality = "FORM";
                            break;
                    }
                    field.AddFunctionality(new MarkFunctionality(fieldFunctionality));
                }
                fields.Add(field);
            }
            return fields;
        }

        /// <summary>
        /// Gets the primary key from a node.
        /// </summary>
        /// <param name="primaryKeyNode">Primary key node.</param>
        /// <param name="namespaceManager">Namespace manager.</param>
        /// <param name="table">Table for the primary key.</param>
        /// <returns>Primary key.</returns>
        private static ICandidateKey GetPrimaryKey(XmlNode primaryKeyNode, XmlNamespaceManager namespaceManager, ITable table)
        {
            if (primaryKeyNode == null)
            {
                throw new ArgumentNullException("primaryKeyNode");
            }
            if (namespaceManager == null)
            {
                throw new ArgumentNullException("namespaceManager");
            }
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            
            var primaryKeyFieldNodes = primaryKeyNode.SelectNodes("ns:titel", namespaceManager);
            if (primaryKeyFieldNodes == null || !primaryKeyFieldNodes.OfType<XmlElement>().Any())
            {
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.MissingChildNode, "titel", primaryKeyNode.LocalName));
            }

            var primaryKey = new CandidateKey(string.Format("PK_{0}", table.NameSource.ToUpper()), string.Format("PK_{0}", table.NameTarget.ToUpper()), string.Format("Primær nøgle på {0}", table.NameTarget))
                                 {
                                     Table = table
                                 };
            foreach (var primaryKeyFieldNode in primaryKeyFieldNodes.OfType<XmlElement>())
            {
                var fieldName = GetNodeValue(primaryKeyFieldNode);
                var field = table.Fields.SingleOrDefault(m => String.Compare(m.NameSource, fieldName, StringComparison.OrdinalIgnoreCase) == 0);
                if (field == null)
                {
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.FieldNotFound, fieldName));
                }
                field.Nullable = false;
                primaryKey.AddField(field);
            }
            return primaryKey;
        }

        /// <summary>
        /// Converts coded values to maps on fields.
        /// </summary>
        /// <param name="codedValueNodeList">Node list of coded values.</param>
        /// <param name="namespaceManager">Namespace manager.</param>
        /// <param name="table">Table on which to map coded values.</param>
        private static void ConvertCodedValuesToMaps(IEnumerable codedValueNodeList, XmlNamespaceManager namespaceManager, ITable table)
        {
            if (codedValueNodeList == null)
            {
                throw new ArgumentNullException("codedValueNodeList");
            }
            if (namespaceManager == null)
            {
                throw new ArgumentNullException("namespaceManager");
            }
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            foreach (var codedValueNode in codedValueNodeList.OfType<XmlElement>())
            {
                var fieldNameNode = codedValueNode.SelectSingleNode("ns:titel", namespaceManager);
                if (fieldNameNode == null)
                {
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.MissingChildNode, "titel", codedValueNode.LocalName));
                }

                var fieldName = GetNodeValue(fieldNameNode);
                var field = table.Fields.SingleOrDefault(m => String.Compare(m.NameSource, fieldName, StringComparison.OrdinalIgnoreCase) == 0);
                if (field == null)
                {
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.FieldNotFound, fieldName));
                }

                var valueNodeList = codedValueNode.SelectNodes("ns:kode", namespaceManager);
                if (valueNodeList == null)
                {
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.MissingChildNode, "kode", codedValueNode.LocalName));
                }

                if (field.DatatypeOfSource == typeof (string))
                {
                    var convert = new Func<string, string, string>((code, value) =>
                        {
                            if (string.IsNullOrEmpty(code))
                            {
                                throw new ArgumentNullException("code");
                            }
                            if (string.IsNullOrEmpty("value"))
                            {
                                throw new ArgumentNullException("value");
                            }
                            field.DatatypeOfTarget = typeof (string);
                            field.LengthOfTarget = Math.Max(field.LengthOfTarget, value.Length);
                            return code.Trim();
                        });
                    field.Map = CreateStaticMap(valueNodeList, convert);
                    continue;
                }
                if (field.DatatypeOfSource == typeof (int?))
                {
                    var convert = new Func<string, string, int?>((code, value) =>
                        {
                            if (string.IsNullOrEmpty(code))
                            {
                                throw new ArgumentNullException("code");
                            }
                            if (string.IsNullOrEmpty("value"))
                            {
                                throw new ArgumentNullException("value");
                            }
                            field.DatatypeOfTarget = typeof (string);
                            field.LengthOfTarget = Math.Max(field.LengthOfTarget, value.Length);
                            return int.Parse(code);
                        });
                    field.Map = CreateStaticMap(valueNodeList, convert);
                    continue;
                }
                if (field.DatatypeOfSource == typeof (long?))
                {
                    var convert = new Func<string, string, long?>((code, value) =>
                        {
                            if (string.IsNullOrEmpty(code))
                            {
                                throw new ArgumentNullException("code");
                            }
                            if (string.IsNullOrEmpty("value"))
                            {
                                throw new ArgumentNullException("value");
                            }
                            field.DatatypeOfTarget = typeof (string);
                            field.LengthOfTarget = Math.Max(field.LengthOfTarget, value.Length);
                            return long.Parse(code);
                        });
                    field.Map = CreateStaticMap(valueNodeList, convert);
                    continue;
                }
                if (field.DatatypeOfSource == typeof(decimal?))
                {
                    var convert = new Func<string, string, decimal?>((code, value) =>
                        {
                            if (string.IsNullOrEmpty(code))
                            {
                                throw new ArgumentNullException("code");
                            }
                            if (string.IsNullOrEmpty("value"))
                            {
                                throw new ArgumentNullException("value");
                            }
                            field.DatatypeOfTarget = typeof (string);
                            field.LengthOfTarget = Math.Max(field.LengthOfTarget, value.Length);
                            decimal d;
                            return decimal.TryParse(code, NumberStyles.Any, new CultureInfo("en-US"), out d) ? d : decimal.Parse(code, Thread.CurrentThread.CurrentUICulture);
                        });
                    field.Map = CreateStaticMap(valueNodeList, convert);
                    continue;
                }
                if (field.DatatypeOfSource == typeof (DateTime?))
                {
                    var convert = new Func<string, string, DateTime?>((code, value) =>
                        {
                            if (string.IsNullOrEmpty(code))
                            {
                                throw new ArgumentNullException("code");
                            }
                            if (string.IsNullOrEmpty("value"))
                            {
                                throw new ArgumentNullException("value");
                            }
                            field.DatatypeOfTarget = typeof (string);
                            field.LengthOfTarget = Math.Max(field.LengthOfTarget, value.Length);
                            if (code.IndexOf('T') > 0)
                            {
                                return new DateTime(int.Parse(code.Substring(0, 4)), int.Parse(code.Substring(4, 2)), int.Parse(code.Substring(6, 2)), int.Parse(code.Substring(9, 2)), int.Parse(code.Substring(11, 2)), int.Parse(code.Substring(13, 2)), int.Parse(code.Substring(15, 2)));
                            }
                            return new DateTime(int.Parse(code.Substring(0, 4)), int.Parse(code.Substring(4, 2)), int.Parse(code.Substring(6, 2)));
                        });
                    field.Map = CreateStaticMap(valueNodeList, convert);
                    continue;
                }
                if (field.DatatypeOfSource == typeof (TimeSpan?))
                {
                    var convert = new Func<string, string, TimeSpan?>((code, value) =>
                        {
                            if (string.IsNullOrEmpty(code))
                            {
                                throw new ArgumentNullException("code");
                            }
                            if (string.IsNullOrEmpty("value"))
                            {
                                throw new ArgumentNullException("value");
                            }
                            field.DatatypeOfTarget = typeof (string);
                            field.LengthOfTarget = Math.Max(field.LengthOfTarget, value.Length);
                            return new TimeSpan(0, int.Parse(code.Substring(0, 2)), int.Parse(code.Substring(2, 2)), int.Parse(code.Substring(4, 2)), int.Parse(code.Substring(7)) == 0 ? 0 : 100/int.Parse(code.Substring(7)));
                        });
                    field.Map = CreateStaticMap(valueNodeList, convert);
                    continue;
                }
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.IllegalValue, field.DatatypeOfSource, "DatatypeOfSource"));
            }
        }

        /// <summary>
        /// Creates an static map from an node list of value for a field.
        /// </summary>
        /// <typeparam name="T">Datatype for the field.</typeparam>
        /// <param name="valueNodeList">Node list of codes and values for the field.</param>
        /// <param name="convertCode">Callback method to convert code.</param>
        /// <returns>Static map.</returns>
        private static IStaticMap<T, string> CreateStaticMap<T>(XmlNodeList valueNodeList, Func<string, string, T> convertCode)
        {
            if (valueNodeList == null)
            {
                throw new ArgumentNullException("valueNodeList");
            }
            if (convertCode == null)
            {
                throw new ArgumentNullException("convertCode");
            }
            var map = new StaticMap<T, string>();
            for (var i = 0; i < valueNodeList.Count; i = i + 2)
            {
                var code = GetNodeValue(valueNodeList[i]);
                var value = GetNodeValue(valueNodeList[i + 1]);
                map.AddRule(convertCode(code, value), value);
            }
            return map;
        }

        /// <summary>
        /// Gets foreign keys from the node list of foreign keys.
        /// </summary>
        /// <param name="foreignKeyNodeList">Node list of foreign keys.</param>
        /// <param name="namespaceManager">Namespace manager.</param>
        /// <param name="dataSource">Data source for the foreign keys.</param>
        /// <param name="table">Tabel on which to add foreign keys.</param>
        /// <returns>Lists of foreign keys.</returns>
        private static IEnumerable<IForeignKey> GetForeignKeys(XmlNodeList foreignKeyNodeList, XmlNamespaceManager namespaceManager, IDataSource dataSource, ITable table)
        {
            if (foreignKeyNodeList == null)
            {
                throw new ArgumentNullException("foreignKeyNodeList");
            }
            if (namespaceManager == null)
            {
                throw new ArgumentNullException("namespaceManager");
            }
            if (dataSource == null)
            {
                throw new ArgumentNullException("dataSource");
            }
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            var foreignKeys = new List<IForeignKey>(foreignKeyNodeList.OfType<XmlElement>().Count());
            foreach (var foreignKeyNode in foreignKeyNodeList.OfType<XmlElement>())
            {
                var foreignTableNode = foreignKeyNode.SelectSingleNode("ns:fremmedtabel", namespaceManager);
                if (foreignTableNode == null)
                {
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.MissingChildNode, "fremmedtabel", foreignKeyNode.LocalName));
                }
                var foreignTableChildNodes = foreignTableNode.SelectNodes("ns:titel", namespaceManager);
                if (foreignTableChildNodes == null || !foreignTableChildNodes.OfType<XmlElement>().Any())
                {
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.MissingChildNode, "titel", foreignTableNode.LocalName));
                }

                var foreignTableName = GetNodeValue(foreignTableChildNodes.OfType<XmlElement>().First());
                var foreignTable = dataSource.Tables.SingleOrDefault(m => String.Compare(m.NameSource, foreignTableName, StringComparison.OrdinalIgnoreCase) == 0);
                if (foreignTable == null)
                {
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.TableNotFound, foreignTableName));
                }

                var foreignTableCandidateKey = foreignTable.CandidateKeys.SingleOrDefault(m =>
                                                                                              {
                                                                                                  if (m.Fields.Count != foreignTableChildNodes.OfType<XmlElement>().Count() - 1)
                                                                                                  {
                                                                                                      return false;
                                                                                                  }
                                                                                                  for (var i = 0; i < m.Fields.Count; i++)
                                                                                                  {
                                                                                                      var fieldName = GetNodeValue(foreignTableChildNodes.OfType<XmlElement>().ElementAt(i + 1));
                                                                                                      if (String.Compare(fieldName, m.Fields[i].Key.NameSource, StringComparison.OrdinalIgnoreCase) != 0)
                                                                                                      {
                                                                                                          return false;
                                                                                                      }
                                                                                                  }
                                                                                                  return true;
                                                                                              });
                if (foreignTableCandidateKey == null)
                {
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.UnableToFindMatchingKey, foreignTable.NameSource));
                }

                Cardinality cardinality;
                var cardinalityNodeList = foreignKeyNode.SelectNodes("ns:kardinalitet", namespaceManager);
                if (cardinalityNodeList == null || !cardinalityNodeList.OfType<XmlElement>().Any())
                {
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.MissingChildNode, "kardinalitet", foreignKeyNode.LocalName));
                }
                if (cardinalityNodeList.OfType<XmlElement>().Count() != 2)
                {
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.InvalidChildNodeCount, "kardinalitet", foreignKeyNode.LocalName));
                }
                if (GetNodeValue(cardinalityNodeList[0]) == "m" && GetNodeValue(cardinalityNodeList[1]) == "m")
                {
                    cardinality = Cardinality.ManyToMany;
                }
                else if (GetNodeValue(cardinalityNodeList[0]) == "m" && GetNodeValue(cardinalityNodeList[1]) == "1")
                {
                    cardinality = Cardinality.ManyToOne;
                }
                else if (GetNodeValue(cardinalityNodeList[0]) == "1" && GetNodeValue(cardinalityNodeList[1]) == "m")
                {
                    cardinality = Cardinality.OneToMany;
                }
                else if (GetNodeValue(cardinalityNodeList[0]) == "1" && GetNodeValue(cardinalityNodeList[1]) == "1")
                {
                    cardinality = Cardinality.OneToOne;
                }
                else
                {
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.InvalidCardinality));
                }

                var nameSource = string.Format("FK_{0}_{1}", table.NameSource.ToUpper(), foreignTable.NameSource.ToUpper());
                var nameTarget = string.Format("FK_{0}_{1}", table.NameTarget.ToUpper(), foreignTable.NameTarget.ToUpper());
                var addFieldNames = foreignKeys.Any(m => string.Compare(nameTarget, m.NameTarget, StringComparison.OrdinalIgnoreCase) == 0);
                var foreignKey = new ForeignKey(foreignTableCandidateKey, nameSource, nameTarget, string.Format("Fremmednøgle fra {0} til {1}", table.NameTarget, foreignTable.NameTarget), cardinality)
                    {
                        Table = table
                    };

                var keyFieldNodeList = foreignKeyNode.SelectNodes("ns:titel", namespaceManager);
                if (keyFieldNodeList == null || !keyFieldNodeList.OfType<XmlElement>().Any())
                {
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.MissingChildNode, "titel", foreignKeyNode.LocalName));
                }
                foreach(var keyFieldNode in keyFieldNodeList.OfType<XmlElement>())
                {
                    var fieldName = GetNodeValue(keyFieldNode);
                    var field = table.Fields.SingleOrDefault(m => String.Compare(m.NameSource, fieldName, StringComparison.OrdinalIgnoreCase) == 0);
                    if (field == null)
                    {
                        throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.FieldNotFound, fieldName));
                    }
                    if (field.Nullable && field.DatatypeOfTarget == typeof (string))
                    {
                        var mapper = new StaticMap<string, string>();
                        mapper.AddRule(string.Empty, null);
                        foreignKey.AddField(field, mapper);
                        continue;
                    }
                    if (field.Nullable && field.DatatypeOfTarget == typeof (int?))
                    {
                        var mapper = new StaticMap<int?, int?>();
                        mapper.AddRule(0, null);
                        foreignKey.AddField(field, mapper);
                        continue;
                    }
                    if (field.Nullable && field.DatatypeOfTarget == typeof (long?))
                    {
                        var mapper = new StaticMap<long?, long?>();
                        mapper.AddRule(0, null);
                        foreignKey.AddField(field, mapper);
                        continue;
                    }
                    if (field.Nullable && field.DatatypeOfTarget == typeof (decimal?))
                    {
                        var mapper = new StaticMap<decimal?, decimal?>();
                        mapper.AddRule(0M, null);
                        foreignKey.AddField(field, mapper);
                        continue;
                    }
                    foreignKey.AddField(field);
                }
                if (addFieldNames)
                {
                    foreach (var keyField in foreignKey.Fields.Select(m => m.Key))
                    {
                        foreignKey.NameSource += string.Format("_{0}", keyField.NameSource);
                        foreignKey.NameTarget += string.Format("_{0}", keyField.NameTarget);
                    }
                }

                foreignKeys.Add(foreignKey);
            }
            return foreignKeys;
        }

        /// <summary>
        /// Gets a list of views from the node list of views.
        /// </summary>
        /// <param name="viewNodeList">Node list of views.</param>
        /// <param name="namespaceManager">Namespace manager.</param>
        /// <returns>List of views.</returns>
        private static IEnumerable<IView> GetViews(XmlNodeList viewNodeList, XmlNamespaceManager namespaceManager)
        {
            if (viewNodeList == null)
            {
                throw new ArgumentNullException("viewNodeList");
            }
            if (namespaceManager == null)
            {
                throw new ArgumentNullException("namespaceManager");
            }
            var views = new List<IView>(viewNodeList.OfType<XmlElement>().Count());
            foreach (var viewNode in viewNodeList.OfType<XmlElement>())
            {
                var viewDescriptionNode = viewNode.SelectSingleNode("ns:saqinfo", namespaceManager);
                if (viewDescriptionNode == null)
                {
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.MissingChildNode, "saqinfo", viewNode.LocalName));
                }
                var viewQueryNode = viewNode.SelectSingleNode("ns:saqdata", namespaceManager);
                if (viewQueryNode == null)
                {
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.MissingChildNode, "saqdata", viewNode.LocalName));
                }
                var viewName = string.Format("AV_VIEW{0}", views.Count + 1);
                var viewDescription = GetNodeValue(viewDescriptionNode);
                var viewQuery = GetNodeValue(viewQueryNode);
                views.Add(new View(viewName, viewName, viewQuery, viewDescription));
            }
            return views;
        }

        /// <summary>
        /// Appends metadata extensions from the metadata file.
        /// </summary>
        /// <param name="documentElement">Document element in the metadata file.</param>
        /// <param name="namespaceManager">Namespace manager.</param>
        /// <param name="dataSource">Data source.</param>
        private static void AppendMetadataExtensions(XmlElement documentElement, XmlNamespaceManager namespaceManager, IDataSource dataSource)
        {
            if (documentElement == null)
            {
                throw new ArgumentNullException("documentElement");
            }
            if (namespaceManager == null)
            {
                throw new ArgumentNullException("namespaceManager");
            }
            if (dataSource == null)
            {
                throw new ArgumentNullException("dataSource");
            }
            foreach (var table in dataSource.Tables)
            {
                var fieldNodeList = documentElement.SelectNodes(string.Format("/ns:arkiveringsversion/ns:tabel[ns:titel='{0}']/ns:feltdef[@metadataExtension:nameSource | @metadataExtension:datatypeOfSource | @metadataExtension:excludeFilter | @metadataExtension:recordEqualFilter | @metadataExtension:recordPoolFilter | @metadataExtension:recordIntervalFilter]", table.NameSource), namespaceManager);
                if (fieldNodeList == null || fieldNodeList.Count == 0)
                {
                    continue;
                }
                foreach (XmlElement fieldNode in fieldNodeList)
                {
                    var field = table.Fields.Single(m => String.Compare(m.NameSource, GetNodeValue(fieldNode.SelectSingleNode("ns:titel", namespaceManager)), StringComparison.OrdinalIgnoreCase) == 0);
                    if (fieldNode.Attributes["nameSource", MetadataExtensionNamespace] != null)
                    {
                        field.NameSource = fieldNode.Attributes["nameSource", MetadataExtensionNamespace].Value;
                    }
                    if (fieldNode.Attributes["datatypeOfSource", MetadataExtensionNamespace] != null)
                    {
                        switch (fieldNode.Attributes["datatypeOfSource", MetadataExtensionNamespace].Value.ToLower())
                        {
                            case "string":
                                field.DatatypeOfSource = typeof (string);
                                field.OriginalDatatype = "Tekst";
                                break;

                            case "num":
                                field.DatatypeOfSource = typeof (int?);
                                if (field.LengthOfSource > 9)
                                {
                                    field.DatatypeOfSource = typeof (long?);
                                }
                                field.OriginalDatatype = "Heltal";
                                break;

                            case "real":
                            case "exp":
                                field.DatatypeOfSource = typeof (decimal?);
                                field.OriginalDatatype = "Decimaltal";
                                break;

                            case "date":
                                field.DatatypeOfSource = typeof (DateTime?);
                                field.OriginalDatatype = "Dato";
                                break;

                            case "timestamp":
                                field.DatatypeOfSource = typeof (DateTime?);
                                field.OriginalDatatype = "Tidsstempel";
                                break;

                            case "time":
                                field.DatatypeOfSource = typeof (TimeSpan?);
                                field.OriginalDatatype = "Tidspunkt";
                                break;

                            default:
                                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.IllegalValue, fieldNode.Attributes["datatypeOfSource", MetadataExtensionNamespace].Value, "datatypeOfSource"));
                        }
                    }
                    if (fieldNode.Attributes["excludeFilter", MetadataExtensionNamespace] != null)
                    {
                        var asString = fieldNode.Attributes["excludeFilter", MetadataExtensionNamespace].Value;
                        if (bool.Parse(asString))
                        {
                            if (table.FieldFilters.Count == 0)
                            {
                                table.AddFieldFilter(new Filter());
                            }
                            table.FieldFilters.ElementAt(0).AddCriteria(new ExcludeFieldCriteria(field));
                            var allFieldFilters = table.FieldFilters;
                            if (allFieldFilters.Any(m => m.Exclude(field)))
                            {
                                field.ColumnId = string.Empty;
                                var newColumnId = 1;
                                foreach (var updateField in table.Fields.Where(f => allFieldFilters.Any(m => m.Exclude(f)) == false))
                                {
                                    updateField.ColumnId = string.Format("c{0}", newColumnId);
                                    newColumnId++;
                                }
                            }
                        }
                    }
                    if (fieldNode.Attributes["recordEqualFilter", MetadataExtensionNamespace] != null)
                    {
                        var filterValues = fieldNode.Attributes["recordEqualFilter", MetadataExtensionNamespace].Value;
                        const string pattern = @"^[0-9A-Za-f]*|[^;][0-9A-Za-z]*";
                        if (Regex.IsMatch(filterValues, pattern) == false)
                        {
                            throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.InvalidFilterValue, filterValues, pattern));
                        }
                        var matches = Regex.Matches(filterValues, pattern);
                        for (var filterNo = 0; filterNo < matches.Count; filterNo++)
                        {
                            while (table.RecordFilters.Count < filterNo + 1)
                            {
                                table.AddRecordFilter(new Filter());
                            }
                            var filterValue = matches[filterNo].Value;
                            if (string.IsNullOrEmpty(filterValue))
                            {
                                continue;
                            }
                            if (field.DatatypeOfSource == typeof (string))
                            {
                                var criteria = new EqualCriteria<string>(field, filterValue);
                                table.RecordFilters.ElementAt(filterNo).AddCriteria(criteria);
                                continue;
                            }
                            if (field.DatatypeOfSource == typeof (int?))
                            {
                                var criteria = new EqualCriteria<int?>(field, int.Parse(filterValue));
                                table.RecordFilters.ElementAt(filterNo).AddCriteria(criteria);
                                continue;
                            }
                            if (field.DatatypeOfSource == typeof (long?))
                            {
                                var criteria = new EqualCriteria<long?>(field, long.Parse(filterValue));
                                table.RecordFilters.ElementAt(filterNo).AddCriteria(criteria);
                                continue;
                            }
                            if (field.DatatypeOfSource == typeof (decimal?))
                            {
                                var criteria = new EqualCriteria<decimal?>(field, decimal.Parse(filterValue));
                                table.RecordFilters.ElementAt(filterNo).AddCriteria(criteria);
                                continue;
                            }
                            if (field.DatatypeOfSource == typeof (DateTime?))
                            {
                                var criteria = new EqualCriteria<DateTime?>(field, DateTime.Parse(filterValue));
                                table.RecordFilters.ElementAt(filterNo).AddCriteria(criteria);
                                continue;
                            }
                            if (field.DatatypeOfSource == typeof (TimeSpan?))
                            {
                                var criteria = new EqualCriteria<TimeSpan?>(field, TimeSpan.Parse(filterValue));
                                table.RecordFilters.ElementAt(filterNo).AddCriteria(criteria);
                                continue;
                            }
                            throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.IllegalValue, field.DatatypeOfSource, "DatatypeOfSource"));
                        }
                    }
                    if (fieldNode.Attributes["recordPoolFilter", MetadataExtensionNamespace] != null)
                    {
                        var filterValues = fieldNode.Attributes["recordPoolFilter", MetadataExtensionNamespace].Value;
                        const string pattern = @"^[0-9A-Za-f]*[,[0-9A-Za-f]*]*|[^;][0-9A-Za-z]*[,[0-9A-Za-f]*]*";
                        if (Regex.IsMatch(filterValues, pattern) == false)
                        {
                            throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.InvalidFilterValue, filterValues, pattern));
                        }
                        var matches = Regex.Matches(filterValues, pattern);
                        for (var filterNo = 0; filterNo < matches.Count; filterNo++)
                        {
                            while (table.RecordFilters.Count < filterNo + 1)
                            {
                                table.AddRecordFilter(new Filter());
                            }
                            var filterValue = matches[filterNo].Value;
                            if (string.IsNullOrEmpty(filterValue))
                            {
                                continue;
                            }
                            if (field.DatatypeOfSource == typeof (string))
                            {
                                var criteria = new PoolCriteria<string>(field, filterValue.Split(','));
                                table.RecordFilters.ElementAt(filterNo).AddCriteria(criteria);
                                continue;
                            }
                            if (field.DatatypeOfSource == typeof (int?))
                            {
                                var criteria = new PoolCriteria<int?>(field, filterValue.Split(',').Select(m => new int?(int.Parse(m))).ToList());
                                table.RecordFilters.ElementAt(filterNo).AddCriteria(criteria);
                                continue;
                            }
                            if (field.DatatypeOfSource == typeof (long?))
                            {
                                var criteria = new PoolCriteria<long?>(field, filterValue.Split(',').Select(m => new long?(int.Parse(m))).ToList());
                                table.RecordFilters.ElementAt(filterNo).AddCriteria(criteria);
                                continue;
                            }
                            if (field.DatatypeOfSource == typeof (decimal?))
                            {
                                var criteria = new PoolCriteria<decimal?>(field, filterValue.Split(',').Select(m => new decimal?(decimal.Parse(m))).ToList());
                                table.RecordFilters.ElementAt(filterNo).AddCriteria(criteria);
                                continue;
                            }
                            if (field.DatatypeOfSource == typeof (DateTime?))
                            {
                                var criteria = new PoolCriteria<DateTime?>(field, filterValue.Split(',').Select(m => new DateTime?(DateTime.Parse(m))).ToList());
                                table.RecordFilters.ElementAt(filterNo).AddCriteria(criteria);
                                continue;
                            }
                            if (field.DatatypeOfSource == typeof (TimeSpan?))
                            {
                                var criteria = new PoolCriteria<TimeSpan?>(field, filterValue.Split(',').Select(m => new TimeSpan?(TimeSpan.Parse(m))).ToList());
                                table.RecordFilters.ElementAt(filterNo).AddCriteria(criteria);
                                continue;
                            }
                            throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.IllegalValue, field.DatatypeOfSource, "DatatypeOfSource"));
                        }
                    }
                    if (fieldNode.Attributes["recordIntervalFilter", MetadataExtensionNamespace] != null)
                    {
                        var filterValues = fieldNode.Attributes["recordIntervalFilter", MetadataExtensionNamespace].Value;
                        const string pattern = @"^[0-9A-Za-f]*-[0-9A-Za-f]*|[^;][0-9A-Za-z]*-[0-9A-Za-f]*";
                        if (Regex.IsMatch(filterValues, pattern) == false)
                        {
                            throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.InvalidFilterValue, filterValues, pattern));
                        }
                        var matches = Regex.Matches(filterValues, pattern);
                        for (var filterNo = 0; filterNo < matches.Count; filterNo++)
                        {
                            while (table.RecordFilters.Count < filterNo + 1)
                            {
                                table.AddRecordFilter(new Filter());
                            }
                            var filterValue = matches[filterNo].Value;
                            if (string.IsNullOrEmpty(filterValue))
                            {
                                continue;
                            }
                            if (field.DatatypeOfSource == typeof (string))
                            {
                                var criteriaValues = filterValue.Split('-');
                                var criteria = new IntervalCriteria<string>(field, criteriaValues.ElementAt(0), criteriaValues.ElementAt(1));
                                table.RecordFilters.ElementAt(filterNo).AddCriteria(criteria);
                                continue;
                            }
                            if (field.DatatypeOfSource == typeof (int?))
                            {
                                var criteriaValues = filterValue.Split('-').Select(int.Parse).ToList();
                                var criteria = new IntervalCriteria<int>(field, criteriaValues.ElementAt(0), criteriaValues.ElementAt(1));
                                table.RecordFilters.ElementAt(filterNo).AddCriteria(criteria);
                                continue;
                            }
                            if (field.DatatypeOfSource == typeof (long?))
                            {
                                var criteriaValues = filterValue.Split('-').Select(long.Parse).ToList();
                                var criteria = new IntervalCriteria<long>(field, criteriaValues.ElementAt(0), criteriaValues.ElementAt(1));
                                table.RecordFilters.ElementAt(filterNo).AddCriteria(criteria);
                                continue;
                            }
                            if (field.DatatypeOfSource == typeof (decimal?))
                            {
                                var criteriaValues = filterValue.Split('-').Select(decimal.Parse).ToList();
                                var criteria = new IntervalCriteria<decimal>(field, criteriaValues.ElementAt(0), criteriaValues.ElementAt(1));
                                table.RecordFilters.ElementAt(filterNo).AddCriteria(criteria);
                                continue;
                            }
                            if (field.DatatypeOfSource == typeof (DateTime?))
                            {
                                var criteriaValues = filterValue.Split('-').Select(DateTime.Parse).ToList();
                                var criteria = new IntervalCriteria<DateTime>(field, criteriaValues.ElementAt(0), criteriaValues.ElementAt(1));
                                table.RecordFilters.ElementAt(filterNo).AddCriteria(criteria);
                                continue;
                            }
                            if (field.DatatypeOfSource == typeof (TimeSpan?))
                            {
                                var criteriaValues = filterValue.Split('-').Select(TimeSpan.Parse).ToList();
                                var criteria = new IntervalCriteria<TimeSpan>(field, criteriaValues.ElementAt(0), criteriaValues.ElementAt(1));
                                table.RecordFilters.ElementAt(filterNo).AddCriteria(criteria);
                                continue;
                            }
                            throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.IllegalValue, field.DatatypeOfSource, "DatatypeOfSource"));
                        }
                    }
                }
            }
            var tableNodeList = documentElement.SelectNodes("/ns:arkiveringsversion/ns:tabel[@metadataExtension:nameSource]", namespaceManager);
            if (tableNodeList == null || tableNodeList.Count == 0)
            {
                return;
            }
            foreach (XmlElement tableNode in tableNodeList)
            {
                var table = dataSource.Tables.Single(m => String.Compare(m.NameSource, GetNodeValue(tableNode.SelectSingleNode("ns:titel", namespaceManager)), StringComparison.OrdinalIgnoreCase) == 0);
                if (tableNode.Attributes["nameSource", MetadataExtensionNamespace] != null)
                {
                    table.NameSource = tableNode.Attributes["nameSource", MetadataExtensionNamespace].Value;
                }
            }
        }

        /// <summary>
        /// Converts a date string to a DateTime.
        /// </summary>
        /// <param name="value">Date string (format: YYYYMMDD)</param>
        /// <returns>DateTime.</returns>
        private static DateTime ToDateTime(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }
            return new DateTime(int.Parse(value.Substring(0, 4)), int.Parse(value.Substring(4, 2)), int.Parse(value.Substring(6, 2)));
        }

        /// <summary>
        /// Gets the value from a node.
        /// </summary>
        /// <param name="node">Node to get value from.</param>
        /// <returns>Value.</returns>
        private static string GetNodeValue(XmlNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            return node.InnerText;
        }

        #endregion
    }
}
