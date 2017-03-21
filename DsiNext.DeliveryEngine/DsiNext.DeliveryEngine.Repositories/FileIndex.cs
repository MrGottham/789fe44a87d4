using System.Collections.Generic;
using System.Xml;

namespace DsiNext.DeliveryEngine.Repositories
{
    public class FileIndex
    {
        #region Member Variables

        public const string Prefix = "dsi";

        private readonly LinkedList<FileInfo> _files;

        #endregion

        #region Constructors

        public FileIndex()
        {
            _files = new LinkedList<FileInfo>();
        }

        #endregion

        public XmlDocument AsXmlDocument()
        {
            var doc = new XmlDocument();
            var rootElement = doc.CreateElement(Prefix, "fileIndex", Namespaces.NsFileIndex);
            doc.AppendChild(rootElement);

            foreach (var file in _files)
            {
                var f = doc.CreateElement(Prefix, "f", Namespaces.NsFileIndex);

                var foN = doc.CreateElement(Prefix, "foN", Namespaces.NsFileIndex);
                foN.InnerText = file.foN;
                f.AppendChild(foN);

                var fiN = doc.CreateElement(Prefix, "fiN", Namespaces.NsFileIndex);
                fiN.InnerText = file.fiN;
                f.AppendChild(fiN);

                var md5 = doc.CreateElement(Prefix, "md5", Namespaces.NsFileIndex);
                md5.InnerText = file.Md5;
                f.AppendChild(md5);

                rootElement.AppendChild(f);
            }

            return doc;
        }
    }
}
