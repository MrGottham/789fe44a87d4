using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Domain.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.Metadata.OldToNew;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Repositories.Metadata
{
    /// <summary>
    /// Tests the metadata repository for converting old delivery format to the new delivery format.
    /// </summary>
    [TestFixture]
    public class OldToNewMetadataRepositoryTests
    {
        /// <summary>
        /// Test that the constructor initialize the metadata repository.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeOldToNewMetadataRepository()
        {
            var oldToNewMetadataRepository = new OldToNewMetadataRepository(RepositoryTestHelper.GetSourcePathForTest(), MockRepository.GenerateMock<IConfigurationValues>());
            Assert.That(oldToNewMetadataRepository, Is.Not.Null);
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the path containing the old delivery format is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfPathIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new OldToNewMetadataRepository(null, MockRepository.GenerateMock<IConfigurationValues>()));
        }

        /// <summary>
        /// Test that the constructor throws an DeliveryEngineRepositoryException if the path containing the old delivery format does not exist.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsDeliveryEngineRepositoryExceptionIfPathDoesNotExist()
        {
            var fixture = new Fixture();
            Assert.Throws<DeliveryEngineRepositoryException>(() => new OldToNewMetadataRepository(new DirectoryInfo(fixture.CreateAnonymous<string>()), MockRepository.GenerateMock<IConfigurationValues>()));
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the configuration values for the repository is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfConfigurationValuesIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new OldToNewMetadataRepository(RepositoryTestHelper.GetSourcePathForTest(), null));
        }

        /// <summary>
        /// Test that DataSourceGet gets the data source.
        /// </summary>
        [Test]
        public void TestThatDataSourceGetGetsDataSource()
        {
            var configurationValuesMock = GetConfigurationValuesMock();

            var oldToNewMetadataRepository = new OldToNewMetadataRepository(RepositoryTestHelper.GetSourcePathForTest(), configurationValuesMock);
            Assert.That(oldToNewMetadataRepository, Is.Not.Null);

            var dataSource = oldToNewMetadataRepository.DataSourceGet();
            Assert.That(dataSource, Is.Not.Null);
            Assert.That(dataSource.NameSource, Is.Not.Null);
            Assert.That(dataSource.NameSource, Is.Not.Empty);
            Assert.That(dataSource.NameSource, Is.EqualTo("DocSysPro"));
            Assert.That(dataSource.NameTarget, Is.Not.Null);
            Assert.That(dataSource.NameTarget, Is.Not.Empty);
            Assert.That(dataSource.NameTarget, Is.EqualTo("DocSysPro"));
            Assert.That(dataSource.Description, Is.Not.Null);
            Assert.That(dataSource.Description, Is.Not.Empty);
            Assert.That(dataSource.Description, Is.EqualTo("DocSysPro"));
            Assert.That(dataSource.ArchiveInformationPackageId, Is.Not.Empty);
            Assert.That(dataSource.ArchiveInformationPackageId, Is.Not.Null);
            Assert.That(dataSource.ArchiveInformationPackageId, Is.EqualTo("40330"));
            Assert.That(dataSource.ArchiveInformationPackageIdPrevious, Is.EqualTo(0));
            Assert.That(dataSource.ArchivePeriodStart, Is.EqualTo(new DateTime(1996, 1, 1, 0, 0 ,0)));
            Assert.That(dataSource.ArchivePeriodEnd, Is.EqualTo(new DateTime(2000, 12, 31, 0, 0, 0)));
            Assert.That(dataSource.ArchiveInformationPacketType, Is.EqualTo(configurationValuesMock.ArchiveInformationPacketType));
            Assert.That(dataSource.ArchiveType, Is.True);
            Assert.That(dataSource.AlternativeSystemNames, Is.Not.Null);
            Assert.That(dataSource.AlternativeSystemNames.Count, Is.EqualTo(3));
            Assert.That(dataSource.SystemPurpose, Is.Not.Null);
            Assert.That(dataSource.SystemPurpose, Is.Not.Empty);
            Assert.That(dataSource.SystemPurpose, Is.EqualTo(configurationValuesMock.SystemPurpose));
            Assert.That(dataSource.SystemContent, Is.Not.Null);
            Assert.That(dataSource.SystemContent, Is.Not.Empty);
            Assert.That(dataSource.SystemContent, Is.EqualTo(configurationValuesMock.SystemContent));
            Assert.That(dataSource.RegionNum, Is.EqualTo(configurationValuesMock.RegionNum));
            Assert.That(dataSource.KomNum, Is.EqualTo(configurationValuesMock.KomNum));
            Assert.That(dataSource.CprNum, Is.EqualTo(configurationValuesMock.CprNum));
            Assert.That(dataSource.CvrNum, Is.EqualTo(configurationValuesMock.CvrNum));
            Assert.That(dataSource.MatrikNum, Is.EqualTo(configurationValuesMock.MatrikNum));
            Assert.That(dataSource.BbrNum, Is.EqualTo(configurationValuesMock.BbrNum));
            Assert.That(dataSource.WhoSygKod, Is.EqualTo(configurationValuesMock.WhoSygKod));
            Assert.That(dataSource.SourceNames, Is.Not.Null);
            Assert.That(dataSource.SourceNames.Count, Is.EqualTo(3));
            Assert.That(dataSource.UserNames, Is.Not.Null);
            Assert.That(dataSource.UserNames.Count, Is.EqualTo(3));
            Assert.That(dataSource.PredecessorNames, Is.Not.Null);
            Assert.That(dataSource.PredecessorNames.Count, Is.EqualTo(3));
            Assert.That(dataSource.FormVersion, Is.Not.Null);
            Assert.That(dataSource.FormVersion, Is.Not.Empty);
            Assert.That(dataSource.FormVersion, Is.EqualTo(configurationValuesMock.FormVersion));
            Assert.That(dataSource.FormClasses, Is.Not.Null);
            Assert.That(dataSource.FormClasses.Count, Is.EqualTo(3));
            Assert.That(dataSource.ContainsDigitalDocuments, Is.EqualTo(configurationValuesMock.ContainsDigitalDocuments));
            Assert.That(dataSource.SearchRelatedOtherRecords, Is.EqualTo(configurationValuesMock.SearchRelatedOtherRecords));
            Assert.That(dataSource.RelatedRecordsNames, Is.Not.Null);
            Assert.That(dataSource.RelatedRecordsNames.Count, Is.EqualTo(3));
            Assert.That(dataSource.SystemFileConcept, Is.EqualTo(configurationValuesMock.SystemFileConcept));
            Assert.That(dataSource.MultipleDataCollection, Is.EqualTo(configurationValuesMock.MultipleDataCollection));
            Assert.That(dataSource.PersonalDataRestrictedInfo, Is.EqualTo(configurationValuesMock.PersonalDataRestrictedInfo));
            Assert.That(dataSource.OtherAccessTypeRestrictions, Is.EqualTo(configurationValuesMock.OtherAccessTypeRestrictions));
            Assert.That(dataSource.ArchiveApproval, Is.Not.Null);
            Assert.That(dataSource.ArchiveApproval, Is.Not.Empty);
            Assert.That(dataSource.ArchiveApproval, Is.EqualTo(configurationValuesMock.ArchiveApproval));
            Assert.That(dataSource.ArchiveRestrictions, Is.Not.Null);
            Assert.That(dataSource.ArchiveRestrictions, Is.Not.Empty);
            Assert.That(dataSource.ArchiveRestrictions, Is.EqualTo(configurationValuesMock.ArchiveRestrictions));

            Assert.That(dataSource.Tables, Is.Not.Null);
            Assert.That(dataSource.Tables.Count(), Is.EqualTo(5));
            foreach(var table in dataSource.Tables)
            {
                Assert.That(table, Is.Not.Null);
                Assert.That(table.Fields, Is.Not.Null);
                Assert.That(table.Fields.Count(), Is.GreaterThan(0));
                Assert.That(table.CandidateKeys, Is.Not.Null);
                Assert.That(table.CandidateKeys.Count(), Is.EqualTo(1));
                Assert.That(table.ForeignKeys, Is.Not.Null);
                Assert.That(table.ForeignKeys.Count(), Is.GreaterThanOrEqualTo(0));
            }
            Assert.That(dataSource.Views, Is.Not.Null);
            Assert.That(dataSource.Views.Count, Is.EqualTo(1));
            Assert.That(dataSource.Creators, Is.Not.Null);
            Assert.That(dataSource.Creators.Count, Is.EqualTo(1));
            Assert.That(dataSource.ContextDocuments, Is.Not.Null);
            Assert.That(dataSource.ContextDocuments.Count, Is.EqualTo(2));

            configurationValuesMock.AssertWasCalled(m => m.ArchiveInformationPacketType);
            configurationValuesMock.AssertWasCalled(m => m.AlternativeSystemNames);
            configurationValuesMock.AssertWasCalled(m => m.SystemPurpose);
            configurationValuesMock.AssertWasCalled(m => m.SystemContent);
            configurationValuesMock.AssertWasCalled(m => m.RegionNum);
            configurationValuesMock.AssertWasCalled(m => m.KomNum);
            configurationValuesMock.AssertWasCalled(m => m.CprNum);
            configurationValuesMock.AssertWasCalled(m => m.CvrNum);
            configurationValuesMock.AssertWasCalled(m => m.MatrikNum);
            configurationValuesMock.AssertWasCalled(m => m.BbrNum);
            configurationValuesMock.AssertWasCalled(m => m.WhoSygKod);
            configurationValuesMock.AssertWasCalled(m => m.SourceNames);
            configurationValuesMock.AssertWasCalled(m => m.UserNames);
            configurationValuesMock.AssertWasCalled(m => m.PredecessorNames);
            configurationValuesMock.AssertWasCalled(m => m.FormVersion);
            configurationValuesMock.AssertWasCalled(m => m.FormClasses);
            configurationValuesMock.AssertWasCalled(m => m.ContainsDigitalDocuments);
            configurationValuesMock.AssertWasCalled(m => m.SearchRelatedOtherRecords);
            configurationValuesMock.AssertWasCalled(m => m.RelatedRecordsNames);
            configurationValuesMock.AssertWasCalled(m => m.SystemFileConcept);
            configurationValuesMock.AssertWasCalled(m => m.MultipleDataCollection);
            configurationValuesMock.AssertWasCalled(m => m.PersonalDataRestrictedInfo);
            configurationValuesMock.AssertWasCalled(m => m.OtherAccessTypeRestrictions);
            configurationValuesMock.AssertWasCalled(m => m.ArchiveApproval);
            configurationValuesMock.AssertWasCalled(m => m.ArchiveRestrictions);
        }

        /// <summary>
        /// Test that DataSourceGet adds functionality to a field.
        /// </summary>
        [Test]
        public void TestThatDataSourceGetAddsFunctionalityToAField()
        {
            var oldToNewMetadataRepository = new OldToNewMetadataRepository(RepositoryTestHelper.GetSourcePathForTest(), GetConfigurationValuesMock());
            Assert.That(oldToNewMetadataRepository, Is.Not.Null);

            var dataSource = oldToNewMetadataRepository.DataSourceGet();
            Assert.That(dataSource, Is.Not.Null);
            Assert.That(dataSource.Tables, Is.Not.Null);

            var table = dataSource.Tables.Single(m => String.Compare(m.NameSource, "SAG", StringComparison.Ordinal) == 0);
            Assert.That(table, Is.Not.Null);
            Assert.That(table.Fields, Is.Not.Null);

            var field = table.Fields.Single(m => String.Compare(m.NameSource, "SagsID", StringComparison.Ordinal) == 0);
            Assert.That(field, Is.Not.Null);
            Assert.That(field.Functionality, Is.Not.Null);
            Assert.That(field.Functionality.OfType<IMarkFunctionality>(), Is.Not.Null);
            Assert.That(field.Functionality.OfType<IMarkFunctionality>().Count(), Is.EqualTo(1));
            Assert.That(field.Functionality.OfType<IMarkFunctionality>().First(), Is.Not.Null);
            Assert.That(field.Functionality.OfType<IMarkFunctionality>().First().Functionality, Is.Not.Null);
            Assert.That(field.Functionality.OfType<IMarkFunctionality>().First().Functionality, Is.Not.Empty);
            Assert.That(field.Functionality.OfType<IMarkFunctionality>().First().Functionality, Is.EqualTo("Sagsidentifikation"));
        }

        /// <summary>
        /// Test that DataSourceGet are mapping coded values.
        /// </summary>
        [Test]
        public void TestThatDataSourceGetAreMappingCodedValues()
        {
            var oldToNewMetadataRepository = new OldToNewMetadataRepository(RepositoryTestHelper.GetSourcePathForTest(), GetConfigurationValuesMock());
            Assert.That(oldToNewMetadataRepository, Is.Not.Null);

            var dataSource = oldToNewMetadataRepository.DataSourceGet();
            Assert.That(dataSource, Is.Not.Null);
            Assert.That(dataSource.Tables, Is.Not.Null);

            var table = dataSource.Tables.Single(m => String.Compare(m.NameSource, "SAGSBEH", StringComparison.Ordinal) == 0);
            Assert.That(table, Is.Not.Null);
            Assert.That(table.Fields, Is.Not.Null);

            var field = table.Fields.Single(m => String.Compare(m.NameSource, "Kontor", StringComparison.Ordinal) == 0);
            Assert.That(field, Is.Not.Null);
            Assert.That(field.DatatypeOfSource, Is.Not.Null);
            Assert.That(field.DatatypeOfSource, Is.EqualTo(typeof (int?)));
            Assert.That(field.LengthOfSource, Is.EqualTo(2));
            Assert.That(field.DatatypeOfTarget, Is.Not.Null);
            Assert.That(field.DatatypeOfTarget, Is.EqualTo(typeof (string)));
            Assert.That(field.LengthOfTarget, Is.EqualTo(9));
            Assert.That(field.Map, Is.Not.Null);
            Assert.That(field.Map, Is.TypeOf(typeof (StaticMap<int?, string>)));
        }

        /// <summary>
        /// Test that DataSourceGet adds candidate keys.
        /// </summary>
        [Test]
        public void TestThatDataSourceGetAddCandidateKeys()
        {
            var oldToNewMetadataRepository = new OldToNewMetadataRepository(RepositoryTestHelper.GetSourcePathForTest(), GetConfigurationValuesMock());
            Assert.That(oldToNewMetadataRepository, Is.Not.Null);

            var dataSource = oldToNewMetadataRepository.DataSourceGet();
            Assert.That(dataSource, Is.Not.Null);
            Assert.That(dataSource.Tables, Is.Not.Null);

            var table = dataSource.Tables.Single(m => String.Compare(m.NameSource, "SAG", StringComparison.Ordinal) == 0);
            Assert.That(table, Is.Not.Null);
            Assert.That(table.CandidateKeys, Is.Not.Null);
            Assert.That(table.CandidateKeys, Is.Not.Empty);
            Assert.That(table.CandidateKeys.Count, Is.EqualTo(1));

            Assert.That(table.CandidateKeys.ElementAt(0).NameSource, Is.Not.Null);
            Assert.That(table.CandidateKeys.ElementAt(0).NameSource, Is.Not.Empty);
            Assert.That(table.CandidateKeys.ElementAt(0).NameSource, Is.EqualTo("PK_SAG"));
            Assert.That(table.CandidateKeys.ElementAt(0).NameTarget, Is.Not.Null);
            Assert.That(table.CandidateKeys.ElementAt(0).NameTarget, Is.Not.Empty);
            Assert.That(table.CandidateKeys.ElementAt(0).NameTarget, Is.EqualTo("PK_SAG"));
            Assert.That(table.CandidateKeys.ElementAt(0).Description, Is.Not.Null);
            Assert.That(table.CandidateKeys.ElementAt(0).Description, Is.Not.Empty);
            Assert.That(table.CandidateKeys.ElementAt(0).Description, Is.EqualTo("Primær nøgle på SAG"));
        }

        /// <summary>
        /// Test that DataSourceGet adds foreign keys.
        /// </summary>
        [Test]
        public void TestThatDataSourceGetAddForeignKeys()
        {
            var oldToNewMetadataRepository = new OldToNewMetadataRepository(RepositoryTestHelper.GetSourcePathForTest(), GetConfigurationValuesMock());
            Assert.That(oldToNewMetadataRepository, Is.Not.Null);

            var dataSource = oldToNewMetadataRepository.DataSourceGet();
            Assert.That(dataSource, Is.Not.Null);
            Assert.That(dataSource.Tables, Is.Not.Null);

            var table = dataSource.Tables.Single(m => String.Compare(m.NameSource, "DOKTABEL", StringComparison.Ordinal) == 0);
            Assert.That(table, Is.Not.Null);
            Assert.That(table.ForeignKeys, Is.Not.Null);

            var foreignKey = table.ForeignKeys.Single(m => String.Compare(m.NameSource, "FK_DOKTABEL_SAG", StringComparison.Ordinal) == 0);
            Assert.That(foreignKey, Is.Not.Null);
            Assert.That(foreignKey.NameSource, Is.Not.Null);
            Assert.That(foreignKey.NameSource, Is.Not.Empty);
            Assert.That(foreignKey.NameSource, Is.EqualTo("FK_DOKTABEL_SAG"));
            Assert.That(foreignKey.NameTarget, Is.Not.Null);
            Assert.That(foreignKey.NameTarget, Is.Not.Empty);
            Assert.That(foreignKey.NameTarget, Is.EqualTo("FK_DOKTABEL_SAG"));
            Assert.That(foreignKey.Description, Is.Not.Null);
            Assert.That(foreignKey.Description, Is.Not.Empty);
            Assert.That(foreignKey.Description, Is.EqualTo("Fremmednøgle fra DOKTABEL til SAG"));
            Assert.That(foreignKey.Table, Is.Not.Null);
            Assert.That(foreignKey.Table, Is.EqualTo(table));
            Assert.That(foreignKey.CandidateKey, Is.Not.Null);
            Assert.That(foreignKey.CandidateKey.NameSource, Is.Not.Null);
            Assert.That(foreignKey.CandidateKey.NameSource, Is.Not.Empty);
            Assert.That(foreignKey.CandidateKey.NameSource, Is.EqualTo("PK_SAG"));
            Assert.That(foreignKey.CandidateKey.NameTarget, Is.Not.Null);
            Assert.That(foreignKey.CandidateKey.NameTarget, Is.Not.Empty);
            Assert.That(foreignKey.CandidateKey.NameTarget, Is.EqualTo("PK_SAG"));
            Assert.That(foreignKey.CandidateKey.Description, Is.Not.Null);
            Assert.That(foreignKey.CandidateKey.Description, Is.Not.Empty);
            Assert.That(foreignKey.CandidateKey.Description, Is.EqualTo("Primær nøgle på SAG"));
            Assert.That(foreignKey.CandidateKey.Table, Is.Not.Null);
            Assert.That(foreignKey.CandidateKey.Table.NameSource, Is.Not.Null);
            Assert.That(foreignKey.CandidateKey.Table.NameSource, Is.Not.Empty);
            Assert.That(foreignKey.CandidateKey.Table.NameSource, Is.EqualTo("SAG"));
            Assert.That(foreignKey.CandidateKey.Table.NameTarget, Is.Not.Null);
            Assert.That(foreignKey.CandidateKey.Table.NameTarget, Is.Not.Empty);
            Assert.That(foreignKey.CandidateKey.Table.NameTarget, Is.EqualTo("SAG"));
            Assert.That(foreignKey.CandidateKey.Fields, Is.Not.Null);
            Assert.That(foreignKey.CandidateKey.Fields.Count, Is.EqualTo(1));
            Assert.That(foreignKey.CandidateKey.Fields[0].Key, Is.Not.Null);
            Assert.That(foreignKey.CandidateKey.Fields[0].Key.NameSource, Is.Not.Null);
            Assert.That(foreignKey.CandidateKey.Fields[0].Key.NameSource, Is.Not.Empty);
            Assert.That(foreignKey.CandidateKey.Fields[0].Key.NameSource, Is.EqualTo("SagsID"));
            Assert.That(foreignKey.Fields, Is.Not.Null);
            Assert.That(foreignKey.Fields.Count, Is.EqualTo(1));
            Assert.That(foreignKey.Fields[0].Key, Is.Not.Null);
            Assert.That(foreignKey.Fields[0].Key.NameSource, Is.Not.Null);
            Assert.That(foreignKey.Fields[0].Key.NameSource, Is.Not.Empty);
            Assert.That(foreignKey.Fields[0].Key.NameSource, Is.EqualTo("SagsID"));
            Assert.That(foreignKey.Fields[0].Value, Is.Not.Null);
            Assert.That(foreignKey.Fields[0].Value, Is.TypeOf(typeof (StaticMap<int?, int?>)));
            Assert.That(foreignKey.Cardinality, Is.EqualTo(Cardinality.ManyToOne));
        }

        /// <summary>
        /// Test that DataSourceGet adds information about views (queries).
        /// </summary>
        [Test]
        public void TestThatDataSourceGetAddViews()
        {
            var oldToNewMetadataRepository = new OldToNewMetadataRepository(RepositoryTestHelper.GetSourcePathForTest(), GetConfigurationValuesMock());
            Assert.That(oldToNewMetadataRepository, Is.Not.Null);

            var dataSource = oldToNewMetadataRepository.DataSourceGet();
            Assert.That(dataSource, Is.Not.Null);
            Assert.That(dataSource.Views, Is.Not.Null);
            Assert.That(dataSource.Views.Count, Is.EqualTo(1));

            var view = dataSource.Views.First();
            Assert.That(view.NameSource, Is.Not.Null);
            Assert.That(view.NameSource, Is.Not.Empty);
            Assert.That(view.NameSource, Is.EqualTo("AV_VIEW1"));
            Assert.That(view.NameTarget, Is.Not.Null);
            Assert.That(view.NameTarget, Is.Not.Empty);
            Assert.That(view.NameTarget, Is.EqualTo("AV_VIEW1"));
            Assert.That(view.Description, Is.Not.Null);
            Assert.That(view.Description, Is.Not.Empty);
            Assert.That(view.Description, Is.EqualTo("Søgning af dokumenter efter sagsnummer og datointerval"));
            Assert.That(view.SqlQuery, Is.Not.Null);
            Assert.That(view.SqlQuery, Is.Not.Empty);
            Assert.That(view.SqlQuery, Is.EqualTo("CREATE TABLE f (snr VARCHAR(16), brevdatofra DATE, brevdatotil DATE)    INSERT INTO f (snr, brevdatofra, brevdatotil) VALUES (,,)    SELECT sag.sagsnr, sag.sagstitel, doktabel.dokumenttitel, doktabel.dato    FROM sag, doktabel    WHERE     sag.sagsnr LIKE (SELECT snr FROM f)+'%'  and    doktabel.sagsid=sag.sagsid and    doktabel.dato <  (SELECT brevdatotil FROM f) and    doktabel.dato > (SELECT brevdatofra FROM f)     DROP TABLE f"));
        }

        /// <summary>
        /// Test that DataSourceGet adds creators.
        /// </summary>
        [Test]
        public void TestThatDataSourceGetAddCreators()
        {
            var oldToNewMetadataRepository = new OldToNewMetadataRepository(RepositoryTestHelper.GetSourcePathForTest(), GetConfigurationValuesMock());
            Assert.That(oldToNewMetadataRepository, Is.Not.Null);

            var dataSource = oldToNewMetadataRepository.DataSourceGet();
            Assert.That(dataSource, Is.Not.Null);
            Assert.That(dataSource.Creators, Is.Not.Null);
            Assert.That(dataSource.Creators.Count, Is.EqualTo(1));

            var creator = dataSource.Creators.First();
            Assert.That(creator.NameSource, Is.Not.Null);
            Assert.That(creator.NameSource, Is.Not.Empty);
            Assert.That(creator.NameSource, Is.EqualTo("Myndighed1"));
            Assert.That(creator.NameTarget, Is.Not.Null);
            Assert.That(creator.NameTarget, Is.Not.Empty);
            Assert.That(creator.NameTarget, Is.EqualTo("Myndighed1"));
            Assert.That(creator.Description, Is.Null);
            Assert.That(creator.PeriodStart, Is.EqualTo(new DateTime(1996, 1, 1, 0, 0, 0)));
            Assert.That(creator.PeriodEnd, Is.EqualTo(new DateTime(1998, 5, 31, 0, 0, 0)));

        }

        /// <summary>
        /// Test that DataSourceGet adds context documents.
        /// </summary>
        [Test]
        public void TestThatDataSourceGetAddContextDocuments()
        {
            var oldToNewMetadataRepository = new OldToNewMetadataRepository(RepositoryTestHelper.GetSourcePathForTest(), GetConfigurationValuesMock());
            Assert.That(oldToNewMetadataRepository, Is.Not.Null);

            var dataSource = oldToNewMetadataRepository.DataSourceGet();
            Assert.That(dataSource, Is.Not.Null);
            Assert.That(dataSource.ContextDocuments, Is.Not.Null);
            Assert.That(dataSource.ContextDocuments.Count, Is.EqualTo(2));

            var contextDocument = dataSource.ContextDocuments.First();
            Assert.That(contextDocument.Id, Is.EqualTo(1));
            Assert.That(contextDocument.NameSource, Is.Not.Null);
            Assert.That(contextDocument.NameSource, Is.Not.Empty);
            Assert.That(contextDocument.NameSource, Is.EqualTo("ER diagram"));
            Assert.That(contextDocument.NameTarget, Is.Not.Null);
            Assert.That(contextDocument.NameTarget, Is.Not.Empty);
            Assert.That(contextDocument.NameTarget, Is.EqualTo("ER diagram"));
            Assert.That(contextDocument.Description, Is.Null);
            Assert.That(contextDocument.DocumentDate, Is.EqualTo(DateTime.Now).Within(3).Seconds);
            Assert.That(contextDocument.DocumentDatePresicion, Is.EqualTo(DateTimePresicion.Month));
            Assert.That(contextDocument.DocumentAuthors, Is.Not.Null);
            Assert.That(contextDocument.DocumentAuthors.Count, Is.EqualTo(2));
            Assert.That(contextDocument.Categories, Is.Not.Null);
            Assert.That(contextDocument.Categories.Count, Is.EqualTo(2));
            Assert.That(contextDocument.Categories.Contains(ContextDocumentCategories.ArchivalTransformationInformation), Is.True);
            Assert.That(contextDocument.Categories.Contains(ContextDocumentCategories.InformationOther), Is.True);
            
            Assert.That(contextDocument.Reference, Is.Not.Null);
            Assert.That(contextDocument.Reference, Is.Not.Empty);

            var directoryName = Path.GetDirectoryName(contextDocument.Reference);
            Assert.That(directoryName, Is.Not.Null);
            Assert.That(directoryName, Is.Not.Empty);

// ReSharper disable AssignNullToNotNullAttribute
            var directoryInfo = new DirectoryInfo(directoryName);
// ReSharper restore AssignNullToNotNullAttribute
            Assert.That(directoryInfo, Is.Not.Null);
            Assert.That(directoryInfo.Exists, Is.True);

            var searchPattern = Path.GetFileName(contextDocument.Reference);
            Assert.That(searchPattern, Is.Not.Null);
            Assert.That(searchPattern, Is.Not.Empty);

// ReSharper disable AssignNullToNotNullAttribute
            var files = directoryInfo.GetFiles(searchPattern);
// ReSharper restore AssignNullToNotNullAttribute
            Assert.That(files, Is.Not.Null);
            Assert.That(files.Count(), Is.EqualTo(1));
        }

        /// <summary>
        /// Test that DataSourceGet throws an DeliveryEngineRepositoryException if the ARVVER.TAB file is not fout.
        /// </summary>
        [Test]
        public void TestThatDataSourceGetThrowsDeliveryEngineRepositoryExceptionIfArkverTabIsNotFound()
        {
            var oldToNewMetadataRepository = new OldToNewMetadataRepository(new DirectoryInfo(Environment.ExpandEnvironmentVariables("%Temp%")), MockRepository.GenerateMock<IConfigurationValues>());
            Assert.That(oldToNewMetadataRepository, Is.Not.Null);

            var exception = Assert.Throws<DeliveryEngineRepositoryException>(() => oldToNewMetadataRepository.DataSourceGet());
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Empty);
            Assert.That(exception.Message, Is.StringContaining("'ARKVER.TAB'"));
        }

        /// <summary>
        /// Gets configuration values for test.
        /// </summary>
        /// <returns>Configuraton values.</returns>
        private static IConfigurationValues GetConfigurationValuesMock()
        {
            var fixture = new Fixture();
            fixture.Customize<IFormClass>(e => e.FromFactory(() => MockRepository.GenerateMock<IFormClass>()));
            fixture.Customize<IDocumentAuthor>(e => e.FromFactory(() => MockRepository.GenerateMock<IDocumentAuthor>()));
            var configurationValuesMock = MockRepository.GenerateMock<IConfigurationValues>();
            configurationValuesMock.Expect(m => m.ArchiveInformationPacketType)
                .Return(fixture.CreateAnonymous<bool>())
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.AlternativeSystemNames)
                .Return(fixture.CreateMany<string>(3).ToList())
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.SystemPurpose)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.SystemContent)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.RegionNum)
                .Return(fixture.CreateAnonymous<bool>())
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.KomNum)
                .Return(fixture.CreateAnonymous<bool>())
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.CprNum)
                .Return(fixture.CreateAnonymous<bool>())
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.CvrNum)
                .Return(fixture.CreateAnonymous<bool>())
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.MatrikNum)
                .Return(fixture.CreateAnonymous<bool>())
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.BbrNum)
                .Return(fixture.CreateAnonymous<bool>())
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.WhoSygKod)
                .Return(fixture.CreateAnonymous<bool>())
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.SourceNames)
                .Return(fixture.CreateMany<string>(3).ToList())
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.UserNames)
                .Return(fixture.CreateMany<string>(3).ToList())
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.PredecessorNames)
                .Return(fixture.CreateMany<string>(3).ToList())
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.FormVersion)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.FormClasses)
                .Return(fixture.CreateMany<IFormClass>(3).ToList())
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.ContainsDigitalDocuments)
                .Return(fixture.CreateAnonymous<bool>())
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.SearchRelatedOtherRecords)
                .Return(fixture.CreateAnonymous<bool>())
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.RelatedRecordsNames)
                .Return(fixture.CreateMany<string>(3).ToList())
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.SystemFileConcept)
                .Return(fixture.CreateAnonymous<bool>())
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.MultipleDataCollection)
                .Return(fixture.CreateAnonymous<bool>())
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.PersonalDataRestrictedInfo)
                .Return(fixture.CreateAnonymous<bool>())
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.OtherAccessTypeRestrictions)
                .Return(fixture.CreateAnonymous<bool>())
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.ArchiveApproval)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.ArchiveRestrictions)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.ContextDocumentDates)
                .Return(new Dictionary<string, KeyValuePair<DateTimePresicion, DateTime>>(2)
                    {
                        {"GENINFO1", new KeyValuePair<DateTimePresicion, DateTime>(DateTimePresicion.Month, DateTime.Now)},
                        {"GENINFO2", new KeyValuePair<DateTimePresicion, DateTime>(DateTimePresicion.Day, DateTime.Now)}
                    })
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.ContextDocumentAuthors)
                .Return(new Dictionary<string, IEnumerable<IDocumentAuthor>>(2)
                    {
                        {"GENINFO1", fixture.CreateMany<IDocumentAuthor>(2).ToList()},
                        {"GENINFO2", fixture.CreateMany<IDocumentAuthor>(1).ToList()}
                    })
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.ContextDocumentCategories)
                .Return(new Dictionary<string, IEnumerable<ContextDocumentCategories>>(2)
                    {
                        {"GENINFO1", new List<ContextDocumentCategories> {ContextDocumentCategories.ArchivalTransformationInformation, ContextDocumentCategories.InformationOther}},
                        {"GENINFO2", new List<ContextDocumentCategories> {ContextDocumentCategories.SystemAdministrativeFunctions, ContextDocumentCategories.SystemDataProvision}}
                    })
                .Repeat.Any();

            return configurationValuesMock;
        }
    }
}
