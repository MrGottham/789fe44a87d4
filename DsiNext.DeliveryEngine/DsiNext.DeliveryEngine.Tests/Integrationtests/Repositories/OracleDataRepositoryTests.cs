using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Domstolene.JFS.CommonLibrary.IoC.Interfaces;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Log;
using DsiNext.DeliveryEngine.Infrastructure.IoC;
using DsiNext.DeliveryEngine.Repositories.Data.Oracle;
using DsiNext.DeliveryEngine.Repositories.DataManipulators;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators;
using DsiNext.DeliveryEngine.Repositories.Metadata.OldToNew;
using DsiNext.DeliveryEngine.Tests.Unittests.Repositories;
using NUnit.Framework;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Integrationtests.Repositories
{
    /// <summary>
    /// Integration tests of data repository for reading data from an Oracle database.
    /// </summary>
    [TestFixture]
    [Category("IntegrationTests")]
    public class OracleDataRepositoryTests
    {
        /// <summary>
        /// Tests that the data repository gets data from the Oracle database.
        /// </summary>
        [Test]
        public void TestThatDataRepositoryGetsDataFromOracle()
        {
            var containerMock = MockRepository.GenerateMock<IContainer>();

            var informationLoggerMock = MockRepository.GenerateMock<IInformationLogger>();
            informationLoggerMock.Expect(m => m.LogInformation(Arg<string>.Is.NotNull))
                                 .WhenCalled(e => Debug.WriteLine(e.Arguments[0]))
                                 .Repeat.Any();

            var metadataRepository = new OldToNewMetadataRepository(RepositoryTestHelper.GetSourcePathForOracleTest(), new ConfigurationValues());
            Assert.That(metadataRepository, Is.Not.Null);

            ICollection<IDataManipulator> dataMainpulatorCollection;
            using (var windsorContainer = new WindsorContainer())
            {
                windsorContainer.Register(Component.For<IContainer>().Instance(containerMock).LifeStyle.Transient);
                windsorContainer.Register(Component.For<IInformationLogger>().Instance(informationLoggerMock).LifeStyle.Transient);
                windsorContainer.Register(Component.For<IMetadataRepository>().Instance(metadataRepository).LifeStyle.Transient);
                var dataManipulatorsConfigurationProvider = new DataManipulatorsConfigurationProvider();
                dataManipulatorsConfigurationProvider.AddConfiguration(windsorContainer);
                dataMainpulatorCollection = windsorContainer.ResolveAll<IDataManipulator>();
                windsorContainer.Dispose();
            }
            containerMock.Expect(m => m.ResolveAll<IDataManipulator>())
                         .Return(dataMainpulatorCollection.ToArray())
                         .Repeat.Any();

            var oracleClientFactory = new OracleClientFactory();
            Assert.That(oracleClientFactory, Is.Not.Null);

            var dataManipulators = new DataManipulators(containerMock);
            Assert.That(dataManipulators, Is.Not.Null);
            Assert.That(dataManipulators, Is.Not.Empty);

            var dataRepository = new OracleDataRepository(oracleClientFactory, dataManipulators);
            Assert.That(dataRepository, Is.Not.Null);
            containerMock.Expect(m => m.Resolve<IDataRepository>())
                         .Return(dataRepository)
                         .Repeat.Any();

            var startTime = DateTime.MinValue;
            var numberOfRecords = 0;
            dataRepository.OnHandleData += (s, e) =>
                {
                    Assert.That(s, Is.Not.Null);
                    Assert.That(e, Is.Not.Null);
                    Assert.That(e.Table, Is.Not.Null);
                    Assert.That(e.Data, Is.Not.Null);

                    numberOfRecords += e.Data.Count();
                    if (e.EndOfData)
                    {
                        // ReSharper disable AccessToModifiedClosure
                        Debug.WriteLine("{0} has been selected for the table named {1} in {2}", numberOfRecords, e.Table.NameTarget, new TimeSpan(DateTime.Now.Ticks - startTime.Ticks));
                        // ReSharper restore AccessToModifiedClosure
                        numberOfRecords = 0;
                    }
                };

            var dataSource = metadataRepository.DataSourceGet();
            Assert.That(dataSource, Is.Not.Null);
            Assert.That(dataSource.Tables, Is.Not.Null);
            Assert.That(dataSource.Tables, Is.Not.Empty);
            foreach (var table in dataSource.Tables)
            {
                startTime = DateTime.Now;
                dataRepository.DataGetFromTable(table);
            }
        }
    }
}
