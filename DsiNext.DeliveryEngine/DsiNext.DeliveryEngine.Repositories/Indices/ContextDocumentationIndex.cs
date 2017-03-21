using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Repositories.Indices
{
    public class ContextDocumentationIndex : XmlFileBase
    {
        #region Constructors

        public ContextDocumentationIndex(IEnumerable<IContextDocument> contextDocuments, FileInfo path, FileIndex fileIndex) 
            : base(path, fileIndex)
        {
            if (contextDocuments == null) throw new ArgumentNullException("contextDocuments");

            foreach (var contextDocument in contextDocuments)
                AddContextDocument(contextDocument);
        }

        #endregion

        #region Properties

        protected override string Namespace
        {
            get { return "http://www.sa.dk/xmlns/diark/1.0"; }
        }

        protected override string NamespaceLocation
        {
            get { return "http://www.sa.dk/xmlns/diark/1.0 ../Schemas/standard/contextDocumentationIndex.xsd"; }
        }

        protected override XmlSchema Schema
        {
            get
            {
                var assembly = GetType().Assembly;
                using (var resourceStream = assembly.GetManifestResourceStream(string.Format("{0}.Schemas.standard.{1}", assembly.GetName().Name, "contextDocumentationIndex.xsd")))
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
            get { return "contextDocumentationIndex"; }
        }

        #endregion

        private void AddContextDocument(IContextDocument contextDocument)
        {
            if (contextDocument == null) throw new ArgumentNullException("contextDocument");

            var doc = AddElement(Root, "document");

            AddElement(doc, "documentID", contextDocument.Id.ToString(CultureInfo.InvariantCulture));
            AddElement(doc, "documentTitle", contextDocument.NameTarget);
            AddElement(doc, "documentDescription", contextDocument.Description, true);
            if (contextDocument.DocumentDate.HasValue)
            {
                var documentDate = contextDocument.DocumentDate.Value;

                string dateValue;
                switch (contextDocument.DocumentDatePresicion)
                {
                    case DateTimePresicion.Year:
                        dateValue = documentDate.ToString("yyyy");
                        break;
                    case DateTimePresicion.Month:
                        dateValue = documentDate.ToString("yyyy-MM");
                        break;
                    case DateTimePresicion.Day:
                        dateValue = documentDate.ToString("yyyy-MM-dd");
                        break;

                    default:
                        throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.IllegalValue, "DocumentDatePresicion", contextDocument.DocumentDatePresicion));
                }

                AddElement(doc, "documentDate", dateValue);
            }

            foreach (var documentAuthor in contextDocument.DocumentAuthors)
            {
                var documentAuthorElement = AddElement(doc, "documentAuthor");

                if (documentAuthor.Author != null)
                    AddElement(documentAuthorElement, "authorName", documentAuthor.Author);

                if (documentAuthor.Institution != null)
                    AddElement(documentAuthorElement, "authorInstitution", documentAuthor.Institution);
            }

            var documentCategory = AddElement(doc, "documentCategory");
            AddCategories(documentCategory, contextDocument.Categories);
        }

        //private void AddCategories(XmlElement documentCategory, IEnumerable<ContextDocumentCategories> categories)
        //{
        //    if (documentCategory == null) throw new ArgumentNullException("documentCategory");

        //    var sortedCategories = new SortedDictionary<int, ContextDocumentCategories>();
        //    foreach (var category in categories)
        //    {
        //        sortedCategories.Add((int)category, category);
        //    }

        //    var informationTypes = new Dictionary<string, XmlElement>();
        //    foreach (var category in sortedCategories)
        //    {
        //        var informationType = String.Empty;

        //        switch (category.Value)
        //        {
        //            case ContextDocumentCategories.SystemPurpose:
        //            case ContextDocumentCategories.SystemRegulations:
        //            case ContextDocumentCategories.SystemContent:
        //            case ContextDocumentCategories.SystemAdministrativeFunctions:
        //            case ContextDocumentCategories.SystemPresentationStructure:
        //            case ContextDocumentCategories.SystemDataProvision:
        //            case ContextDocumentCategories.SystemDataTransfer:
        //            case ContextDocumentCategories.SystemPreviousSubsequentFunctions:
        //            case ContextDocumentCategories.SystemAgencyQualityControl:
        //            case ContextDocumentCategories.SystemPublication:
        //            case ContextDocumentCategories.SystemInformationOther:
        //                informationType = "systemInformation";
        //                break;
                        
        //            case ContextDocumentCategories.OperationalSystemInformation:
        //            case ContextDocumentCategories.OperationalSystemConvertedInformation:
        //            case ContextDocumentCategories.OperationalSystemSOA:
        //            case ContextDocumentCategories.OperationalSystemInformationOther:
        //                informationType = "operationalInformation";
        //                break;

        //            case ContextDocumentCategories.ArchivalProvisions:
        //            case ContextDocumentCategories.ArchivalTransformationInformation:
        //            case ContextDocumentCategories.ArchivalSubmissionInformationOther:
        //                informationType = "submissionInformation";
        //                break;

        //            case ContextDocumentCategories.ArchivistNotes:
        //            case ContextDocumentCategories.ArchivalTestNotes:
        //            case ContextDocumentCategories.ArchivalIngestInformationOther:
        //                informationType = "ingestInformation";
        //                break;

        //            case ContextDocumentCategories.ArchivalMigrationInformation:
        //            case ContextDocumentCategories.ArchivalMigrationInformationOther:
        //                informationType = "archivalPreservationInformation";
        //                break;

        //            case ContextDocumentCategories.InformationOther:
        //                informationType = "informationOther";
        //                break;
        //        }

        //        if (informationTypes.ContainsKey(informationType) == false)
        //            informationTypes.Add(informationType, AddElement(documentCategory, informationType));

        //        var categoryName = category.Value.ToString();
        //        categoryName = Char.ToLower(categoryName[0]) + categoryName.Substring(1);
        //        AddElement(informationTypes[informationType], categoryName, "true");
        //    }
        //}

        private void AddCategories(XmlElement documentCategory, IEnumerable<ContextDocumentCategories> categories)
        {
            if (documentCategory == null) throw new ArgumentNullException("documentCategory");
            if (categories == null) throw new ArgumentNullException("categories");

            var cat = new HashSet<ContextDocumentCategories>();
            foreach (var category in categories)
            {
                cat.Add(category);
            }

            var systemInformation = AddElement(documentCategory, "systemInformation");
            AddElement(systemInformation, "systemPurpose", cat.Contains(ContextDocumentCategories.SystemPurpose) ? "true" : "false");
            AddElement(systemInformation, "systemRegulations", cat.Contains(ContextDocumentCategories.SystemRegulations) ? "true" : "false");
            AddElement(systemInformation, "systemContent", cat.Contains(ContextDocumentCategories.SystemContent) ? "true" : "false");
            AddElement(systemInformation, "systemAdministrativeFunctions", cat.Contains(ContextDocumentCategories.SystemAdministrativeFunctions) ? "true" : "false");
            AddElement(systemInformation, "systemPresentationStructure", cat.Contains(ContextDocumentCategories.SystemPresentationStructure) ? "true" : "false");
            AddElement(systemInformation, "systemDataProvision", cat.Contains(ContextDocumentCategories.SystemDataProvision) ? "true" : "false");
            AddElement(systemInformation, "systemDataTransfer", cat.Contains(ContextDocumentCategories.SystemDataTransfer) ? "true" : "false");
            AddElement(systemInformation, "systemPreviousSubsequentFunctions", cat.Contains(ContextDocumentCategories.SystemPreviousSubsequentFunctions) ? "true" : "false");
            AddElement(systemInformation, "systemAgencyQualityControl", cat.Contains(ContextDocumentCategories.SystemAgencyQualityControl) ? "true" : "false");
            AddElement(systemInformation, "systemPublication", cat.Contains(ContextDocumentCategories.SystemPublication) ? "true" : "false");
            AddElement(systemInformation, "systemInformationOther", cat.Contains(ContextDocumentCategories.SystemInformationOther) ? "true" : "false");

            var operationalInformation = AddElement(documentCategory, "operationalInformation");
            AddElement(operationalInformation, "operationalSystemInformation", cat.Contains(ContextDocumentCategories.OperationalSystemInformation) ? "true" : "false");
            AddElement(operationalInformation, "operationalSystemConvertedInformation", cat.Contains(ContextDocumentCategories.OperationalSystemConvertedInformation) ? "true" : "false");
            AddElement(operationalInformation, "operationalSystemSOA", cat.Contains(ContextDocumentCategories.OperationalSystemSOA) ? "true" : "false");
            AddElement(operationalInformation, "operationalSystemInformationOther", cat.Contains(ContextDocumentCategories.OperationalSystemInformationOther) ? "true" : "false");

            var submissionInformation = AddElement(documentCategory, "submissionInformation");
            AddElement(submissionInformation, "archivalProvisions", cat.Contains(ContextDocumentCategories.ArchivalProvisions) ? "true" : "false");
            AddElement(submissionInformation, "archivalTransformationInformation", cat.Contains(ContextDocumentCategories.ArchivalTransformationInformation) ? "true" : "false");
            AddElement(submissionInformation, "archivalInformationOther", cat.Contains(ContextDocumentCategories.ArchivalSubmissionInformationOther) ? "true" : "false");

            var ingestInformation = AddElement(documentCategory, "ingestInformation");
            AddElement(ingestInformation, "archivistNotes", cat.Contains(ContextDocumentCategories.ArchivistNotes) ? "true" : "false");
            AddElement(ingestInformation, "archivalTestNotes", cat.Contains(ContextDocumentCategories.ArchivalTestNotes) ? "true" : "false");
            AddElement(ingestInformation, "archivalInformationOther", cat.Contains(ContextDocumentCategories.ArchivalIngestInformationOther) ? "true" : "false");

            var archivalPreservationInformation = AddElement(documentCategory, "archivalPreservationInformation");
            AddElement(archivalPreservationInformation, "archivalMigrationInformation", cat.Contains(ContextDocumentCategories.ArchivalMigrationInformation) ? "true" : "false");
            AddElement(archivalPreservationInformation, "archivalInformationOther", cat.Contains(ContextDocumentCategories.ArchivalMigrationInformationOther) ? "true" : "false");

            var informationOther = AddElement(documentCategory, "informationOther");
            AddElement(informationOther, "informationOther", cat.Contains(ContextDocumentCategories.InformationOther) ? "true" : "false");
        }
    }
}
