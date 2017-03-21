using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.Metadata.OldToNew;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Repositories.Metadata
{
    /// <summary>
    /// Tests the metadata repository with Oracle support for converting old delivery format to the new delivery format.
    /// </summary>
    [TestFixture]
    public class OldToNewMetadataRepositoryForOracleTests
    {
        /// <summary>
        /// Test that the constructor initialize the metadata repository.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeOldToNewMetadataRepository()
        {
            var oldToNewMetadataRepository = new OldToNewMetadataRepository(RepositoryTestHelper.GetSourcePathForOracleTest(), MockRepository.GenerateMock<IConfigurationValues>());
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
            Assert.Throws<ArgumentNullException>(() => new OldToNewMetadataRepository(RepositoryTestHelper.GetSourcePathForOracleTest(), null));
        }

        /// <summary>
        /// Test that DataSourceGet gets the data source.
        /// </summary>
        [Test]
        public void TestThatDataSourceGetGetsDataSource()
        {
            var oldToNewMetadataRepository = new OldToNewMetadataRepository(RepositoryTestHelper.GetSourcePathForOracleTest(), GetConfigurationValuesMock(new Fixture()));
            Assert.That(oldToNewMetadataRepository, Is.Not.Null);

            var dataSource = oldToNewMetadataRepository.DataSourceGet();
            Assert.That(dataSource, Is.Not.Null);
            Assert.That(dataSource.NameSource, Is.Not.Null);
            Assert.That(dataSource.NameSource, Is.Not.Empty);
            Assert.That(dataSource.NameSource, Is.EqualTo("Civil-, Straffe-, Foged-, Auktions- og Skiftesystem"));
            Assert.That(dataSource.NameTarget, Is.Not.Null);
            Assert.That(dataSource.NameTarget, Is.Not.Empty);
            Assert.That(dataSource.NameTarget, Is.EqualTo("Civil-, Straffe-, Foged-, Auktions- og Skiftesystem"));
            Assert.That(dataSource.Description, Is.Not.Null);
            Assert.That(dataSource.Description, Is.Not.Empty);
            Assert.That(dataSource.Description, Is.EqualTo("Civil-, Straffe-, Foged-, Auktions- og Skiftesystem"));
            Assert.That(dataSource.ArchiveInformationPackageId, Is.Not.Empty);
            Assert.That(dataSource.ArchiveInformationPackageId, Is.Not.Null);
            Assert.That(dataSource.ArchiveInformationPackageId, Is.EqualTo("12549"));
            Assert.That(dataSource.ArchiveInformationPackageIdPrevious, Is.EqualTo(10701));
            Assert.That(dataSource.ArchivePeriodStart, Is.EqualTo(new DateTime(1994, 5, 24, 0, 0, 0)));
            Assert.That(dataSource.ArchivePeriodEnd, Is.EqualTo(new DateTime(2009, 6, 30, 0, 0, 0)));
            Assert.That(dataSource.Creators, Is.Not.Null);
            Assert.That(dataSource.Creators.Count, Is.EqualTo(1));
            Assert.That(dataSource.Creators.ElementAt(0).NameSource, Is.Not.Null);
            Assert.That(dataSource.Creators.ElementAt(0).NameSource, Is.Not.Empty);
            Assert.That(dataSource.Creators.ElementAt(0).NameSource, Is.EqualTo("Retten i Åbenrå (retskreds 43)"));
            Assert.That(dataSource.Creators.ElementAt(0).NameTarget, Is.Not.Null);
            Assert.That(dataSource.Creators.ElementAt(0).NameTarget, Is.Not.Empty);
            Assert.That(dataSource.Creators.ElementAt(0).NameTarget, Is.EqualTo("Retten i Åbenrå (retskreds 43)"));
            Assert.That(dataSource.Creators.ElementAt(0).Description, Is.Null);
            Assert.That(dataSource.Creators.ElementAt(0).PeriodStart, Is.EqualTo(dataSource.ArchivePeriodStart));
            Assert.That(dataSource.Creators.ElementAt(0).PeriodEnd, Is.EqualTo(dataSource.ArchivePeriodEnd));

            Assert.That(dataSource.Tables, Is.Not.Null);
            Assert.That(dataSource.Tables.Count, Is.EqualTo(54));

            Assert.That(dataSource.ContextDocuments, Is.Not.Null);
            Assert.That(dataSource.ContextDocuments.Count, Is.EqualTo(5));
        }

        /// <summary>
        /// Gets configuration values for test.
        /// </summary>
        /// <returns>Configuraton values.</returns>
        private static IConfigurationValues GetConfigurationValuesMock(Fixture fixture)
        {
            var configurationValuesMock = MockRepository.GenerateMock<IConfigurationValues>();
            configurationValuesMock.Expect(m => m.ArchiveInformationPacketType)
                .Return(fixture.CreateAnonymous<bool>())
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
            configurationValuesMock.Expect(m => m.FormVersion)
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.ContainsDigitalDocuments)
                .Return(fixture.CreateAnonymous<bool>())
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.SearchRelatedOtherRecords)
                .Return(fixture.CreateAnonymous<bool>())
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
                .Return(null)
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.AlternativeSystemNames)
                .Return(new List<string>(0))
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.SourceNames)
                .Return(new List<string>(0))
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.UserNames)
                .Return(new List<string>(0))
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.PredecessorNames)
                .Return(new List<string>(0))
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.FormClasses)
                .Return(new List<IFormClass>(0))
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.RelatedRecordsNames)
                .Return(new List<string>(0))
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.ContextDocumentDates)
                .Return(new List<KeyValuePair<string, KeyValuePair<DateTimePresicion, DateTime>>>(0))
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.ContextDocumentAuthors)
                .Return(new List<KeyValuePair<string, IEnumerable<IDocumentAuthor>>>(0))
                .Repeat.Any();
            configurationValuesMock.Expect(m => m.ContextDocumentCategories)
                .Return(new List<KeyValuePair<string, IEnumerable<ContextDocumentCategories>>>(0))
                .Repeat.Any();
            return configurationValuesMock;
        }
    }
}
