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
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.BusinessLogic.DataValidators
{
    /// <summary>
    /// Tests validator for primary key on data for a target table.
    /// </summary>
    [TestFixture]
    public class PrimaryKeyDataValidatorTests
    {
        /// <summary>
        /// Tests that the constructor initialize the data validator.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeDataValidator()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));

            var dataRepository = fixture.CreateAnonymous<IDataRepository>();
            using (var dataValidator = new PrimaryKeyDataValidator(dataRepository))
            {
                Assert.That(dataValidator, Is.Not.Null);
                Assert.That(dataValidator.DataRepository, Is.Not.Null);
                Assert.That(dataValidator.DataRepository, Is.EqualTo(dataRepository));
                dataValidator.Dispose();
            }
        }

        /// <summary>
        /// Tests that the constructor throws an ArgumentNullException if the data repository is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfDataRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new PrimaryKeyDataValidator(null));
        }

        /// <summary>
        /// Test that Dispose can be called.
        /// </summary>
        [Test]
        public void TestThatDisposeCanBeCalled()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));

            using (var dataValidator = new PrimaryKeyDataValidator(fixture.CreateAnonymous<IDataRepository>()))
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

            using (var dataValidator = new PrimaryKeyDataValidator(fixture.CreateAnonymous<IDataRepository>()))
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
            fixture.Customize<ICommand>(e => e.FromFactory(() => MockRepository.GenerateMock<ICommand>()));

            using (var dataValidator = new PrimaryKeyDataValidator(fixture.CreateAnonymous<IDataRepository>()))
            {
                Assert.That(dataValidator, Is.Not.Null);
                
                dataValidator.Dispose();

                var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>(1)
                    {
                        {fixture.CreateAnonymous<ITable>(), new List<IEnumerable<IDataObjectBase>>((0))}
                    };
                Assert.Throws<ObjectDisposedException>(() => dataValidator.Validate(fixture.CreateAnonymous<ITable>(), targetTableData, true, fixture.CreateAnonymous<ICommand>()));
            }
        }

        /// <summary>
        /// Test that Validate throws an DeliveryEngineMetadataException if candidate keys on a data table is null.
        /// </summary>
        [Test]
        public void TestThatValidateThrowsDeliveryEngineMetadataExceptionIfCandidateKeysOnDataTableIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    tableMock.Expect(m => m.CandidateKeys)
                             .Return(null)
                             .Repeat.Any();
                    return tableMock;
                }));
            fixture.Customize<ICommand>(e => e.FromFactory(() => MockRepository.GenerateMock<ICommand>()));

            using (var dataValidator = new PrimaryKeyDataValidator(fixture.CreateAnonymous<IDataRepository>()))
            {
                Assert.That(dataValidator, Is.Not.Null);

                var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>(1)
                    {
                        {fixture.CreateAnonymous<ITable>(), new List<IEnumerable<IDataObjectBase>>((0))}
                    };
                Assert.Throws<DeliveryEngineMetadataException>(() => dataValidator.Validate(fixture.CreateAnonymous<ITable>(), targetTableData, true, fixture.CreateAnonymous<ICommand>()));

                targetTableData.Keys.ElementAt(0).AssertWasCalled(m => m.CandidateKeys);
                targetTableData.Keys.ElementAt(0).AssertWasCalled(m => m.NameSource);

                dataValidator.Dispose();
            }
        }

        /// <summary>
        /// Test that Validate throws an DeliveryEngineMetadataException if candidate keys on a data table is missing.
        /// </summary>
        [Test]
        public void TestThatValidateThrowsDeliveryEngineMetadataExceptionIfCandidateKeysOnDataTableIsMissing()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataRepository>()));
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    tableMock.Expect(m => m.CandidateKeys)
                             .Return(new ReadOnlyObservableCollection<ICandidateKey>(new ObservableCollection<ICandidateKey>(new List<ICandidateKey>(0))))
                             .Repeat.Any();
                    return tableMock;
                }));
            fixture.Customize<ICommand>(e => e.FromFactory(() => MockRepository.GenerateMock<ICommand>()));

            using (var dataValidator = new PrimaryKeyDataValidator(fixture.CreateAnonymous<IDataRepository>()))
            {
                Assert.That(dataValidator, Is.Not.Null);

                var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>(1)
                    {
                        {fixture.CreateAnonymous<ITable>(), new List<IEnumerable<IDataObjectBase>>((0))}
                    };
                Assert.Throws<DeliveryEngineMetadataException>(() => dataValidator.Validate(fixture.CreateAnonymous<ITable>(), targetTableData, true, fixture.CreateAnonymous<ICommand>()));

                targetTableData.Keys.ElementAt(0).AssertWasCalled(m => m.CandidateKeys);
                targetTableData.Keys.ElementAt(0).AssertWasCalled(m => m.NameSource);

                dataValidator.Dispose();
            }
        }

        /// <summary>
        /// Test that Validate throws an DeliveryEngineMetadataException if fields on a candidate key is null.
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
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    tableMock.Expect(m => m.CandidateKeys)
                             .Return(new ReadOnlyObservableCollection<ICandidateKey>(new ObservableCollection<ICandidateKey>(fixture.CreateMany<ICandidateKey>(1).ToList())))
                             .Repeat.Any();
                    return tableMock;
                }));
            fixture.Customize<ICommand>(e => e.FromFactory(() => MockRepository.GenerateMock<ICommand>()));

            using (var dataValidator = new PrimaryKeyDataValidator(fixture.CreateAnonymous<IDataRepository>()))
            {
                Assert.That(dataValidator, Is.Not.Null);

                var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>(1)
                    {
                        {fixture.CreateAnonymous<ITable>(), new List<IEnumerable<IDataObjectBase>>((0))}
                    };
                Assert.Throws<DeliveryEngineMetadataException>(() => dataValidator.Validate(fixture.CreateAnonymous<ITable>(), targetTableData, true, fixture.CreateAnonymous<ICommand>()));

                targetTableData.Keys.ElementAt(0).CandidateKeys.ElementAt(0).AssertWasCalled(m => m.Fields);
                targetTableData.Keys.ElementAt(0).CandidateKeys.ElementAt(0).AssertWasCalled(m => m.NameSource);
                targetTableData.Keys.ElementAt(0).AssertWasCalled(m => m.NameSource);

                dataValidator.Dispose();
            }
        }

        /// <summary>
        /// Test that Validate throws an DeliveryEngineMetadataException if fields on a candidate key is missing.
        /// </summary>
        [Test]
        public void TestThatValidateThrowsDeliveryEngineMetadataExceptionIfFieldsOnCandidateKeyIsMissing()
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
                                    .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(new List<KeyValuePair<IField, IMap>>(0))))
                                    .Repeat.Any();
                    return candidateKeyMock;
                }));
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    tableMock.Expect(m => m.CandidateKeys)
                             .Return(new ReadOnlyObservableCollection<ICandidateKey>(new ObservableCollection<ICandidateKey>(fixture.CreateMany<ICandidateKey>(1).ToList())))
                             .Repeat.Any();
                    return tableMock;
                }));
            fixture.Customize<ICommand>(e => e.FromFactory(() => MockRepository.GenerateMock<ICommand>()));

            using (var dataValidator = new PrimaryKeyDataValidator(fixture.CreateAnonymous<IDataRepository>()))
            {
                Assert.That(dataValidator, Is.Not.Null);

                var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>(1)
                    {
                        {fixture.CreateAnonymous<ITable>(), new List<IEnumerable<IDataObjectBase>>((0))}
                    };
                Assert.Throws<DeliveryEngineMetadataException>(() => dataValidator.Validate(fixture.CreateAnonymous<ITable>(), targetTableData, true, fixture.CreateAnonymous<ICommand>()));

                targetTableData.Keys.ElementAt(0).CandidateKeys.ElementAt(0).AssertWasCalled(m => m.Fields);
                targetTableData.Keys.ElementAt(0).CandidateKeys.ElementAt(0).AssertWasCalled(m => m.NameSource);
                targetTableData.Keys.ElementAt(0).AssertWasCalled(m => m.NameSource);

                dataValidator.Dispose();
            }
        }

        /// <summary>
        /// Test that Validate throws an DeliveryEngineValidateException if an unique constraint violation occurs on a candidate key.
        /// </summary>
        [Test]
        public void TestThatValidateThrowsDeliveryEngineValidateExceptionIfAnUniqueConstraintViolationOccursOnCandidateKey()
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
                    return fieldMock;
                }));
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    tableMock.Expect(m => m.Fields)
                             .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(fixture.CreateMany<IField>(10).ToList())))
                             .Repeat.Any();
                    var candidateKeyMockFields = new List<KeyValuePair<IField, IMap>>(3)
                        {
                            new KeyValuePair<IField, IMap>(tableMock.Fields.ElementAt(0), null),
                            new KeyValuePair<IField, IMap>(tableMock.Fields.ElementAt(1), null),
                            new KeyValuePair<IField, IMap>(tableMock.Fields.ElementAt(2), null)
                        };
                    var candidateKeyMock = MockRepository.GenerateMock<ICandidateKey>();
                    candidateKeyMock.Expect(m => m.NameSource)
                                    .Return(fixture.CreateAnonymous<string>())
                                    .Repeat.Any();
                    candidateKeyMock.Expect(m => m.Table)
                                    .Return(tableMock)
                                    .Repeat.Any();
                    candidateKeyMock.Expect(m => m.Fields)
                                    .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(candidateKeyMockFields)))
                                    .Repeat.Any();
                    tableMock.Expect(m => m.CandidateKeys)
                             .Return(new ReadOnlyObservableCollection<ICandidateKey>(new ObservableCollection<ICandidateKey>(new List<ICandidateKey> {candidateKeyMock})))
                             .Repeat.Any();
                    return tableMock;
                }));
            fixture.Customize<ICommand>(e => e.FromFactory(() => MockRepository.GenerateMock<ICommand>()));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            dataRepositoryMock.Expect(m => m.GetDataQueryer())
                              .Throw(new NotSupportedException())
                              .Repeat.Any();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));

            using (var dataValidator = new PrimaryKeyDataValidator(fixture.CreateAnonymous<IDataRepository>()))
            {
                Assert.That(dataValidator, Is.Not.Null);

                var dataTableMock = fixture.CreateAnonymous<ITable>();
                var dataTableRows = new List<List<IDataObjectBase>>(250);
                while (dataTableRows.Count < dataTableRows.Capacity - 1)
                {
                    var dataTableColumns = new List<IDataObjectBase>(dataTableMock.Fields.Count);
                    while (dataTableColumns.Count < dataTableColumns.Capacity)
                    {
                        var dataObjectMock = MockRepository.GenerateMock<IDataObjectBase>();
                        dataObjectMock.Expect(m => m.Field)
                                      .Return(dataTableMock.Fields[dataTableColumns.Count])
                                      .Repeat.Any();
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
                dataTableRows.Add(dataTableRows[dataTableRows.Count - 1]);
                var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>(1)
                    {
                        {dataTableMock, dataTableRows}
                    };
                Assert.Throws<DeliveryEngineValidateException>(() => dataValidator.Validate(fixture.CreateAnonymous<ITable>(), targetTableData, true, fixture.CreateAnonymous<ICommand>()));

                dataRepositoryMock.AssertWasCalled(m => m.GetDataQueryer());

                targetTableData.Keys.ElementAt(0)
                               .CandidateKeys.ElementAt(0)
                               .AssertWasCalled(m => m.ValidateObjectData = Arg<object>.Is.NotNull);
                targetTableData.Keys.ElementAt(0).CandidateKeys.ElementAt(0).AssertWasCalled(m => m.NameSource);

                dataValidator.Dispose();
            }
        }

        /// <summary>
        /// Test that Validate throws an DeliveryEngineValidateException if an unique constraint violation occurs on a candidate key when using a data queryer and end of data is false.
        /// </summary>
        [Test]
        public void TestThatValidateUsingDataQueryerThrowsDeliveryEngineValidateExceptionIfAnUniqueConstraintViolationOccursOnCandidateKeyWhereEndOfDataIsFalse()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<ICommand>(e => e.FromFactory(() => MockRepository.GenerateMock<ICommand>()));
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
                                  .Return(2)
                                  .Repeat.Any();
                    return dataQueyerMock;
                }));

            var tableMock = MockRepository.GenerateMock<ITable>();
            var fieldCollection = fixture.CreateMany<IField>(5).ToList();
            var candidateKey = MockRepository.GenerateMock<ICandidateKey>();
            candidateKey.Expect(m => m.Table)
                        .Return(tableMock)
                        .Repeat.Any();
            candidateKey.Expect(m => m.Fields)
                        .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(fieldCollection.GetRange(0, 3).Select(field => new KeyValuePair<IField, IMap>(field, null)))))
                        .Repeat.Any();
            tableMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            tableMock.Expect(m => m.Fields)
                     .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(fieldCollection)))
                     .Repeat.Any();
            tableMock.Expect(m => m.CandidateKeys)
                     .Return(new ReadOnlyObservableCollection<ICandidateKey>(new ObservableCollection<ICandidateKey>(new List<ICandidateKey> {candidateKey})))
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

            using (var dataValidator = new PrimaryKeyDataValidator(fixture.CreateAnonymous<IDataRepository>()))
            {
                Assert.That(dataValidator, Is.Not.Null);

                var targetTable = fixture.CreateAnonymous<ITable>();
                var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>
                    {
                        {targetTable, fixture.CreateMany<IEnumerable<IDataObjectBase>>(250).ToList()}
                    };
                Assert.Throws<DeliveryEngineValidateException>(() => dataValidator.Validate(targetTable, targetTableData, false, fixture.CreateAnonymous<ICommand>()));

                dataValidator.Dispose();
            }

            dataRepositoryMock.AssertWasCalled(m => m.GetDataQueryer());
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.GetDataQueryer().AssertWasCalled(m => m.GetNumberOfEqualKeyValues(Arg<IKey>.Is.Equal(candidateKey), Arg<IEnumerable<KeyValuePair<string, object>>>.Is.NotNull, Arg<string>.Is.NotNull));
            // ReSharper restore ImplicitlyCapturedClosure
            dataRepositoryMock.GetDataQueryer().AssertWasCalled(m => m.Dispose());
        }

        /// <summary>
        /// Test that Validate throws an DeliveryEngineValidateException if an unique constraint violation occurs on a candidate key when using a data queryer and end of data is true.
        /// </summary>
        [Test]
        public void TestThatValidateUsingDataQueryerThrowsDeliveryEngineValidateExceptionIfAnUniqueConstraintViolationOccursOnCandidateKeyWhereEndOfDataIsTrue()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<ICommand>(e => e.FromFactory(() => MockRepository.GenerateMock<ICommand>()));
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
            fixture.Customize<IDataQueryer>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataQueryer>()));

            var tableMock = MockRepository.GenerateMock<ITable>();
            var fieldCollection = fixture.CreateMany<IField>(5).ToList();
            var candidateKey = MockRepository.GenerateMock<ICandidateKey>();
            candidateKey.Expect(m => m.Table)
                        .Return(tableMock)
                        .Repeat.Any();
            candidateKey.Expect(m => m.Fields)
                        .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(fieldCollection.GetRange(0, 3).Select(field => new KeyValuePair<IField, IMap>(field, null)))))
                        .Repeat.Any();
            tableMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            tableMock.Expect(m => m.Fields)
                     .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(fieldCollection)))
                     .Repeat.Any();
            tableMock.Expect(m => m.CandidateKeys)
                     .Return(new ReadOnlyObservableCollection<ICandidateKey>(new ObservableCollection<ICandidateKey>(new List<ICandidateKey> {candidateKey})))
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

            using (var dataValidator = new PrimaryKeyDataValidator(fixture.CreateAnonymous<IDataRepository>()))
            {
                Assert.That(dataValidator, Is.Not.Null);

                var targetTable = fixture.CreateAnonymous<ITable>();
                var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>
                    {
                        {targetTable, fixture.CreateMany<IEnumerable<IDataObjectBase>>(249).ToList()}
                    };
                ((IList<IEnumerable<IDataObjectBase>>) targetTableData.ElementAt(0).Value).Add(targetTableData.ElementAt(0).Value.ElementAt(targetTableData.ElementAt(0).Value.Count() - 1));
                Assert.Throws<DeliveryEngineValidateException>(() => dataValidator.Validate(targetTable, targetTableData, true, fixture.CreateAnonymous<ICommand>()));

                dataValidator.Dispose();
            }

            dataRepositoryMock.AssertWasCalled(m => m.GetDataQueryer());
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.GetDataQueryer().AssertWasNotCalled(m => m.GetNumberOfEqualKeyValues(Arg<IKey>.Is.Anything, Arg<IEnumerable<KeyValuePair<string, object>>>.Is.Anything, Arg<string>.Is.Anything));
            // ReSharper restore ImplicitlyCapturedClosure
            dataRepositoryMock.GetDataQueryer().AssertWasCalled(m => m.Dispose());
        }

        /// <summary>
        /// Test that Validate validates primary key.
        /// </summary>
        [Test]
        public void TestThatValidateValidatesPrimaryKey()
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
                    return fieldMock;
                }));
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    tableMock.Expect(m => m.Fields)
                             .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(fixture.CreateMany<IField>(10).ToList())))
                             .Repeat.Any();
                    var candidateKeyMockFields = new List<KeyValuePair<IField, IMap>>(3)
                        {
                            new KeyValuePair<IField, IMap>(tableMock.Fields.ElementAt(0), null),
                            new KeyValuePair<IField, IMap>(tableMock.Fields.ElementAt(1), null),
                            new KeyValuePair<IField, IMap>(tableMock.Fields.ElementAt(2), null)
                        };
                    var candidateKeyMock = MockRepository.GenerateMock<ICandidateKey>();
                    candidateKeyMock.Expect(m => m.NameSource)
                                    .Return(fixture.CreateAnonymous<string>())
                                    .Repeat.Any();
                    candidateKeyMock.Expect(m => m.Table)
                                    .Return(tableMock)
                                    .Repeat.Any();
                    candidateKeyMock.Expect(m => m.Fields)
                                    .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(candidateKeyMockFields)))
                                    .Repeat.Any();
                    tableMock.Expect(m => m.CandidateKeys)
                             .Return(new ReadOnlyObservableCollection<ICandidateKey>(new ObservableCollection<ICandidateKey>(new List<ICandidateKey> {candidateKeyMock})))
                             .Repeat.Any();
                    return tableMock;
                }));
            fixture.Customize<ICommand>(e => e.FromFactory(() => MockRepository.GenerateMock<ICommand>()));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            dataRepositoryMock.Expect(m => m.GetDataQueryer())
                              .Throw(new NotSupportedException())
                              .Repeat.Any();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));

            using (var dataValidator = new PrimaryKeyDataValidator(fixture.CreateAnonymous<IDataRepository>()))
            {
                Assert.That(dataValidator, Is.Not.Null);

                var dataTableMock = fixture.CreateAnonymous<ITable>();
                var dataTableRows = new List<List<IDataObjectBase>>(250);
                while (dataTableRows.Count < dataTableRows.Capacity)
                {
                    var dataTableColumns = new List<IDataObjectBase>(dataTableMock.Fields.Count);
                    while (dataTableColumns.Count < dataTableColumns.Capacity)
                    {
                        var dataObjectMock = MockRepository.GenerateMock<IDataObjectBase>();
                        dataObjectMock.Expect(m => m.Field)
                                      .Return(dataTableMock.Fields[dataTableColumns.Count])
                                      .Repeat.Any();
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
                var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>(1)
                    {
                        {dataTableMock, dataTableRows}
                    };
                dataValidator.Validate(fixture.CreateAnonymous<ITable>(), targetTableData, true, fixture.CreateAnonymous<ICommand>());

                dataRepositoryMock.AssertWasCalled(m => m.GetDataQueryer());

                dataValidator.Dispose();
            }
        }

        /// <summary>
        /// Test that Validate validates primary key when using a data queyer and end of data is false.
        /// </summary>
        [Test]
        public void TestThatValidateUsingDataQueryerValidatesPrimaryKeyWhereEndOfDataIsFalse()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<ICommand>(e => e.FromFactory(() => MockRepository.GenerateMock<ICommand>()));
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

            var tableMock = MockRepository.GenerateMock<ITable>();
            var fieldCollection = fixture.CreateMany<IField>(5).ToList();
            var candidateKey = MockRepository.GenerateMock<ICandidateKey>();
            candidateKey.Expect(m => m.Table)
                        .Return(tableMock)
                        .Repeat.Any();
            candidateKey.Expect(m => m.Fields)
                        .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(fieldCollection.GetRange(0, 3).Select(field => new KeyValuePair<IField, IMap>(field, null)))))
                        .Repeat.Any();
            tableMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            tableMock.Expect(m => m.Fields)
                     .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(fieldCollection)))
                     .Repeat.Any();
            tableMock.Expect(m => m.CandidateKeys)
                     .Return(new ReadOnlyObservableCollection<ICandidateKey>(new ObservableCollection<ICandidateKey>(new List<ICandidateKey> {candidateKey})))
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

            using (var dataValidator = new PrimaryKeyDataValidator(fixture.CreateAnonymous<IDataRepository>()))
            {
                Assert.That(dataValidator, Is.Not.Null);

                var targetTable = fixture.CreateAnonymous<ITable>();
                var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>
                    {
                        {targetTable, fixture.CreateMany<IEnumerable<IDataObjectBase>>(250).ToList()}
                    };
                dataValidator.Validate(targetTable, targetTableData, false, fixture.CreateAnonymous<ICommand>());

                dataValidator.Dispose();
            }

            dataRepositoryMock.AssertWasCalled(m => m.GetDataQueryer());
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.GetDataQueryer().AssertWasCalled(m => m.GetNumberOfEqualKeyValues(Arg<IKey>.Is.Equal(candidateKey), Arg<IEnumerable<KeyValuePair<string, object>>>.Is.NotNull, Arg<string>.Is.NotNull), opt => opt.Repeat.Times(250));
            // ReSharper restore ImplicitlyCapturedClosure
            dataRepositoryMock.GetDataQueryer().AssertWasCalled(m => m.Dispose());
        }

        /// <summary>
        /// Test that Validate validates primary key when using a data queyer and end of data is true.
        /// </summary>
        [Test]
        public void TestThatValidateUsingDataQueryerValidatesPrimaryKeyWhereEndOfDataIsTrue()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<ICommand>(e => e.FromFactory(() => MockRepository.GenerateMock<ICommand>()));
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
            fixture.Customize<IDataQueryer>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataQueryer>()));

            var tableMock = MockRepository.GenerateMock<ITable>();
            var fieldCollection = fixture.CreateMany<IField>(5).ToList();
            var candidateKey = MockRepository.GenerateMock<ICandidateKey>();
            candidateKey.Expect(m => m.Table)
                        .Return(tableMock)
                        .Repeat.Any();
            candidateKey.Expect(m => m.Fields)
                        .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(fieldCollection.GetRange(0, 3).Select(field => new KeyValuePair<IField, IMap>(field, null)))))
                        .Repeat.Any();
            tableMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            tableMock.Expect(m => m.Fields)
                     .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(fieldCollection)))
                     .Repeat.Any();
            tableMock.Expect(m => m.CandidateKeys)
                     .Return(new ReadOnlyObservableCollection<ICandidateKey>(new ObservableCollection<ICandidateKey>(new List<ICandidateKey> {candidateKey})))
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

            using (var dataValidator = new PrimaryKeyDataValidator(fixture.CreateAnonymous<IDataRepository>()))
            {
                Assert.That(dataValidator, Is.Not.Null);

                var targetTable = fixture.CreateAnonymous<ITable>();
                var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>
                    {
                        {targetTable, fixture.CreateMany<IEnumerable<IDataObjectBase>>(250).ToList()}
                    };
                dataValidator.Validate(targetTable, targetTableData, true, fixture.CreateAnonymous<ICommand>());

                dataValidator.Dispose();
            }

            dataRepositoryMock.AssertWasCalled(m => m.GetDataQueryer());
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.GetDataQueryer().AssertWasNotCalled(m => m.GetNumberOfEqualKeyValues(Arg<IKey>.Is.Anything, Arg<IEnumerable<KeyValuePair<string, object>>>.Is.Anything, Arg<string>.Is.Anything));
            // ReSharper restore ImplicitlyCapturedClosure
            dataRepositoryMock.GetDataQueryer().AssertWasCalled(m => m.Dispose());
        }

        /// <summary>
        /// Test that Validate raise OnValidation for each primary key validation.
        /// </summary>
        [Test]
        public void TestThatValidateRaiseOnValidationForEachPrimaryKeyValidation()
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
                    return fieldMock;
                }));
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameSource)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    tableMock.Expect(m => m.Fields)
                             .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(fixture.CreateMany<IField>(10).ToList())))
                             .Repeat.Any();
                    var candidateKeyMockFields = new List<KeyValuePair<IField, IMap>>(3)
                        {
                            new KeyValuePair<IField, IMap>(tableMock.Fields.ElementAt(0), null),
                            new KeyValuePair<IField, IMap>(tableMock.Fields.ElementAt(1), null),
                            new KeyValuePair<IField, IMap>(tableMock.Fields.ElementAt(2), null)
                        };
                    var candidateKeyMock = MockRepository.GenerateMock<ICandidateKey>();
                    candidateKeyMock.Expect(m => m.NameSource)
                                    .Return(fixture.CreateAnonymous<string>())
                                    .Repeat.Any();
                    candidateKeyMock.Expect(m => m.Table)
                                    .Return(tableMock)
                                    .Repeat.Any();
                    candidateKeyMock.Expect(m => m.Fields)
                                    .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(candidateKeyMockFields)))
                                    .Repeat.Any();
                    tableMock.Expect(m => m.CandidateKeys)
                             .Return(new ReadOnlyObservableCollection<ICandidateKey>(new ObservableCollection<ICandidateKey>(new List<ICandidateKey> {candidateKeyMock})))
                             .Repeat.Any();
                    return tableMock;
                }));
            fixture.Customize<ICommand>(e => e.FromFactory(() => MockRepository.GenerateMock<ICommand>()));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            dataRepositoryMock.Expect(m => m.GetDataQueryer())
                              .Throw(new NotSupportedException())
                              .Repeat.Any();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));

            using (var dataValidator = new PrimaryKeyDataValidator(fixture.CreateAnonymous<IDataRepository>()))
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

                Assert.That(eventCalled, Is.EqualTo(0));
                var dataTableMock = fixture.CreateAnonymous<ITable>();
                var dataTableRows = new List<List<IDataObjectBase>>(250);
                while (dataTableRows.Count < dataTableRows.Capacity)
                {
                    var dataTableColumns = new List<IDataObjectBase>(dataTableMock.Fields.Count);
                    while (dataTableColumns.Count < dataTableColumns.Capacity)
                    {
                        var dataObjectMock = MockRepository.GenerateMock<IDataObjectBase>();
                        dataObjectMock.Expect(m => m.Field)
                                      .Return(dataTableMock.Fields[dataTableColumns.Count])
                                      .Repeat.Any();
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
                var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>(1)
                    {
                        {dataTableMock, dataTableRows}
                    };
                dataValidator.Validate(fixture.CreateAnonymous<ITable>(), targetTableData, true, fixture.CreateAnonymous<ICommand>());
                Assert.That(eventCalled, Is.EqualTo(dataTableRows.Count));

                dataRepositoryMock.AssertWasCalled(m => m.GetDataQueryer());

            }
        }

        /// <summary>
        /// Test that Validate raise OnValidation for each primary key validation when using a data queryer.
        /// </summary>
        [Test]
        public void TestThatValidateUsingDataQueryerRaiseOnValidationForEachPrimaryKeyValidation()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<ICommand>(e => e.FromFactory(() => MockRepository.GenerateMock<ICommand>()));
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
            fixture.Customize<IDataQueryer>(e => e.FromFactory(() =>MockRepository.GenerateMock<IDataQueryer>()));

            var tableMock = MockRepository.GenerateMock<ITable>();
            var fieldCollection = fixture.CreateMany<IField>(5).ToList();
            var candidateKey = MockRepository.GenerateMock<ICandidateKey>();
            candidateKey.Expect(m => m.Table)
                        .Return(tableMock)
                        .Repeat.Any();
            candidateKey.Expect(m => m.Fields)
                        .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(fieldCollection.GetRange(0, 3).Select(field => new KeyValuePair<IField, IMap>(field, null)))))
                        .Repeat.Any();
            tableMock.Expect(m => m.NameSource)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            tableMock.Expect(m => m.Fields)
                     .Return(new ReadOnlyObservableCollection<IField>(new ObservableCollection<IField>(fieldCollection)))
                     .Repeat.Any();
            tableMock.Expect(m => m.CandidateKeys)
                     .Return(new ReadOnlyObservableCollection<ICandidateKey>(new ObservableCollection<ICandidateKey>(new List<ICandidateKey> {candidateKey})))
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

            using (var dataValidator = new PrimaryKeyDataValidator(fixture.CreateAnonymous<IDataRepository>()))
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

                var targetTable = fixture.CreateAnonymous<ITable>();
                var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>
                    {
                        {targetTable, fixture.CreateMany<IEnumerable<IDataObjectBase>>(250).ToList()}
                    };
                Assert.That(eventCalled, Is.EqualTo(0));
                dataValidator.Validate(targetTable, targetTableData, true, fixture.CreateAnonymous<ICommand>());
                Assert.That(eventCalled, Is.EqualTo(250));

                dataValidator.Dispose();
            }

            dataRepositoryMock.AssertWasCalled(m => m.GetDataQueryer());
            // ReSharper disable ImplicitlyCapturedClosure
            dataRepositoryMock.GetDataQueryer().AssertWasNotCalled(m => m.GetNumberOfEqualKeyValues(Arg<IKey>.Is.Anything, Arg<IEnumerable<KeyValuePair<string, object>>>.Is.Anything, Arg<string>.Is.Anything));
            // ReSharper restore ImplicitlyCapturedClosure
            dataRepositoryMock.GetDataQueryer().AssertWasCalled(m => m.Dispose());
        }
    }
}
