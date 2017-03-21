using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.Indices;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Repositories.Tables
{
    public class TableXsd
    {
        #region Member Variables

        //private readonly XmlSchemaSequence _rowTypeSequence;
        private readonly FileInfo _path;
        private readonly FileIndex _fileIndex;

        #endregion

        #region Constructors

        public TableXsd(ITable t, int tableNo, FileInfo path, FileIndex fileIndex)
        {
            if (t == null) throw new ArgumentNullException("t");
            if (path == null) throw new ArgumentNullException("path");
            if (fileIndex == null) throw new ArgumentNullException("fileIndex");
            if (tableNo < 1) throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.IllegalValue, tableNo, "tableNo"));

            _path = path;
            _fileIndex = fileIndex;
            TargetNamespace = String.Format("http://www.sa.dk/xmlns/siard/1.0/schema0/table{0}.xsd", tableNo);

            InitSchema();
        }

        #endregion

        private void InitSchema()
        {
            Schema = new XmlSchema
            {
                TargetNamespace = TargetNamespace,
                ElementFormDefault = XmlSchemaForm.Qualified,
                AttributeFormDefault = XmlSchemaForm.Unqualified
            };

            var namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
            namespaceManager.AddNamespace(String.Empty, TargetNamespace);

            RowTypeSequence = new XmlSchemaSequence();
            var rowType = new XmlSchemaComplexType { Name = "rowType", Particle = RowTypeSequence };

            var row = new XmlSchemaElement
            {
                Name = "row",
                MinOccurs = 1,
                MaxOccursString = "unbounded",
                SchemaTypeName = new XmlQualifiedName(rowType.Name)
            };

            var tableSequence = new XmlSchemaSequence();
            tableSequence.Items.Add(row);

            var tableComplexType = new XmlSchemaComplexType { Particle = tableSequence };


            var table = new XmlSchemaElement { Name = "table", SchemaType = tableComplexType };
            Schema.Items.Add(table);
            Schema.Items.Add(rowType);
        }

        protected XmlSchemaSequence RowTypeSequence { get; set; }

        private string TargetNamespace { get; set; }

        public XmlSchema Schema { get; private set; }

        public void AddColumn(string columnId, Type type, bool nillable)
        {
            if (columnId == null) throw new ArgumentNullException("columnId");
            if (type == null) throw new ArgumentNullException("type");

            var c = new XmlSchemaElement
                        {
                            Name = columnId,
                            SchemaTypeName = XsdType(type),
                            IsNillable = nillable
                        };

            RowTypeSequence.Items.Add(c);
        }

        public void Perist()
        {
            try
            {
                using (var filestream = new FileStream(_path.FullName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (var xmlwriter = new XmlTextWriter(filestream, Encoding.Unicode))
                    {
                        xmlwriter.Formatting = Formatting.Indented;
                        Schema.Write(xmlwriter);
                    }
                }
                _path.Refresh();
            }
            catch (Exception ex)
            {
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.FileWriteError, _path.FullName), ex);
            }

            _fileIndex.AddFile(_path); 
        }

        private static XmlQualifiedName XsdType(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            var name = type.Name;
            if (type == typeof (int?))
            {
                name = typeof (int).Name;
            }
            if (type == typeof (long?))
            {
                name = typeof (long).Name;
            }
            if (type == typeof (decimal?))
            {
                name = typeof (decimal).Name;
            }
            if (type == typeof (DateTime?))
            {
                name = typeof (DateTime).Name;
            }
            if (type == typeof (TimeSpan?))
            {
                name = typeof (TimeSpan).Name;
            }

            switch (name)
            {
                case "char":
                case "Char":
                case "char[]":
                case "Char[]":
                case "string":
                case "String":
                    return new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");

                case "int":
                case "Int16":
                case "Int32":
                case "Int64":
                case "long":
                case "Long":
                    return new XmlQualifiedName("integer", "http://www.w3.org/2001/XMLSchema");

                case "decimal":
                case "Decimal":
                case "double":
                case "Double":
                case "Single":
                    return new XmlQualifiedName("decimal", "http://www.w3.org/2001/XMLSchema");

                case "bool":
                case "Boolean":
                    return new XmlQualifiedName("boolean", "http://www.w3.org/2001/XMLSchema");

                /*
                typer er redundante i forhold til string typer
                case "": return new XmlQualifiedName("hexBinary", "http://www.w3.org/2001/XMLSchema");
                */

                case "DateTime":
                    return new XmlQualifiedName("date", "http://www.w3.org/2001/XMLSchema");

                case "TimeSpan":
                    return new XmlQualifiedName("time", "http://www.w3.org/2001/XMLSchema");

                default:
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.DataTypeNotSupported, type.Name));
            }
        }
    }
}
