using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.DataManipulators;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Resources;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Repositories.DataManipulators
{
    /// <summary>
    /// Tests the basic worker to manipulate missing foreign key values.
    /// </summary>
    [TestFixture]
    public class MissingForeignKeyWorkerBaseTests
    {
        /// <summary>
        /// Own class for testing the basic worker to manipulate missing foreign key values.
        /// </summary>
        private class MyMissingForeignKeyWorker : MissingForeignKeyWorkerBase
        {
            #region Constructor

            /// <summary>
            /// Creates a own class for testing the basic worker to manipulate missing foreign key values.
            /// </summary>
            /// <param name="targetTableName">Target name of the table to validate against.</param>
            /// <param name="metadataRepository">Metadata repository.</param>
            public MyMissingForeignKeyWorker(string targetTableName, IMetadataRepository metadataRepository)
                : base(targetTableName, metadataRepository)
            {
            }

            #endregion

            #region Properties

            /// <summary>
            /// Indicates whether the data is manipulated.
            /// </summary>
            public bool DataIsManipulated
            {
                get;
                private set;
            }

            /// <summary>
            /// Indicates whether the data is finalized and manipulated.
            /// </summary>
            public bool DataIsFinalized
            {
                get;
                private set;
            }

            /// <summary>
            /// Indicates whether the ManipulatingField has been called.
            /// </summary>
            public bool ManipulatingFieldCalled
            {
                get;
                private set;

            }

            /// <summary>
            /// Gets an unique identification for the worker.
            /// </summary>
            public new string WorkerId
            {
                get
                {
                    return base.WorkerId;
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Manipulates missing foreign key values for a given table.
            /// </summary>
            /// <param name="table">Tabel to manipulate data for missing foreign key values.</param>
            /// <param name="dataRepository">Data repository.</param>
            /// <param name="dataToManipulate">Data for the table where to manipulate data for missing foreign key values.</param>
            /// <returns>Manipulates data for the table.</returns>
            protected override IEnumerable<IEnumerable<IDataObjectBase>> Manipulate(ITable table, IDataRepository dataRepository, IList<IEnumerable<IDataObjectBase>> dataToManipulate)
            {
                DataIsManipulated = true;
                return dataToManipulate;
            }

            /// <summary>
            /// Finalize missing foreign key values for a given table.
            /// </summary>
            /// <param name="table">Table to finalize data manipulation for missing foreign key values.</param>
            /// <param name="dataRepository">Data repository.</param>
            /// <param name="dataToManipulate">The last manipulated data which has been received.</param>
            /// <returns>Finalized and manipulated data for the table.</returns>
            protected override IEnumerable<IEnumerable<IDataObjectBase>> Finalize(ITable table, IDataRepository dataRepository, IList<IEnumerable<IDataObjectBase>> dataToManipulate)
            {
                DataIsFinalized = true;
                return base.Finalize(table, dataRepository, dataToManipulate);
            }

            /// <summary>
            /// Indicates whether the worker is manipulating a given field.
            /// </summary>
            /// <param name="fieldName">Name of the field on which to exam for use in the worker.</param>
            /// <param name="workOnTable">The table which the worker are allocated to.</param>
            /// <returns>True if the worker use the field otherwise false.</returns>
            protected override bool ManipulatingField(string fieldName, ITable workOnTable)
            {
                ManipulatingFieldCalled = true;
                return base.ManipulatingField(fieldName, workOnTable);
            }

            /// <summary>
            /// Get the dictionary name for a given key.
            /// </summary>
            /// <param name="key">Key.</param>
            /// <returns>Dictionary name.</returns>
            public new string GetDictionaryName(IKey key)
            {
                return base.GetDictionaryName(key);
            }

            /// <summary>
            /// Gets a data queryer for executing queries on a given data repository.
            /// </summary>
            /// <param name="dataRepository">Data repository on which to execute queries.</param>
            /// <returns>Data queryer for executing queries.</returns>
            public new IDataQueryer GetDataQueryer(IDataRepository dataRepository)
            {
                return base.GetDataQueryer(dataRepository);
            }

            #endregion
        }

        /// <summary>
        /// Tests that the constructor initialize the worker to manipulate missing foreign key values.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeWorker()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var worker = new MyMissingForeignKeyWorker(dataSourceMock.Tables.ElementAt(1).NameTarget, fixture.CreateAnonymous<IMetadataRepository>());
            Assert.That(worker, Is.Not.Null);
            Assert.That(worker.ForeignKeyTable, Is.Not.Null);
            Assert.That(worker.ForeignKeyTable, Is.EqualTo(dataSourceMock.Tables.ElementAt(1)));
            Assert.That(worker.WorkerId, Is.Not.Null);
            Assert.That(worker.WorkerId, Is.Not.Empty);

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentNullException if name of the target table is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfTargetTableNameIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IMetadataRepository>()));

            Assert.Throws<ArgumentNullException>(() => new MyMissingForeignKeyWorker(null, fixture.CreateAnonymous<IMetadataRepository>()));
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentNullException if name of the target table is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfTargetTableNameIsEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IMetadataRepository>()));

            Assert.Throws<ArgumentNullException>(() => new MyMissingForeignKeyWorker(string.Empty, fixture.CreateAnonymous<IMetadataRepository>()));
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentNullException if metadata repository is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfMetadataRepositoryIsNull()
        {
            var fixture = new Fixture();

            Assert.Throws<ArgumentNullException>(() => new MyMissingForeignKeyWorker(fixture.CreateAnonymous<string>(), null));
        }

        /// <summary>
        /// Tests that the constructor throws an DeliveryEngineSystemException if name of the target table does not exists in the data source.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsDeliveryEngineSystemExceptionIfTargetTableNameNotInDataSource()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));
            fixture.Customize<IDataSource>(e => e.FromFactory(() =>
                {
                    var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
                    dataSourceMock.Expect(m => m.Tables)
                                  .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                                  .Repeat.Any();
                    return dataSourceMock;
                }));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var exception = Assert.Throws<DeliveryEngineSystemException>(() => new MyMissingForeignKeyWorker(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<IMetadataRepository>()));
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Empty);
            Assert.That(exception.Message, Is.StringStarting(Resource.GetExceptionMessage(ExceptionMessage.TableNotFound, string.Empty)));

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
        }

        /// <summary>
        /// Tests that ManipulateData throws an ArgumentNullException if the table is null.
        /// </summary>
        [Test]
        public void TestThatManipulateDataThrowsArgumentNullExceptionIfTableIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var worker = new MyMissingForeignKeyWorker(dataSourceMock.Tables.ElementAt(1).NameTarget, fixture.CreateAnonymous<IMetadataRepository>());
            Assert.That(worker, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => worker.ManipulateData(null, fixture.CreateAnonymous<IDataRepository>(), new Collection<IEnumerable<IDataObjectBase>>()));
        }

        /// <summary>
        /// Tests that ManipulateData throws an ArgumentNullException if the data repository is null.
        /// </summary>
        [Test]
        public void TestThatManipulateDataThrowsArgumentNullExceptionIfDataRepositoryIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var worker = new MyMissingForeignKeyWorker(dataSourceMock.Tables.ElementAt(1).NameTarget, fixture.CreateAnonymous<IMetadataRepository>());
            Assert.That(worker, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => worker.ManipulateData(dataSourceMock.Tables.ElementAt(0), null, new Collection<IEnumerable<IDataObjectBase>>()));
        }

        /// <summary>
        /// Tests that ManipulateData throws an ArgumentNullException if data for the table is null.
        /// </summary>
        [Test]
        public void TestThatManipulateDataThrowsArgumentNullExceptionIfDataIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var worker = new MyMissingForeignKeyWorker(dataSourceMock.Tables.ElementAt(1).NameTarget, fixture.CreateAnonymous<IMetadataRepository>());
            Assert.That(worker, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => worker.ManipulateData(dataSourceMock.Tables.ElementAt(0), fixture.CreateAnonymous<IDataRepository>(), null));
        }

        /// <summary>
        /// Tests that ManipulateData manipulates data.
        /// </summary>
        [Test]
        public void TestThatManipulateDataManipulatesData()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var worker = new MyMissingForeignKeyWorker(dataSourceMock.Tables.ElementAt(1).NameTarget, fixture.CreateAnonymous<IMetadataRepository>());
            Assert.That(worker, Is.Not.Null);
            Assert.That(worker.DataIsManipulated, Is.False);

            var manipulatedData = worker.ManipulateData(dataSourceMock.Tables.ElementAt(0), fixture.CreateAnonymous<IDataRepository>(), new Collection<IEnumerable<IDataObjectBase>>());
            Assert.That(manipulatedData, Is.Not.Null);
            Assert.That(manipulatedData, Is.Empty);

            Assert.That(worker.DataIsManipulated, Is.True);
        }

        /// <summary>
        /// Tests that FinalizeDataManipulation throws an ArgumentNullException if the table is null.
        /// </summary>
        [Test]
        public void TestThatFinalizeDataManipulationThrowsArgumentNullExceptionIfTableIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var worker = new MyMissingForeignKeyWorker(dataSourceMock.Tables.ElementAt(1).NameTarget, fixture.CreateAnonymous<IMetadataRepository>());
            Assert.That(worker, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => worker.FinalizeDataManipulation(null, fixture.CreateAnonymous<IDataRepository>(), new Collection<IEnumerable<IDataObjectBase>>()));
        }

        /// <summary>
        /// Tests that FinalizeDataManipulation throws an ArgumentNullException if the data repository is null.
        /// </summary>
        [Test]
        public void TestThatFinalizeDataManipulationThrowsArgumentNullExceptionIfDataRepositoryIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var worker = new MyMissingForeignKeyWorker(dataSourceMock.Tables.ElementAt(1).NameTarget, fixture.CreateAnonymous<IMetadataRepository>());
            Assert.That(worker, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => worker.FinalizeDataManipulation(dataSourceMock.Tables.ElementAt(0), null, new Collection<IEnumerable<IDataObjectBase>>()));
        }

        /// <summary>
        /// Tests that FinalizeDataManipulation throws an ArgumentNullException if data for the table is null.
        /// </summary>
        [Test]
        public void TestThatFinalizeDataManipulationThrowsArgumentNullExceptionIfDataIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));
            fixture.Customize<ITable>(e => e.FromFactory(() =>
            {
                var tableMock = MockRepository.GenerateMock<ITable>();
                tableMock.Expect(m => m.NameTarget)
                         .Return(fixture.CreateAnonymous<string>())
                         .Repeat.Any();
                return tableMock;
            }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));
            // ReSharper restore ImplicitlyCapturedClosure

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var worker = new MyMissingForeignKeyWorker(dataSourceMock.Tables.ElementAt(1).NameTarget, fixture.CreateAnonymous<IMetadataRepository>());
            Assert.That(worker, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => worker.FinalizeDataManipulation(dataSourceMock.Tables.ElementAt(0), fixture.CreateAnonymous<IDataRepository>(), null));
        }

        /// <summary>
        /// Tests that FinalizeDataManipulation finalize data manipulation.
        /// </summary>
        [Test]
        public void TestThatFinalizeDataManipulationFinalizeDataManipulation()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var worker = new MyMissingForeignKeyWorker(dataSourceMock.Tables.ElementAt(1).NameTarget, fixture.CreateAnonymous<IMetadataRepository>());
            Assert.That(worker, Is.Not.Null);
            Assert.That(worker.DataIsFinalized, Is.False);

            var manipulatedData = worker.FinalizeDataManipulation(dataSourceMock.Tables.ElementAt(0), fixture.CreateAnonymous<IDataRepository>(), new Collection<IEnumerable<IDataObjectBase>>());
            Assert.That(manipulatedData, Is.Not.Null);
            Assert.That(manipulatedData, Is.Empty);

            Assert.That(worker.DataIsFinalized, Is.True);
        }

        /// <summary>
        /// Test that IsManipulatingField throws an ArgumentNullException if name of the field is null.
        /// </summary>
        [Test]
        public void TestThatIsManipulatingFieldThrowsArgumentNullExceptionIfFieldNameIfNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var worker = new MyMissingForeignKeyWorker(dataSourceMock.Tables.ElementAt(1).NameTarget, fixture.CreateAnonymous<IMetadataRepository>());
            Assert.That(worker, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => worker.IsManipulatingField(null, fixture.CreateAnonymous<ITable>()));
        }

        /// <summary>
        /// Test that IsManipulatingField throws an ArgumentNullException if name of the field is null.
        /// </summary>
        [Test]
        public void TestThatIsManipulatingFieldThrowsArgumentNullExceptionIfFieldNameIfEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var worker = new MyMissingForeignKeyWorker(dataSourceMock.Tables.ElementAt(1).NameTarget, fixture.CreateAnonymous<IMetadataRepository>());
            Assert.That(worker, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => worker.IsManipulatingField(string.Empty, fixture.CreateAnonymous<ITable>()));
        }

        /// <summary>
        /// Test that IsManipulatingField throws an ArgumentNullException if the table for which the worker is allocated to is null.
        /// </summary>
        [Test]
        public void TestThatIsManipulatingFieldThrowsArgumentNullExceptionIfTableIfEmpty()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var worker = new MyMissingForeignKeyWorker(dataSourceMock.Tables.ElementAt(1).NameTarget, fixture.CreateAnonymous<IMetadataRepository>());
            Assert.That(worker, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => worker.IsManipulatingField(fixture.CreateAnonymous<string>(), null));
        }

        /// <summary>
        /// Test that IsManipulatingField calls and return result from the virtual method ManipulatingField. 
        /// </summary>
        [Test]
        public void TestThatIsManipulatingFieldCallsAndReturnResultFromManipulatingField()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var worker = new MyMissingForeignKeyWorker(dataSourceMock.Tables.ElementAt(1).NameTarget, fixture.CreateAnonymous<IMetadataRepository>());
            Assert.That(worker, Is.Not.Null);

            Assert.That(worker.ManipulatingFieldCalled, Is.False);
            Assert.That(worker.IsManipulatingField(fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<ITable>()), Is.False);
            Assert.That(worker.ManipulatingFieldCalled, Is.True);
        }

        /// <summary>
        /// Tests that GetDictionaryName throws an ArgumentNullException if the key is null.
        /// </summary>
        [Test]
        public void TestThatGetDictionaryNameThrowsArgumentNullExceptionIfKeyIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var worker = new MyMissingForeignKeyWorker(dataSourceMock.Tables.ElementAt(1).NameTarget, fixture.CreateAnonymous<IMetadataRepository>());
            Assert.That(worker, Is.Not.Null);
            Assert.That(worker.ForeignKeyTable, Is.Not.Null);
            Assert.That(worker.ForeignKeyTable, Is.EqualTo(dataSourceMock.Tables.ElementAt(1)));

            Assert.Throws<ArgumentNullException>(() => worker.GetDictionaryName(null));

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
        }

        /// <summary>
        /// Tests that GetDictionaryName gets the dictionary name for the key.
        /// </summary>
        [Test]
        public void TestThatGetDictionaryNameGetsDictionaryName()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return fieldMock;
                }));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IKey>(e => e.FromFactory(() =>
                {
                    var keyMock = MockRepository.GenerateMock<IKey>();
                    keyMock.Expect(m => m.Table)
                           .Return(fixture.CreateAnonymous<ITable>())
                           .Repeat.Any();
                    keyMock.Expect(m => m.Fields)
                           .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(fixture.CreateMany<KeyValuePair<IField, IMap>>(3).ToList())))
                           .Repeat.Any();
                    return keyMock;
                }));
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var worker = new MyMissingForeignKeyWorker(dataSourceMock.Tables.ElementAt(1).NameTarget, fixture.CreateAnonymous<IMetadataRepository>());
            Assert.That(worker, Is.Not.Null);
            Assert.That(worker.ForeignKeyTable, Is.Not.Null);
            Assert.That(worker.ForeignKeyTable, Is.EqualTo(dataSourceMock.Tables.ElementAt(1)));

            var dictionaryKeyMock = fixture.CreateAnonymous<IKey>();
            var dictionaryName = worker.GetDictionaryName(dictionaryKeyMock);
            Assert.That(dictionaryName, Is.Not.Null);
            Assert.That(dictionaryName, Is.Not.Empty);
            Assert.That(dictionaryName, Is.EqualTo(string.Format("{0}:{1}({2}:{3},{4}:{5},{6}:{7})", dictionaryKeyMock.Table.NameTarget, "{null}", dictionaryKeyMock.Fields.ElementAt(0).Key.NameTarget, "{null}", dictionaryKeyMock.Fields.ElementAt(1).Key.NameTarget, "{null}", dictionaryKeyMock.Fields.ElementAt(2).Key.NameTarget, "{null}")));

            dictionaryKeyMock.AssertWasCalled(m => m.Fields);
            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
        }

        /// <summary>
        /// Tests that GetDataQueryer throws an ArgumentNullException if the data repository is null.
        /// </summary>
        [Test]
        public void TestThatGetDataQueryerThrowsArgumentNullExceptionIfDataRepositoryIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var worker = new MyMissingForeignKeyWorker(dataSourceMock.Tables.ElementAt(1).NameTarget, fixture.CreateAnonymous<IMetadataRepository>());
            Assert.That(worker, Is.Not.Null);
            Assert.That(worker.ForeignKeyTable, Is.Not.Null);
            Assert.That(worker.ForeignKeyTable, Is.EqualTo(dataSourceMock.Tables.ElementAt(1)));

            Assert.Throws<ArgumentNullException>(() => worker.GetDataQueryer(null));

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
        }

        /// <summary>
        /// Test that GetDataQueryer returns a data queryer for the data repository.
        /// </summary>
        [Test]
        public void TestThatGetDataQueryerReturnsDataQueryerForDataRepository()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var dataQueyerMock = MockRepository.GenerateMock<IDataQueryer>();
            fixture.Customize<IDataQueryer>(e => e.FromFactory(() => dataQueyerMock));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            dataRepositoryMock.Expect(m => m.GetDataQueryer())
                              .Return(dataQueyerMock)
                              .Repeat.Any();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));

            var worker = new MyMissingForeignKeyWorker(dataSourceMock.Tables.ElementAt(1).NameTarget, fixture.CreateAnonymous<IMetadataRepository>());
            Assert.That(worker, Is.Not.Null);
            Assert.That(worker.ForeignKeyTable, Is.Not.Null);
            Assert.That(worker.ForeignKeyTable, Is.EqualTo(dataSourceMock.Tables.ElementAt(1)));

            using (var dataQueryer = worker.GetDataQueryer(fixture.CreateAnonymous<IDataRepository>()))
            {
                Assert.That(dataQueryer, Is.Not.Null);
                Assert.That(dataQueryer, Is.EqualTo(dataQueyerMock));

                dataQueryer.Dispose();
            }

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
            dataRepositoryMock.AssertWasCalled(m => m.GetDataQueryer());
        }

        /// <summary>
        /// Test that GetDataQueryer returns null if GetDataQueryer in the data repository throws an NotSupportedException.
        /// </summary>
        [Test]
        public void TestThatGetDataQueryerReturnsNullIfGetDataQueryerInDataRepositoryThrowsNotSupportedException()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            dataRepositoryMock.Expect(m => m.GetDataQueryer())
                              .Throw(fixture.CreateAnonymous<NotSupportedException>())
                              .Repeat.Any();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));

            var worker = new MyMissingForeignKeyWorker(dataSourceMock.Tables.ElementAt(1).NameTarget, fixture.CreateAnonymous<IMetadataRepository>());
            Assert.That(worker, Is.Not.Null);
            Assert.That(worker.ForeignKeyTable, Is.Not.Null);
            Assert.That(worker.ForeignKeyTable, Is.EqualTo(dataSourceMock.Tables.ElementAt(1)));

            var dataQueryer = worker.GetDataQueryer(fixture.CreateAnonymous<IDataRepository>());
            Assert.That(dataQueryer, Is.Null);

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
            dataRepositoryMock.AssertWasCalled(m => m.GetDataQueryer());
        }

        /// <summary>
        /// Test that GetDataQueryer throws DeliveryEngineRepositoryException if GetDataQueryer in the data repository throws an DeliveryEngineRepositoryException.
        /// </summary>
        [Test]
        public void TestThatGetDataQueryerThrowsDeliveryEngineRepositoryExceptionIfGetDataQueryerInDataRepositoryThrowsDeliveryEngineRepositoryException()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            dataRepositoryMock.Expect(m => m.GetDataQueryer())
                              .Throw(fixture.CreateAnonymous<DeliveryEngineRepositoryException>())
                              .Repeat.Any();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));

            var worker = new MyMissingForeignKeyWorker(dataSourceMock.Tables.ElementAt(1).NameTarget, fixture.CreateAnonymous<IMetadataRepository>());
            Assert.That(worker, Is.Not.Null);
            Assert.That(worker.ForeignKeyTable, Is.Not.Null);
            Assert.That(worker.ForeignKeyTable, Is.EqualTo(dataSourceMock.Tables.ElementAt(1)));

            Assert.Throws<DeliveryEngineRepositoryException>(() => worker.GetDataQueryer(fixture.CreateAnonymous<IDataRepository>()));

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
            dataRepositoryMock.AssertWasCalled(m => m.GetDataQueryer());
        }

        /// <summary>
        /// Test that GetDataQueryer throws DeliveryEngineSystemException if GetDataQueryer in the data repository throws an Exception.
        /// </summary>
        [Test]
        public void TestThatGetDataQueryerThrowsDeliveryEngineSystemExceptionIfGetDataQueryerInDataRepositoryThrowsException()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));

            var dataSourceMock = MockRepository.GenerateMock<IDataSource>();
            dataSourceMock.Expect(m => m.Tables)
                          .Return(new ReadOnlyObservableCollection<ITable>(new ObservableCollection<ITable>(fixture.CreateMany<ITable>(5).ToList())))
                          .Repeat.Any();
            fixture.Customize<IDataSource>(e => e.FromFactory(() => dataSourceMock));

            var metadataRepositoryMock = MockRepository.GenerateMock<IMetadataRepository>();
            metadataRepositoryMock.Expect(m => m.DataSourceGet())
                                  .Return(fixture.CreateAnonymous<IDataSource>())
                                  .Repeat.Any();
            fixture.Customize<IMetadataRepository>(e => e.FromFactory(() => metadataRepositoryMock));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            dataRepositoryMock.Expect(m => m.GetDataQueryer())
                              .Throw(fixture.CreateAnonymous<Exception>())
                              .Repeat.Any();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));

            var worker = new MyMissingForeignKeyWorker(dataSourceMock.Tables.ElementAt(1).NameTarget, fixture.CreateAnonymous<IMetadataRepository>());
            Assert.That(worker, Is.Not.Null);
            Assert.That(worker.ForeignKeyTable, Is.Not.Null);
            Assert.That(worker.ForeignKeyTable, Is.EqualTo(dataSourceMock.Tables.ElementAt(1)));

            Assert.Throws<DeliveryEngineSystemException>(() => worker.GetDataQueryer(fixture.CreateAnonymous<IDataRepository>()));

            metadataRepositoryMock.AssertWasCalled(m => m.DataSourceGet());
            dataRepositoryMock.AssertWasCalled(m => m.GetDataQueryer());
        }
    }
}
