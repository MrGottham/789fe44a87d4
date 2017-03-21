using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DsiNext.DeliveryEngine.BusinessLogic.DataValidators;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Commands;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Events;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.BusinessLogic.DataValidators
{
    /// <summary>
    /// Tests validator for foreign keys on data for a target table.
    /// </summary>
    [TestFixture]
    public class ForeignKeysDataValidatorTests
    {
        /// <summary>
        /// Test that the constructor initialize the data validator.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeDataValidator()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));

            var dataRepository = fixture.CreateAnonymous<IDataRepository>();
            Assert.That(dataRepository, Is.Not.Null);

            using (var dataValidator = new ForeignKeysDataValidator(dataRepository))
            {
                Assert.That(dataValidator, Is.Not.Null);
                Assert.That(dataValidator.DataRepository, Is.Not.Null);
                Assert.That(dataValidator.DataRepository, Is.EqualTo(dataRepository));
                dataValidator.Dispose();
            }
        }

        /// <summary>
        /// Test that the constructor throws an ArgumentNullException if the data repository er null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfDataRepositoryErNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ForeignKeysDataValidator(null));
        }

        /// <summary>
        /// Test that Dispose can be called.
        /// </summary>
        [Test]
        public void TestThatDisposeCanBeCalled()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));

            using (var dataValidator = new ForeignKeysDataValidator(fixture.CreateAnonymous<IDataRepository>()))
            {
                Assert.That(dataValidator, Is.Not.Null);

                dataValidator.Dispose();
            }
        }

        /// <summary>
        /// Test that Dispose can be called more than one time.
        /// </summary>
        [Test]
        public void TestThatDisposeCanBeCalledMoreThanOnce()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));

            using (var dataValidator = new ForeignKeysDataValidator(fixture.CreateAnonymous<IDataRepository>()))
            {
                Assert.That(dataValidator, Is.Not.Null);

                dataValidator.Dispose();
                dataValidator.Dispose();
            }
        }

        /// <summary>
        /// Test that Validate throws an ObjectDisposedException if the data validator is disposed.
        /// </summary>
        [Test]
        public void TestThatValidateThrowsObjectDisposedExceptionIfDataValidatorIsDisposed()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));
            fixture.Customize<IForeignKeysValidationCommand>(e => e.FromFactory(() => MockRepository.GenerateMock<IForeignKeysValidationCommand>()));

            using (var dataValidator = new ForeignKeysDataValidator(fixture.CreateAnonymous<IDataRepository>()))
            {
                Assert.That(dataValidator, Is.Not.Null);

                dataValidator.Dispose();

                var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>(1)
                    {
                        {fixture.CreateAnonymous<ITable>(), new List<IEnumerable<IDataObjectBase>>((0))}
                    };
                Assert.Throws<ObjectDisposedException>(() => dataValidator.Validate(fixture.CreateAnonymous<ITable>(), targetTableData, true, fixture.CreateAnonymous<IForeignKeysValidationCommand>()));
            }
        }

        /// <summary>
        /// Test that Validate throws an DeliveryEngineMetadataException if the candidate key on a foreign key is null.
        /// </summary>
        [Test]
        public void TestThatValidateThrowsDeliveryEngineMetadataExceptionIfCandidateKeyOnForeignKeyIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));
            fixture.Customize<IForeignKey>(e => e.FromFactory(() =>
                {
                    var foreignKeyMock = MockRepository.GenerateMock<IForeignKey>();
                    foreignKeyMock.Expect(m => m.NameSource)
                                  .Return(fixture.CreateAnonymous<string>())
                                  .Repeat.Any();
                    foreignKeyMock.Expect(m => m.CandidateKey)
                                  .Return(null)
                                  .Repeat.Any();
                    return foreignKeyMock;
                }));
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    tableMock.Expect(m => m.ForeignKeys)
                             .Return(new ReadOnlyObservableCollection<IForeignKey>(new ObservableCollection<IForeignKey>(fixture.CreateMany<IForeignKey>(1).ToList())))
                             .Repeat.Any();
                    return tableMock;
                }));
            fixture.Customize<IForeignKeysValidationCommand>(e => e.FromFactory(() => MockRepository.GenerateMock<IForeignKeysValidationCommand>()));

            using (var dataValidator = new ForeignKeysDataValidator(fixture.CreateAnonymous<IDataRepository>()))
            {
                Assert.That(dataValidator, Is.Not.Null);

                var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>(1)
                    {
                        {fixture.CreateAnonymous<ITable>(), new List<IEnumerable<IDataObjectBase>>((0))}
                    };
                Assert.Throws<DeliveryEngineMetadataException>(() => dataValidator.Validate(fixture.CreateAnonymous<ITable>(), targetTableData, true, fixture.CreateAnonymous<IForeignKeysValidationCommand>()));

                targetTableData.ElementAt(0).Key.ForeignKeys.ElementAt(0).AssertWasCalled(m => m.CandidateKey);
                targetTableData.ElementAt(0).Key.ForeignKeys.ElementAt(0).AssertWasCalled(m => m.NameSource);
                targetTableData.ElementAt(0).Key.AssertWasCalled(m => m.NameSource);

                dataValidator.Dispose();
            }
        }

        /// <summary>
        /// Test that Validate throws an DeliveryEngineMetadataException if fields on the candidate key is null.
        /// </summary>
        [Test]
        public void TestThatValidateThrowsDeliveryEngineMetadataExceptionIfFieldsOnCandidateKeyIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));
            fixture.Customize<ICandidateKey>(e => e.FromFactory(() =>
                {
                    var candidateKeyMock = MockRepository.GenerateMock<ICandidateKey>();
                    candidateKeyMock.Expect(m => m.NameSource)
                                    .Return(fixture.CreateAnonymous<string>())
                                    .Repeat.Any();
                    candidateKeyMock.Expect(m => m.Fields)
                                    .Return(null)
                                    .Repeat.Any();
                    return candidateKeyMock;
                }));
            fixture.Customize<IForeignKey>(e => e.FromFactory(() =>
                {
                    var foreignKeyMock = MockRepository.GenerateMock<IForeignKey>();
                    foreignKeyMock.Expect(m => m.NameSource)
                                  .Return(fixture.CreateAnonymous<string>())
                                  .Repeat.Any();
                    foreignKeyMock.Expect(m => m.CandidateKey)
                                  .Return(fixture.CreateAnonymous<ICandidateKey>())
                                  .Repeat.Any();
                    return foreignKeyMock;
                }));
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    tableMock.Expect(m => m.ForeignKeys)
                             .Return(new ReadOnlyObservableCollection<IForeignKey>(new ObservableCollection<IForeignKey>(fixture.CreateMany<IForeignKey>(1).ToList())))
                             .Repeat.Any();
                    return tableMock;
                }));
            fixture.Customize<IForeignKeysValidationCommand>(e => e.FromFactory(() => MockRepository.GenerateMock<IForeignKeysValidationCommand>()));

            using (var dataValidator = new ForeignKeysDataValidator(fixture.CreateAnonymous<IDataRepository>()))
            {
                Assert.That(dataValidator, Is.Not.Null);

                var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>(1)
                    {
                        {fixture.CreateAnonymous<ITable>(), new List<IEnumerable<IDataObjectBase>>((0))}
                    };
                Assert.Throws<DeliveryEngineMetadataException>(() => dataValidator.Validate(fixture.CreateAnonymous<ITable>(), targetTableData, true, fixture.CreateAnonymous<IForeignKeysValidationCommand>()));

                targetTableData.ElementAt(0).Key.ForeignKeys.ElementAt(0).CandidateKey.AssertWasCalled(m => m.Fields);
                targetTableData.ElementAt(0).Key.ForeignKeys.ElementAt(0).CandidateKey.AssertWasCalled(m => m.NameSource);
                targetTableData.ElementAt(0).Key.ForeignKeys.ElementAt(0).AssertWasNotCalled(m => m.Fields);
                targetTableData.ElementAt(0).Key.ForeignKeys.ElementAt(0).AssertWasCalled(m => m.NameSource);
                targetTableData.ElementAt(0).Key.AssertWasCalled(m => m.NameSource);

                dataValidator.Dispose();
            }
        }

        /// <summary>
        /// Test that Validate throws an DeliveryEngineMetadataException if fields on a foreign key is null.
        /// </summary>
        [Test]
        public void TestThatValidateThrowsDeliveryEngineMetadataExceptionIfFieldsOnForeignKeyIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() => MockRepository.GenerateMock<IField>()));
            fixture.Customize<KeyValuePair<IField, IMap>>(e => e.FromFactory(() => new KeyValuePair<IField, IMap>(fixture.CreateAnonymous<IField>(), null)));
            fixture.Customize<ICandidateKey>(e => e.FromFactory(() =>
                {
                    var candidateKeyMock = MockRepository.GenerateMock<ICandidateKey>();
                    candidateKeyMock.Expect(m => m.NameSource)
                                    .Return(fixture.CreateAnonymous<string>())
                                    .Repeat.Any();
                    candidateKeyMock.Expect(m => m.Fields)
                                    .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(fixture.CreateMany<KeyValuePair<IField, IMap>>(3).ToList())))
                                    .Repeat.Any();
                    return candidateKeyMock;
                }));
            fixture.Customize<IForeignKey>(e => e.FromFactory(() =>
                {
                    var foreignKeyMock = MockRepository.GenerateMock<IForeignKey>();
                    foreignKeyMock.Expect(m => m.NameSource)
                                  .Return(fixture.CreateAnonymous<string>())
                                  .Repeat.Any();
                    foreignKeyMock.Expect(m => m.CandidateKey)
                                  .Return(fixture.CreateAnonymous<ICandidateKey>())
                                  .Repeat.Any();
                    foreignKeyMock.Expect(m => m.Fields)
                                  .Return(null)
                                  .Repeat.Any();
                    return foreignKeyMock;
                }));
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    tableMock.Expect(m => m.ForeignKeys)
                             .Return(new ReadOnlyObservableCollection<IForeignKey>(new ObservableCollection<IForeignKey>(fixture.CreateMany<IForeignKey>(1).ToList())))
                             .Repeat.Any();
                    return tableMock;
                }));
            fixture.Customize<IForeignKeysValidationCommand>(e => e.FromFactory(() => MockRepository.GenerateMock<IForeignKeysValidationCommand>()));

            using (var dataValidator = new ForeignKeysDataValidator(fixture.CreateAnonymous<IDataRepository>()))
            {

                Assert.That(dataValidator, Is.Not.Null);

                var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>(1)
                    {
                        {fixture.CreateAnonymous<ITable>(), new List<IEnumerable<IDataObjectBase>>((0))}
                    };
                Assert.Throws<DeliveryEngineMetadataException>(() => dataValidator.Validate(fixture.CreateAnonymous<ITable>(), targetTableData, true, fixture.CreateAnonymous<IForeignKeysValidationCommand>()));

                targetTableData.ElementAt(0).Key.ForeignKeys.ElementAt(0).CandidateKey.AssertWasCalled(m => m.Fields);
                targetTableData.ElementAt(0).Key.ForeignKeys.ElementAt(0).CandidateKey.AssertWasCalled(m => m.NameSource);
                targetTableData.ElementAt(0).Key.ForeignKeys.ElementAt(0).AssertWasCalled(m => m.Fields);
                targetTableData.ElementAt(0).Key.ForeignKeys.ElementAt(0).AssertWasCalled(m => m.NameSource);
                targetTableData.ElementAt(0).Key.AssertWasCalled(m => m.NameSource);

                dataValidator.Dispose();
            }
        }

        /// <summary>
        /// Test that Validate throws an DeliveryEngineMetadataException if count of fields on a foreign key does not match count of fields on the candidate key is null.
        /// </summary>
        [Test]
        public void TestThatValidateThrowsDeliveryEngineMetadataExceptionIfCountOfFieldsOnForeignKeyDoesNotMatchCountOfFieldOnCandidateKey()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() => MockRepository.GenerateMock<IField>()));
            fixture.Customize<KeyValuePair<IField, IMap>>(e => e.FromFactory(() => new KeyValuePair<IField, IMap>(fixture.CreateAnonymous<IField>(), null)));
            fixture.Customize<ICandidateKey>(e => e.FromFactory(() =>
                {
                    var candidateKeyMock = MockRepository.GenerateMock<ICandidateKey>();
                    candidateKeyMock.Expect(m => m.NameSource)
                                    .Return(fixture.CreateAnonymous<string>())
                                    .Repeat.Any();
                    candidateKeyMock.Expect(m => m.Fields)
                                    .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(fixture.CreateMany<KeyValuePair<IField, IMap>>(3).ToList())))
                                    .Repeat.Any();
                    return candidateKeyMock;
                }));
            fixture.Customize<IForeignKey>(e => e.FromFactory(() =>
                {
                    var foreignKeyMock = MockRepository.GenerateMock<IForeignKey>();
                    foreignKeyMock.Expect(m => m.NameSource)
                                  .Return(fixture.CreateAnonymous<string>())
                                  .Repeat.Any();
                    foreignKeyMock.Expect(m => m.CandidateKey)
                                  .Return(fixture.CreateAnonymous<ICandidateKey>())
                                  .Repeat.Any();
                    foreignKeyMock.Expect(m => m.Fields)
                                  .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(fixture.CreateMany<KeyValuePair<IField, IMap>>(5).ToList())))
                                  .Repeat.Any();
                    return foreignKeyMock;
                }));
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    tableMock.Expect(m => m.ForeignKeys)
                             .Return(new ReadOnlyObservableCollection<IForeignKey>(new ObservableCollection<IForeignKey>(fixture.CreateMany<IForeignKey>(1).ToList())))
                             .Repeat.Any();
                    return tableMock;
                }));
            fixture.Customize<IForeignKeysValidationCommand>(e => e.FromFactory(() => MockRepository.GenerateMock<IForeignKeysValidationCommand>()));

            using (var dataValidator = new ForeignKeysDataValidator(fixture.CreateAnonymous<IDataRepository>()))
            {
                Assert.That(dataValidator, Is.Not.Null);

                var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>(1)
                    {
                        {fixture.CreateAnonymous<ITable>(), new List<IEnumerable<IDataObjectBase>>((0))}
                    };
                Assert.Throws<DeliveryEngineMetadataException>(() => dataValidator.Validate(fixture.CreateAnonymous<ITable>(), targetTableData, true, fixture.CreateAnonymous<IForeignKeysValidationCommand>()));

                targetTableData.ElementAt(0).Key.ForeignKeys.ElementAt(0).CandidateKey.AssertWasCalled(m => m.Fields);
                targetTableData.ElementAt(0).Key.ForeignKeys.ElementAt(0).CandidateKey.AssertWasCalled(m => m.NameSource);
                targetTableData.ElementAt(0).Key.ForeignKeys.ElementAt(0).AssertWasCalled(m => m.Fields);
                targetTableData.ElementAt(0).Key.ForeignKeys.ElementAt(0).AssertWasCalled(m => m.NameSource);
                targetTableData.ElementAt(0).Key.AssertWasCalled(m => m.NameSource);

                dataValidator.Dispose();
            }
        }

        /// <summary>
        /// Test that Validate throws an DeliveryEngineValidateException if the relationship is not found on the foreign key.
        /// </summary>
        [Test]
        public void TestThatValidateThrowsDeliveryEngineValidateExceptionIfRelationshipIsNotFoundOnForeignKey()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    fieldMock.Expect(m => m.DatatypeOfTarget)
                             .Return(typeof (string))
                             .Repeat.Any();
                    fieldMock.Expect(m => m.Nullable)
                             .Return(true)
                             .Repeat.Any();
                    return fieldMock;
                }));

            var foreignDataTableMockFields = fixture.CreateMany<IField>(5).ToList();
            var foreignDataTableMockCandidateKeyFields = new List<KeyValuePair<IField, IMap>>(2)
                {
                    new KeyValuePair<IField, IMap>(foreignDataTableMockFields.ElementAt(0), null),
                    new KeyValuePair<IField, IMap>(foreignDataTableMockFields.ElementAt(1), null)
                };
            var foreignDataTableMock = MockRepository.GenerateMock<ITable>();
            var foreignDataTableMockCandidateKey = MockRepository.GenerateMock<ICandidateKey>();
            foreignDataTableMockCandidateKey.Expect(m => m.NameSource)
                                            .Return(fixture.CreateAnonymous<string>())
                                            .Repeat.Any();
            foreignDataTableMockCandidateKey.Expect(m => m.Fields)
                                            .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(foreignDataTableMockCandidateKeyFields)))
                                            .Repeat.Any();
            foreignDataTableMockCandidateKey.Expect(m => m.Table)
                                            .Return(foreignDataTableMock)
                                            .Repeat.Any();
            foreignDataTableMock.Expect(m => m.NameSource)
                                .Return(fixture.CreateAnonymous<string>())
                                .Repeat.Any();
            foreignDataTableMock.Expect(m => m.NameTarget)
                                .Return(fixture.CreateAnonymous<string>())
                                .Repeat.Any();
            foreignDataTableMock.Expect(m => m.Fields)
                                .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(foreignDataTableMockFields)))
                                .Repeat.Any();
            foreignDataTableMock.Expect(m => m.CandidateKeys)
                                .Return(new ReadOnlyObservableCollection<ICandidateKey>(new ObservableCollection<ICandidateKey>(new List<ICandidateKey> {foreignDataTableMockCandidateKey})))
                                .Repeat.Any();
            var foreignDataTableRows = new List<List<IDataObjectBase>>(5);
            while (foreignDataTableRows.Count < foreignDataTableRows.Capacity)
            {
                var foreignDataTableColumns = new List<IDataObjectBase>(foreignDataTableMock.Fields.Count);
                while (foreignDataTableColumns.Count < foreignDataTableColumns.Capacity)
                {
                    var dataObjectMock = MockRepository.GenerateMock<IDataObjectBase>();
                    dataObjectMock.Expect(m => m.Field)
                                  .Return(foreignDataTableMock.Fields[foreignDataTableColumns.Count])
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.GetSourceValue<string>())
                                  .Return(fixture.CreateAnonymous<string>())
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.GetTargetValue<string>(Arg<IMap>.Is.Anything))
                                  .Return(dataObjectMock.GetSourceValue<string>())
                                  .Repeat.Any();
                    foreignDataTableColumns.Add(dataObjectMock);
                }
                foreignDataTableRows.Add(foreignDataTableColumns);
            }

            var dataTableMockFields = fixture.CreateMany<IField>(10).ToList();
            var dataTableMockPrimaryKeyFields = new List<KeyValuePair<IField, IMap>>(1)
                {
                    new KeyValuePair<IField, IMap>(dataTableMockFields.ElementAt(0), null)
                };
            var dataTableMockPrimaryKey = MockRepository.GenerateMock<ICandidateKey>();
            dataTableMockPrimaryKey.Expect(m => m.Fields)
                                   .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(dataTableMockPrimaryKeyFields)))
                                   .Repeat.Any();
            var dataTableMockForeignKeyFields = new List<KeyValuePair<IField, IMap>>(2)
                {
                    new KeyValuePair<IField, IMap>(dataTableMockFields.ElementAt(0), null),
                    new KeyValuePair<IField, IMap>(dataTableMockFields.ElementAt(1), null)
                };
            var dataTableMockForeignKey = MockRepository.GenerateMock<IForeignKey>();
            dataTableMockForeignKey.Expect(m => m.NameSource)
                                   .Return(fixture.CreateAnonymous<string>())
                                   .Repeat.Any();
            dataTableMockForeignKey.Expect(m => m.CandidateKey)
                                   .Return(foreignDataTableMockCandidateKey)
                                   .Repeat.Any();
            dataTableMockForeignKey.Expect(m => m.Fields)
                                   .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(dataTableMockForeignKeyFields)))
                                   .Repeat.Any();
            var dataTableMock = MockRepository.GenerateMock<ITable>();
            dataTableMock.Expect(m => m.NameSource)
                         .Return(fixture.CreateAnonymous<string>())
                         .Repeat.Any();
            dataTableMock.Expect(m => m.Fields)
                         .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(dataTableMockFields)))
                         .Repeat.Any();
            dataTableMock.Expect(m => m.PrimaryKey)
                         .Return(dataTableMockPrimaryKey)
                         .Repeat.Any();
            dataTableMock.Expect(m => m.ForeignKeys)
                         .Return(new ReadOnlyObservableCollection<IForeignKey>(new ObservableCollection<IForeignKey>(new List<IForeignKey> {dataTableMockForeignKey})))
                         .Repeat.Any();
            var random = new Random(DateTime.Now.Millisecond);
            var dataTableRows = new List<List<IDataObjectBase>>(250);
            while (dataTableRows.Count < dataTableRows.Capacity)
            {
                var r = random.Next(0, foreignDataTableRows.Count - 1);
                var dataTableColumns = new List<IDataObjectBase>(dataTableMock.Fields.Count);
                while (dataTableColumns.Count < dataTableColumns.Capacity)
                {
                    var dataObjectMock = MockRepository.GenerateMock<IDataObjectBase>();
                    dataObjectMock.Expect(m => m.Field)
                                  .Return(dataTableMock.Fields[dataTableColumns.Count])
                                  .Repeat.Any();
                    if (dataTableColumns.Count < dataTableMock.ForeignKeys.ElementAt(0).Fields.Count && dataTableRows.Count < dataTableRows.Capacity - 1)
                    {
                        var sourceValue = foreignDataTableRows[r][dataTableColumns.Count].GetSourceValue<string>();
                        var targetValue = foreignDataTableRows[r][dataTableColumns.Count].GetTargetValue<string>(null);
                        dataObjectMock.Expect(m => m.GetSourceValue<string>())
                                      .Return(sourceValue)
                                      .Repeat.Any();
                        dataObjectMock.Expect(m => m.GetTargetValue<string>(Arg<IMap>.Is.Anything))
                                      .Return(targetValue)
                                      .Repeat.Any();
                        dataTableColumns.Add(dataObjectMock);
                        continue;
                    }
                    dataObjectMock.Expect(m => m.GetSourceValue<string>())
                                  .Return(fixture.CreateAnonymous<string>())
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.GetTargetValue<string>(Arg<IMap>.Is.Anything))
                                  .Return(dataObjectMock.GetSourceValue<string>())
                                  .Repeat.Any();
                    dataTableColumns.Add(dataObjectMock);
                }
                dataTableRows.Add(dataTableColumns);
            }

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.Expect(m => m.DataGetFromTable(Arg<ITable>.Is.Equal(foreignDataTableMock)))
                              .WhenCalled(e =>
                                  {
                                      var eventArgMock = MockRepository.GenerateMock<IHandleDataEventArgs>();
                                      eventArgMock.Expect(m => m.Table)
                                                  .Return(foreignDataTableMock)
                                                  .Repeat.Any();
                                      eventArgMock.Expect(m => m.Data)
                                                  .Return(foreignDataTableRows)
                                                  .Repeat.Any();
                                      dataRepositoryMock.Raise(n => n.OnHandleData += null, this, eventArgMock);
                                  })
                              .Repeat.Any();
            // ReSharper restore ImplicitlyCapturedClosure
            dataRepositoryMock.Expect(m => m.GetDataQueryer())
                              .Throw(new NotSupportedException())
                              .Repeat.Any();
            dataRepositoryMock.Expect(m => m.Clone())
                              .Return(dataRepositoryMock)
                              .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));
            // ReSharper restore ImplicitlyCapturedClosure

            using (var dataValidator = new ForeignKeysDataValidator(fixture.CreateAnonymous<IDataRepository>()))
            {
                Assert.That(dataValidator, Is.Not.Null);

                var foreignKeysValidationCommand = MockRepository.GenerateMock<IForeignKeysValidationCommand>();
                foreignKeysValidationCommand.Expect(m => m.RemoveMissingRelationshipsOnForeignKeys)
                                            .Return(false)
                                            .Repeat.Any();
                foreignKeysValidationCommand.Expect(m => m.NumberOfForeignTablesToCache)
                                            .Return(10)
                                            .Repeat.Any();
                fixture.Customize<IForeignKeysValidationCommand>(e => e.FromFactory(() => foreignKeysValidationCommand));

                var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>(1)
                    {
                        {dataTableMock, dataTableRows}
                    };
                Assert.Throws<DeliveryEngineValidateException>(() => dataValidator.Validate(targetTableData.ElementAt(0).Key, targetTableData, true, fixture.CreateAnonymous<IForeignKeysValidationCommand>()));

                dataRepositoryMock.AssertWasCalled(m => m.GetDataQueryer());
                foreignKeysValidationCommand.AssertWasCalled(m => m.NumberOfForeignTablesToCache);
                dataRepositoryMock.AssertWasCalled(m => m.Clone());
                // ReSharper disable ImplicitlyCapturedClosure
                dataRepositoryMock.AssertWasCalled(m => m.DataGetFromTable(Arg<ITable>.Is.Equal(foreignDataTableMock)));
                // ReSharper restore ImplicitlyCapturedClosure
                foreignKeysValidationCommand.AssertWasCalled(m => m.RemoveMissingRelationshipsOnForeignKeys);
                targetTableData.ElementAt(0).Key.ForeignKeys.ElementAt(0).AssertWasCalled(m => m.ValidateObjectData = Arg<object>.Is.NotNull);
                targetTableData.ElementAt(0).Key.ForeignKeys.ElementAt(0).AssertWasCalled(m => m.NameSource);
                targetTableData.ElementAt(0).Key.AssertWasCalled(m => m.NameSource);

                dataValidator.Dispose();
            }
        }

        /// <summary>
        /// Test that Validate removes missing relationships on foreign keys.
        /// </summary>
        [Test]
        public void TestThatValidateRemoveMissingRelationshipsOnForeignKeys()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    fieldMock.Expect(m => m.DatatypeOfTarget)
                             .Return(typeof (string))
                             .Repeat.Any();
                    fieldMock.Expect(m => m.Nullable)
                             .Return(true)
                             .Repeat.Any();
                    return fieldMock;
                }));

            var foreignDataTableMockFields = fixture.CreateMany<IField>(5).ToList();
            var foreignDataTableMockCandidateKeyFields = new List<KeyValuePair<IField, IMap>>(2)
                {
                    new KeyValuePair<IField, IMap>(foreignDataTableMockFields.ElementAt(0), null),
                    new KeyValuePair<IField, IMap>(foreignDataTableMockFields.ElementAt(1), null)
                };
            var foreignDataTableMock = MockRepository.GenerateMock<ITable>();
            var foreignDataTableMockCandidateKey = MockRepository.GenerateMock<ICandidateKey>();
            foreignDataTableMockCandidateKey.Expect(m => m.NameSource)
                                            .Return(fixture.CreateAnonymous<string>())
                                            .Repeat.Any();
            foreignDataTableMockCandidateKey.Expect(m => m.Fields)
                                            .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(foreignDataTableMockCandidateKeyFields)))
                                            .Repeat.Any();
            foreignDataTableMockCandidateKey.Expect(m => m.Table)
                                            .Return(foreignDataTableMock)
                                            .Repeat.Any();
            foreignDataTableMock.Expect(m => m.NameSource)
                                .Return(fixture.CreateAnonymous<string>())
                                .Repeat.Any();
            foreignDataTableMock.Expect(m => m.NameTarget)
                                .Return(fixture.CreateAnonymous<string>())
                                .Repeat.Any();
            foreignDataTableMock.Expect(m => m.Fields)
                                .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(foreignDataTableMockFields)))
                                .Repeat.Any();
            foreignDataTableMock.Expect(m => m.CandidateKeys)
                                .Return(new ReadOnlyObservableCollection<ICandidateKey>(new ObservableCollection<ICandidateKey>(new List<ICandidateKey> {foreignDataTableMockCandidateKey})))
                                .Repeat.Any();
            var foreignDataTableRows = new List<List<IDataObjectBase>>(5);
            while (foreignDataTableRows.Count < foreignDataTableRows.Capacity)
            {
                var foreignDataTableColumns = new List<IDataObjectBase>(foreignDataTableMock.Fields.Count);
                while (foreignDataTableColumns.Count < foreignDataTableColumns.Capacity)
                {
                    var dataObjectMock = MockRepository.GenerateMock<IDataObjectBase>();
                    dataObjectMock.Expect(m => m.Field)
                                  .Return(foreignDataTableMock.Fields[foreignDataTableColumns.Count])
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.GetSourceValue<string>())
                                  .Return(fixture.CreateAnonymous<string>())
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.GetTargetValue<string>(Arg<IMap>.Is.Anything))
                                  .Return(dataObjectMock.GetSourceValue<string>())
                                  .Repeat.Any();
                    foreignDataTableColumns.Add(dataObjectMock);
                }
                foreignDataTableRows.Add(foreignDataTableColumns);
            }

            var dataTableMockFields = fixture.CreateMany<IField>(10).ToList();
            var dataTableMockForeignKeyFields = new List<KeyValuePair<IField, IMap>>(2)
                {
                    new KeyValuePair<IField, IMap>(dataTableMockFields.ElementAt(0), null),
                    new KeyValuePair<IField, IMap>(dataTableMockFields.ElementAt(1), null)
                };
            var dataTableMockForeignKey = MockRepository.GenerateMock<IForeignKey>();
            dataTableMockForeignKey.Expect(m => m.NameSource)
                                   .Return(fixture.CreateAnonymous<string>())
                                   .Repeat.Any();
            dataTableMockForeignKey.Expect(m => m.CandidateKey)
                                   .Return(foreignDataTableMockCandidateKey)
                                   .Repeat.Any();
            dataTableMockForeignKey.Expect(m => m.Fields)
                                   .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(dataTableMockForeignKeyFields)))
                                   .Repeat.Any();
            var dataTableMock = MockRepository.GenerateMock<ITable>();
            dataTableMock.Expect(m => m.NameSource)
                         .Return(fixture.CreateAnonymous<string>())
                         .Repeat.Any();
            dataTableMock.Expect(m => m.Fields)
                         .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(dataTableMockFields)))
                         .Repeat.Any();
            dataTableMock.Expect(m => m.ForeignKeys)
                         .Return(new ReadOnlyObservableCollection<IForeignKey>(new ObservableCollection<IForeignKey>(new List<IForeignKey> {dataTableMockForeignKey})))
                         .Repeat.Any();
            var random = new Random(DateTime.Now.Millisecond);
            var dataTableRows = new List<List<IDataObjectBase>>(250);
            while (dataTableRows.Count < dataTableRows.Capacity)
            {
                var r = random.Next(0, foreignDataTableRows.Count - 1);
                var dataTableColumns = new List<IDataObjectBase>(dataTableMock.Fields.Count);
                while (dataTableColumns.Count < dataTableColumns.Capacity)
                {
                    var dataObjectMock = MockRepository.GenerateMock<IDataObjectBase>();
                    dataObjectMock.Expect(m => m.Field)
                                  .Return(dataTableMock.Fields[dataTableColumns.Count])
                                  .Repeat.Any();
                    if (dataTableColumns.Count < dataTableMock.ForeignKeys.ElementAt(0).Fields.Count && dataTableRows.Count < dataTableRows.Capacity - 1)
                    {
                        var sourceValue = foreignDataTableRows[r][dataTableColumns.Count].GetSourceValue<string>();
                        var targetValue = foreignDataTableRows[r][dataTableColumns.Count].GetTargetValue<string>(null);
                        dataObjectMock.Expect(m => m.GetSourceValue<string>())
                                      .Return(sourceValue)
                                      .Repeat.Any();
                        dataObjectMock.Expect(m => m.GetTargetValue<string>(Arg<IMap>.Is.Anything))
                                      .Return(targetValue)
                                      .Repeat.Any();
                        dataTableColumns.Add(dataObjectMock);
                        continue;
                    }
                    dataObjectMock.Expect(m => m.GetSourceValue<string>())
                                  .Return(fixture.CreateAnonymous<string>())
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.GetTargetValue<string>(Arg<IMap>.Is.Anything))
                                  .Return(dataObjectMock.GetSourceValue<string>())
                                  .Repeat.Any();
                    dataTableColumns.Add(dataObjectMock);
                }
                dataTableRows.Add(dataTableColumns);
            }

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.Expect(m => m.DataGetFromTable(Arg<ITable>.Is.Equal(foreignDataTableMock)))
                              .WhenCalled(e =>
                                  {
                                      var eventArgMock = MockRepository.GenerateMock<IHandleDataEventArgs>();
                                      eventArgMock.Expect(m => m.Table)
                                                  .Return(foreignDataTableMock)
                                                  .Repeat.Any();
                                      eventArgMock.Expect(m => m.Data)
                                                  .Return(foreignDataTableRows)
                                                  .Repeat.Any();
                                      dataRepositoryMock.Raise(n => n.OnHandleData += null, this, eventArgMock);
                                  })
                              .Repeat.Any();
            // ReSharper restore ImplicitlyCapturedClosure
            dataRepositoryMock.Expect(m => m.GetDataQueryer())
                              .Throw(new NotSupportedException())
                              .Repeat.Any();
            dataRepositoryMock.Expect(m => m.Clone())
                              .Return(dataRepositoryMock)
                              .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));
            // ReSharper restore ImplicitlyCapturedClosure

            using (var dataValidator = new ForeignKeysDataValidator(fixture.CreateAnonymous<IDataRepository>()))
            {
                Assert.That(dataValidator, Is.Not.Null);

                var foreignKeysValidationCommand = MockRepository.GenerateMock<IForeignKeysValidationCommand>();
                foreignKeysValidationCommand.Expect(m => m.RemoveMissingRelationshipsOnForeignKeys)
                                            .Return(true)
                                            .Repeat.Any();
                foreignKeysValidationCommand.Expect(m => m.NumberOfForeignTablesToCache)
                                            .Return(10)
                                            .Repeat.Any();
                fixture.Customize<IForeignKeysValidationCommand>(e => e.FromFactory(() => foreignKeysValidationCommand));

                var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>(1)
                    {
                        {dataTableMock, dataTableRows}
                    };
                var countBeforeValidation = targetTableData.ElementAt(0).Value.Count();
                dataValidator.Validate(targetTableData.ElementAt(0).Key, targetTableData, true, fixture.CreateAnonymous<IForeignKeysValidationCommand>());
                Assert.That(targetTableData.ElementAt(0).Value.Count(), Is.EqualTo(countBeforeValidation - 1));

                dataRepositoryMock.AssertWasCalled(m => m.GetDataQueryer());
                foreignKeysValidationCommand.AssertWasCalled(m => m.NumberOfForeignTablesToCache);
                dataRepositoryMock.AssertWasCalled(m => m.Clone());
                // ReSharper disable ImplicitlyCapturedClosure
                dataRepositoryMock.AssertWasCalled(m => m.DataGetFromTable(Arg<ITable>.Is.Equal(foreignDataTableMock)));
                // ReSharper restore ImplicitlyCapturedClosure
                foreignKeysValidationCommand.AssertWasCalled(m => m.RemoveMissingRelationshipsOnForeignKeys);

                dataValidator.Dispose();
            }
        }

        /// <summary>
        /// Test that Validate throws an DeliveryEngineValidateException if there are to many relationships on the foreign key.
        /// </summary>
        [Test]
        public void TestThatValidateThrowsDeliveryEngineValidateExceptionIfThereAreTooManyRelationshipsIsOnForeignKey()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    fieldMock.Expect(m => m.DatatypeOfTarget)
                             .Return(typeof (string))
                             .Repeat.Any();
                    fieldMock.Expect(m => m.Nullable)
                             .Return(true)
                             .Repeat.Any();
                    return fieldMock;
                }));

            var foreignDataTableMockFields = fixture.CreateMany<IField>(5).ToList();
            var foreignDataTableMockCandidateKeyFields = new List<KeyValuePair<IField, IMap>>(2)
                {
                    new KeyValuePair<IField, IMap>(foreignDataTableMockFields.ElementAt(0), null),
                    new KeyValuePair<IField, IMap>(foreignDataTableMockFields.ElementAt(1), null)
                };
            var foreignDataTableMock = MockRepository.GenerateMock<ITable>();
            var foreignDataTableMockCandidateKey = MockRepository.GenerateMock<ICandidateKey>();
            foreignDataTableMockCandidateKey.Expect(m => m.NameSource)
                                            .Return(fixture.CreateAnonymous<string>())
                                            .Repeat.Any();
            foreignDataTableMockCandidateKey.Expect(m => m.Fields)
                                            .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(foreignDataTableMockCandidateKeyFields)))
                                            .Repeat.Any();
            foreignDataTableMockCandidateKey.Expect(m => m.Table)
                                            .Return(foreignDataTableMock)
                                            .Repeat.Any();
            foreignDataTableMock.Expect(m => m.NameSource)
                                .Return(fixture.CreateAnonymous<string>())
                                .Repeat.Any();
            foreignDataTableMock.Expect(m => m.NameTarget)
                                .Return(fixture.CreateAnonymous<string>())
                                .Repeat.Any();
            foreignDataTableMock.Expect(m => m.Fields)
                                .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(foreignDataTableMockFields)))
                                .Repeat.Any();
            foreignDataTableMock.Expect(m => m.CandidateKeys)
                                .Return(new ReadOnlyObservableCollection<ICandidateKey>(new ObservableCollection<ICandidateKey>(new List<ICandidateKey> {foreignDataTableMockCandidateKey})))
                                .Repeat.Any();
            var foreignDataTableRows = new List<List<IDataObjectBase>>(6);
            while (foreignDataTableRows.Count < foreignDataTableRows.Capacity)
            {
                var foreignDataTableColumns = new List<IDataObjectBase>(foreignDataTableMock.Fields.Count);
                while (foreignDataTableColumns.Count < foreignDataTableColumns.Capacity)
                {
                    var dataObjectMock = MockRepository.GenerateMock<IDataObjectBase>();
                    dataObjectMock.Expect(m => m.Field)
                                  .Return(foreignDataTableMock.Fields[foreignDataTableColumns.Count])
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.GetSourceValue<string>())
                                  .Return(fixture.CreateAnonymous<string>())
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.GetTargetValue<string>(Arg<IMap>.Is.Anything))
                                  .Return(dataObjectMock.GetSourceValue<string>())
                                  .Repeat.Any();
                    foreignDataTableColumns.Add(dataObjectMock);
                }
                for (var i = 0; i < 2; i++)
                {
                    foreignDataTableRows.Add(foreignDataTableColumns);
                }
            }

            var dataTableMockFields = fixture.CreateMany<IField>(10).ToList();
            var dataTableMockPrimaryKeyFields = new List<KeyValuePair<IField, IMap>>(1)
                {
                    new KeyValuePair<IField, IMap>(dataTableMockFields.ElementAt(0), null)
                };
            var dataTableMockPrimaryKey = MockRepository.GenerateMock<ICandidateKey>();
            dataTableMockPrimaryKey.Expect(m => m.Fields)
                                   .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(dataTableMockPrimaryKeyFields)))
                                   .Repeat.Any();
            var dataTableMockForeignKeyFields = new List<KeyValuePair<IField, IMap>>(2)
                {
                    new KeyValuePair<IField, IMap>(dataTableMockFields.ElementAt(0), null),
                    new KeyValuePair<IField, IMap>(dataTableMockFields.ElementAt(1), null)
                };
            var dataTableMockForeignKey = MockRepository.GenerateMock<IForeignKey>();
            dataTableMockForeignKey.Expect(m => m.NameSource)
                                   .Return(fixture.CreateAnonymous<string>())
                                   .Repeat.Any();
            dataTableMockForeignKey.Expect(m => m.CandidateKey)
                                   .Return(foreignDataTableMockCandidateKey)
                                   .Repeat.Any();
            dataTableMockForeignKey.Expect(m => m.Fields)
                                   .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(dataTableMockForeignKeyFields)))
                                   .Repeat.Any();
            var dataTableMock = MockRepository.GenerateMock<ITable>();
            dataTableMock.Expect(m => m.NameSource)
                         .Return(fixture.CreateAnonymous<string>())
                         .Repeat.Any();
            dataTableMock.Expect(m => m.Fields)
                         .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(dataTableMockFields)))
                         .Repeat.Any();
            dataTableMock.Expect(m => m.PrimaryKey)
                         .Return(dataTableMockPrimaryKey)
                         .Repeat.Any();
            dataTableMock.Expect(m => m.ForeignKeys)
                         .Return(new ReadOnlyObservableCollection<IForeignKey>(new ObservableCollection<IForeignKey>(new List<IForeignKey> {dataTableMockForeignKey})))
                         .Repeat.Any();
            var random = new Random(DateTime.Now.Millisecond);
            var dataTableRows = new List<List<IDataObjectBase>>(250);
            while (dataTableRows.Count < dataTableRows.Capacity)
            {
                var r = random.Next(0, foreignDataTableRows.Count - 1);
                var dataTableColumns = new List<IDataObjectBase>(dataTableMock.Fields.Count);
                while (dataTableColumns.Count < dataTableColumns.Capacity)
                {
                    var dataObjectMock = MockRepository.GenerateMock<IDataObjectBase>();
                    dataObjectMock.Expect(m => m.Field)
                                  .Return(dataTableMock.Fields[dataTableColumns.Count])
                                  .Repeat.Any();
                    if (dataTableColumns.Count < dataTableMock.ForeignKeys.ElementAt(0).Fields.Count)
                    {
                        var sourceValue = foreignDataTableRows[r][dataTableColumns.Count].GetSourceValue<string>();
                        var targetValue = foreignDataTableRows[r][dataTableColumns.Count].GetTargetValue<string>(null);
                        dataObjectMock.Expect(m => m.GetSourceValue<string>())
                                      .Return(sourceValue)
                                      .Repeat.Any();
                        dataObjectMock.Expect(m => m.GetTargetValue<string>(Arg<IMap>.Is.Anything))
                                      .Return(targetValue)
                                      .Repeat.Any();
                        dataTableColumns.Add(dataObjectMock);
                        continue;
                    }
                    dataObjectMock.Expect(m => m.GetSourceValue<string>())
                                  .Return(fixture.CreateAnonymous<string>())
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.GetTargetValue<string>(Arg<IMap>.Is.Anything))
                                  .Return(dataObjectMock.GetSourceValue<string>())
                                  .Repeat.Any();
                    dataTableColumns.Add(dataObjectMock);
                }
                dataTableRows.Add(dataTableColumns);
            }

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.Expect(m => m.DataGetFromTable(Arg<ITable>.Is.Equal(foreignDataTableMock)))
                              .WhenCalled(e =>
                                  {
                                      var eventArgMock = MockRepository.GenerateMock<IHandleDataEventArgs>();
                                      eventArgMock.Expect(m => m.Table)
                                                  .Return(foreignDataTableMock)
                                                  .Repeat.Any();
                                      eventArgMock.Expect(m => m.Data)
                                                  .Return(foreignDataTableRows)
                                                  .Repeat.Any();
                                      dataRepositoryMock.Raise(n => n.OnHandleData += null, this, eventArgMock);
                                  })
                              .Repeat.Any();
            // ReSharper restore ImplicitlyCapturedClosure
            dataRepositoryMock.Expect(m => m.GetDataQueryer())
                              .Throw(new NotSupportedException())
                              .Repeat.Any();
            dataRepositoryMock.Expect(m => m.Clone())
                              .Return(dataRepositoryMock)
                              .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));
            // ReSharper restore ImplicitlyCapturedClosure

            using (var dataValidator = new ForeignKeysDataValidator(fixture.CreateAnonymous<IDataRepository>()))
            {
                Assert.That(dataValidator, Is.Not.Null);

                var foreignKeysValidationCommand = MockRepository.GenerateMock<IForeignKeysValidationCommand>();
                foreignKeysValidationCommand.Expect(m => m.NumberOfForeignTablesToCache)
                                            .Return(10)
                                            .Repeat.Any();
                fixture.Customize<IForeignKeysValidationCommand>(e => e.FromFactory(() => foreignKeysValidationCommand));

                var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>(1)
                    {
                        {dataTableMock, dataTableRows}
                    };
                Assert.Throws<DeliveryEngineValidateException>(() => dataValidator.Validate(targetTableData.ElementAt(0).Key, targetTableData, true, fixture.CreateAnonymous<IForeignKeysValidationCommand>()));

                dataRepositoryMock.AssertWasCalled(m => m.GetDataQueryer());
                foreignKeysValidationCommand.AssertWasCalled(m => m.NumberOfForeignTablesToCache);
                dataRepositoryMock.AssertWasCalled(m => m.Clone());
                // ReSharper disable ImplicitlyCapturedClosure
                dataRepositoryMock.AssertWasCalled(m => m.DataGetFromTable(Arg<ITable>.Is.Equal(foreignDataTableMock)));
                // ReSharper restore ImplicitlyCapturedClosure
                targetTableData.ElementAt(0).Key.ForeignKeys.ElementAt(0).AssertWasCalled(m => m.ValidateObjectData = Arg<object>.Is.NotNull);
                targetTableData.ElementAt(0).Key.ForeignKeys.ElementAt(0).AssertWasCalled(m => m.NameSource);
                targetTableData.ElementAt(0).Key.AssertWasCalled(m => m.NameSource);

                dataValidator.Dispose();
            }
        }

        /// <summary>
        /// Test that Validate validates foreign keys.
        /// </summary>
        [Test]
        public void TestThatValidateValidatesForeignKeys()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    fieldMock.Expect(m => m.DatatypeOfTarget)
                             .Return(typeof (string))
                             .Repeat.Any();
                    fieldMock.Expect(m => m.Nullable)
                             .Return(true)
                             .Repeat.Any();
                    return fieldMock;
                }));

            var foreignDataTableMockFields = fixture.CreateMany<IField>(5).ToList();
            var foreignDataTableMockCandidateKeyFields = new List<KeyValuePair<IField, IMap>>(2)
                {
                    new KeyValuePair<IField, IMap>(foreignDataTableMockFields.ElementAt(0), null),
                    new KeyValuePair<IField, IMap>(foreignDataTableMockFields.ElementAt(1), null)
                };
            var foreignDataTableMock = MockRepository.GenerateMock<ITable>();
            var foreignDataTableMockCandidateKey = MockRepository.GenerateMock<ICandidateKey>();
            foreignDataTableMockCandidateKey.Expect(m => m.NameSource)
                                            .Return(fixture.CreateAnonymous<string>())
                                            .Repeat.Any();
            foreignDataTableMockCandidateKey.Expect(m => m.Fields)
                                            .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(foreignDataTableMockCandidateKeyFields)))
                                            .Repeat.Any();
            foreignDataTableMockCandidateKey.Expect(m => m.Table)
                                            .Return(foreignDataTableMock)
                                            .Repeat.Any();
            foreignDataTableMock.Expect(m => m.NameSource)
                                .Return(fixture.CreateAnonymous<string>())
                                .Repeat.Any();
            foreignDataTableMock.Expect(m => m.NameTarget)
                                .Return(fixture.CreateAnonymous<string>())
                                .Repeat.Any();
            foreignDataTableMock.Expect(m => m.Fields)
                                .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(foreignDataTableMockFields)))
                                .Repeat.Any();
            foreignDataTableMock.Expect(m => m.CandidateKeys)
                                .Return(new ReadOnlyObservableCollection<ICandidateKey>(new ObservableCollection<ICandidateKey>(new List<ICandidateKey> {foreignDataTableMockCandidateKey})))
                                .Repeat.Any();
            var foreignDataTableRows = new List<List<IDataObjectBase>>(5);
            while (foreignDataTableRows.Count < foreignDataTableRows.Capacity)
            {
                var foreignDataTableColumns = new List<IDataObjectBase>(foreignDataTableMock.Fields.Count);
                while (foreignDataTableColumns.Count < foreignDataTableColumns.Capacity)
                {
                    var dataObjectMock = MockRepository.GenerateMock<IDataObjectBase>();
                    dataObjectMock.Expect(m => m.Field)
                                  .Return(foreignDataTableMock.Fields[foreignDataTableColumns.Count])
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.GetSourceValue<string>())
                                  .Return(fixture.CreateAnonymous<string>())
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.GetTargetValue<string>(Arg<IMap>.Is.Anything))
                                  .Return(dataObjectMock.GetSourceValue<string>())
                                  .Repeat.Any();
                    foreignDataTableColumns.Add(dataObjectMock);
                }
                foreignDataTableRows.Add(foreignDataTableColumns);
            }

            var dataTableMockFields = fixture.CreateMany<IField>(10).ToList();
            var dataTableMockForeignKeyFields = new List<KeyValuePair<IField, IMap>>(2)
                {
                    new KeyValuePair<IField, IMap>(dataTableMockFields.ElementAt(0), null),
                    new KeyValuePair<IField, IMap>(dataTableMockFields.ElementAt(1), null)
                };
            var dataTableMockForeignKey = MockRepository.GenerateMock<IForeignKey>();
            dataTableMockForeignKey.Expect(m => m.NameSource)
                                   .Return(fixture.CreateAnonymous<string>())
                                   .Repeat.Any();
            dataTableMockForeignKey.Expect(m => m.CandidateKey)
                                   .Return(foreignDataTableMockCandidateKey)
                                   .Repeat.Any();
            dataTableMockForeignKey.Expect(m => m.Fields)
                                   .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(dataTableMockForeignKeyFields)))
                                   .Repeat.Any();
            var dataTableMock = MockRepository.GenerateMock<ITable>();
            dataTableMock.Expect(m => m.NameSource)
                         .Return(fixture.CreateAnonymous<string>())
                         .Repeat.Any();
            dataTableMock.Expect(m => m.Fields)
                         .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(dataTableMockFields)))
                         .Repeat.Any();
            dataTableMock.Expect(m => m.ForeignKeys)
                         .Return(new ReadOnlyObservableCollection<IForeignKey>(new ObservableCollection<IForeignKey>(new List<IForeignKey> {dataTableMockForeignKey})))
                         .Repeat.Any();
            var random = new Random(DateTime.Now.Millisecond);
            var dataTableRows = new List<List<IDataObjectBase>>(250);
            while (dataTableRows.Count < dataTableRows.Capacity)
            {
                var r = random.Next(0, foreignDataTableRows.Count - 1);
                var dataTableColumns = new List<IDataObjectBase>(dataTableMock.Fields.Count);
                while (dataTableColumns.Count < dataTableColumns.Capacity)
                {
                    var dataObjectMock = MockRepository.GenerateMock<IDataObjectBase>();
                    dataObjectMock.Expect(m => m.Field)
                                  .Return(dataTableMock.Fields[dataTableColumns.Count])
                                  .Repeat.Any();
                    if (dataTableColumns.Count < dataTableMock.ForeignKeys.ElementAt(0).Fields.Count)
                    {
                        var sourceValue = foreignDataTableRows[r][dataTableColumns.Count].GetSourceValue<string>();
                        var targetValue = foreignDataTableRows[r][dataTableColumns.Count].GetTargetValue<string>(null);
                        dataObjectMock.Expect(m => m.GetSourceValue<string>())
                                      .Return(sourceValue)
                                      .Repeat.Any();
                        dataObjectMock.Expect(m => m.GetTargetValue<string>(Arg<IMap>.Is.Anything))
                                      .Return(targetValue)
                                      .Repeat.Any();
                        dataTableColumns.Add(dataObjectMock);
                        continue;
                    }
                    dataObjectMock.Expect(m => m.GetSourceValue<string>())
                                  .Return(fixture.CreateAnonymous<string>())
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.GetTargetValue<string>(Arg<IMap>.Is.Anything))
                                  .Return(dataObjectMock.GetSourceValue<string>())
                                  .Repeat.Any();
                    dataTableColumns.Add(dataObjectMock);
                }
                dataTableRows.Add(dataTableColumns);
            }

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.Expect(m => m.DataGetFromTable(Arg<ITable>.Is.Equal(foreignDataTableMock)))
                              .WhenCalled(e =>
                                  {
                                      var eventArgMock = MockRepository.GenerateMock<IHandleDataEventArgs>();
                                      eventArgMock.Expect(m => m.Table)
                                                  .Return(foreignDataTableMock)
                                                  .Repeat.Any();
                                      eventArgMock.Expect(m => m.Data)
                                                  .Return(foreignDataTableRows)
                                                  .Repeat.Any();
                                      dataRepositoryMock.Raise(n => n.OnHandleData += null, this, eventArgMock);
                                  })
                              .Repeat.Any();
            // ReSharper restore ImplicitlyCapturedClosure
            dataRepositoryMock.Expect(m => m.GetDataQueryer())
                              .Throw(new NotSupportedException())
                              .Repeat.Any();
            dataRepositoryMock.Expect(m => m.Clone())
                              .Return(dataRepositoryMock)
                              .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));
            // ReSharper restore ImplicitlyCapturedClosure

            using (var dataValidator = new ForeignKeysDataValidator(fixture.CreateAnonymous<IDataRepository>()))
            {
                Assert.That(dataValidator, Is.Not.Null);

                var foreignKeysValidationCommand = MockRepository.GenerateMock<IForeignKeysValidationCommand>();
                foreignKeysValidationCommand.Expect(m => m.NumberOfForeignTablesToCache)
                                            .Return(10)
                                            .Repeat.Any();
                fixture.Customize<IForeignKeysValidationCommand>(e => e.FromFactory(() => foreignKeysValidationCommand));

                var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>(1)
                    {
                        {dataTableMock, dataTableRows}
                    };
                dataValidator.Validate(targetTableData.ElementAt(0).Key, targetTableData, true, fixture.CreateAnonymous<IForeignKeysValidationCommand>());

                dataRepositoryMock.AssertWasCalled(m => m.GetDataQueryer());
                foreignKeysValidationCommand.AssertWasCalled(m => m.NumberOfForeignTablesToCache);
                dataRepositoryMock.AssertWasCalled(m => m.Clone());
                // ReSharper disable ImplicitlyCapturedClosure
                dataRepositoryMock.AssertWasCalled(m => m.DataGetFromTable(Arg<ITable>.Is.Equal(foreignDataTableMock)));
                // ReSharper restore ImplicitlyCapturedClosure

                dataValidator.Dispose();
            }
        }

        /// <summary>
        /// Test that Validate validates foreign keys when using a data queyer.
        /// </summary>
        [Test]
        public void TestThatValidateUsingDataQueryerValidatesForeignKeys()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof(object)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    fieldMock.Expect(m => m.DatatypeOfSource)
                             .Return(typeof (string))
                             .Repeat.Any();
                    fieldMock.Expect(m => m.DatatypeOfTarget)
                             .Return(typeof (string))
                             .Repeat.Any();
                    return fieldMock;
                }));
            fixture.Customize<IDataQueryer>(e => e.FromFactory(() =>
                {
                    var dataQueyerMock = MockRepository.GenerateMock<IDataQueryer>();
                    dataQueyerMock.Expect(m => m.GetNumberOfEqualKeyValues(Arg<IKey>.Is.NotNull, Arg<IEnumerable<KeyValuePair<string, object>>>.Is.NotNull, Arg<string>.Is.NotNull))
                                  .Return(1)
                                  .Repeat.Any();
                    return dataQueyerMock;
                }));

            //var foreignTableMock = MockRepository.GenerateMock<ITable>();
            var foreignTableFieldCollection = fixture.CreateMany<IField>(5).ToList();
            var foreignTableMock = MockRepository.GenerateMock<ITable>();
            foreignTableMock.Expect(m => m.Fields)
                            .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(foreignTableFieldCollection)))
                            .Repeat.Any();
            var foreignTableCandidateKey = MockRepository.GenerateMock<ICandidateKey>();
            foreignTableCandidateKey.Expect(m => m.Table)
                                    .Return(foreignTableMock)
                                    .Repeat.Any();
            foreignTableCandidateKey.Expect(m => m.Fields)
                                    .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(foreignTableFieldCollection.GetRange(0, 3).Select(field => new KeyValuePair<IField, IMap>(field, null)))))
                                    .Repeat.Any();

            var tableMock = MockRepository.GenerateMock<ITable>();
            var fieldCollection = fixture.CreateMany<IField>(5).ToList();
            var foreignKey = MockRepository.GenerateMock<IForeignKey>();
            foreignKey.Expect(m => m.Table)
                      .Return(tableMock)
                      .Repeat.Any();
            foreignKey.Expect(m => m.Fields)
                      .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(fieldCollection.GetRange(0, 3).Select(field => new KeyValuePair<IField, IMap>(field, null)))))
                      .Repeat.Any();
            foreignKey.Expect(m => m.CandidateKey)
                      .Return(foreignTableCandidateKey)
                      .Repeat.Any();
            tableMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            tableMock.Expect(m => m.Fields)
                     .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(fieldCollection)))
                     .Repeat.Any();
            tableMock.Expect(m => m.ForeignKeys)
                     .Return(new ReadOnlyObservableCollection<IForeignKey>(new ObservableCollection<IForeignKey>(new List<IForeignKey> {foreignKey})))
                     .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<ITable>(e => e.FromFactory(() => tableMock));
            // ReSharper restore ImplicitlyCapturedClosure

            fixture.Customize<IEnumerable<IDataObjectBase>>(e => e.FromFactory(() =>
                {
                    var dataObjectCollection = new List<IDataObjectBase>(tableMock.Fields.Count);
                    for (var i = 0; i < tableMock.Fields.Count; i++)
                    {
                        var dataObjectMock = MockRepository.GenerateMock<IDataObjectBase>();
                        dataObjectMock.Expect(m => m.Field)
                                      .Return(tableMock.Fields.ElementAt(i))
                                      .Repeat.Any();
                        dataObjectMock.Expect(m => m.GetSourceValue<string>())
                                      .Return(fixture.CreateAnonymous<string>())
                                      .Repeat.Any();
                        dataObjectMock.Expect(m => m.GetTargetValue<string>(Arg<IMap>.Is.Anything))
                                      .Return(dataObjectMock.GetSourceValue<string>())
                                      .Repeat.Any();
                        dataObjectCollection.Add(dataObjectMock);
                    }
                    return dataObjectCollection;
                }));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            dataRepositoryMock.Expect(m => m.GetDataQueryer())
                              .Return(fixture.CreateAnonymous<IDataQueryer>())
                              .Repeat.Any();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));

            using (var dataValidator = new ForeignKeysDataValidator(fixture.CreateAnonymous<IDataRepository>()))
            {
                Assert.That(dataValidator, Is.Not.Null);

                var eventCalled = 0;
                dataValidator.OnValidation += (s, e) =>
                    {
                        Assert.That(s, Is.Not.Null);
                        Assert.That(e, Is.Not.Null);
                        Assert.That(e.Data, Is.Not.Null);
                        eventCalled++;
                    };

                var foreignKeysValidationCommand = MockRepository.GenerateMock<IForeignKeysValidationCommand>();
                foreignKeysValidationCommand.Expect(m => m.NumberOfForeignTablesToCache)
                                            .Return(10)
                                            .Repeat.Any();
                fixture.Customize<IForeignKeysValidationCommand>(e => e.FromFactory(() => foreignKeysValidationCommand));

                var targetTable = fixture.CreateAnonymous<ITable>();
                var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>
                    {
                        {targetTable, fixture.CreateMany<IEnumerable<IDataObjectBase>>(250).ToList()}
                    };
                Assert.That(eventCalled, Is.EqualTo(0));
                dataValidator.Validate(targetTable, targetTableData, true, fixture.CreateAnonymous<IForeignKeysValidationCommand>());
                Assert.That(eventCalled, Is.EqualTo(250));

                foreignKeysValidationCommand.AssertWasCalled(m => m.NumberOfForeignTablesToCache);

                dataValidator.Dispose();
            }

            dataRepositoryMock.AssertWasCalled(m => m.GetDataQueryer());
            dataRepositoryMock.GetDataQueryer().AssertWasCalled(m => m.GetNumberOfEqualKeyValues(Arg<IKey>.Is.Equal(foreignKey.CandidateKey), Arg<IEnumerable<KeyValuePair<string, object>>>.Is.NotNull, Arg<string>.Is.NotNull), opt => opt.Repeat.Times(250));
            dataRepositoryMock.GetDataQueryer().AssertWasCalled(m => m.Dispose());
        }

        /// <summary>
        /// Test that Validate raise OnValidation for each foreign key validation.
        /// </summary>
        [Test]
        public void TestThatValidateRaiseOnValidationForEachForeignKeyValidation()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    fieldMock.Expect(m => m.DatatypeOfTarget)
                             .Return(typeof (string))
                             .Repeat.Any();
                    fieldMock.Expect(m => m.Nullable)
                             .Return(true)
                             .Repeat.Any();
                    return fieldMock;
                }));

            var foreignDataTableMockFields = fixture.CreateMany<IField>(5).ToList();
            var foreignDataTableMockCandidateKeyFields = new List<KeyValuePair<IField, IMap>>(2)
                {
                    new KeyValuePair<IField, IMap>(foreignDataTableMockFields.ElementAt(0), null),
                    new KeyValuePair<IField, IMap>(foreignDataTableMockFields.ElementAt(1), null)
                };
            var foreignDataTableMock = MockRepository.GenerateMock<ITable>();
            var foreignDataTableMockCandidateKey = MockRepository.GenerateMock<ICandidateKey>();
            foreignDataTableMockCandidateKey.Expect(m => m.NameSource)
                                            .Return(fixture.CreateAnonymous<string>())
                                            .Repeat.Any();
            foreignDataTableMockCandidateKey.Expect(m => m.Fields)
                                            .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(foreignDataTableMockCandidateKeyFields)))
                                            .Repeat.Any();
            foreignDataTableMockCandidateKey.Expect(m => m.Table)
                                            .Return(foreignDataTableMock)
                                            .Repeat.Any();
            foreignDataTableMock.Expect(m => m.NameSource)
                                .Return(fixture.CreateAnonymous<string>())
                                .Repeat.Any();
            foreignDataTableMock.Expect(m => m.NameTarget)
                                .Return(fixture.CreateAnonymous<string>())
                                .Repeat.Any();
            foreignDataTableMock.Expect(m => m.Fields)
                                .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(foreignDataTableMockFields)))
                                .Repeat.Any();
            foreignDataTableMock.Expect(m => m.CandidateKeys)
                                .Return(new ReadOnlyObservableCollection<ICandidateKey>(new ObservableCollection<ICandidateKey>(new List<ICandidateKey> {foreignDataTableMockCandidateKey})))
                                .Repeat.Any();
            var foreignDataTableRows = new List<List<IDataObjectBase>>(5);
            while (foreignDataTableRows.Count < foreignDataTableRows.Capacity)
            {
                var foreignDataTableColumns = new List<IDataObjectBase>(foreignDataTableMock.Fields.Count);
                while (foreignDataTableColumns.Count < foreignDataTableColumns.Capacity)
                {
                    var dataObjectMock = MockRepository.GenerateMock<IDataObjectBase>();
                    dataObjectMock.Expect(m => m.Field)
                                  .Return(foreignDataTableMock.Fields[foreignDataTableColumns.Count])
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.GetSourceValue<string>())
                                  .Return(fixture.CreateAnonymous<string>())
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.GetTargetValue<string>(Arg<IMap>.Is.Anything))
                                  .Return(dataObjectMock.GetSourceValue<string>())
                                  .Repeat.Any();
                    foreignDataTableColumns.Add(dataObjectMock);
                }
                foreignDataTableRows.Add(foreignDataTableColumns);
            }

            var dataTableMockFields = fixture.CreateMany<IField>(10).ToList();
            var dataTableMockForeignKeyFields = new List<KeyValuePair<IField, IMap>>(2)
                {
                    new KeyValuePair<IField, IMap>(dataTableMockFields.ElementAt(0), null),
                    new KeyValuePair<IField, IMap>(dataTableMockFields.ElementAt(1), null)
                };
            var dataTableMockForeignKey = MockRepository.GenerateMock<IForeignKey>();
            dataTableMockForeignKey.Expect(m => m.NameSource)
                                   .Return(fixture.CreateAnonymous<string>())
                                   .Repeat.Any();
            dataTableMockForeignKey.Expect(m => m.CandidateKey)
                                   .Return(foreignDataTableMockCandidateKey)
                                   .Repeat.Any();
            dataTableMockForeignKey.Expect(m => m.Fields)
                                   .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(dataTableMockForeignKeyFields)))
                                   .Repeat.Any();
            var dataTableMock = MockRepository.GenerateMock<ITable>();
            dataTableMock.Expect(m => m.NameSource)
                         .Return(fixture.CreateAnonymous<string>())
                         .Repeat.Any();
            dataTableMock.Expect(m => m.Fields)
                         .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(dataTableMockFields)))
                         .Repeat.Any();
            dataTableMock.Expect(m => m.ForeignKeys)
                         .Return(new ReadOnlyObservableCollection<IForeignKey>(new ObservableCollection<IForeignKey>(new List<IForeignKey> {dataTableMockForeignKey})))
                         .Repeat.Any();
            var random = new Random(DateTime.Now.Millisecond);
            var dataTableRows = new List<List<IDataObjectBase>>(250);
            while (dataTableRows.Count < dataTableRows.Capacity)
            {
                var r = random.Next(0, foreignDataTableRows.Count - 1);
                var dataTableColumns = new List<IDataObjectBase>(dataTableMock.Fields.Count);
                while (dataTableColumns.Count < dataTableColumns.Capacity)
                {
                    var dataObjectMock = MockRepository.GenerateMock<IDataObjectBase>();
                    dataObjectMock.Expect(m => m.Field)
                                  .Return(dataTableMock.Fields[dataTableColumns.Count])
                                  .Repeat.Any();
                    if (dataTableColumns.Count < dataTableMock.ForeignKeys.ElementAt(0).Fields.Count)
                    {
                        var sourceValue = foreignDataTableRows[r][dataTableColumns.Count].GetSourceValue<string>();
                        var targetValue = foreignDataTableRows[r][dataTableColumns.Count].GetTargetValue<string>(null);
                        dataObjectMock.Expect(m => m.GetSourceValue<string>())
                                      .Return(sourceValue)
                                      .Repeat.Any();
                        dataObjectMock.Expect(m => m.GetTargetValue<string>(Arg<IMap>.Is.Anything))
                                      .Return(targetValue)
                                      .Repeat.Any();
                        dataTableColumns.Add(dataObjectMock);
                        continue;
                    }
                    dataObjectMock.Expect(m => m.GetSourceValue<string>())
                                  .Return(fixture.CreateAnonymous<string>())
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.GetTargetValue<string>(Arg<IMap>.Is.Anything))
                                  .Return(dataObjectMock.GetSourceValue<string>())
                                  .Repeat.Any();
                    dataTableColumns.Add(dataObjectMock);
                }
                dataTableRows.Add(dataTableColumns);
            }

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.Expect(m => m.DataGetFromTable(Arg<ITable>.Is.Equal(foreignDataTableMock)))
                              .WhenCalled(e =>
                                  {
                                      var eventArgMock = MockRepository.GenerateMock<IHandleDataEventArgs>();
                                      eventArgMock.Expect(m => m.Table)
                                                  .Return(foreignDataTableMock)
                                                  .Repeat.Any();
                                      eventArgMock.Expect(m => m.Data)
                                                  .Return(foreignDataTableRows)
                                                  .Repeat.Any();
                                      dataRepositoryMock.Raise(n => n.OnHandleData += null, this, eventArgMock);
                                  })
                              .Repeat.Any();
            // ReSharper restore ImplicitlyCapturedClosure
            dataRepositoryMock.Expect(m => m.GetDataQueryer())
                              .Throw(new NotSupportedException())
                              .Repeat.Any();
            dataRepositoryMock.Expect(m => m.Clone())
                              .Return(dataRepositoryMock)
                              .Repeat.Any();
            // ReSharper disable ImplicitlyCapturedClosure
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));
            // ReSharper restore ImplicitlyCapturedClosure

            using (var dataValidator = new ForeignKeysDataValidator(fixture.CreateAnonymous<IDataRepository>()))
            {
                Assert.That(dataValidator, Is.Not.Null);

                var eventCalled = 0;
                dataValidator.OnValidation += (s, e) =>
                    {
                        Assert.That(s, Is.Not.Null);
                        Assert.That(e, Is.Not.Null);
                        Assert.That(e.Data, Is.Not.Null);
                        eventCalled++;
                    };

                var foreignKeysValidationCommand = MockRepository.GenerateMock<IForeignKeysValidationCommand>();
                foreignKeysValidationCommand.Expect(m => m.NumberOfForeignTablesToCache)
                                            .Return(10)
                                            .Repeat.Any();
                fixture.Customize<IForeignKeysValidationCommand>(e => e.FromFactory(() => foreignKeysValidationCommand));

                Assert.That(eventCalled, Is.EqualTo(0));
                var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>(1)
                    {
                        {dataTableMock, dataTableRows}
                    };
                dataValidator.Validate(targetTableData.ElementAt(0).Key, targetTableData, true, fixture.CreateAnonymous<IForeignKeysValidationCommand>());
                Assert.That(eventCalled, Is.EqualTo(dataTableRows.Count));

                dataRepositoryMock.AssertWasCalled(m => m.GetDataQueryer());
                foreignKeysValidationCommand.AssertWasCalled(m => m.NumberOfForeignTablesToCache);
                dataRepositoryMock.AssertWasCalled(m => m.Clone());
                // ReSharper disable ImplicitlyCapturedClosure
                dataRepositoryMock.AssertWasCalled(m => m.DataGetFromTable(Arg<ITable>.Is.Equal(foreignDataTableMock)));
                // ReSharper restore ImplicitlyCapturedClosure

                dataValidator.Dispose();
            }
        }
    }
}
