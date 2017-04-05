using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.Indices;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Repositories
{
    public abstract class XmlFileBase
    {
        private readonly FileInfo _path;
        private readonly FileIndex _fileIndex;
        private XmlSchema _schema;

        #region Member Variables

        protected const string XsdNamespace = "http://www.w3.org/2001/XMLSchema-instance";

        #endregion

        #region Constructors

        protected XmlFileBase(FileInfo path, FileIndex fileIndex)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (fileIndex == null && (this is FileIndex == false)) throw new ArgumentNullException(nameof(fileIndex));

            _path = path;
            _fileIndex = fileIndex;

            InitDocument();
        }

        protected XmlFileBase(FileInfo path, FileIndex fileIndex, XmlSchema xsd, string @namespace, string namespaceLocation)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (xsd == null) throw new ArgumentNullException(nameof(xsd));
            if (@namespace == null) throw new ArgumentNullException(nameof(@namespace));
            if (namespaceLocation == null) throw new ArgumentNullException(nameof(namespaceLocation));
            if (fileIndex == null && (this is FileIndex == false)) throw new ArgumentNullException(nameof(fileIndex));

            _path = path;
            _fileIndex = fileIndex;
            Namespace = @namespace;
            NamespaceLocation = namespaceLocation;

            InitDocument();
        }

        #endregion

        private void InitDocument()
        {
            Document = new XmlDocument();

            Document.Schemas.Add(Schema);

            var decleration = Document.CreateXmlDeclaration("1.0", "utf-8", null);
            Document.AppendChild(decleration);

            var root = Document.CreateElement(RootName, Namespace);
            Document.AppendChild(root);

            var schemaLocation = Document.CreateAttribute("xsi", "schemaLocation", XsdNamespace);
            schemaLocation.Value = NamespaceLocation;
            Root.SetAttributeNode(schemaLocation);
        }

        protected XmlDocument Document { get; private set; }

        protected XmlElement Root => Document.DocumentElement;

        protected virtual string Namespace { get; }

        protected virtual string NamespaceLocation { get; }

        protected virtual XmlSchema Schema
        {
            get
            {
                if (_schema == null)
                {
                    _schema = new XmlSchema();
                    _schema.Namespaces.Add("xmlns", Namespace);
                    _schema.Namespaces.Add("xsi", XsdNamespace);
                }
                return _schema;
            }
            set
            {
                if (value == null) throw new NullReferenceException("Schema" + "");

                _schema = value;
            }
        }

        protected abstract string RootName { get; }

        #region AddElements

        protected XmlElement AddElement(XmlElement parent, string name)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (name == null) throw new ArgumentNullException(nameof(name));

            var element = Document.CreateElement(name, Namespace);
            parent.AppendChild(element);
            return element;
        }

        protected XmlElement AddElement(XmlElement parent, string name, string value, bool skipNullOrWhiteSpace = false)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (name == null) throw new ArgumentNullException(nameof(name));

            if (skipNullOrWhiteSpace && String.IsNullOrWhiteSpace(value)) return null;

            var element = Document.CreateElement(name, Namespace);
            element.InnerText = value;
            parent.AppendChild(element);

            return element;
        }

        protected XmlElement AddBooleanElement(XmlElement parent, string name, bool value)
        {
            return AddElement(parent, name, value ? "true" : "false");
        }

        protected XmlElement AddDateTimeElement(XmlElement parent, string name, DateTime value)
        {
            return AddElement(parent, name, value.ToString("yyyy-MM-dd"));
        }

        protected void AddElementCollection(XmlElement parent, string name, IEnumerable<string> values)
        {
            foreach (var value in values)
                AddElement(parent, name, value);
        }

        protected XmlElement AddNillableElement(XmlElement parent, string name)
        {
            var nillable = Document.CreateAttribute("xsi", "nil", XsdNamespace);
            nillable.Value = "true";

            var element = Document.CreateElement(name, Namespace);
            element.SetAttributeNode(nillable);
            parent.AppendChild(element);

            return element;
        }

        #endregion

        public string Content()
        {
            var result = String.Empty;

            using (var xmlwriter = new XmlTextWriter(result, Encoding.Unicode))
            {
                xmlwriter.Formatting = Formatting.Indented;
                
                Document.WriteTo(xmlwriter);
            }

            return result;
        }

        public void Persist()
        {
            Validate();

            try
            {
                using (var filestream = new FileStream(_path.FullName, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (var xmlwriter = new XmlTextWriter(filestream, Encoding.UTF8))
                    {
                        xmlwriter.Formatting = Formatting.Indented;
                        Document.WriteTo(xmlwriter);
                    }
                }
                _path.Refresh();
            }
            catch (Exception ex)
            {
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.FileWriteError, _path.FullName), ex);
            }

            if (this is FileIndex)
            {
                return;
            }
            _fileIndex.AddFile(_path);
        }

        protected virtual void Validate()
        {
            Document.Validate(ValidationEventHandler);
        }

        protected static void ValidationEventHandler(object sender, ValidationEventArgs eventArgs)
        {
            if (eventArgs == null)
            {
                throw new ArgumentNullException(nameof(eventArgs));
            }
            switch (eventArgs.Severity)
            {
                case XmlSeverityType.Warning:
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.RepositoryError, MethodBase.GetCurrentMethod().Name, eventArgs.Message));

                case XmlSeverityType.Error:
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.RepositoryError, MethodBase.GetCurrentMethod().Name, eventArgs.Message));
            }
        }

        protected virtual string MakeSqlIdentifier(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }
            var removeChars = new[] {'-', ',', '@'};
            value = removeChars.Aggregate(value, (current, chr) => current.Replace(chr.ToString(CultureInfo.InvariantCulture), string.Empty));
            return value.Replace(" ", "_");
        }
    }
}
