using System;
using System.Collections.Generic;
using System.Linq;
using DsiNext.DeliveryEngine.BusinessLogic.DataValidators;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Commands;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.BusinessLogic.DataValidators
{
    /// <summary>
    /// Tests mapping validator for on data for a target table.
    /// </summary>
    [TestFixture]
    public class MappingDataValidatorTests
    {
        /// <summary>
        /// Tests that the constructor initialize the data validator.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeDataValidator()
        {
            var mappingDataValidator = new MappingDataValidator();
            Assert.That(mappingDataValidator, Is.Not.Null);
        }

        /// <summary>
        /// Test that Validate validates mappers on each data object.
        /// </summary>
        [Test]
        public void TestThatValidateValidatesMappersOnEachDataObject()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));
            fixture.Customize<ICommand>(e => e.FromFactory(() => MockRepository.GenerateMock<ICommand>()));

            fixture.Customize<IMap>(e => e.FromFactory(() =>
                {
                    var mapMock = MockRepository.GenerateMock<IMap>();
                    mapMock.Expect(m => m.MapValue<string, string>(Arg<string>.Is.NotNull))
                           .Return(string.Empty)
                           .Repeat.Any();
                    return mapMock;
                }));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.DatatypeOfSource)
                             .Return(typeof (string))
                             .Repeat.Any();
                    fieldMock.Expect(m => m.DatatypeOfTarget)
                             .Return(typeof (string))
                             .Repeat.Any();
                    fieldMock.Expect(m => m.Map)
                             .Return(fixture.CreateAnonymous<IMap>())
                             .Repeat.Any();
                    return fieldMock;
                }));
            fixture.Customize<IDataObjectBase>(e => e.FromFactory(() =>
                {
                    var dataObjectMock = MockRepository.GenerateMock<IDataObjectBase>();
                    dataObjectMock.Expect(m => m.Field)
                                  .Return(fixture.CreateAnonymous<IField>())
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.GetSourceValue<string>())
                                  .Return(fixture.CreateAnonymous<string>())
                                  .Repeat.Any();
                    return dataObjectMock;
                }));
            fixture.Customize<IEnumerable<IDataObjectBase>>(e => e.FromFactory(() => fixture.CreateMany<IDataObjectBase>(10).ToList()));

            var mappingDataValidator = new MappingDataValidator();
            Assert.That(mappingDataValidator, Is.Not.Null);

            var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>
                {
                    {fixture.CreateAnonymous<ITable>(), fixture.CreateMany<IEnumerable<IDataObjectBase>>(5).ToList()}
                };
            mappingDataValidator.Validate(fixture.CreateAnonymous<ITable>(), targetTableData, true, fixture.CreateAnonymous<ICommand>());

            foreach (var dataObject in from dataTable in targetTableData.Keys from dataRow in targetTableData[dataTable] from dataObject in dataRow.Where(m => m.Field != null && m.Field.Map != null) select dataObject)
            {
                dataObject.Field.Map.AssertWasCalled(m => m.MapValue<string, string>(Arg<string>.Is.NotNull));
            }
        }

        /// <summary>
        /// Test that Validate throws an DeliveryEngineMappingException if DeliveryEngineMappingException occurs in mapper.
        /// </summary>
        [Test]
        public void TestThatValidateThrowsDeliveryEngineMappingExceptionIfDeliveryEngineMappingExceptionOccursInMapper()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));
            fixture.Customize<ICommand>(e => e.FromFactory(() => MockRepository.GenerateMock<ICommand>()));
            fixture.Customize<IDeliveryEngineMappingExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineMappingExceptionInfo>()));

            fixture.Customize<IMap>(e => e.FromFactory(() =>
                {
                    var mapMock = MockRepository.GenerateMock<IMap>();
                    mapMock.Expect(m => m.MapValue<string, string>(Arg<string>.Is.NotNull))
                           .Throw(fixture.CreateAnonymous<DeliveryEngineMappingException>())
                           .Repeat.Any();
                    return mapMock;
                }));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    fieldMock.Expect(m => m.Table)
                             .Return(fixture.CreateAnonymous<ITable>())
                             .Repeat.Any();
                    fieldMock.Expect(m => m.DatatypeOfSource)
                             .Return(typeof (string))
                             .Repeat.Any();
                    fieldMock.Expect(m => m.DatatypeOfTarget)
                             .Return(typeof (string))
                             .Repeat.Any();
                    fieldMock.Expect(m => m.Map)
                             .Return(fixture.CreateAnonymous<IMap>())
                             .Repeat.Any();
                    return fieldMock;
                }));
            fixture.Customize<IDataObjectBase>(e => e.FromFactory(() =>
                {
                    var dataObjectMock = MockRepository.GenerateMock<IDataObjectBase>();
                    dataObjectMock.Expect(m => m.Field)
                                  .Return(fixture.CreateAnonymous<IField>())
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.GetSourceValue<string>())
                                  .Return(fixture.CreateAnonymous<string>())
                                  .Repeat.Any();
                    return dataObjectMock;
                }));
            fixture.Customize<IEnumerable<IDataObjectBase>>(e => e.FromFactory(() => fixture.CreateMany<IDataObjectBase>(10).ToList()));

            var mappingDataValidator = new MappingDataValidator();
            Assert.That(mappingDataValidator, Is.Not.Null);

            var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>
                {
                    {fixture.CreateAnonymous<ITable>(), fixture.CreateMany<IEnumerable<IDataObjectBase>>(5).ToList()}
                };
            Assert.Throws<DeliveryEngineMappingException>(() => mappingDataValidator.Validate(fixture.CreateAnonymous<ITable>(), targetTableData, true, fixture.CreateAnonymous<ICommand>()));
        }

        /// <summary>
        /// Test that Validate throws an DeliveryEngineMappingException if Exception occurs in mapper.
        /// </summary>
        [Test]
        public void TestThatValidateThrowsDeliveryEngineMappingExceptionIfExceptionOccursInMapper()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));
            fixture.Customize<ICommand>(e => e.FromFactory(() => MockRepository.GenerateMock<ICommand>()));

            fixture.Customize<IMap>(e => e.FromFactory(() =>
                {
                    var mapMock = MockRepository.GenerateMock<IMap>();
                    mapMock.Expect(m => m.MapValue<string, string>(Arg<string>.Is.NotNull))
                           .Throw(fixture.CreateAnonymous<Exception>())
                           .Repeat.Any();
                    return mapMock;
                }));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.NameTarget)
                             .Return(fixture.CreateAnonymous<string>())
                             .Repeat.Any();
                    fieldMock.Expect(m => m.Table)
                             .Return(fixture.CreateAnonymous<ITable>())
                             .Repeat.Any();
                    fieldMock.Expect(m => m.DatatypeOfSource)
                             .Return(typeof (string))
                             .Repeat.Any();
                    fieldMock.Expect(m => m.DatatypeOfTarget)
                             .Return(typeof (string))
                             .Repeat.Any();
                    fieldMock.Expect(m => m.Map)
                             .Return(fixture.CreateAnonymous<IMap>())
                             .Repeat.Any();
                    return fieldMock;
                }));
            fixture.Customize<IDataObjectBase>(e => e.FromFactory(() =>
                {
                    var dataObjectMock = MockRepository.GenerateMock<IDataObjectBase>();
                    dataObjectMock.Expect(m => m.Field)
                                  .Return(fixture.CreateAnonymous<IField>())
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.GetSourceValue<string>())
                                  .Return(fixture.CreateAnonymous<string>())
                                  .Repeat.Any();
                    return dataObjectMock;
                }));
            fixture.Customize<IEnumerable<IDataObjectBase>>(e => e.FromFactory(() => fixture.CreateMany<IDataObjectBase>(10).ToList()));

            var mappingDataValidator = new MappingDataValidator();
            Assert.That(mappingDataValidator, Is.Not.Null);

            var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>
                {
                    {fixture.CreateAnonymous<ITable>(), fixture.CreateMany<IEnumerable<IDataObjectBase>>(5).ToList()}
                };
            Assert.Throws<DeliveryEngineMappingException>(() => mappingDataValidator.Validate(fixture.CreateAnonymous<ITable>(), targetTableData, true, fixture.CreateAnonymous<ICommand>()));
        }

        /// <summary>
        /// Test that Validate raise OnValidation for each data row.
        /// </summary>
        [Test]
        public void TestThatValidateRaisesOnValidationForEachDataRow()
        {
            var fixture = new Fixture();
            fixture.Customize<Type>(e => e.FromFactory(() => typeof (object)));
            fixture.Customize<ITable>(e => e.FromFactory(() => MockRepository.GenerateMock<ITable>()));
            fixture.Customize<ICommand>(e => e.FromFactory(() => MockRepository.GenerateMock<ICommand>()));

            fixture.Customize<IMap>(e => e.FromFactory(() =>
                {
                    var mapMock = MockRepository.GenerateMock<IMap>();
                    mapMock.Expect(m => m.MapValue<string, string>(Arg<string>.Is.NotNull))
                           .Return(string.Empty)
                           .Repeat.Any();
                    return mapMock;
                }));
            fixture.Customize<IField>(e => e.FromFactory(() =>
                {
                    var fieldMock = MockRepository.GenerateMock<IField>();
                    fieldMock.Expect(m => m.DatatypeOfSource)
                             .Return(typeof (string))
                             .Repeat.Any();
                    fieldMock.Expect(m => m.DatatypeOfTarget)
                             .Return(typeof (string))
                             .Repeat.Any();
                    fieldMock.Expect(m => m.Map)
                             .Return(fixture.CreateAnonymous<IMap>())
                             .Repeat.Any();
                    return fieldMock;
                }));
            fixture.Customize<IDataObjectBase>(e => e.FromFactory(() =>
                {
                    var dataObjectMock = MockRepository.GenerateMock<IDataObjectBase>();
                    dataObjectMock.Expect(m => m.Field)
                                  .Return(fixture.CreateAnonymous<IField>())
                                  .Repeat.Any();
                    dataObjectMock.Expect(m => m.GetSourceValue<string>())
                                  .Return(fixture.CreateAnonymous<string>())
                                  .Repeat.Any();
                    return dataObjectMock;
                }));
            fixture.Customize<IEnumerable<IDataObjectBase>>(e => e.FromFactory(() => fixture.CreateMany<IDataObjectBase>(10).ToList()));

            var mappingDataValidator = new MappingDataValidator();
            Assert.That(mappingDataValidator, Is.Not.Null);

            var eventCalled = 0;
            mappingDataValidator.OnValidation += (s, e) =>
                {
                    Assert.That(s, Is.Not.Null);
                    Assert.That(e, Is.Not.Null);
                    Assert.That(e.Data, Is.Not.Null);
                    eventCalled++;
                };

            Assert.That(eventCalled, Is.EqualTo(0));
            var targetTableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>
                {
                    {fixture.CreateAnonymous<ITable>(), fixture.CreateMany<IEnumerable<IDataObjectBase>>(5).ToList()}
                };
            mappingDataValidator.Validate(fixture.CreateAnonymous<ITable>(), targetTableData, true, fixture.CreateAnonymous<ICommand>());
            Assert.That(eventCalled, Is.EqualTo(targetTableData.ElementAt(0).Value.Count()));
        }
    }
}
