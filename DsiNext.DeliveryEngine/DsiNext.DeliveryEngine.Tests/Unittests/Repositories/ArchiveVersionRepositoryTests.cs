using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Resources;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Repositories
{
    /// <summary>
    /// Tests the repository for writing the delivery format.
    /// </summary>
    [TestFixture]
    public class ArchiveVersionRepositoryTests
    {
        /// <summary>
        /// Test that the constructor initialize the repository for writing delivery format.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeOldToArchiveVersionRepository()
        {
            var archivePath = ConfigurationManager.AppSettings["ArchivePath"];
            if (string.IsNullOrEmpty(archivePath))
            {
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.ApplicationSettingMissing, "SourcePath"));
            }

            var archiveVersionRepository = new ArchiveVersionRepository(new DirectoryInfo(archivePath));
            Assert.That(archiveVersionRepository, Is.Not.Null);
        }

        //[Test]
        //public void Test()
        //{

        //    var fixture = new Fixture();
        //    fixture.Customize<IConfigurationRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IConfigurationRepository>()));
        //    fixture.Customize<IDocumentRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDocumentRepository>()));

        //    fixture.Customize<ITable>(e => e.FromFactory(() =>
        //    {
        //        var tableMock = MockRepository.GenerateMock<ITable>();
        //        tableMock.Expect(m => m.NameTarget)
        //            .Return(fixture.CreateAnonymous<string>())
        //            .Repeat.Any();
        //        return tableMock;
        //    }));

        //    var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
        //    dataSourceMock.Expect(m => m.Tables)
        //        .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
        //        .Repeat.Any();
        //    fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

        //    var archiveVersionRepository = new ArchiveVersionRepository(new P);
        //    archiveVersionRepository.DataSource = dataSourceMock;
        //    archiveVersionRepository.ArchiveMetaData();
        //}
    }
}
