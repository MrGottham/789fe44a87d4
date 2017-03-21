using Domstolene.JFS.CommonLibrary.IoC;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Repositories.Metadata.OldToNew;
using NUnit.Framework;

namespace DsiNext.DeliveryEngine.Tests.Integrationtests.Repositories
{
    /// <summary>
    /// Integration tests of IMetadataRepository implementations.
    /// </summary>
    [TestFixture]
    [Category("IntegrationTests")]
    public class MetadataRepositoryTests
    {
        /// <summary>
        /// Test that the metadata repository can be initialized.
        /// </summary>
        [Test]
        public void TestThatMetadataRepositoryCanBeInitialized()
        {
            var container = ContainerFactory.Create();
            var metadataRepository = container.Resolve<IMetadataRepository>();
            Assert.That(metadataRepository, Is.Not.Null);
        }

        /// <summary>
        /// Test that DataSourceGet can read the data source from the OldToNew metadata repository.
        /// </summary>
        [Test]
        public void TestThatDataSourceGetCanReadDataSourceFromOldToNewMetadataRepository()
        {
            var container = ContainerFactory.Create();
            var metadataRepository = container.Resolve<IMetadataRepository>();
            Assert.That(metadataRepository, Is.Not.Null);
            Assert.That(metadataRepository, Is.TypeOf(typeof (OldToNewMetadataRepository)));

            var dataSource = metadataRepository.DataSourceGet();
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

            Assert.That(dataSource.Tables, Is.Not.Null);
            Assert.That(dataSource.Tables.Count, Is.EqualTo(5));

            Assert.That(dataSource.Tables[0], Is.Not.Null);
            Assert.That(dataSource.Tables[0].NameSource, Is.Not.Null);
            Assert.That(dataSource.Tables[0].NameSource, Is.Not.Empty);
            Assert.That(dataSource.Tables[0].NameSource, Is.EqualTo("SAG"));
            Assert.That(dataSource.Tables[0].NameTarget, Is.Not.Null);
            Assert.That(dataSource.Tables[0].NameTarget, Is.Not.Empty);
            Assert.That(dataSource.Tables[0].NameTarget, Is.EqualTo("SAG"));
            Assert.That(dataSource.Tables[0].Description, Is.Not.Null);
            Assert.That(dataSource.Tables[0].Description, Is.Not.Empty);
            Assert.That(dataSource.Tables[0].Description, Is.EqualTo("Tabel med oplysninger om sager"));
            Assert.That(dataSource.Tables[0].Fields, Is.Not.Null);
            Assert.That(dataSource.Tables[0].Fields.Count, Is.EqualTo(7));
            Assert.That(dataSource.Tables[0].CandidateKeys, Is.Not.Null);
            Assert.That(dataSource.Tables[0].CandidateKeys.Count, Is.EqualTo(1));
            Assert.That(dataSource.Tables[0].CandidateKeys[0].Fields, Is.Not.Null);
            Assert.That(dataSource.Tables[0].CandidateKeys[0].Fields.Count, Is.EqualTo(1));

            Assert.That(dataSource.Tables[1], Is.Not.Null);
            Assert.That(dataSource.Tables[1].NameSource, Is.Not.Null);
            Assert.That(dataSource.Tables[1].NameSource, Is.Not.Empty);
            Assert.That(dataSource.Tables[1].NameSource, Is.EqualTo("SAGSBEH"));
            Assert.That(dataSource.Tables[1].NameTarget, Is.Not.Null);
            Assert.That(dataSource.Tables[1].NameTarget, Is.Not.Empty);
            Assert.That(dataSource.Tables[1].NameTarget, Is.EqualTo("SAGSBEH"));
            Assert.That(dataSource.Tables[1].Description, Is.Not.Null);
            Assert.That(dataSource.Tables[1].Description, Is.Not.Empty);
            Assert.That(dataSource.Tables[1].Description, Is.EqualTo("Tabel over sagsbehandlere"));
            Assert.That(dataSource.Tables[1].Fields, Is.Not.Null);
            Assert.That(dataSource.Tables[1].Fields.Count, Is.EqualTo(6));
            Assert.That(dataSource.Tables[1].CandidateKeys, Is.Not.Null);
            Assert.That(dataSource.Tables[1].CandidateKeys.Count, Is.EqualTo(1));
            Assert.That(dataSource.Tables[1].CandidateKeys[0].Fields, Is.Not.Null);
            Assert.That(dataSource.Tables[1].CandidateKeys[0].Fields.Count, Is.EqualTo(1));

            Assert.That(dataSource.Tables[2], Is.Not.Null);
            Assert.That(dataSource.Tables[2].NameSource, Is.Not.Null);
            Assert.That(dataSource.Tables[2].NameSource, Is.Not.Empty);
            Assert.That(dataSource.Tables[2].NameSource, Is.EqualTo("DOKTABEL"));
            Assert.That(dataSource.Tables[2].NameTarget, Is.Not.Null);
            Assert.That(dataSource.Tables[2].NameTarget, Is.Not.Empty);
            Assert.That(dataSource.Tables[2].NameTarget, Is.EqualTo("DOKTABEL"));
            Assert.That(dataSource.Tables[2].Description, Is.Not.Null);
            Assert.That(dataSource.Tables[2].Description, Is.Not.Empty);
            Assert.That(dataSource.Tables[2].Description, Is.EqualTo("Tabel med oplysninger om dokumenter"));
            Assert.That(dataSource.Tables[2].Fields, Is.Not.Null);
            Assert.That(dataSource.Tables[2].Fields.Count, Is.EqualTo(4));
            Assert.That(dataSource.Tables[2].CandidateKeys, Is.Not.Null);
            Assert.That(dataSource.Tables[2].CandidateKeys.Count, Is.EqualTo(1));
            Assert.That(dataSource.Tables[2].CandidateKeys[0].Fields, Is.Not.Null);
            Assert.That(dataSource.Tables[2].CandidateKeys[0].Fields.Count, Is.EqualTo(1));

            Assert.That(dataSource.Tables[3], Is.Not.Null);
            Assert.That(dataSource.Tables[3].NameSource, Is.Not.Null);
            Assert.That(dataSource.Tables[3].NameSource, Is.Not.Empty);
            Assert.That(dataSource.Tables[3].NameSource, Is.EqualTo("INDKSTRM"));
            Assert.That(dataSource.Tables[3].NameTarget, Is.Not.Null);
            Assert.That(dataSource.Tables[3].NameTarget, Is.Not.Empty);
            Assert.That(dataSource.Tables[3].NameTarget, Is.EqualTo("INDKSTRM"));
            Assert.That(dataSource.Tables[3].Description, Is.Not.Null);
            Assert.That(dataSource.Tables[3].Description, Is.Not.Empty);
            Assert.That(dataSource.Tables[3].Description, Is.EqualTo("Tabel med indekstermer"));
            Assert.That(dataSource.Tables[3].Fields, Is.Not.Null);
            Assert.That(dataSource.Tables[3].Fields.Count, Is.EqualTo(3));
            Assert.That(dataSource.Tables[3].CandidateKeys, Is.Not.Null);
            Assert.That(dataSource.Tables[3].CandidateKeys.Count, Is.EqualTo(1));
            Assert.That(dataSource.Tables[3].CandidateKeys[0].Fields, Is.Not.Null);
            Assert.That(dataSource.Tables[3].CandidateKeys[0].Fields.Count, Is.EqualTo(1));

            Assert.That(dataSource.Tables[4], Is.Not.Null);
            Assert.That(dataSource.Tables[4].NameSource, Is.Not.Null);
            Assert.That(dataSource.Tables[4].NameSource, Is.Not.Empty);
            Assert.That(dataSource.Tables[4].NameSource, Is.EqualTo("M2MTABEL"));
            Assert.That(dataSource.Tables[4].NameTarget, Is.Not.Null);
            Assert.That(dataSource.Tables[4].NameTarget, Is.Not.Empty);
            Assert.That(dataSource.Tables[4].NameTarget, Is.EqualTo("M2MTABEL"));
            Assert.That(dataSource.Tables[4].Description, Is.Not.Null);
            Assert.That(dataSource.Tables[4].Description, Is.Not.Empty);
            Assert.That(dataSource.Tables[4].Description, Is.EqualTo("Mellemtabel der gør en mange til mange relation mellem SAG og INDKSTRM mulig"));
            Assert.That(dataSource.Tables[4].Fields, Is.Not.Null);
            Assert.That(dataSource.Tables[4].Fields.Count, Is.EqualTo(3));
            Assert.That(dataSource.Tables[4].CandidateKeys, Is.Not.Null);
            Assert.That(dataSource.Tables[4].CandidateKeys.Count, Is.EqualTo(1));
            Assert.That(dataSource.Tables[4].CandidateKeys[0].Fields, Is.Not.Null);
            Assert.That(dataSource.Tables[4].CandidateKeys[0].Fields.Count, Is.EqualTo(1));
        }
    }
}
