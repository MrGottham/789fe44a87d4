using System;
using System.Collections.Generic;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Repositories.Metadata.OldToNew
{
    /// <summary>
    /// Interface for configuration values to the metadata repository for converting old delivery format to the new delivery format.
    /// </summary>
    public interface IConfigurationValues
    {
        /// <summary>
        /// Packet type for the archive.
        /// True if this is an end delivery otherwise false.
        /// </summary>
        bool ArchiveInformationPacketType
        {
            get;
        }

        /// <summary>
        /// Alternative system names.
        /// </summary>
        IEnumerable<string> AlternativeSystemNames
        {
            get;
        }

        /// <summary>
        /// Description the system purpose.
        /// </summary>
        string SystemPurpose
        {
            get;
        }

        /// <summary>
        /// Description of the content in the system.
        /// </summary>
        string SystemContent
        {
            get;
        }

        /// <summary>
        /// Indication of whether, in systematic form is reigstreret region numbers in the system.
        /// </summary>
        bool RegionNum
        {
            get;
        }

        /// <summary>
        /// Indication of whether, in systematic form is registered local numbers in the system.
        /// </summary>
        bool KomNum
        {
            get;
        }

        /// <summary>
        /// Indication of whether, in systematic form registered civil registration numbers in the system.
        /// </summary>
        bool CprNum
        {
            get;
        }

        /// <summary>
        /// Indication of whether, in systematic form is registered VAT numbers in the system.
        /// </summary>
        bool CvrNum
        {
            get;
        }

        /// <summary>
        /// Indication of whether, in systematic form, registered cadastral data in the system.
        /// </summary>
        bool MatrikNum
        {
            get;
        }

        /// <summary>
        /// Indication of whether, in systematic form is registered BBR numbers in the system.
        /// </summary>
        bool BbrNum
        {
            get;
        }

        /// <summary>
        /// Indication of whether, in systematic form is registered WHO disease codes numbers in the system.
        /// </summary>
        bool WhoSygKod
        {
            get;
        }

        /// <summary>
        /// Other IT systems that have supplied data to the system.
        /// </summary>
        IEnumerable<string> SourceNames
        {
            get;
        }

        /// <summary>
        /// User of data in the system.
        /// </summary>
        IEnumerable<string> UserNames
        {
            get;
        }

        /// <summary>
        /// Earlier systems having the same function.
        /// </summary>
        IEnumerable<string> PredecessorNames
        {
            get;
        }

        /// <summary>
        /// FORM version.
        /// </summary>
        string FormVersion
        {
            get;
        }
        
        /// <summary>
        /// FORM classifications.
        /// </summary>
        IEnumerable<IFormClass> FormClasses
        {
            get;
        }

        /// <summary>
        /// Indication of digital documents is included in delivery.
        /// </summary>
        bool ContainsDigitalDocuments
        {
            get;
        }

        /// <summary>
        /// Search agent for other cases or documents.
        /// </summary>
        bool SearchRelatedOtherRecords
        {
            get;
        }

        /// <summary>
        /// Reference to the records this delivery is seeking means to.
        /// </summary>
        IEnumerable<string> RelatedRecordsNames
        {
            get;
        }

        /// <summary>
        /// Indication of whether the system has a case definition.
        /// </summary>
        bool SystemFileConcept
        {
            get;
        }

        /// <summary>
        /// Indication of whether the system is part of an SOA architecture.
        /// </summary>
        bool MultipleDataCollection
        {
            get;
        }

        /// <summary>
        /// Indication of whether the system contains personal informations.
        /// </summary>
        bool PersonalDataRestrictedInfo
        {
            get;
        }

        /// <summary>
        /// Indication of whether the system contains information that could lead to longer access time.
        /// </summary>
        bool OtherAccessTypeRestrictions
        {
            get;
        }

        /// <summary>
        /// Approval archive.
        /// </summary>
        string ArchiveApproval
        {
            get;
        }

        /// <summary>
        /// Constraints of availability.
        /// </summary>
        string ArchiveRestrictions
        {
            get;
        }

        /// <summary>
        /// Document dates for context documents.
        /// </summary>
        IEnumerable<KeyValuePair<string, KeyValuePair<DateTimePresicion, DateTime>>> ContextDocumentDates
        {
            get;
        }
            
        /// <summary>
        /// Document authors for context documents.
        /// </summary>
        IEnumerable<KeyValuePair<string, IEnumerable<IDocumentAuthor>>> ContextDocumentAuthors
        {
            get;
        }

        /// <summary>
        /// Categories for context documents.
        /// </summary>
        IEnumerable<KeyValuePair<string, IEnumerable<ContextDocumentCategories>>> ContextDocumentCategories
        {
            get;
        }
    }
}
