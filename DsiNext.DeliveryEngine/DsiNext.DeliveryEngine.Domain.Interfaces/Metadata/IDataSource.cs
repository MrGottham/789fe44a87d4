using System;
using System.Collections.ObjectModel;

namespace DsiNext.DeliveryEngine.Domain.Interfaces.Metadata
{
    public interface IDataSource : INamedObject
    {
        /// <summary>
        /// Connection string.
        /// </summary>
        string ConnectionString
        {
            get;
            set;
        }

        /// <summary>
        /// Unique package ID for the archive.
        /// </summary>
        string ArchiveInformationPackageId
        {
            get;
            set;
        }

        /// <summary>
        /// Unique package ID for the previous archive.
        /// </summary>
        int ArchiveInformationPackageIdPrevious
        {
            get;
            set;
        }

        /// <summary>
        /// Start of the periode for the archive.
        /// </summary>
        DateTime ArchivePeriodStart
        {
            get;
            set;
        }

        /// <summary>
        /// End of the periode for the archive.
        /// </summary>
        DateTime ArchivePeriodEnd
        {
            get;
            set;
        }

        /// <summary>
        /// Packet type for the archive.
        /// True if this is an end delivery otherwise false.
        /// </summary>
        bool ArchiveInformationPacketType
        {
            get;
            set;
        }

        /// <summary>
        /// Mark for the period type of the archive.
        /// True if the archive period has ended or false
        /// if the the period type i a current picture.
        /// </summary>
        bool ArchiveType
        {
            get;
            set;
        }

        /// <summary>
        /// Description the system purpose.
        /// </summary>
        string SystemPurpose
        {
            get;
            set;
        }

        /// <summary>
        /// Description of the content in the system.
        /// </summary>
        string SystemContent
        {
            get;
            set;
        }

        /// <summary>
        /// Indication of whether, in systematic form is reigstreret region numbers in the system.
        /// </summary>
        bool RegionNum
        {
            get;
            set;
        }

        /// <summary>
        /// Indication of whether, in systematic form is registered local numbers in the system.
        /// </summary>
        bool KomNum
        {
            get;
            set;
        }

        /// <summary>
        /// Indication of whether, in systematic form registered civil registration numbers in the system.
        /// </summary>
        bool CprNum
        {
            get;
            set;
        }

        /// <summary>
        /// Indication of whether, in systematic form is registered VAT numbers in the system.
        /// </summary>
        bool CvrNum
        {
            get;
            set;
        }

        /// <summary>
        /// Indication of whether, in systematic form, registered cadastral data in the system.
        /// </summary>
        bool MatrikNum
        {
            get;
            set;
        }

        /// <summary>
        /// Indication of whether, in systematic form is registered BBR numbers in the system.
        /// </summary>
        bool BbrNum
        {
            get;
            set;
        }

        /// <summary>
        /// Indication of whether, in systematic form is registered WHO disease codes numbers in the system.
        /// </summary>
        bool WhoSygKod
        {
            get;
            set;
        }

        /// <summary>
        /// FORM version.
        /// </summary>
        string FormVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Indication of digital documents is included in delivery.
        /// </summary>
        bool ContainsDigitalDocuments
        {
            get;
            set;
        }
        
        /// <summary>
        /// Search agent for other cases or documents.
        /// </summary>
        bool SearchRelatedOtherRecords
        {
            get;
            set;
        }

        /// <summary>
        /// Indication of whether the system has a case definition.
        /// </summary>
        bool SystemFileConcept
        {
            get;
            set;
        }

        /// <summary>
        /// Indication of whether the system is part of an SOA architecture.
        /// </summary>
        bool MultipleDataCollection
        {
            get;
            set;
        }

        /// <summary>
        /// Indication of whether the system contains personal informations.
        /// </summary>
        bool PersonalDataRestrictedInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Indication of whether the system contains information that could lead to longer access time.
        /// </summary>
        bool OtherAccessTypeRestrictions
        {
            get;
            set;
        }

        /// <summary>
        /// Approval archive.
        /// </summary>
        string ArchiveApproval
        {
            get;
            set;
        }

        /// <summary>
        /// Constraints of availability.
        /// </summary>
        string ArchiveRestrictions
        {
            get;
            set;
        }

        /// <summary>
        /// Tables.
        /// </summary>
        ReadOnlyObservableCollection<ITable> Tables
        {
            get;
        }

        /// <summary>
        /// Information about the views (queries).
        /// </summary>
        ReadOnlyObservableCollection<IView> Views
        {
            get;
        }

        /// <summary>
        /// Creators of the archive.
        /// </summary>
        ReadOnlyObservableCollection<ICreator> Creators
        {
            get;
        }

        /// <summary>
        /// Alternative system names.
        /// </summary>
        ReadOnlyObservableCollection<string> AlternativeSystemNames
        {
            get;
        }

        /// <summary>
        /// Other IT systems that have supplied data to the system.
        /// </summary>
        ReadOnlyObservableCollection<string> SourceNames
        {
            get;
        }

        /// <summary>
        /// User of data in the system.
        /// </summary>
        ReadOnlyObservableCollection<string> UserNames
        {
            get;
        }

        /// <summary>
        /// Earlier systems having the same function.
        /// </summary>
        ReadOnlyObservableCollection<string> PredecessorNames
        {
            get;
        }

        /// <summary>
        /// FORM classifications.
        /// </summary>
        ReadOnlyObservableCollection<IFormClass> FormClasses
        {
            get;
        }

        /// <summary>
        /// Reference to the records this delivery is seeking means to.
        /// </summary>
        ReadOnlyObservableCollection<string> RelatedRecordsNames
        {
            get;
        }

        /// <summary>
        /// Context documentation.
        /// </summary>
        ReadOnlyObservableCollection<IContextDocument> ContextDocuments
        {
            get;
        }

        /// <summary>
        /// Add table to the archive.
        /// </summary>
        /// <param name="table">Table.</param>
        void AddTable(ITable table);

        /// <summary>
        /// Add information about a view (query).
        /// </summary>
        /// <param name="view">Information about a view (query).</param>
        void AddView(IView view);

        /// <summary>
        /// Add creator of the archive.
        /// </summary>
        /// <param name="creator">Creator.</param>
        void AddCreator(ICreator creator);

        /// <summary>
        /// Add an alternative system name.
        /// </summary>
        /// <param name="alternativeSystemName">Alternative system name.</param>
        void AddAlternativeSystemName(string alternativeSystemName);

        /// <summary>
        /// Add another IT system that have supplied data to the system.
        /// </summary>
        /// <param name="sourceName">Name of the other IT system.</param>
        void AddSourceName(string sourceName);

        /// <summary>
        /// Add an user of data in the system.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        void AddUserName(string userName);

        /// <summary>
        /// Add an earlier system having the same function.
        /// </summary>
        /// <param name="predecessorName">Name of the earlier system having the same function.</param>
        void AddPredecessorName(string predecessorName);

        /// <summary>
        /// Add a FORM classification.
        /// </summary>
        /// <param name="formClass">FORM classification.</param>
        void AddFormClass(IFormClass formClass);

        /// <summary>
        /// Add a reference to the records this delivery is seeking means to.
        /// </summary>
        /// <param name="relatedRecordsName">Name of the reference.</param>
        void AddRelatedRecordsName(string relatedRecordsName);

        /// <summary>
        /// Add a context document to the context documentation.
        /// </summary>
        /// <param name="contextDocument">Context document.</param>
        void AddContextDocument(IContextDocument contextDocument);
    }
}
