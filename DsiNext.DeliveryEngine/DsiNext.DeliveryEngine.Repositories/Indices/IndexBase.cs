using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace DsiNext.DeliveryEngine.Repositories.Indices
{
    public abstract class IndexBase : XmlFileBase
    {
        #region Member Variables

        private const string XsdNamespace = "http://www.w3.org/2001/XMLSchema-instance";

        #endregion

        #region Constructors

        protected IndexBase()
        {
            InitDocument();
        }

        #endregion

        private void InitDocument()
        {
            var schema = new XmlSchema();
            schema.Namespaces.Add("xmlns", Namespace);
            schema.Namespaces.Add("xsi", XsdNamespace);
            Document.Schemas.Add(schema);

            var root = Document.CreateElement(RootName, Namespace);
            Document.AppendChild(root);

            var schemaLocation = Document.CreateAttribute("xsi", "schemaLocation", NamespaceLocation);
            Root.SetAttributeNode(schemaLocation);
        }

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

        protected void AddBooleanElement(XmlElement parent, string name, bool value)
        {
            AddElement(parent, name, value ? "true" : "false");
        }

        protected void AddDateTimeElement(XmlElement parent, string name, DateTime value)
        {
            AddElement(parent, name, value.ToString("yyyy-MM-dd"));
        }

        protected void AddElementCollection(XmlElement parent, string name, IEnumerable<string> values)
        {
            foreach (var value in values)
                AddElement(parent, name, value);
        }
    }
}
