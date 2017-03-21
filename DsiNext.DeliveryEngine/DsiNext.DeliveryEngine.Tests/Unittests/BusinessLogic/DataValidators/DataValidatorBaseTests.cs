using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DsiNext.DeliveryEngine.BusinessLogic.DataValidators;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Commands;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Events;
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
    /// Tests the base class for a data validator.
    /// </summary>
    [TestFixture]
    public class DataValidatorBaseTests
    {
        /// <summary>
        /// Private class for testing the the base class for a data validator.
        /// </summary>
        private class MyDataValidator : DataValidatorBase<IForeignKeysValidationCommand>
        {
            #region Properties

            /// <summary>
            /// Indicates whether the validation is executed.
            /// </summary>
            public bool ValidationExecuted
            {
                get;
                private set;
            }

            /// <summary>
            /// Indicates whether this is the last data for the target table.
            /// </summary>
            public bool EndOfData
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Validating data for a target table.
            /// </summary>
            /// <param name="targetTable">Target table.</param>
            /// <param name="targetTableData">Data for the target table.</param>
            /// <param name="endOfData">Indicates whether this is the last data for the target table.</param>
            /// <param name="command">Command which to validate with.</param>
            protected override void ValidateData(ITable targetTable, IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>> targetTableData, bool endOfData, IForeignKeysValidationCommand command)
            {
                Assert.That(targetTable, Is.Not.Null);
                Assert.That(targetTableData, Is.Not.Null);
                Assert.That(command, Is.Not.Null);
                ValidationExecuted = true;
                EndOfData = endOfData;
            }

            /// <summary>
            /// Raises the OnValidation event.
            /// </summary>
            /// <param name="sender">Object which raises the event.</param>
            /// <param name="eventArgs">Arguments to the event.</param>
            public new void RaiseOnValidationEvent(object sender, IDataValidatorEventArgs eventArgs)
            {
                base.RaiseOnValidationEvent(sender, eventArgs);
            }

            /// <summary>
            /// Gets the dictionary name for a given key.
            /// </summary>
            /// <param name="key">Key for which to get dictionary name.</param>
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
        /// Tests that the constructor initialize the data validator.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeDataValidator()
        {
            var dataValidator = new MyDataValidator();
            Assert.That(dataValidator, Is.Not.Null);
            Assert.That(dataValidator.ValidationExecuted, Is.False);
            Assert.That(dataValidator.EndOfData, Is.False);
        }

        /// <summary>
        /// Tests that Validate throws an ArgumentNullException if the target table is null.
        /// </summary>
        [Test]
        public void TestThatValidateThrowsArgumentNullExceptionIfTargetTableIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>(e => e.FromFactory(() => MockRepository.GenerateMock<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>()));
            fixture.Customize<IForeignKeysValidationCommand>(e => e.FromFactory(() => MockRepository.GenerateMock<IForeignKeysValidationCommand>()));

            var dataValidator = new MyDataValidator();
            Assert.That(dataValidator, Is.Not.Null);
            Assert.That(dataValidator.ValidationExecuted, Is.False);
            Assert.That(dataValidator.EndOfData, Is.False);

            Assert.Throws<ArgumentNullException>(() => dataValidator.Validate(null, fixture.CreateAnonymous<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>(), true, fixture.CreateAnonymous<IForeignKeysValidationCommand>()));
        }

        /// <summary>
        /// Tests that Validate throws an ArgumentNullException if data for the target table is null.
        /// </summary>
        [Test]
        public void TestThatValidateThrowsArgumentNullExceptionIfTargetTableDataIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));
            fixture.Customize<IForeignKeysValidationCommand>(e => e.FromFactory(() => MockRepository.GenerateMock<IForeignKeysValidationCommand>()));

            var dataValidator = new MyDataValidator();
            Assert.That(dataValidator, Is.Not.Null);
            Assert.That(dataValidator.ValidationExecuted, Is.False);
            Assert.That(dataValidator.EndOfData, Is.False);

            Assert.Throws<ArgumentNullException>(() => dataValidator.Validate(fixture.CreateAnonymous<ITable>(), null, true, fixture.CreateAnonymous<IForeignKeysValidationCommand>()));
        }

        /// <summary>
        /// Tests that Validate throws an ArgumentNullException if the command is null.
        /// </summary>
        [Test]
        public void TestThatValidateThrowsArgumentNullExceptionIfCommandIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));
            fixture.Customize<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>(e => e.FromFactory(() => MockRepository.GenerateMock<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>()));

            var dataValidator = new MyDataValidator();
            Assert.That(dataValidator, Is.Not.Null);
            Assert.That(dataValidator.ValidationExecuted, Is.False);
            Assert.That(dataValidator.EndOfData, Is.False);

            Assert.Throws<ArgumentNullException>(() => dataValidator.Validate(fixture.CreateAnonymous<ITable>(), fixture.CreateAnonymous<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>(), true, null));
        }

        /// <summary>
        /// Tests the Validate validates data in the target table where indication of ending data is false.
        /// </summary>
        [Test]
        public void TestThatValidateValidatesDataWhereEndOfDataIsFalse()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));
            fixture.Customize<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>(e => e.FromFactory(() => MockRepository.GenerateMock<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>()));
            fixture.Customize<IForeignKeysValidationCommand>(e => e.FromFactory(() => MockRepository.GenerateMock<IForeignKeysValidationCommand>()));

            var dataValidator = new MyDataValidator();
            Assert.That(dataValidator, Is.Not.Null);
            Assert.That(dataValidator.ValidationExecuted, Is.False);
            Assert.That(dataValidator.EndOfData, Is.False);

            dataValidator.Validate(fixture.CreateAnonymous<ITable>(), fixture.CreateAnonymous<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>(), false, fixture.CreateAnonymous<IForeignKeysValidationCommand>());
            Assert.That(dataValidator.ValidationExecuted, Is.True);
            Assert.That(dataValidator.EndOfData, Is.False);
        }

        /// <summary>
        /// Tests the Validate validates data in the target table where indication of ending data is true.
        /// </summary>
        [Test]
        public void TestThatValidateValidatesDataWhereEndOfDataIsTrue()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));
            fixture.Customize<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>(e => e.FromFactory(() => MockRepository.GenerateMock<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>()));
            fixture.Customize<IForeignKeysValidationCommand>(e => e.FromFactory(() => MockRepository.GenerateMock<IForeignKeysValidationCommand>()));

            var dataValidator = new MyDataValidator();
            Assert.That(dataValidator, Is.Not.Null);
            Assert.That(dataValidator.ValidationExecuted, Is.False);
            Assert.That(dataValidator.EndOfData, Is.False);

            dataValidator.Validate(fixture.CreateAnonymous<ITable>(), fixture.CreateAnonymous<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>(), true, fixture.CreateAnonymous<IForeignKeysValidationCommand>());
            Assert.That(dataValidator.ValidationExecuted, Is.True);
            Assert.That(dataValidator.EndOfData, Is.True);
        }

        /// <summary>
        /// Tests the Validate does not validate data if the command is another type.
        /// </summary>
        [Test]
        public void TestThatValidateDoesNotValidateDataIfCommandIsAnotherType()
        {
            var fixture = new Fixture();
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));
            fixture.Customize<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>(e => e.FromFactory(() => MockRepository.GenerateMock<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>()));
            fixture.Customize<ICommand>(e => e.FromFactory(() => MockRepository.GenerateMock<ICommand>()));

            var dataValidator = new MyDataValidator();
            Assert.That(dataValidator, Is.Not.Null);
            Assert.That(dataValidator.ValidationExecuted, Is.False);
            Assert.That(dataValidator.EndOfData, Is.False);

            dataValidator.Validate(fixture.CreateAnonymous<ITable>(), fixture.CreateAnonymous<IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>>(), true, fixture.CreateAnonymous<ICommand>());
            Assert.That(dataValidator.ValidationExecuted, Is.False);
            Assert.That(dataValidator.EndOfData, Is.False);
        }

        /// <summary>
        /// Tests that RaiseOnValidationEvent throws an ArgumentNullException if the object which raises the event is null.
        /// </summary>
        [Test]
        public void TestThatRaiseOnValidationEventThrowsArgumentNullExceptionIfSenderIsNull()
        {
            var fixture = new Fixture();
            fixture.Customize<IDataValidatorEventArgs>(e => e.FromFactory(() => MockRepository.GenerateMock<IDataValidatorEventArgs>()));

            var dataValidator = new MyDataValidator();
            Assert.That(dataValidator, Is.Not.Null);
            Assert.That(dataValidator.ValidationExecuted, Is.False);
            Assert.That(dataValidator.EndOfData, Is.False);

            Assert.Throws<ArgumentNullException>(() => dataValidator.RaiseOnValidationEvent(null, fixture.CreateAnonymous<IDataValidatorEventArgs>()));
        }

        /// <summary>
        /// Tests that RaiseOnValidationEvent throws an ArgumentNullException if the argument to the event is null.
        /// </summary>
        [Test]
        public void TestThatRaiseOnValidationEventThrowsArgumentNullExceptionIfEventArgsIsNull()
        {
            var fixture = new Fixture();

            var dataValidator = new MyDataValidator();
            Assert.That(dataValidator, Is.Not.Null);
            Assert.That(dataValidator.ValidationExecuted, Is.False);
            Assert.That(dataValidator.EndOfData, Is.False);

            Assert.Throws<ArgumentNullException>(() => dataValidator.RaiseOnValidationEvent(fixture.CreateAnonymous<object>(), null));
        }

        /// <summary>
        /// Tests that RaiseOnValidationEvent raise the OnValidation event.
        /// </summary>
        [Test]
        public void TestThatRaiseOnValidationEventRaiseOnValidation()
        {
            var fixture = new Fixture();

            var dataValidationEventArgsMock = MockRepository.GenerateMock<IDataValidatorEventArgs>();
            dataValidationEventArgsMock.Expect(m => m.Data)
                                       .Return(fixture.CreateAnonymous<object>())
                                       .Repeat.Any();
            fixture.Customize<IDataValidatorEventArgs>(e => e.FromFactory(() => dataValidationEventArgsMock));

            var dataValidator = new MyDataValidator();
            Assert.That(dataValidator, Is.Not.Null);
            Assert.That(dataValidator.ValidationExecuted, Is.False);
            Assert.That(dataValidator.EndOfData, Is.False);

            var eventCalled = false;
            dataValidator.OnValidation += (s, e) =>
                {
                    Assert.That(s, Is.Not.Null);
                    Assert.That(e, Is.Not.Null);
                    Assert.That(e.Data, Is.Not.Null);
                    eventCalled = true;
                };

            Assert.That(eventCalled, Is.False);
            dataValidator.RaiseOnValidationEvent(fixture.CreateAnonymous<object>(), fixture.CreateAnonymous<IDataValidatorEventArgs>());
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that GetDictionaryName throws an ArgumentNullException if the key is null.
        /// </summary>
        [Test]
        public void TestThatGetDictionaryNameThrowsArgumentNullExceptionIfKeyIsNull()
        {
            var dataValidator = new MyDataValidator();
            Assert.That(dataValidator, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataValidator.GetDictionaryName(null));
        }

        /// <summary>
        /// Test that GetDictionaryName gets the dictionary name for a given key.
        /// </summary>
        [Test]
        public void TestThatGetDictionaryNameGetsDictionaryNameForKey()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<IMap>(e => e.FromFactory(() => MockRepository.GenerateMock<IMap>()));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return fieldMock;
                }));
            fixture.Customize<ITable>(e => e.FromFactory(() =>
                {
                    var tableMock = MockRepository.GenerateMock<ITable>();
                    tableMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    return tableMock;
                }));
            fixture.Customize<IKey>(e => e.FromFactory(() =>
                {
                    var candidateKeyMock = MockRepository.GenerateMock<ICandidateKey>();
                    candidateKeyMock.Expect(m => m.Table)
                                    .Return(fixture.CreateAnonymous<ITable>())
                                    .Repeat.Any();
                    candidateKeyMock.Expect(m => m.Fields)
                                    .Return(new ReadOnlyObservableCollection<KeyValuePair<IField, IMap>>(new ObservableCollection<KeyValuePair<IField, IMap>>(fixture.CreateMany<KeyValuePair<IField, IMap>>(3).ToList())))
                                    .Repeat.Any();
                    return candidateKeyMock;
                }));

            var dataValidator = new MyDataValidator();
            Assert.That(dataValidator, Is.Not.Null);

            var keyMock = fixture.CreateAnonymous<IKey>();
            var dictionaryName = dataValidator.GetDictionaryName(keyMock);
            Assert.That(dictionaryName, Is.Not.Null);
            Assert.That(dictionaryName, Is.Not.Empty);
            Assert.That(dictionaryName, Is.EqualTo(string.Format("{0}:{1}({2}:{3},{4}:{5},{6}:{7})", keyMock.Table.NameTarget, "{null}", keyMock.Fields.ElementAt(0).Key.NameTarget, "{null}", keyMock.Fields.ElementAt(1).Key.NameTarget, "{null}", keyMock.Fields.ElementAt(2).Key.NameTarget, "{null}")));

            keyMock.AssertWasCalled(m => m.Fields);
            keyMock.AssertWasCalled(m => m.Table, opt => opt.Repeat.Times(3));
        }

        /// <summary>
        /// Test that GetDataQueryer throws an ArgumentNullException if the data repository is null.
        /// </summary>
        [Test]
        public void TestThatGetDataQueryerThrowsArgumentNullExceptionIfDataRepositoryIsNull()
        {
            var dataValidator = new MyDataValidator();
            Assert.That(dataValidator, Is.Not.Null);

            Assert.Throws<ArgumentNullException>(() => dataValidator.GetDataQueryer(null));
        }

        /// <summary>
        /// Test that GetDataQueryer returns a data queryer for the data repository.
        /// </summary>
        [Test]
        public void TestThatGetDataQueryerReturnsDataQueryerForDataRepository()
        {
            var fixture = new Fixture();

            var dataQueryerMock = MockRepository.GenerateMock<IDataQueryer>();
            fixture.Customize<IDataQueryer>(e => e.FromFactory(() => dataQueryerMock));

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            dataRepositoryMock.Expect(m => m.GetDataQueryer())
                              .Return(dataQueryerMock)
                              .Repeat.Any();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));

            var dataValidator = new MyDataValidator();
            Assert.That(dataValidator, Is.Not.Null);

            using (var dataQueryer = dataValidator.GetDataQueryer(fixture.CreateAnonymous<IDataRepository>()))
            {
                Assert.That(dataQueryer, Is.Not.Null );
                Assert.That(dataQueryer, Is.EqualTo(dataQueryerMock));

                dataQueryer.Dispose();
            }

            dataRepositoryMock.AssertWasCalled(m => m.GetDataQueryer());
            dataQueryerMock.AssertWasCalled(m => m.Dispose());
        }

        /// <summary>
        /// Test that GetDataQueryer returns null if GetDataQueryer in the data repository throws an NotSupportedException.
        /// </summary>
        [Test]
        public void TestThatGetDataQueryerReturnsNullIfGetDataQueryerInDataRepositoryThrowsNotSupportedException()
        {
            var fixture = new Fixture();

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            dataRepositoryMock.Expect(m => m.GetDataQueryer())
                              .Throw(fixture.CreateAnonymous<NotSupportedException>())
                              .Repeat.Any();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));

            var dataValidator = new MyDataValidator();
            Assert.That(dataValidator, Is.Not.Null);

            var dataQueyer = dataValidator.GetDataQueryer(fixture.CreateAnonymous<IDataRepository>());
            Assert.That(dataQueyer, Is.Null);

            dataRepositoryMock.AssertWasCalled(m => m.GetDataQueryer());
        }

        /// <summary>
        /// Test that GetDataQueryer throws DeliveryEngineRepositoryException if GetDataQueryer in the data repository throws an DeliveryEngineRepositoryException.
        /// </summary>
        [Test]
        public void TestThatGetDataQueryerThrowsDeliveryEngineRepositoryExceptionIfGetDataQueryerInDataRepositoryThrowsDeliveryEngineRepositoryException()
        {
            var fixture = new Fixture();

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            dataRepositoryMock.Expect(m => m.GetDataQueryer())
                              .Throw(fixture.CreateAnonymous<DeliveryEngineRepositoryException>())
                              .Repeat.Any();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));

            var dataValidator = new MyDataValidator();
            Assert.That(dataValidator, Is.Not.Null);

            Assert.Throws<DeliveryEngineRepositoryException>(() => dataValidator.GetDataQueryer(fixture.CreateAnonymous<IDataRepository>()));

            dataRepositoryMock.AssertWasCalled(m => m.GetDataQueryer());
        }

        /// <summary>
        /// Test that GetDataQueryer throws DeliveryEngineSystemException if GetDataQueryer in the data repository throws an Exception.
        /// </summary>
        [Test]
        public void TestThatGetDataQueryerThrowsDeliveryEngineSystemExceptionIfGetDataQueryerInDataRepositoryThrowsException()
        {
            var fixture = new Fixture();

            var dataRepositoryMock = MockRepository.GenerateMock<IDataRepository>();
            dataRepositoryMock.Expect(m => m.GetDataQueryer())
                              .Throw(fixture.CreateAnonymous<Exception>())
                              .Repeat.Any();
            fixture.Customize<IDataRepository>(e => e.FromFactory(() => dataRepositoryMock));

            var dataValidator = new MyDataValidator();
            Assert.That(dataValidator, Is.Not.Null);

            Assert.Throws<DeliveryEngineSystemException>(() => dataValidator.GetDataQueryer(fixture.CreateAnonymous<IDataRepository>()));

            dataRepositoryMock.AssertWasCalled(m => m.GetDataQueryer());
        }
    }
}
