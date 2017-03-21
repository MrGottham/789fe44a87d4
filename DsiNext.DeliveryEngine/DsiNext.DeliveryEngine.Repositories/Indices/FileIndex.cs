using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Schema;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Repositories.Indices
{
    public class FileIndex : XmlFileBase
    {
        private readonly DirectoryInfo _destinationFolder;
        private readonly IDictionary<FileInfo, XmlElement> _checksums;
        private readonly MD5CryptoServiceProvider _md5CryptoServiceProvider;

        public FileIndex(FileInfo path, DirectoryInfo destinationFolder)
            : base(path, null)
        {
            if (destinationFolder == null) throw new ArgumentNullException("destinationFolder");

            _destinationFolder = destinationFolder;
            _checksums = new Dictionary<FileInfo, XmlElement>();
            _md5CryptoServiceProvider = new MD5CryptoServiceProvider();
        }

        #region Properties

        protected override string Namespace
        {
            get { return "http://www.sa.dk/xmlns/diark/1.0"; }
        }

        protected override string NamespaceLocation
        {
            get { return "http://www.sa.dk/xmlns/diark/1.0 ../Schemas/standard/fileIndex.xsd"; }
        }

        protected override XmlSchema Schema
        {
            get
            {
                var assembly = GetType().Assembly;
                using (var resourceStream = assembly.GetManifestResourceStream(string.Format("{0}.Schemas.standard.{1}", assembly.GetName().Name, "fileIndex.xsd")))
                {
                    if (resourceStream == null)
                    {
                        throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.ResourceNotFound, "fileIndex.xsd"));
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
            get { return "fileIndex"; }
        }

        #endregion

        public void AddFile(FileInfo file)
        {
            if (file == null) throw new ArgumentNullException("file");
            if (file.Exists == false) throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.FileNotFound, file.FullName));

            if (_checksums.ContainsKey(file))
            {
                _checksums[file].InnerText = CheckSum(file);
                return;
            }

            var foN = file.DirectoryName;
            var fiN = file.Name;

            if (foN == null || fiN == null)
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.FileNotFound, file.FullName));

            if (foN.StartsWith(_destinationFolder.FullName))
                foN = foN.Substring(_destinationFolder.FullName.Length + 1);

            var f = AddElement(Root, "f");

            AddElement(f, "foN", foN);
            AddElement(f, "fiN", fiN);
            var checksum = AddElement(f, "md5", CheckSum(file));
            _checksums.Add(file, checksum);
        }

        private string CheckSum(FileInfo path)
        {
            if (path == null) throw new ArgumentNullException("path");

            Byte[] hash;
            using (var fs = new FileStream(path.FullName, FileMode.Open))
            {
                hash = _md5CryptoServiceProvider.ComputeHash(fs);
            }

            return BitConverter.ToString(hash).Replace("-", "");
        }
    }
}
