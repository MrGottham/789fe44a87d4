using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Reflection;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Domain.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Repositories.Metadata.OldToNew
{
    /// <summary>
    /// Configuration values to the metadata repository for converting old delivery format to the new delivery format.
    /// </summary>
    public class ConfigurationValues : ApplicationSettingsBase, IConfigurationValues
    {
        #region Properties

        /// <summary>
        /// Packet type for the archive.
        /// True if this is an end delivery otherwise false.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue("false")]
        [SettingsSerializeAs(SettingsSerializeAs.String)]
        public virtual bool ArchiveInformationPacketType
        {
            get
            {
                return GetValue<bool>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Alternative system names.
        /// </summary>
        public virtual IEnumerable<string> AlternativeSystemNames
        {
            get
            {
                return AlternativeSystemNameCollection
                    .OfType<string>()
                    .Where(m => string.IsNullOrEmpty(m) == false)
                    .ToList();
            }
        }

        /// <summary>
        /// Collection for alternative system names.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue(@"<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""/>")]
        [SettingsSerializeAs(SettingsSerializeAs.Xml)]
        public virtual StringCollection AlternativeSystemNameCollection
        {
            get
            {
                return GetValue<StringCollection>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Description the system purpose.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue("")]
        [SettingsSerializeAs(SettingsSerializeAs.String)]
        public virtual string SystemPurpose
        {
            get
            {
                return GetValue<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Description of the content in the system.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue("")]
        [SettingsSerializeAs(SettingsSerializeAs.String)]
        public virtual string SystemContent
        {
            get
            {
                return GetValue<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Indication of whether, in systematic form is reigstreret region numbers in the system.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue("false")]
        [SettingsSerializeAs(SettingsSerializeAs.String)]
        public virtual bool RegionNum
        {
            get
            {
                return GetValue<bool>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Indication of whether, in systematic form is registered local numbers in the system.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue("false")]
        [SettingsSerializeAs(SettingsSerializeAs.String)]
        public virtual bool KomNum
        {
            get
            {
                return GetValue<bool>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Indication of whether, in systematic form registered civil registration numbers in the system.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue("false")]
        [SettingsSerializeAs(SettingsSerializeAs.String)]
        public virtual bool CprNum
        {
            get
            {
                return GetValue<bool>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Indication of whether, in systematic form is registered VAT numbers in the system.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue("false")]
        [SettingsSerializeAs(SettingsSerializeAs.String)]
        public virtual bool CvrNum
        {
            get
            {
                return GetValue<bool>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Indication of whether, in systematic form, registered cadastral data in the system.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue("false")]
        [SettingsSerializeAs(SettingsSerializeAs.String)]
        public virtual bool MatrikNum
        {
            get
            {
                return GetValue<bool>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Indication of whether, in systematic form is registered BBR numbers in the system.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue("false")]
        [SettingsSerializeAs(SettingsSerializeAs.String)]
        public virtual bool BbrNum
        {
            get
            {
                return GetValue<bool>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Indication of whether, in systematic form is registered WHO disease codes numbers in the system.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue("false")]
        [SettingsSerializeAs(SettingsSerializeAs.String)]
        public virtual bool WhoSygKod
        {
            get
            {
                return GetValue<bool>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Other IT systems that have supplied data to the system.
        /// </summary>
        public virtual IEnumerable<string> SourceNames
        {
            get
            {
                return SourceNameCollection
                    .OfType<string>()
                    .Where(m => string.IsNullOrEmpty(m) == false)
                    .ToList();
            }
        }

        /// <summary>
        /// Collection of Other IT systems that have supplied data to the system.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue(@"<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""/>")]
        [SettingsSerializeAs(SettingsSerializeAs.Xml)]
        public virtual StringCollection SourceNameCollection
        {
            get
            {
                return GetValue<StringCollection>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// User of data in the system.
        /// </summary>
        public virtual IEnumerable<string> UserNames
        {
            get
            {
                return UserNameCollection
                    .OfType<string>()
                    .Where(m => string.IsNullOrEmpty(m) == false)
                    .ToList();
            }
        }

        /// <summary>
        /// Collection for user of data in the system.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue(@"<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""/>")]
        [SettingsSerializeAs(SettingsSerializeAs.Xml)]
        public virtual StringCollection UserNameCollection
        {
            get
            {
                return GetValue<StringCollection>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Earlier systems having the same function.
        /// </summary>
        public virtual IEnumerable<string> PredecessorNames
        {
            get
            {
                return PredecessorNameCollection
                    .OfType<string>()
                    .Where(m => string.IsNullOrEmpty(m) == false)
                    .ToList();
            }
        }

        /// <summary>
        /// Collection for earlier systems having the same function.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue(@"<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""/>")]
        [SettingsSerializeAs(SettingsSerializeAs.Xml)]
        public virtual StringCollection PredecessorNameCollection
        {
            get
            {
                return GetValue<StringCollection>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// FORM version.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue("")]
        [SettingsSerializeAs(SettingsSerializeAs.String)]
        public virtual string FormVersion
        {
            get
            {
                return GetValue<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// FORM classifications.
        /// </summary>
        public virtual IEnumerable<IFormClass> FormClasses
        {
            get
            {
                return FormClassCollection
                    .Where(m => m != null)
                    .ToList();
            }
        }

        /// <summary>
        /// Collection of FORM classifications.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue(@"<ArrayOfFormClass xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""/>")]
        [SettingsSerializeAs(SettingsSerializeAs.Xml)]
        public virtual List<FormClass> FormClassCollection
        {
            get
            {
                return GetValue<List<FormClass>>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Indication of digital documents is included in delivery.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue("false")]
        [SettingsSerializeAs(SettingsSerializeAs.String)]
        public virtual bool ContainsDigitalDocuments
        {
            get
            {
                return GetValue<bool>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Search agent for other cases or documents.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue("false")]
        [SettingsSerializeAs(SettingsSerializeAs.String)]
        public virtual bool SearchRelatedOtherRecords
        {
            get
            {
                return GetValue<bool>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Reference to the records this delivery is seeking means to.
        /// </summary>
        public virtual IEnumerable<string> RelatedRecordsNames
        {
            get
            {
                return RelatedRecordsNameCollection
                    .OfType<string>()
                    .Where(m => string.IsNullOrEmpty(m) == false)
                    .ToList();
            }
        }

        /// <summary>
        /// Collection of reference to the records this delivery is seeking means to.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue(@"<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""/>")]
        [SettingsSerializeAs(SettingsSerializeAs.Xml)]
        public virtual StringCollection RelatedRecordsNameCollection
        {
            get
            {
                return GetValue<StringCollection>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Indication of whether the system has a case definition.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue("false")]
        [SettingsSerializeAs(SettingsSerializeAs.String)]
        public virtual bool SystemFileConcept
        {
            get
            {
                return GetValue<bool>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Indication of whether the system is part of an SOA architecture.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue("false")]
        [SettingsSerializeAs(SettingsSerializeAs.String)]
        public virtual bool MultipleDataCollection
        {
            get
            {
                return GetValue<bool>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Indication of whether the system contains personal informations.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue("false")]
        [SettingsSerializeAs(SettingsSerializeAs.String)]
        public virtual bool PersonalDataRestrictedInfo
        {
            get
            {
                return GetValue<bool>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Indication of whether the system contains information that could lead to longer access time.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue("false")]
        [SettingsSerializeAs(SettingsSerializeAs.String)]
        public virtual bool OtherAccessTypeRestrictions
        {
            get
            {
                return GetValue<bool>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Approval archive.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue("")]
        [SettingsSerializeAs(SettingsSerializeAs.String)]
        public virtual string ArchiveApproval
        {
            get
            {
                return GetValue<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Constraints of availability.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue("")]
        [SettingsSerializeAs(SettingsSerializeAs.String)]
        public virtual string ArchiveRestrictions
        {
            get
            {
                return GetValue<string>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Document dates for context documents.
        /// </summary>
        public virtual IEnumerable<KeyValuePair<string, KeyValuePair<DateTimePresicion, DateTime>>> ContextDocumentDates
        {
            get
            {
                var contextDocumentDates = new Dictionary<string, KeyValuePair<DateTimePresicion, DateTime>>();
                foreach (var element in ContextDocumentDateCollection)
                {
                    var key = element.Split(';').ElementAt(0).ToUpper();
                    var value = new KeyValuePair<DateTimePresicion, DateTime>((DateTimePresicion) Enum.Parse(typeof(DateTimePresicion), element.Split(';').ElementAt(1)), DateTime.Parse(element.Split(';').ElementAt(2)));
                    contextDocumentDates.Add(key, value);
                }
                return contextDocumentDates;
            }
        }

        /// <summary>
        /// Collection of strings that contains document dates for context documents.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue(@"<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""/>")]
        [SettingsSerializeAs(SettingsSerializeAs.Xml)]
        public virtual StringCollection ContextDocumentDateCollection
        {
            get
            {
                return GetValue<StringCollection>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Document authors for context documents.
        /// </summary>
        public virtual IEnumerable<KeyValuePair<string, IEnumerable<IDocumentAuthor>>> ContextDocumentAuthors
        {
            get
            {
                var contextDocumentAuthors = new Dictionary<string, IEnumerable<IDocumentAuthor>>();
                foreach (var element in ContextDocumentAuthorCollection)
                {
                    var key = element.Split(';').First().ToUpper();
                    var values = element.Substring(element.IndexOf(';')).Split(';').Where(m => string.IsNullOrEmpty(m) == false).ToList();
                    var documentAuthors = new List<IDocumentAuthor>(values.Count);
                    documentAuthors.AddRange(values.Select(value =>
                        {
                            var pos = value.IndexOf('|');
                            return pos >= 0 ? new DocumentAuthor(value.Substring(pos + 1), value.Substring(0, pos)) : new DocumentAuthor(value);
                        }));
                    contextDocumentAuthors.Add(key, documentAuthors);
                }
                return contextDocumentAuthors;
            }
        }

        /// <summary>
        /// Collection of strings that contains document authors for context documents.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue(@"<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""/>")]
        [SettingsSerializeAs(SettingsSerializeAs.Xml)]
        public virtual StringCollection ContextDocumentAuthorCollection
        {
            get
            {
                return GetValue<StringCollection>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Categories for context documents.
        /// </summary>
        public virtual IEnumerable<KeyValuePair<string, IEnumerable<ContextDocumentCategories>>> ContextDocumentCategories
        {
            get
            {
                var contextDocumentCategories = new Dictionary<string, IEnumerable<ContextDocumentCategories>>();
                foreach (var element in ContextDocumentCategoryCollection)
                {
                    var key = element.Split(';').First().ToUpper();
                    var values = element.Substring(element.IndexOf(';')).Split(';').Where(m => string.IsNullOrEmpty(m) == false).ToList();
                    var categories = new List<ContextDocumentCategories>(values.Count);
                    categories.AddRange(values.Select(value => (ContextDocumentCategories) Enum.Parse(typeof (ContextDocumentCategories), value, true)));
                    contextDocumentCategories.Add(key, categories);
                }
                return contextDocumentCategories;
            }
        }

        /// <summary>
        /// Collection of strings that contains categories for context documents.
        /// </summary>
        [ApplicationScopedSetting]
        [DefaultSettingValue(@"<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""/>")]
        [SettingsSerializeAs(SettingsSerializeAs.Xml)]
        public virtual StringCollection ContextDocumentCategoryCollection
        {
            get
            {
                return GetValue<StringCollection>(MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the configuration value for a property.
        /// </summary>
        /// <typeparam name="TValue">Type of configuration values.</typeparam>
        /// <param name="propertyName">Property name.</param>
        /// <returns>Configuraiton value.</returns>
        protected virtual TValue GetValue<TValue>(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }
            try
            {
                var value = this[propertyName];
                if (value is TValue)
                {
                    return (TValue) value;
                }
                var valueType = typeof (TValue);
                try
                {
                    var parseMethod = valueType.GetMethod("Parse", new[] {typeof (string)});
                    if (parseMethod == null)
                    {
                        throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.MethodNotFoundOnType, "Parse", valueType.Name));
                    }
                    return (TValue) parseMethod.Invoke(valueType, new object[] {value.ToString()});
                }
                catch (TargetInvocationException ex)
                {
                    throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.RepositoryError, propertyName, ex.InnerException.Message), ex.InnerException);
                }
            }
            catch (DeliveryEngineRepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.RepositoryError, propertyName, ex.Message), ex);
            }
        }

        #endregion
    }
}
