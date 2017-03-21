using System;
using System.Collections.ObjectModel;
using System.Reflection;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Domain.Metadata
{
    /// <summary>
    /// Datasource for an archive.
    /// </summary>
    public class DataSource : NamedObject, IDataSource
    {
        #region Private variables

        private string _connectionString;
        private string _archiveInformationPackageId;
        private int _archiveInformationPackageIdPrevious;
        private DateTime _archivePeriodStart;
        private DateTime _archivePeriodEnd;
        private bool _archiveInformationPacketType;
        private bool _archiveType;
        private string _systemPurpose;
        private string _systemContent;
        private bool _regionNum;
        private bool _komNum;
        private bool _cprNum;
        private bool _cvrNum;
        private bool _matrikNum;
        private bool _bbrNum;
        private bool _whoSygKod;
        private string  _formVersion;
        private bool _containsDigitalDocuments;
        private bool _searchRelatedOtherRecords;
        private bool _systemFileConcept;
        private bool _multipleDataCollection;
        private bool _personalDataRestrictedInfo;
        private bool _otherAccessTypeRestrictions;
        private string _archiveApproval;
        private string _archiveRestrictions;
        private readonly ObservableCollection<ITable> _tables = new ObservableCollection<ITable>();
        private readonly ObservableCollection<IView> _views = new ObservableCollection<IView>();
        private readonly ObservableCollection<ICreator> _creators = new ObservableCollection<ICreator>();
        private readonly ObservableCollection<string> _alternativeSystemNames = new ObservableCollection<string>();
        private readonly ObservableCollection<string> _sourceNames = new ObservableCollection<string>();
        private readonly ObservableCollection<string> _userNames = new ObservableCollection<string>();
        private readonly ObservableCollection<string> _predecessorNames = new ObservableCollection<string>();
        private readonly ObservableCollection<IFormClass> _formClasses = new ObservableCollection<IFormClass>();
        private readonly ObservableCollection<string> _relatedRecordsName = new ObservableCollection<string>();
        private readonly ObservableCollection<IContextDocument> _contextDocuments = new ObservableCollection<IContextDocument>();

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a datasource for an archive.
        /// </summary>
        /// <param name="archiveInformationPackageId">Unique package ID for the archive.</param>
        /// <param name="nameSource">System name in the source repository.</param>
        /// <param name="nameTarget">System name in the target repository.</param>
        /// <param name="archivePeriodStart">Start of the periode for the archive.</param>
        /// <param name="archivePeriodEnd">End of the periode for the archive.</param>
        public DataSource(string archiveInformationPackageId, string nameSource, string nameTarget, DateTime archivePeriodStart, DateTime archivePeriodEnd)
            : this(archiveInformationPackageId, nameSource, nameTarget, null, archivePeriodStart, archivePeriodEnd)
        {
        }

        /// <summary>
        /// Creates a datasource for an archive.
        /// </summary>
        /// <param name="archiveInformationPackageId">Unique package ID for the archive.</param>
        /// <param name="nameSource">System name in the source repository.</param>
        /// <param name="nameTarget">System name in the target repository.</param>
        /// <param name="description">Description.</param>
        /// <param name="archivePeriodStart">Start of the periode for the archive.</param>
        /// <param name="archivePeriodEnd">End of the periode for the archive.</param>
        public DataSource(string archiveInformationPackageId, string nameSource, string nameTarget, string description, DateTime archivePeriodStart, DateTime archivePeriodEnd)
            : base(nameSource, nameTarget, description)
        {
            if (string.IsNullOrEmpty(archiveInformationPackageId))
            {
                throw new ArgumentNullException("archiveInformationPackageId");
            }
            _archiveInformationPackageId = archiveInformationPackageId;
            _archivePeriodStart = archivePeriodStart;
            _archivePeriodEnd = archivePeriodEnd;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Connection string.
        /// </summary>
        public virtual string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }
                if (_connectionString == value)
                {
                    return;
                }
                _connectionString = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Unique package ID for the archive.
        /// </summary>
        public virtual string ArchiveInformationPackageId
        {
            get
            {
                return _archiveInformationPackageId;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }
                if (_archiveInformationPackageId == value)
                {
                    return;
                }
                _archiveInformationPackageId = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Unique package ID for the previous archive.
        /// </summary>
        public virtual int ArchiveInformationPackageIdPrevious
        {
            get
            {
                return _archiveInformationPackageIdPrevious;
            }
            set
            {
                if (_archiveInformationPackageIdPrevious == value)
                {
                    return;
                }
                _archiveInformationPackageIdPrevious = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Start of the periode for the archive.
        /// </summary>
        public virtual DateTime ArchivePeriodStart
        {
            get
            {
                return _archivePeriodStart;
            }
            set
            {
                if (_archivePeriodStart == value)
                {
                    return;
                }
                _archivePeriodStart = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// End of the periode for the archive.
        /// </summary>
        public virtual DateTime ArchivePeriodEnd
        {
            get
            {
                return _archivePeriodEnd;
            }
            set
            {
                if (_archivePeriodEnd == value)
                {
                    return;
                }
                _archivePeriodEnd = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Packet type for the archive.
        /// True if this is an end delivery otherwise false.
        /// </summary>
        public virtual bool ArchiveInformationPacketType
        {
            get
            {
                return _archiveInformationPacketType;
            }
            set
            {
                if (_archiveInformationPacketType == value)
                {
                    return;
                }
                _archiveInformationPacketType = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Mark for the period type of the archive.
        /// True if the archive period has ended or false
        /// if the the period type i a current picture.
        /// </summary>
        public virtual bool ArchiveType
        {
            get
            {
                return _archiveType;
            }
            set
            {
                if (_archiveType == value)
                {
                    return;
                }
                _archiveType = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Description the system purpose.
        /// </summary>
        public virtual string SystemPurpose
        {
            get
            {
                return _systemPurpose;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }
                if (_systemPurpose == value)
                {
                    return;
                }
                _systemPurpose = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Description of the content in the system.
        /// </summary>
        public virtual string SystemContent
        {
            get
            {
                return _systemContent;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }
                if (_systemContent == value)
                {
                    return;
                }
                _systemContent = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Indication of whether, in systematic form is reigstreret region numbers in the system.
        /// </summary>
        public virtual bool RegionNum
        {
            get
            {
                return _regionNum;
            }
            set
            {
                if (_regionNum == value)
                {
                    return;
                }
                _regionNum = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Indication of whether, in systematic form is registered local numbers in the system.
        /// </summary>
        public virtual bool KomNum
        {
            get
            {
                return _komNum;
            }
            set
            {
                if (_komNum == value)
                {
                    return;
                }
                _komNum = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Indication of whether, in systematic form registered civil registration numbers in the system.
        /// </summary>
        public virtual bool CprNum
        {
            get
            {
                return _cprNum;
            }
            set
            {
                if (_cprNum == value)
                {
                    return;
                }
                _cprNum = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Indication of whether, in systematic form is registered VAT numbers in the system.
        /// </summary>
        public virtual bool CvrNum
        {
            get
            {
                return _cvrNum;
            }
            set
            {
                if (_cvrNum == value)
                {
                    return;
                }
                _cvrNum = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Indication of whether, in systematic form, registered cadastral data in the system.
        /// </summary>
        public virtual bool MatrikNum
        {
            get
            {
                return _matrikNum;
            }
            set
            {
                if (_matrikNum == value)
                {
                    return;
                }
                _matrikNum = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Indication of whether, in systematic form is registered BBR numbers in the system.
        /// </summary>
        public virtual bool BbrNum
        {
            get
            {
                return _bbrNum;
            }
            set
            {
                if (_bbrNum == value)
                {
                    return;
                }
                _bbrNum = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Indication of whether, in systematic form is registered WHO disease codes numbers in the system.
        /// </summary>
        public virtual bool WhoSygKod
        {
            get
            {
                return _whoSygKod;
            }
            set
            {
                if (_whoSygKod == value)
                {
                    return;
                }
                _whoSygKod = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// FORM version.
        /// </summary>
        public virtual string FormVersion
        {
            get
            {
                return _formVersion;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }
                if (_formVersion == value)
                {
                    return;
                }
                _formVersion = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Indication of digital documents is included in delivery.
        /// </summary>
        public virtual bool ContainsDigitalDocuments
        {
            get
            {
                return _containsDigitalDocuments;
            }
            set
            {
                if (_containsDigitalDocuments == value)
                {
                    return;
                }
                _containsDigitalDocuments = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Search agent for other cases or documents.
        /// </summary>
        public virtual bool SearchRelatedOtherRecords
        {
            get
            {
                return _searchRelatedOtherRecords;
            }
            set
            {
                if (_searchRelatedOtherRecords == value)
                {
                    return;
                }
                _searchRelatedOtherRecords = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Indication of whether the system has a case definition.
        /// </summary>
        public virtual bool SystemFileConcept
        {
            get
            {
                return _systemFileConcept;
            }
            set
            {
                if (_systemFileConcept == value)
                {
                    return;
                }
                _systemFileConcept = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Indication of whether the system is part of an SOA architecture.
        /// </summary>
        public virtual bool MultipleDataCollection
        {
            get
            {
                return _multipleDataCollection;
            }
            set
            {
                if (_multipleDataCollection == value)
                {
                    return;
                }
                _multipleDataCollection = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Indication of whether the system contains personal informations.
        /// </summary>
        public virtual bool PersonalDataRestrictedInfo
        {
            get
            {
                return _personalDataRestrictedInfo;
            }
            set
            {
                if (_personalDataRestrictedInfo == value)
                {
                    return;
                }
                _personalDataRestrictedInfo = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Indication of whether the system contains information that could lead to longer access time.
        /// </summary>
        public virtual bool OtherAccessTypeRestrictions
        {
            get
            {
                return _otherAccessTypeRestrictions;
            }
            set
            {
                if (_otherAccessTypeRestrictions == value)
                {
                    return;
                }
                _otherAccessTypeRestrictions = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Approval archive.
        /// </summary>
        public virtual string ArchiveApproval
        {
            get
            {
                return _archiveApproval;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }
                if (_archiveApproval == value)
                {
                    return;
                }
                _archiveApproval = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Constraints of availability.
        /// </summary>
        public virtual string ArchiveRestrictions
        {
            get
            {
                return _archiveRestrictions;
            }
            set
            {
                if (_archiveRestrictions == value)
                {
                    return;
                }
                _archiveRestrictions = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Tables.
        /// </summary>
        public virtual ReadOnlyObservableCollection<ITable> Tables
        {
            get
            {
                return new ReadOnlyObservableCollection<ITable>(_tables);
            }
        }

        /// <summary>
        /// Information about the views (queries).
        /// </summary>
        public virtual ReadOnlyObservableCollection<IView> Views
        {
            get
            {
                return new ReadOnlyObservableCollection<IView>(_views);
            }
        }

        /// <summary>
        /// Creators of the archive.
        /// </summary>
        public virtual ReadOnlyObservableCollection<ICreator> Creators
        {
            get
            {
                return new ReadOnlyObservableCollection<ICreator>(_creators);
            }
        }

        /// <summary>
        /// Alternative system names.
        /// </summary>
        public virtual ReadOnlyObservableCollection<string> AlternativeSystemNames
        {
            get
            {
                return new ReadOnlyObservableCollection<string>(_alternativeSystemNames);
            }
        }

        /// <summary>
        /// Other IT systems that have supplied data to the system.
        /// </summary>
        public virtual ReadOnlyObservableCollection<string> SourceNames
        {
            get
            {
                return new ReadOnlyObservableCollection<string>(_sourceNames);
            }
        }

        /// <summary>
        /// User of data in the system.
        /// </summary>
        public virtual ReadOnlyObservableCollection<string> UserNames
        {
            get
            {
                return new ReadOnlyObservableCollection<string>(_userNames);
            }
        }

        /// <summary>
        /// Earlier systems having the same function.
        /// </summary>
        public virtual ReadOnlyObservableCollection<string> PredecessorNames
        {
            get
            {
                return new ReadOnlyObservableCollection<string>(_predecessorNames);
            }
        }

        /// <summary>
        /// FORM classifications.
        /// </summary>
        public virtual ReadOnlyObservableCollection<IFormClass> FormClasses
        {
            get
            {
                return new ReadOnlyObservableCollection<IFormClass>(_formClasses);
            }
        }

        /// <summary>
        /// Reference to the records this delivery is seeking means to.
        /// </summary>
        public virtual ReadOnlyObservableCollection<string> RelatedRecordsNames
        {
            get
            {
                return new ReadOnlyObservableCollection<string>(_relatedRecordsName);
            }
        }

        /// <summary>
        /// Context documentation.
        /// </summary>
        public virtual ReadOnlyObservableCollection<IContextDocument> ContextDocuments
        {
            get
            {
                return new ReadOnlyObservableCollection<IContextDocument>(_contextDocuments);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add table to the archive.
        /// </summary>
        /// <param name="table">Table.</param>
        public virtual void AddTable(ITable table)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            _tables.Add(table);
        }

        /// <summary>
        /// Add information about a view (query).
        /// </summary>
        /// <param name="view">Information about a view (query).</param>
        public virtual void AddView(IView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }
            _views.Add(view);
        }

        /// <summary>
        /// Add creator of the archive.
        /// </summary>
        /// <param name="creator">Creator.</param>
        public virtual void AddCreator(ICreator creator)
        {
            if (creator == null)
            {
                throw new ArgumentNullException("creator");
            }
            _creators.Add(creator);
        }

        /// <summary>
        /// Add an alternative system name.
        /// </summary>
        /// <param name="alternativeSystemName">Alternative system name.</param>
        public virtual void AddAlternativeSystemName(string alternativeSystemName)
        {
            if (string.IsNullOrEmpty(alternativeSystemName))
            {
                throw new ArgumentNullException("alternativeSystemName");
            }
            _alternativeSystemNames.Add(alternativeSystemName);
        }

        /// <summary>
        /// Add another IT system that have supplied data to the system.
        /// </summary>
        /// <param name="sourceName">Name of the other IT system.</param>
        public virtual void AddSourceName(string sourceName)
        {
            if (string.IsNullOrEmpty(sourceName))
            {
                throw new ArgumentNullException("sourceName");
            }
            _sourceNames.Add(sourceName);
        }

        /// <summary>
        /// Add an user of data in the system.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        public virtual void AddUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("userName");
            }
            _userNames.Add(userName);
        }

        /// <summary>
        /// Add an earlier system having the same function.
        /// </summary>
        /// <param name="predecessorName">Name of the earlier system having the same function.</param>
        public virtual void AddPredecessorName(string predecessorName)
        {
            if (string.IsNullOrEmpty(predecessorName))
            {
                throw new ArgumentNullException("predecessorName");
            }
            _predecessorNames.Add(predecessorName);
        }

        /// <summary>
        /// Add a FORM classification.
        /// </summary>
        /// <param name="formClass">FORM classification.</param>
        public virtual void AddFormClass(IFormClass formClass)
        {
            if (formClass == null)
            {
                throw new ArgumentNullException("formClass");
            }
            _formClasses.Add(formClass);
        }

        /// <summary>
        /// Add a reference to the records this delivery is seeking means to.
        /// </summary>
        /// <param name="relatedRecordsName">Name of the reference.</param>
        public virtual void AddRelatedRecordsName(string relatedRecordsName)
        {
            if (string.IsNullOrEmpty(relatedRecordsName))
            {
                throw new ArgumentNullException("relatedRecordsName");
            }
            _relatedRecordsName.Add(relatedRecordsName);
        }

        /// <summary>
        /// Add a context document to the context documentation.
        /// </summary>
        /// <param name="contextDocument">Context document.</param>
        public virtual void AddContextDocument(IContextDocument contextDocument)
        {
            if (contextDocument == null)
            {
                throw new ArgumentNullException("contextDocument");
            }
            _contextDocuments.Add(contextDocument);
        }

        #endregion
    }
}
