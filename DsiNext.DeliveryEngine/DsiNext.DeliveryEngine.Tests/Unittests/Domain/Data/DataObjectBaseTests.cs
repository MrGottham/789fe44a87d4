using System;
using System.Globalization;
using DsiNext.DeliveryEngine.Domain.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Resources;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Domain.Data
{
    /// <summary>
    /// Tests the basic data object in the delivery engine.
    /// </summary>
    [TestFixture]
    public class DataObjectBaseTests
    {
        /// <summary>
        /// Own class for testing the basic data object in the delivery engine.
        /// </summary>
        private class MyDataObject : DataObjectBase
        {
            #region Private variables

            private readonly Fixture _fixture;

            #endregion

            #region Constructor

            /// <summary>
            /// Creates own class for testing the basic data object in the delivery engine.
            /// </summary>
            /// <param name="field">Field reference for the data object.</param>
            /// <param name="fixture">AutoFixture for creating source values.</param>
            public MyDataObject(IField field, Fixture fixture)
                : base(field)
            {
                _fixture = fixture;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Get the source value for the data object.
            /// </summary>
            /// <typeparam name="TSourceValue">Type of the source value.</typeparam>
            /// <returns>Source value.</returns>
            public override TSourceValue GetSourceValue<TSourceValue>()
            {
                return _fixture.CreateAnonymous<TSourceValue>();
            }

            /// <summary>
            /// Updates the source value on the data object. 
            /// </summary>
            /// <typeparam name="TSourceValue">Type of the source value.</typeparam>
            /// <param name="sourceValue">New source value.</param>
            public override void UpdateSourceValue<TSourceValue>(TSourceValue sourceValue)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Clone the data object.
            /// </summary>
            /// <returns>Cloned data object.</returns>
            public override object Clone()
            {
                throw new NotSupportedException();
            }

            #endregion
        }

        /// <summary>
        /// Test that the constructor initialize an basic data object.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeAnDataObject()
        {
            var fieldMock = MockRepository.GenerateMock<IField>();
            Assert.That(fieldMock, Is.Not.Null);

            var dataObject = new MyDataObject(fieldMock, new Fixture());
            Assert.That(dataObject, Is.Not.Null);
            Assert.That(dataObject.Field, Is.Not.Null);
            Assert.That(dataObject.Field, Is.EqualTo(fieldMock));
        }

        /// <summary>
        /// Test that constructor throws an ArgumentNullExcpetion if the field reference is null.
        /// </summary>
        [Test]
        public void TestThatConstructorThrowsArgumentNullExceptionIfFieldIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new MyDataObject(null, new Fixture()));
        }

        /// <summary>
        /// Test that GetSourceValue gets the source value.
        /// </summary>
        [Test]
        public void TestThatGetSourceValueGetsSourceValue()
        {
            var fieldMock = MockRepository.GenerateMock<IField>();
            Assert.That(fieldMock, Is.Not.Null);

            var dataObject = new MyDataObject(fieldMock, new Fixture());
            Assert.That(dataObject, Is.Not.Null);

            var sourceValue = dataObject.GetSourceValue<string>();
            Assert.That(sourceValue, Is.Not.Null);
            Assert.That(sourceValue, Is.Not.Empty);
        }

        /// <summary>
        /// Test that GetTargetValue gets the unmapped target value where the DatatypeOfSource is equal to the type of the target value.
        /// </summary>
        [Test]
        public void TestThatGetTargetValueGetsUnmappedTargetValueWhereDatatypeOfSourceIsEqualToTypeOfTargetValue()
        {
            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.DatatypeOfSource)
                .Return(typeof(string))
                .Repeat.Any();
            fieldMock.Expect(m => m.Map)
                .Return(null)
                .Repeat.Any();
            Assert.That(fieldMock, Is.Not.Null);

            var dataObject = new MyDataObject(fieldMock, new Fixture());
            Assert.That(dataObject, Is.Not.Null);

            var targetValue = dataObject.GetTargetValue<string>();
            Assert.That(targetValue, Is.Not.Null);
            Assert.That(targetValue, Is.Not.Empty);

            fieldMock.AssertWasCalled(m => m.DatatypeOfSource);
            fieldMock.AssertWasCalled(m => m.Map);
        }

        /// <summary>
        /// Test that GetTargetValue gets the unmapped target value where the source value is null and the type of the target is string.
        /// </summary>
        [Test]
        public void TestThatGetTargetValueGetsUnmappedTargetValueWhereSourceValueIsNullAndTypeOfTargetIsString()
        {
            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.Map)
                     .Return(null)
                     .Repeat.Any();
            Assert.That(fieldMock, Is.Not.Null);

            var fixture = new Fixture();
            fixture.Inject<string>(null);
            var dataObject = new MyDataObject(fieldMock, fixture);
            Assert.That(dataObject, Is.Not.Null);

            var targetValue = dataObject.GetTargetValue<string>();
            Assert.That(targetValue, Is.Null);

            fieldMock.AssertWasCalled(m => m.DatatypeOfSource);
            fieldMock.AssertWasCalled(m => m.Map);
        }

        /// <summary>
        /// Test that GetTargetValue gets the unmapped target value where the source value is null and the type of the target is nullable.
        /// </summary>
        [Test]
        public void TestThatGetTargetValueGetsUnmappedTargetValueWhereSourceValueIsNullAndTypeOfTargetIsNullable()
        {
            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.DatatypeOfSource)
                .Return(typeof(DateTime?))
                .Repeat.Any();
            fieldMock.Expect(m => m.Map)
                .Return(null)
                .Repeat.Any();
            Assert.That(fieldMock, Is.Not.Null);

            var fixture = new Fixture();
            fixture.Inject<DateTime?>(null);
            var dataObject = new MyDataObject(fieldMock, fixture);
            Assert.That(dataObject, Is.Not.Null);

            var targetValue = dataObject.GetTargetValue<DateTime?>();
            Assert.That(targetValue, Is.Null);

            fieldMock.AssertWasCalled(m => m.DatatypeOfSource);
            fieldMock.AssertWasCalled(m => m.Map);
        }

        /// <summary>
        /// Test that GetTargetValue gets the unmapped target value where the source value is not null and the type of the target is nullable.
        /// </summary>
        [Test]
        public void TestThatGetTargetValueGetsUnmappedTargetValueWhereSourceValueIsNotNullAndTypeOfTargetIsNullable()
        {
            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.Map)
                     .Return(null)
                     .Repeat.Any();
            Assert.That(fieldMock, Is.Not.Null);

            var fixture = new Fixture();
            fixture.Customize<string>(e => e.FromFactory(() => fixture.CreateAnonymous<int>().ToString(CultureInfo.InvariantCulture)));
            var dataObject = new MyDataObject(fieldMock, fixture);
            Assert.That(dataObject, Is.Not.Null);

            var targetValue = dataObject.GetTargetValue<int?>();
            Assert.That(targetValue, Is.Not.Null);

            fieldMock.AssertWasCalled(m => m.DatatypeOfSource);
            fieldMock.AssertWasCalled(m => m.Map);
        }

        /// <summary>
        /// Test that GetTargetValue gets the unmapped target value where the DatatypeOfSource is not equal to the type of the target value and the target type is string.
        /// </summary>
        [Test]
        public void TestThatGetTargetValueGetsUnmappedTargetValueWhereDatatypeOfSourceIsNotEqualToTypeOfTargetValueAndTypeOfTargetValueIsString()
        {
            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.DatatypeOfSource)
                .Return(typeof(int))
                .Repeat.Any();
            fieldMock.Expect(m => m.Map)
                .Return(null)
                .Repeat.Any();
            Assert.That(fieldMock, Is.Not.Null);

            var dataObject = new MyDataObject(fieldMock, new Fixture());
            Assert.That(dataObject, Is.Not.Null);

            var targetValue = dataObject.GetTargetValue<string>();
            Assert.That(targetValue, Is.Not.Empty);
            Assert.That(targetValue, Is.Not.Null);

            fieldMock.AssertWasCalled(m => m.DatatypeOfSource);
            fieldMock.AssertWasCalled(m => m.Map);
        }

        /// <summary>
        /// Test that GetTargetValue gets the unmapped target value where the DatatypeOfSource is not equal to the type of the target value and the target type is not string.
        /// </summary>
        [Test]
        public void TestThatGetTargetValueGetsUnmappedTargetValueWhereDatatypeOfSourceIsNotEqualToTypeOfTargetValueAndTypeOfTargetValueIsNotString()
        {
            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.DatatypeOfSource)
                .Return(typeof(int))
                .Repeat.Any();
            fieldMock.Expect(m => m.Map)
                .Return(null)
                .Repeat.Any();
            Assert.That(fieldMock, Is.Not.Null);

            var dataObject = new MyDataObject(fieldMock, new Fixture());
            Assert.That(dataObject, Is.Not.Null);

            var targetValue = dataObject.GetTargetValue<decimal>();
            Assert.That(targetValue, Is.GreaterThan(0M));

            fieldMock.AssertWasCalled(m => m.DatatypeOfSource);
            fieldMock.AssertWasCalled(m => m.Map);
        }

        /// <summary>
        /// Test that GetTargetValue for a unmapped value throws a DeliveryEngineSystemException if Parse fails.
        /// </summary>
        [Test]
        public void TestThatGetTargetValueForUnmappedValueThrowsDeliveryEngineSystemExceptionIfParseFails()
        {
            var fixture = new Fixture();

            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameTarget)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.NameTarget)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            fieldMock.Expect(m => m.Table)
                     .Return(tableMock)
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.Map)
                     .Return(null)
                     .Repeat.Any();
            Assert.That(fieldMock, Is.Not.Null);

            var dataObject = new MyDataObject(fieldMock, fixture);
            Assert.That(dataObject, Is.Not.Null);

            var exception = Assert.Throws<DeliveryEngineSystemException>(() => dataObject.GetTargetValue<int?>());
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Null);
            Assert.That(exception.Message, Is.Not.Empty);
            Assert.That(exception.InnerException, Is.Not.Null);

            fieldMock.AssertWasCalled(m => m.DatatypeOfSource);
            fieldMock.AssertWasCalled(m => m.Map);
            fieldMock.AssertWasCalled(m => m.NameTarget);

            tableMock.AssertWasCalled(m => m.NameTarget);
        }

        /// <summary>
        /// Test that GetTargetValue gets the mapped target value.
        /// </summary>
        [Test]
        public void TestThatGetTargetValueGetsMappedTargetValue()
        {
            var fixture = new Fixture();

            var mapMock = MockRepository.GenerateMock<IMap>();
            mapMock.Expect(m => m.MapValue<string, string>(Arg<string>.Is.NotNull))
                .Return(fixture.CreateAnonymous<string>())
                .Repeat.Any();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.DatatypeOfSource)
                .Return(typeof (string))
                .Repeat.Any();
            fieldMock.Expect(m => m.Map)
                .Return(mapMock)
                .Repeat.Any();
            Assert.That(fieldMock, Is.Not.Null);

            var dataObject = new MyDataObject(fieldMock, new Fixture());
            Assert.That(dataObject, Is.Not.Null);

            var targetValue = dataObject.GetTargetValue<string>();
            Assert.That(targetValue, Is.Not.Null);
            Assert.That(targetValue, Is.Not.Empty);

            fieldMock.AssertWasCalled(m => m.DatatypeOfSource);
            fieldMock.AssertWasCalled(m => m.Map);
            mapMock.AssertWasCalled(m => m.MapValue<string, string>(Arg<string>.Is.NotNull));
        }

        /// <summary>
        /// Test that GetTargetValue throws DeliveryEngineMappingException with an inner exception if a DeliveryEngineMappingException occurs in the mapper.
        /// </summary>
        [Test]
        public void TestThatGetTargetValueThrowsDeliveryEngineMappingExceptionWithInnerExceptionIfDeliveryEngineMappingExceptionOccursInMapper()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineMappingExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineMappingExceptionInfo>()));

            var mapException = fixture.CreateAnonymous<DeliveryEngineMappingException>();
            var mapMock = MockRepository.GenerateMock<IMap>();
            mapMock.Expect(m => m.MapValue<string, string>(Arg<string>.Is.NotNull))
                   .Throw(mapException)
                   .Repeat.Any();

            var tableMock = MockRepository.GenerateMock<ITable>();
            tableMock.Expect(m => m.NameTarget)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.Table)
                     .Return(tableMock)
                     .Repeat.Any();
            fieldMock.Expect(m => m.NameTarget)
                     .Return(fixture.CreateAnonymous<string>())
                     .Repeat.Any();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.Map)
                     .Return(mapMock)
                     .Repeat.Any();
            Assert.That(fieldMock, Is.Not.Null);

            var dataObject = new MyDataObject(fieldMock, new Fixture());
            Assert.That(dataObject, Is.Not.Null);

            var exception = Assert.Throws<DeliveryEngineSystemException>(() => dataObject.GetTargetValue<string>());
            Assert.That(exception, Is.Not.Null);

            var innerException = exception.InnerException;
            Assert.That(innerException, Is.Not.Null);
            Assert.That(innerException.Message, Is.Not.Null);
            Assert.That(innerException.Message, Is.Not.Empty);
            Assert.That(innerException.Message, Is.Not.StartsWith(Resource.GetExceptionMessage(ExceptionMessage.UnableToMapValueForField, dataObject.GetSourceValue<string>(), fieldMock.NameTarget, tableMock.NameTarget, string.Empty)));

            fieldMock.AssertWasCalled(m => m.NameTarget);
            tableMock.AssertWasCalled(m => m.NameTarget);
        }

        /// <summary>
        /// Test that GetTargetValue with a mapper gets the target value if the mapper is null.
        /// </summary>
        [Test]
        public void TestThatGetTargetValueWithMapperGetsTargetValueIfMapperIsNull()
        {
            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof (string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.Map)
                     .Return(null)
                     .Repeat.Any();
            Assert.That(fieldMock, Is.Not.Null);

            var dataObject = new MyDataObject(fieldMock, new Fixture());
            Assert.That(dataObject, Is.Not.Null);

            var targetValue = dataObject.GetTargetValue<string>(null);
            Assert.That(targetValue, Is.Not.Null);
            Assert.That(targetValue, Is.Not.Empty);

            fieldMock.AssertWasCalled(m => m.DatatypeOfSource);
            fieldMock.AssertWasCalled(m => m.Map);
        }

        /// <summary>
        /// Test that GetTargetValue with a mapper gets the target value from the mapper.
        /// </summary>
        [Test]
        public void TestThatGetTargetValueWithMapperGetsTargetValueFromMapper()
        {
            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof(string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.Map)
                     .Return(null)
                     .Repeat.Any();
            Assert.That(fieldMock, Is.Not.Null);

            var mapperMock = MockRepository.GenerateMock<IMap>();
            mapperMock.Expect(m => m.MapValue<string, string>(Arg<string>.Is.NotNull))
                  .Return(null)
                  .Repeat.Any();

            var dataObject = new MyDataObject(fieldMock, new Fixture());
            Assert.That(dataObject, Is.Not.Null);

            var targetValue = dataObject.GetTargetValue<string>(mapperMock);
            Assert.That(targetValue, Is.Null);

            fieldMock.AssertWasCalled(m => m.DatatypeOfSource);
            fieldMock.AssertWasCalled(m => m.Map);
            mapperMock.AssertWasCalled(m => m.MapValue<string, string>(Arg<string>.Is.NotNull));
        }

        /// <summary>
        /// Test that GetTargetValue with a mapper get original target value if an DeliveryEngineMappingException occurs.
        /// </summary>
        [Test]
        public void TestThatGetTargetValueWithMapperGetsOriginalTargetValueIfAnDeliveryEngineMappingExceptionOccurs()
        {
            var fixture = new Fixture();
            fixture.Customize<IDeliveryEngineMappingExceptionInfo>(e => e.FromFactory(() => MockRepository.GenerateMock<IDeliveryEngineMappingExceptionInfo>()));

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.DatatypeOfSource)
                     .Return(typeof(string))
                     .Repeat.Any();
            fieldMock.Expect(m => m.Map)
                     .Return(null)
                     .Repeat.Any();
            Assert.That(fieldMock, Is.Not.Null);

            var mapperMock = MockRepository.GenerateMock<IMap>();
            mapperMock.Expect(m => m.MapValue<string, string>(Arg<string>.Is.NotNull))
                  .Throw(fixture.CreateAnonymous<DeliveryEngineMappingException>())
                  .Repeat.Any();

            var dataObject = new MyDataObject(fieldMock, new Fixture());
            Assert.That(dataObject, Is.Not.Null);

            var targetValue = dataObject.GetTargetValue<string>(mapperMock);
            Assert.That(targetValue, Is.Not.Null);
            Assert.That(targetValue, Is.Not.Empty);

            fieldMock.AssertWasCalled(m => m.DatatypeOfSource);
            fieldMock.AssertWasCalled(m => m.Map);
            mapperMock.AssertWasCalled(m => m.MapValue<string, string>(Arg<string>.Is.NotNull));
        }

        /// <summary>
        /// Test that UpdateSourceValue throws an NotSupportedException.
        /// </summary>
        [Test]
        public void TestThatUpdateSourceValueThrowsNotSupportedException()
        {
            var fieldMock = MockRepository.GenerateMock<IField>();
            Assert.That(fieldMock, Is.Not.Null);

            var dataObject = new MyDataObject(fieldMock, new Fixture());
            Assert.That(dataObject, Is.Not.Null);

            Assert.Throws<NotSupportedException>(() => dataObject.UpdateSourceValue<object>(null));

        }

        /// <summary>
        /// Test that Clone throws an NotSupportedException.
        /// </summary>
        [Test]
        public void TestThatCloneThrowsNotSupportedException()
        {
            var fieldMock = MockRepository.GenerateMock<IField>();
            Assert.That(fieldMock, Is.Not.Null);

            var dataObject = new MyDataObject(fieldMock, new Fixture());
            Assert.That(dataObject, Is.Not.Null);

            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<NotSupportedException>(() => dataObject.Clone());
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }
    }
}
