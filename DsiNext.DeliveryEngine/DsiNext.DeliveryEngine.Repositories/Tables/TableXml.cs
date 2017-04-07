using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.Indices;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Repositories.Tables
{
    public class TableXml : XmlFileBase, IDisposable
    {
        #region Private variables

        private readonly FileInfo _shadowFile;
        private readonly FileStream _shadowFileStream;
        private readonly XmlTextWriter _xmlTextWriter;

        #endregion

        #region Constructors

        public TableXml(TableXsd tableXsd, FileInfo path, FileIndex fileIndex, int tableNo)
            : base(path, fileIndex, tableXsd.Schema, TableNamespace(tableXsd), TableNamespaceLocation(TableNamespace(tableXsd), tableNo))
        {
            path.Refresh();
            if (path.Exists == false)
            {
                // We will now use Document from the base class.
                return;
            }

            // We will now void to load large XML documents (> 32MB) into 
            // the memory because this can make an OutOfMemoryException
            // Therefore we will set the Document to NULL and use
            // a XmlTextWriter instead.
            long maxSizeForUsingXmlDocument = 1024 * 1024 * 32;
            if (path.Length < maxSizeForUsingXmlDocument)
            {
                try
                {
                    using (var fileStream = new FileStream(path.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                    {
                        using (var xmlReader = XmlReader.Create(fileStream, CreateXmlReaderSettings(base.Schema, ValidationType.None)))
                        {
                            Document.Load(xmlReader);
                            xmlReader.Close();
                        }
                    }
                    return;
                }
                catch (OutOfMemoryException)
                {
                    Document = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }

            Document = null;

            _shadowFile = new FileInfo($"{path.FullName}.shadow");
            try
            {
                DeleteFile(_shadowFile);

                _shadowFileStream = new FileStream(_shadowFile.FullName, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read);
                _xmlTextWriter = new XmlTextWriter(_shadowFileStream, Encoding.UTF8)
                {
                    Formatting = Formatting.Indented
                };

                using (var sourceFileStream = new FileStream(path.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var xmlReader = XmlReader.Create(sourceFileStream, CreateXmlReaderSettings(base.Schema, ValidationType.None)))
                    {
                        while (xmlReader.Read())
                        {
                            switch (xmlReader.NodeType)
                            {
                                case XmlNodeType.Element:
                                    _xmlTextWriter.WriteStartElement(xmlReader.Prefix, xmlReader.LocalName, xmlReader.NamespaceURI);
                                    _xmlTextWriter.WriteAttributes(xmlReader, true);
                                    if (xmlReader.IsEmptyElement)
                                    {
                                        _xmlTextWriter.WriteEndElement();
                                    }
                                    break;

                                case XmlNodeType.Text:
                                    _xmlTextWriter.WriteString(xmlReader.Value);
                                    break;

                                case XmlNodeType.Whitespace:
                                case XmlNodeType.SignificantWhitespace:
                                    _xmlTextWriter.WriteWhitespace(xmlReader.Value);
                                    break;

                                case XmlNodeType.CDATA:
                                    _xmlTextWriter.WriteCData(xmlReader.Value);
                                    break;

                                case XmlNodeType.EntityReference:
                                    _xmlTextWriter.WriteEntityRef(xmlReader.Name);
                                    break;

                                case XmlNodeType.XmlDeclaration:
                                case XmlNodeType.ProcessingInstruction:
                                    _xmlTextWriter.WriteProcessingInstruction(xmlReader.Name, xmlReader.Value);
                                    break;

                                case XmlNodeType.DocumentType:
                                    _xmlTextWriter.WriteDocType(xmlReader.Name, xmlReader.GetAttribute("PUBLIC"), xmlReader.GetAttribute("SYSTEM"), xmlReader.Value);
                                    break;

                                case XmlNodeType.Comment:
                                    _xmlTextWriter.WriteComment(xmlReader.Value);
                                    break;

                                case XmlNodeType.EndElement:
                                    if (string.Compare(xmlReader.LocalName, "table", StringComparison.Ordinal) == 0)
                                    {
                                        break;
                                    }
                                    _xmlTextWriter.WriteFullEndElement();
                                    break;
                            }
                        }
                    }
                }
            }
            catch
            {
                _xmlTextWriter?.Close();
                _shadowFileStream?.Close();
                _shadowFileStream?.Dispose();

                DeleteFile(_shadowFile);

                throw;
            }
        }

        #endregion

        private XmlElement CurrentRow { get; set; }

        public void AddRow()
        {
            if (Document != null)
            {
                CurrentRow = AddElement(Root, "row");
                return;
            }

            _xmlTextWriter.WriteStartElement(string.Empty, "row", Namespace);
        }

        public void AddField(string columnId, object value)
        {
            if (columnId == null) throw new ArgumentNullException(nameof(columnId));

            if (Document != null)
            {
                if (value != null)
                {
                    AddElement(CurrentRow, columnId, XmlValue(value));
                    return;
                }
                AddNillableElement(CurrentRow, columnId);
                return;
            }

            _xmlTextWriter.WriteStartElement(string.Empty, columnId, Namespace);
            if (value != null)
            {
                _xmlTextWriter.WriteString(XmlValue(value));
                _xmlTextWriter.WriteEndElement();
                return;
            }
            _xmlTextWriter.WriteAttributeString("xsi", "nil", XsdNamespace, "true");
            _xmlTextWriter.WriteEndElement();
        }

        public void CloseRow()
        {
            if (Document != null)
            {
                return;
            }

            _xmlTextWriter.WriteFullEndElement();
        }

        private string XmlValue(object value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            var name = value.GetType().Name;

            if (value is DateTime)
            {
                name = "DateTime";
            }

            if (value is TimeSpan)
            {
                name = "TimeSpan";
            }

            switch (name)
            {
                case "char":
                case "Char":
                case "char[]":
                case "Char[]":
                case "string":
                case "String":
                case "int":
                case "Int16":
                case "Int32":
                case "Int64":
                case "long":
                case "Long":
                    return value.ToString();

                case "decimal":
                case "Decimal":
                    if (value is decimal)
                    {
                        return ((decimal) value).ToString("0.00", CultureInfo.InvariantCulture);
                    }
                    return Convert.ToDecimal(value).ToString("0.00", CultureInfo.InvariantCulture);

                case "double":
                case "Double":
                    if (value is double)
                    {
                        return ((double) value).ToString("0.00", CultureInfo.InvariantCulture);
                    }
                    return Convert.ToDouble(value).ToString("0.00", CultureInfo.InvariantCulture);

                case "Single":
                    if (value is Single)
                    {
                        return ((Single) value).ToString("0.00", CultureInfo.InvariantCulture);
                    }
                    return Convert.ToSingle(value).ToString("0.00", CultureInfo.InvariantCulture);

                case "bool":
                case "Boolean":
                    return ((bool) value) ? "true" : "false";

                case "DateTime":
                    return ((DateTime) value).ToString(@"yyyy-MM-ddTHH:mm:ss");

                case "TimeSpan":
                    return ((TimeSpan) value).ToString(@"hh\:mm\:ss");

                default:
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.DataTypeNotSupported, value.GetType().Name));
            }
        }

        #region Overrides of XmlFileBase

        protected override string RootName => "table";

        public override void Persist(object syncRoot = null)
        {
            if (syncRoot == null)
            {
                throw new ArgumentNullException(nameof(syncRoot));
            }

            if (Document != null)
            {
                lock (syncRoot)
                {
                    base.Persist(syncRoot);
                }
                Document = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                return;
            }

            // Always write the end element and flush the writer 
            // before validating.
            _xmlTextWriter.WriteFullEndElement();
            _xmlTextWriter.Flush();

            // Validation.
            _shadowFileStream.Seek(0, SeekOrigin.Begin);
            using (var xmlReader = XmlReader.Create(_shadowFileStream, CreateXmlReaderSettings(Schema, ValidationType.Schema)))
            {
                while (xmlReader.Read())
                {
                }
            }

            // Close the writer and the shadow file stream.
            _xmlTextWriter.Close();
            _shadowFileStream.Close();

            // Rename the shadow file to the archive table file.
            try
            {
                lock (syncRoot)
                {
                    DeleteFile(FilePath);

                    _shadowFile.CopyTo(FilePath.FullName);
                    DeleteFile(_shadowFile);

                    FilePath.Refresh();
                    FileIndex.AddFile(FilePath);
                }
            }
            catch (Exception ex)
            {
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.FileWriteError, FilePath.FullName), ex);
            }
        }

        #endregion

        public void Dispose()
        {
            _xmlTextWriter?.Close();
            _shadowFileStream?.Close();
            _shadowFileStream?.Dispose();

            if (_shadowFile != null)
            {
                DeleteFile(_shadowFile);
            }
        }

        private static void DeleteFile(FileInfo fileInfo)
        {
            if (fileInfo == null) throw new ArgumentNullException(nameof(fileInfo));

            fileInfo.Refresh();
            while (fileInfo.Exists)
            {
                fileInfo.Delete();
                fileInfo.Refresh();
            }
        }

        private static XmlReaderSettings CreateXmlReaderSettings(XmlSchema schema, ValidationType validationType)
        {
            if (schema == null) throw new ArgumentNullException(nameof(schema));

            var xmlReaderSettings = new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
                IgnoreWhitespace = true,
                ValidationType = validationType,
                Schemas = new XmlSchemaSet(new NameTable())
            };
            xmlReaderSettings.Schemas.Add(schema);
            xmlReaderSettings.ValidationEventHandler += ValidationEventHandler;
            return xmlReaderSettings;
        }

        private static string TableNamespace(TableXsd tableXsd)
        {
            if (tableXsd == null) throw new ArgumentNullException(nameof(tableXsd));

            return tableXsd.Schema.TargetNamespace;
        }

        private static string TableNamespaceLocation(string @namespace, int tableNo)
        {
            if (@namespace == null) throw new ArgumentNullException(nameof(@namespace));

            return $"{@namespace} table{tableNo}.xsd";
        }
    }
}
