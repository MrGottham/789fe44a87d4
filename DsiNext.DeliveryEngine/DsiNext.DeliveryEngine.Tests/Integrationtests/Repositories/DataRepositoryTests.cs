using System.Linq;
using Domstolene.JFS.CommonLibrary.IoC;
using DsiNext.DeliveryEngine.Repositories.Data.OldToNew;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Repositories.Metadata.OldToNew;
using NUnit.Framework;

namespace DsiNext.DeliveryEngine.Tests.Integrationtests.Repositories
{
    /// <summary>
    /// Integration tests of IDataRepository implementations.
    /// </summary>
    [TestFixture]
    [Category("IntegrationTests")]
    public class DataRepositoryTests
    {
        /// <summary>
        /// Test that the data repository can be initialized.
        /// </summary>
        [Test]
        public void TestThatDataRepositoryCanBeInitialized()
        {
            var container = ContainerFactory.Create();
            var dataRepository = container.Resolve<IDataRepository>();
            Assert.That(dataRepository, Is.Not.Null);
        }

        /// <summary>
        /// Test that DataGetForTargetTable can read the data from the OldToNew data repository.
        /// </summary>
        [Test]
        public void TestThatDataGetForTargetTableCanReadDataFromOldToNewDataRepository()
        {
            var container = ContainerFactory.Create();

            var dataRepository = container.Resolve<IDataRepository>();
            Assert.That(dataRepository, Is.Not.Null);
            Assert.That(dataRepository, Is.TypeOf(typeof(OldToNewDataRepository)));

            var metadataRepository = container.Resolve<IMetadataRepository>();
            Assert.That(metadataRepository, Is.Not.Null);
            Assert.That(metadataRepository, Is.TypeOf(typeof(OldToNewMetadataRepository)));

            var dataSource = metadataRepository.DataSourceGet();
            Assert.That(dataSource, Is.Not.Null);
            Assert.That(dataSource.Tables, Is.Not.Null);
            Assert.That(dataSource.Tables.Count, Is.EqualTo(5));
            foreach (var table in dataSource.Tables)
            {
                Assert.That(table, Is.Not.Null);
            }

            var eventCalled = 0;
            dataRepository.OnHandleData += (s, e) =>
                {
                    Assert.That(s, Is.Not.Null);
                    Assert.That(e, Is.Not.Null);

                    Assert.That(e.Table, Is.Not.Null);
                    Assert.That(e.Data, Is.Not.Null);
                    if (Equals(e.Table, dataSource.Tables.ElementAt(0)))
                    {
                        Assert.That(e.Data, Is.Not.Null);
                        Assert.That(e.Data.Count(), Is.EqualTo(11));
                    }
                    if (Equals(e.Table, dataSource.Tables.ElementAt(1)))
                    {
                        Assert.That(e.Data, Is.Not.Null);
                        Assert.That(e.Data.Count(), Is.EqualTo(7));
                    }
                    if (Equals(e.Table, dataSource.Tables.ElementAt(2)))
                    {
                        Assert.That(e.Data, Is.Not.Null);
                        Assert.That(e.Data.Count(), Is.EqualTo(22));
                    }
                    if (Equals(e.Table, dataSource.Tables.ElementAt(3)))
                    {
                        Assert.That(e.Data, Is.Not.Null);
                        Assert.That(e.Data.Count(), Is.EqualTo(13));
                    }
                    if (Equals(e.Table, dataSource.Tables.ElementAt(4)))
                    {
                        Assert.That(e.Data, Is.Not.Null);
                        Assert.That(e.Data.Count(), Is.EqualTo(5));
                    }
                    Assert.That(e.EndOfData, Is.True);
                    eventCalled++;
                };

            foreach (var table in dataSource.Tables)
            {
                dataRepository.DataGetForTargetTable(table.NameTarget, dataSource);
            }
            Assert.That(eventCalled, Is.EqualTo(dataSource.Tables.Count));
        }

        /// <summary>
        /// Test that DataGetFromTable can read the data from the OldToNew data repository.
        /// </summary>
        [Test]
        public void TestThatDataGetFromTableCanReadDataFromOldToNewDataRepository()
        {
            var container = ContainerFactory.Create();

            var dataRepository = container.Resolve<IDataRepository>();
            Assert.That(dataRepository, Is.Not.Null);
            Assert.That(dataRepository, Is.TypeOf(typeof (OldToNewDataRepository)));
            
            var metadataRepository = container.Resolve<IMetadataRepository>();
            Assert.That(metadataRepository, Is.Not.Null);
            Assert.That(metadataRepository, Is.TypeOf(typeof(OldToNewMetadataRepository)));

            var dataSource = metadataRepository.DataSourceGet();
            Assert.That(dataSource, Is.Not.Null);
            Assert.That(dataSource.Tables, Is.Not.Null);
            Assert.That(dataSource.Tables.Count, Is.EqualTo(5));
            foreach (var table in dataSource.Tables)
            {
                Assert.That(table, Is.Not.Null);
            }

            var eventCalled = 0;
            dataRepository.OnHandleData += (s, e) =>
                {
                    Assert.That(s, Is.Not.Null);
                    Assert.That(e, Is.Not.Null);

                    Assert.That(e.Table, Is.Not.Null);
                    Assert.That(e.Data, Is.Not.Null);
                    if (Equals(e.Table, dataSource.Tables.ElementAt(0)))
                    {
                        Assert.That(e.Data, Is.Not.Null);
                        Assert.That(e.Data.Count(), Is.EqualTo(11));
                    }
                    if (Equals(e.Table, dataSource.Tables.ElementAt(1)))
                    {
                        Assert.That(e.Data, Is.Not.Null);
                        Assert.That(e.Data.Count(), Is.EqualTo(7));
                    }
                    if (Equals(e.Table, dataSource.Tables.ElementAt(2)))
                    {
                        Assert.That(e.Data, Is.Not.Null);
                        Assert.That(e.Data.Count(), Is.EqualTo(22));
                    }
                    if (Equals(e.Table, dataSource.Tables.ElementAt(3)))
                    {
                        Assert.That(e.Data, Is.Not.Null);
                        Assert.That(e.Data.Count(), Is.EqualTo(13));
                    }
                    if (Equals(e.Table, dataSource.Tables.ElementAt(4)))
                    {
                        Assert.That(e.Data, Is.Not.Null);
                        Assert.That(e.Data.Count(), Is.EqualTo(5));
                    }
                    Assert.That(e.EndOfData, Is.True);
                    eventCalled++;
                };

            foreach (var table in dataSource.Tables)
            {
                dataRepository.DataGetFromTable(table);
            }
            Assert.That(eventCalled, Is.EqualTo(dataSource.Tables.Count));
        }
    }
}
