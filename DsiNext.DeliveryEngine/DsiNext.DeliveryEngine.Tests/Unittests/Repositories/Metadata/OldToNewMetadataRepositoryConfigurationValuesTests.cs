using System;
using System.Linq;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Repositories.Metadata.OldToNew;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Repositories.Metadata
{
    /// <summary>
    /// Tests configuration values to the metadata repository for converting old delivery format to the new delivery format.
    /// </summary>
    [TestFixture]
    public class OldToNewMetadataRepositoryConfigurationValuesTests
    {
        /// <summary>
        /// Test that the constructor initialize the configuration values.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeConfigurationValues()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);
        }

        /// <summary>
        /// Test that ArchiveInformationPacketType gets the configuration value.
        /// </summary>
        [Test]
        public void TestThatArchiveInformationPacketTypeGetsConfiguredValue()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.ArchiveInformationPacketType;
            Assert.That(value, Is.True);
        }

        /// <summary>
        /// Test that AlternativeSystemNames gets the configuration value.
        /// </summary>
        [Test]
        public void TestThatAlternativeSystemNamesGetsConfiguredValue()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.AlternativeSystemNames;
            Assert.That(value, Is.Not.Null);
            Assert.That(value.Count(), Is.EqualTo(2));
        }

        /// <summary>
        /// Test that SystemPurpose gets the configuration value.
        /// </summary>
        [Test]
        public void TestThatSystemPurposeGetsConfiguredValue()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.SystemPurpose;
            Assert.That(value, Is.Not.Null);
            Assert.That(value, Is.Not.Empty);
            Assert.That(value, Is.EqualTo("Text for system purpose"));
        }

        /// <summary>
        /// Test that SystemContent gets the configuration value.
        /// </summary>
        [Test]
        public void TestThatSystemContentGetsConfiguredValue()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.SystemContent;
            Assert.That(value, Is.Not.Null);
            Assert.That(value, Is.Not.Empty);
            Assert.That(value, Is.EqualTo("Text for system content"));
        }

        /// <summary>
        /// Test that RegionNum gets the configuration value.
        /// </summary>
        [Test]
        public void TestThatRegionNumGetsConfiguredValue()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.RegionNum;
            Assert.That(value, Is.True);
        }

        /// <summary>
        /// Test that KomNum gets the configuration value.
        /// </summary>
        [Test]
        public void TestThatKomNumGetsConfiguredValue()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.KomNum;
            Assert.That(value, Is.True);
        }

        /// <summary>
        /// Test that CprNum gets the configuration value.
        /// </summary>
        [Test]
        public void TestThatCprNumGetsConfiguredValue()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.CprNum;
            Assert.That(value, Is.True);
        }

        /// <summary>
        /// Test that CvrNum gets the configuration value.
        /// </summary>
        [Test]
        public void TestThatCvrNumGetsConfiguredValue()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.CvrNum;
            Assert.That(value, Is.True);
        }

        /// <summary>
        /// Test that MatrikNum gets the configuration value.
        /// </summary>
        [Test]
        public void TestThatMatrikNumGetsConfiguredValue()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.MatrikNum;
            Assert.That(value, Is.True);
        }

        /// <summary>
        /// Test that BbrNum gets the configuration value.
        /// </summary>
        [Test]
        public void TestThatBbrNumGetsConfiguredValue()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.BbrNum;
            Assert.That(value, Is.True);
        }

        /// <summary>
        /// Test that WhoSygKod gets the configuration value.
        /// </summary>
        [Test]
        public void TestThatWhoSygKodGetsConfiguredValue()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.WhoSygKod;
            Assert.That(value, Is.True);
        }

        /// <summary>
        /// Test that SourceNames gets the configuration value.
        /// </summary>
        [Test]
        public void TestThatSourceNamesGetsConfiguredValue()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.SourceNames;
            Assert.That(value, Is.Not.Null);
            Assert.That(value.Count(), Is.EqualTo(2));
        }

        /// <summary>
        /// Test that UserNames gets the configuration value.
        /// </summary>
        [Test]
        public void TestThatUserNamesGetsConfiguredValue()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.UserNames;
            Assert.That(value, Is.Not.Null);
            Assert.That(value.Count(), Is.EqualTo(2));
        }

        /// <summary>
        /// Test that PredecessorNames gets the configuration value.
        /// </summary>
        [Test]
        public void TestThatPredecessorNamesGetsConfiguredValue()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.PredecessorNames;
            Assert.That(value, Is.Not.Null);
            Assert.That(value.Count(), Is.EqualTo(2));
        }

        /// <summary>
        /// Test that FormVersion gets the configuration value.
        /// </summary>
        [Test]
        public void TestThatFormVersionGetsConfiguredValue()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.FormVersion;
            Assert.That(value, Is.Not.Null);
            Assert.That(value, Is.Not.Empty);
            Assert.That(value, Is.EqualTo("FORM version"));
        }

        /// <summary>
        /// Test that FormClasses gets the configuration value.
        /// </summary>
        [Test]
        public void TestThatFormClassesGetsConfiguredValue()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.FormClasses;
            Assert.That(value, Is.Not.Null);
            Assert.That(value.Count(), Is.EqualTo(2));
        }

        /// <summary>
        /// Test that ContainsDigitalDocuments gets the configuration value.
        /// </summary>
        [Test]
        public void TestThaContainsDigitalDocumentsGetsConfiguredValue()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.ContainsDigitalDocuments;
            Assert.That(value, Is.True);
        }

        /// <summary>
        /// Test that SearchRelatedOtherRecords gets the configuration value.
        /// </summary>
        [Test]
        public void TestThatSearchRelatedOtherRecordsGetsConfiguredValue()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.SearchRelatedOtherRecords;
            Assert.That(value, Is.True);
        }

        /// <summary>
        /// Test that RelatedRecordsNames gets the configuration value.
        /// </summary>
        [Test]
        public void TestThatRelatedRecordsNamesGetsConfiguredValue()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.RelatedRecordsNames;
            Assert.That(value, Is.Not.Null);
            Assert.That(value.Count(), Is.EqualTo(2));
        }

        /// <summary>
        /// Test that SystemFileConcept gets the configuration value.
        /// </summary>
        [Test]
        public void TestThatSystemFileConceptGetsConfiguredValue()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.SystemFileConcept;
            Assert.That(value, Is.True);
        }

        /// <summary>
        /// Test that MultipleDataCollection gets the configuration value.
        /// </summary>
        [Test]
        public void TestThatMultipleDataCollectionGetsConfiguredValue()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.MultipleDataCollection;
            Assert.That(value, Is.True);
        }

        /// <summary>
        /// Test that PersonalDataRestrictedInfo gets the configuration value.
        /// </summary>
        [Test]
        public void TestThatPersonalDataRestrictedInfoGetsConfiguredValue()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.PersonalDataRestrictedInfo;
            Assert.That(value, Is.True);
        }

        /// <summary>
        /// Test that OtherAccessTypeRestrictions gets the configuration value.
        /// </summary>
        [Test]
        public void TestThatOtherAccessTypeRestrictionsGetsConfiguredValue()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.OtherAccessTypeRestrictions;
            Assert.That(value, Is.True);
        }

        /// <summary>
        /// Test that ArchiveApproval gets the configuration value.
        /// </summary>
        [Test]
        public void TestThatArchiveApprovalGetsConfiguredValue()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.ArchiveApproval;
            Assert.That(value, Is.Not.Null);
            Assert.That(value, Is.Not.Empty);
            Assert.That(value, Is.EqualTo("DSI"));
        }

        /// <summary>
        /// Test that ArchiveRestrictions gets the configuration value.
        /// </summary>
        [Test]
        public void TestThatArchiveRestrictionsGetsConfiguredValue()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.ArchiveRestrictions;
            Assert.That(value, Is.Not.Null);
            Assert.That(value, Is.Not.Empty);
            Assert.That(value, Is.EqualTo("Text for archive restrictions"));
        }

        /// <summary>
        /// Test that ContextDocumentDates gets configuration values for document date to context documents.
        /// </summary>
        [Test]
        public void TestThatContextDocumentDatesGetsConfiguredValues()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.ContextDocumentDates.ToList();
            Assert.That(value, Is.Not.Null);
            Assert.That(value.Count, Is.EqualTo(2));

            var contextDocumentDate = value.ElementAt(0);
            Assert.That(contextDocumentDate.Key, Is.Not.Null);
            Assert.That(contextDocumentDate.Key, Is.Not.Empty);
            Assert.That(contextDocumentDate.Key, Is.EqualTo("GENINFO1"));
            Assert.That(contextDocumentDate.Value, Is.Not.Null);
            Assert.That(contextDocumentDate.Value.Key, Is.EqualTo(DateTimePresicion.Month));
            Assert.That(contextDocumentDate.Value.Value, Is.EqualTo(new DateTime(2012, 8, 1)));

            contextDocumentDate = value.ElementAt(1);
            Assert.That(contextDocumentDate.Key, Is.Not.Null);
            Assert.That(contextDocumentDate.Key, Is.Not.Empty);
            Assert.That(contextDocumentDate.Key, Is.EqualTo("GENINFO2"));
            Assert.That(contextDocumentDate.Value, Is.Not.Null);
            Assert.That(contextDocumentDate.Value.Key, Is.EqualTo(DateTimePresicion.Month));
            Assert.That(contextDocumentDate.Value.Value, Is.EqualTo(new DateTime(2012, 8, 1)));
        }

        /// <summary>
        /// Test that ContextDocumentAuthors gets configuration values for document author to context documents.
        /// </summary>
        [Test]
        public void TestThatContextDocumentAuthorsGetsConfiguredValues()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.ContextDocumentAuthors.ToList();
            Assert.That(value, Is.Not.Null);
            Assert.That(value.Count, Is.EqualTo(2));

            var contextDocumentAuthors = value.ElementAt(0);
            Assert.That(contextDocumentAuthors.Key, Is.Not.Null);
            Assert.That(contextDocumentAuthors.Key, Is.Not.Empty);
            Assert.That(contextDocumentAuthors.Key, Is.EqualTo("GENINFO1"));
            Assert.That(contextDocumentAuthors.Value, Is.Not.Null);
            Assert.That(contextDocumentAuthors.Value, Is.Not.Empty);
            Assert.That(contextDocumentAuthors.Value.Count(), Is.EqualTo(2));
            Assert.That(contextDocumentAuthors.Value.SingleOrDefault(m => m.Institution.Equals("Institution 1") && m.Author != null && m.Author.Equals("Author name 1")), Is.Not.Null);
            Assert.That(contextDocumentAuthors.Value.SingleOrDefault(m => m.Institution.Equals("Institution 2") && m.Author == null), Is.Not.Null);

            contextDocumentAuthors = value.ElementAt(1);
            Assert.That(contextDocumentAuthors.Key, Is.Not.Null);
            Assert.That(contextDocumentAuthors.Key, Is.Not.Empty);
            Assert.That(contextDocumentAuthors.Key, Is.EqualTo("GENINFO2"));
            Assert.That(contextDocumentAuthors.Value, Is.Not.Null);
            Assert.That(contextDocumentAuthors.Value, Is.Not.Empty);
            Assert.That(contextDocumentAuthors.Value.Count(), Is.EqualTo(2));
            Assert.That(contextDocumentAuthors.Value.SingleOrDefault(m => m.Institution.Equals("Institution 3") && m.Author != null && m.Author.Equals("Author name 3")), Is.Not.Null);
            Assert.That(contextDocumentAuthors.Value.SingleOrDefault(m => m.Institution.Equals("Institution 4") && m.Author == null), Is.Not.Null);
        }

        /// <summary>
        /// Test that ContextDocumentCategories gets configuration values for categories to context documents.
        /// </summary>
        [Test]
        public void TestThatContextDocumentCategoriesGetsConfiguredValues()
        {
            var fixture = new Fixture();
            fixture.Customize<IConfigurationValues>(e => e.FromFactory(() => new ConfigurationValues()));

            var configurationValues = fixture.CreateAnonymous<IConfigurationValues>();
            Assert.That(configurationValues, Is.Not.Null);

            var value = configurationValues.ContextDocumentCategories.ToList();
            Assert.That(value, Is.Not.Null);
            Assert.That(value.Count, Is.EqualTo(2));

            var contextDocumentCategory = value.ElementAt(0);
            Assert.That(contextDocumentCategory.Key, Is.Not.Null);
            Assert.That(contextDocumentCategory.Key, Is.Not.Empty);
            Assert.That(contextDocumentCategory.Key, Is.EqualTo("GENINFO1"));
            Assert.That(contextDocumentCategory.Value, Is.Not.Null);
            Assert.That(contextDocumentCategory.Value, Is.Not.Empty);
            Assert.That(contextDocumentCategory.Value.Count(), Is.EqualTo(1));
            Assert.That(contextDocumentCategory.Value.Contains(ContextDocumentCategories.ArchivalTransformationInformation), Is.True);

            contextDocumentCategory = value.ElementAt(1);
            Assert.That(contextDocumentCategory.Key, Is.Not.Null);
            Assert.That(contextDocumentCategory.Key, Is.Not.Empty);
            Assert.That(contextDocumentCategory.Key, Is.EqualTo("GENINFO2"));
            Assert.That(contextDocumentCategory.Value, Is.Not.Null);
            Assert.That(contextDocumentCategory.Value, Is.Not.Empty);
            Assert.That(contextDocumentCategory.Value.Count(), Is.EqualTo(2));
            Assert.That(contextDocumentCategory.Value.Contains(ContextDocumentCategories.SystemAdministrativeFunctions), Is.True);
            Assert.That(contextDocumentCategory.Value.Contains(ContextDocumentCategories.SystemDataProvision), Is.True);
        }
    }
}
