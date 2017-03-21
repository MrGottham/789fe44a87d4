using System;
using System.Globalization;
using System.IO;
using System.Xml.Schema;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Repositories.Indices
{
    public class ArchiveIndex : XmlFileBase
    {
        #region Constructors

        public ArchiveIndex(IDataSource dataSource, FileInfo path, FileIndex fileIndex) : base(path, fileIndex)
        {
            if (dataSource == null) throw new ArgumentNullException("dataSource");

            Build(dataSource);
        }

        #endregion

        #region Properties

        protected override string Namespace
        {
            get { return "http://www.sa.dk/xmlns/diark/1.0"; }
        }

        protected override string NamespaceLocation
        {
            get { return "http://www.sa.dk/xmlns/diark/1.0 ../Schemas/standard/archiveIndex.xsd"; }
        }

        protected override XmlSchema Schema
        {
            get
            {
                var assembly = GetType().Assembly;
                using (var resourceStream = assembly.GetManifestResourceStream(string.Format("{0}.Schemas.standard.{1}", assembly.GetName().Name, "archiveIndex.xsd")))
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
            get { return "archiveIndex"; }
        }

        #endregion

        private void Build(IDataSource dataSource)
        {
            if (dataSource == null) throw new ArgumentNullException("dataSource");

            AddElement(Root, "archiveInformationPackageID", dataSource.ArchiveInformationPackageId);
            if (dataSource.ArchiveInformationPackageIdPrevious > 0)
            {
                AddElement(Root, "archiveInformationPackageIDPrevious", dataSource.ArchiveInformationPackageIdPrevious.ToString("00000000", CultureInfo.InvariantCulture));
            }
            AddDateTimeElement(Root, "archivePeriodStart", dataSource.ArchivePeriodStart);
            AddDateTimeElement(Root, "archivePeriodEnd", dataSource.ArchivePeriodEnd);
            AddBooleanElement(Root, "archiveInformationPacketType", dataSource.ArchiveInformationPacketType);

            var archiveCreatorList = AddElement(Root, "archiveCreatorList");
            foreach (var creator in dataSource.Creators)
            {
                AddElement(archiveCreatorList, "creatorName", creator.NameTarget);
                AddElement(archiveCreatorList, "creationPeriodStart", creator.PeriodStart.ToString("yyyy-MM-dd"));
                AddElement(archiveCreatorList, "creationPeriodEnd", creator.PeriodEnd.ToString("yyyy-MM-dd"));
            }

            AddBooleanElement(Root, "archiveType", dataSource.ArchiveType);
            AddElement(Root, "systemName", dataSource.NameTarget);
            AddElementCollection(Root, "alternativeName", dataSource.AlternativeSystemNames);
            AddElement(Root, "systemPurpose", dataSource.SystemPurpose);
            AddElement(Root, "systemContent", dataSource.SystemContent);
            AddBooleanElement(Root, "regionNum", dataSource.RegionNum);
            AddBooleanElement(Root, "komNum", dataSource.KomNum);
            AddBooleanElement(Root, "cprNum", dataSource.CprNum);
            AddBooleanElement(Root, "cvrNum", dataSource.CvrNum);
            AddBooleanElement(Root, "matrikNum", dataSource.MatrikNum);
            AddBooleanElement(Root, "bbrNum", dataSource.BbrNum);
            AddBooleanElement(Root, "whoSygKod", dataSource.WhoSygKod);
            AddElementCollection(Root, "sourceName", dataSource.SourceNames);
            AddElementCollection(Root, "userName", dataSource.UserNames);
            AddElementCollection(Root, "predecessorName", dataSource.PredecessorNames);

            if (String.IsNullOrWhiteSpace(dataSource.FormVersion) == false)
            {
                var form = AddElement(Root, "form");
                AddElement(form, "formVersion", dataSource.FormVersion);

                var classList = AddElement(form, "classList");
                foreach (var @class in dataSource.FormClasses)
                {
                    AddElement(classList, "formClass", @class.FormClassName);
                    AddElement(classList, "formClassText", @class.FormClassText);
                }
            }

            AddBooleanElement(Root, "containsDigitalDocuments", dataSource.ContainsDigitalDocuments);
            AddBooleanElement(Root, "searchRelatedOtherRecords", dataSource.SearchRelatedOtherRecords);
            AddElementCollection(Root, "relatedRecordsName", dataSource.RelatedRecordsNames);
            AddBooleanElement(Root, "systemFileConcept", dataSource.SystemFileConcept);
            AddBooleanElement(Root, "multipleDataCollection", dataSource.MultipleDataCollection);
            AddBooleanElement(Root, "personalDataRestrictedInfo", dataSource.PersonalDataRestrictedInfo);
            AddBooleanElement(Root, "otherAccessTypeRestrictions", dataSource.OtherAccessTypeRestrictions);
            AddElement(Root, "archiveApproval", dataSource.ArchiveApproval);
            AddElement(Root, "archiveRestrictions", dataSource.ArchiveRestrictions, true);
        }
    }
}
