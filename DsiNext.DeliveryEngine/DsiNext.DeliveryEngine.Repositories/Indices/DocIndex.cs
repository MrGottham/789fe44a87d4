using System;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Repositories.Indices
{
    public class DocIndex : IndexBase
    {
        #region Member Variables
        #endregion

        #region Constructors

        public DocIndex() { }

        #endregion

        #region Overrides of IndexBase

        protected override string Namespace
        {
            get { return "http://www.sa.dk/xmlns/diark/1.0"; }
        }

        protected override string NamespaceLocation
        {
            get { return "http://www.sa.dk/xmlns/diark/1.0 ../../docIndex.xsd"; }
        }

        protected override string RootName
        {
            get { return "docIndex"; }
        }

        #endregion

        private void AddDocument(IDocument document)
        {
            var doc = AddElement(Root, "doc");

            AddElement(doc, "dID", document.Id.ToString());
            // TODO AddDocument
        }
    }
}
