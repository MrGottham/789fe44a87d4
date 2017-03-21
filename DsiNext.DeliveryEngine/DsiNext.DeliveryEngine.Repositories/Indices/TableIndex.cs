using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Repositories.Indices
{
    public class TableIndex : XmlFileBase
    {
        #region Constructors

        public TableIndex(IDataSource dataSource, FileInfo path, FileIndex fileIndex) : base(path, fileIndex)
        {
            if (dataSource == null) throw new ArgumentNullException("dataSource");

            AddMetaData(dataSource);
        }

        #endregion

        #region Properties

        protected override string Namespace
        {
            get { return "http://www.sa.dk/xmlns/diark/1.0"; }
        }

        protected override string NamespaceLocation
        {
            get { return "http://www.sa.dk/xmlns/diark/1.0 ../Schemas/standard/tableIndex.xsd"; }
        }

        protected override XmlSchema Schema
        {
            get
            {
                var assembly = GetType().Assembly;
                using (var resourceStream = assembly.GetManifestResourceStream(string.Format("{0}.Schemas.standard.{1}", assembly.GetName().Name, "tableIndex.xsd")))
                {
                    if (resourceStream == null)
                    {
                        throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.ResourceNotFound, "archiveIndex.xsd"));
                    }
                    var schema = XmlSchema.Read(resourceStream, ValidationEventHandler);
                    schema.Namespaces.Add("xmlns", schema.TargetNamespace);
                    schema.Namespaces.Add("xsi", XsdNamespace);

                    resourceStream.Close();

                    return schema;
                }
            }
        }

        protected override string RootName
        {
            get { return "siardDiark"; }
        }

        private XmlElement TableElement { get; set; }

        #endregion

        private void AddMetaData(IDataSource dataSource)
        {
            if (dataSource == null) throw new ArgumentNullException("dataSource");

            AddElement(Root, "version", "1.0");
            AddElement(Root, "dbName", MakeSqlIdentifier(dataSource.NameTarget), true);
            AddElement(Root, "databaseProduct", dataSource.Description, true);
            
            TableElement = AddElement(Root, "tables");

            if (dataSource.Views.Count == 0) return;

            var views = AddElement(Root, "views");
            foreach (var view in dataSource.Views)
                AddView(views, view);
        }

        public void AddTable(ITable table, string tableFolder, int rowCount)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            if (tableFolder == null)
            {
                throw new ArgumentNullException("tableFolder");
            }
            if (rowCount < 1)
            {
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.IllegalValue, rowCount, "rowCount"));
            }

            var namespaceManager = new XmlNamespaceManager(Document.NameTable);
            namespaceManager.AddNamespace("ns", Namespace);
            var rowsElement = (XmlElement) TableElement.SelectSingleNode(String.Format("ns:table[ns:name = '{0}']/ns:rows", table.NameTarget), namespaceManager);
            if (rowsElement != null)
            {
                var rows = int.Parse(rowsElement.InnerText);
                rowsElement.InnerText = Convert.ToString(rows + rowCount);
                return;
            }
            var tableElement = AddElement(TableElement, "table");
            AddElement(tableElement, "name", table.NameTarget);
            AddElement(tableElement, "folder", tableFolder);
            AddElement(tableElement, "description", table.Description);
            var columnsElement = AddElement(tableElement, "columns");
            foreach (var field in table.Fields.Where(m => ArchiveVersionRepository.ExcludeField(m) == false))
            {
                var columnElement = AddElement(columnsElement, "column");
                AddElement(columnElement, "name", field.NameTarget);
                AddElement(columnElement, "columnID", field.ColumnId);
                AddElement(columnElement, "type", Sql1999DataType(field.DatatypeOfTarget, field.LengthOfTarget));
                AddElement(columnElement, "typeOriginal", field.OriginalDatatype, true);
                AddElement(columnElement, "defaultValue", field.DefaultValue, true);
                AddBooleanElement(columnElement, "nullable", field.Nullable);
                AddElement(columnElement, "description", field.Description);
                foreach (var function in field.Functionality)
                {
                    AddElement(columnElement, "functionalDescription", function.XmlValue);
                }
            }
            AddPrimaryKey(tableElement, table.PrimaryKey);
            if (table.ForeignKeys.Count > 0)
                AddForeignKeys(tableElement, table.ForeignKeys);
            AddElement(tableElement, "rows", rowCount.ToString(CultureInfo.InvariantCulture));
        }

        private static string Sql1999DataType(Type datatypeOfTarget, int lengthOfTarget)
        {
            if (datatypeOfTarget == null) throw new ArgumentNullException("datatypeOfTarget");

            var name = datatypeOfTarget.Name;
            if (datatypeOfTarget == typeof (int?))
            {
                name = typeof (int).Name;
            }
            if (datatypeOfTarget == typeof (long?))
            {
                name = typeof (long).Name;
            }
            if (datatypeOfTarget == typeof (decimal?))
            {
                name = typeof (decimal).Name;
            }
            if (datatypeOfTarget == typeof (DateTime?))
            {
                name = typeof (DateTime).Name;
            }
            if (datatypeOfTarget == typeof (TimeSpan?))
            {
                name = typeof (TimeSpan).Name;
            }

            switch (name)
            {
                case "char":
                case "Char":
                    return "CHAR(1)";

                case "char[]":
                case "Char[]":
                case "string":
                case "String":
                    return string.Format("NATIONAL CHARACTER VARYING({0})", lengthOfTarget);

                case "int":
                case "Int16":
                case "Int32":
                case "Int64":
                case "long":
                case "Long":
                    return "INTEGER";

                case "decimal":
                case "Decimal":
                case "double":
                case "Double":
                case "Single":
                    return string.Format("NUMERIC({0},2)", lengthOfTarget);

                case "bool":
                case "Boolean":
                    return "BOOLEAN";

                case "DateTime":
                    return "TIMESTAMP(1)WITHOUT TIME ZONE";

                case "TimeSpan":
                    return "TIME(1)WITHOUT TIME ZONE";

                default:
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.DataTypeNotSupported, datatypeOfTarget.Name));
            }
        }

        private void AddView(XmlElement parent, IView view)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (view == null) throw new ArgumentNullException("view");

            var viewElement = AddElement(parent, "view");
            AddElement(viewElement, "name", view.NameTarget); 
            AddElement(viewElement, "queryOriginal", view.SqlQuery);
            AddElement(viewElement, "description", view.Description, true);
        }

        private void AddPrimaryKey(XmlElement parent, ICandidateKey key)
        {
            var primaryKey = AddElement(parent, "primaryKey");
            AddElement(primaryKey, "name", key.NameTarget);

            foreach (var field in key.Fields)
                AddElement(primaryKey, "column", field.Key.NameTarget);
        }

        private void AddForeignKeys(XmlElement parent, IEnumerable<IForeignKey> keys)
        {
            var foreignKeys = AddElement(parent, "foreignKeys");

            foreach (var key in keys)
            {
                var foreignKey = AddElement(foreignKeys, "foreignKey");
                AddElement(foreignKey, "name", key.NameTarget);
                AddElement(foreignKey, "referencedTable", key.CandidateKey.Table.NameTarget);

                for (var i = 0; i < key.Fields.Count; i++)
                {
                    var reference = AddElement(foreignKey, "reference");

                    AddElement(reference, "column", key.Fields[i].Key.NameTarget);
                    AddElement(reference, "referenced", key.CandidateKey.Fields[i].Key.NameTarget);
                }
            }
        }

        protected override void Validate()
        {
            if (TableElement.HasChildNodes)
            {
                base.Validate();
                return;
            }

            var dummy = AddDumyTableForValidation();

            try
            {
                base.Validate();
            }
            finally
            {
                TableElement.RemoveChild(dummy);
            }
        }

        private XmlElement AddDumyTableForValidation()
        {
            var dummy = AddElement(TableElement, "table");
            AddElement(dummy, "name", "dummy");
            AddElement(dummy, "folder", "dummy");
            AddElement(dummy, "description", "dummy");

            var columns = AddElement(dummy, "columns");
            var column = AddElement(columns, "column");
            AddElement(column, "name", "dummy");
            AddElement(column, "columnID", "c1");
            AddElement(column, "type", "INTEGER");
            AddBooleanElement(column, "nullable", true);
            AddElement(column, "description", "dummy");

            var primaryKey = AddElement(dummy, "primaryKey");
            AddElement(primaryKey, "name", "dummy");
            AddElement(primaryKey, "column", "dummy");

            AddElement(dummy, "rows", "1");

            return dummy;
        }
    }
}
