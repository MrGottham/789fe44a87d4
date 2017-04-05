using System;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.Indices;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Repositories.Tables
{
    public class TableXml : XmlFileBase
    {
        #region Constructors

        public TableXml(TableXsd tableXsd, FileInfo path, FileIndex fileIndex, int tableNo)
            : base(path, fileIndex, tableXsd.Schema, Namespace(tableXsd), NamespaceLocation(Namespace(tableXsd), tableNo))
        {}

        #endregion

        private XmlElement CurrentRow { get; set; }

        public void AddRow()
        {
            CurrentRow = AddElement(Root, "row");
        }

        public void AddField(string columnId, object value)
        {
            if (columnId == null) throw new ArgumentNullException("columnId");

            if (value != null)
                AddElement(CurrentRow, columnId, XmlValue(value));
            else
                AddNillableElement(CurrentRow, columnId);
        }

        private string XmlValue(object value)
        {
            if (value == null) throw new ArgumentNullException("value");


            var name = value.GetType().Name;

            if (value is DateTime?)
            {
                name = "DateTime";
            }

            if (value is TimeSpan?)
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

        protected override string RootName
        {
            get { return "table"; }
        }

        #endregion

        private static new string Namespace(TableXsd tableXsd)
        {
            if (tableXsd == null) throw new ArgumentNullException("tableXsd");

            return tableXsd.Schema.TargetNamespace;
        }

        private static new string NamespaceLocation(string @namespace, int tableNo)
        {
            if (@namespace == null) throw new ArgumentNullException("namespace");

            return String.Format("{0} table{1}.xsd", @namespace, tableNo);
        }

        public static TableXml Load(TableXsd tableXsd, FileInfo tablePath, FileIndex fileIndex, int tableNo)
        {
            var tableXml = new TableXml(tableXsd, tablePath, fileIndex, tableNo);
            using (var fileStream = tablePath.Open(FileMode.Open, FileAccess.Read, FileShare.None))
            {
                var readerSettings = new XmlReaderSettings
                    {
                        IgnoreComments = true,
                        IgnoreProcessingInstructions = true,
                        IgnoreWhitespace = true,
                        ValidationType = ValidationType.Schema,
                        Schemas = new XmlSchemaSet(new NameTable())
                    };
                readerSettings.Schemas.Add(tableXml.Schema);
                readerSettings.ValidationEventHandler += ValidationEventHandler;
                var reader = XmlReader.Create(fileStream, readerSettings);
                try
                {
                    tableXml.Document.Load(reader);
                }
                finally
                {
                    reader.Close();
                }
                fileStream.Close();
            }
            return tableXml;
        }
    }
}
