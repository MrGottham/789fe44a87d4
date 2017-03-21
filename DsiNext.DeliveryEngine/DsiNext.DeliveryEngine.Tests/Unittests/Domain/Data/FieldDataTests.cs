using System.Globalization;
using DsiNext.DeliveryEngine.Domain.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Domain.Data
{
    /// <summary>
    /// Tests data object for a field.
    /// </summary>
    [TestFixture]
    public class FieldDataTests
    {
        /// <summary>
        /// Test that constructor initialize field data.
        /// </summary>
        [Test]
        public void TestThatConstructorInitializeFieldData()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();
            fieldMock.Expect(m => m.DatatypeOfSource)
                .Return(typeof (int))
                .Repeat.Any();
            fieldMock.Expect(m => m.Map)
                .Return(null)
                .Repeat.Any();

            var sourceValue = fixture.CreateAnonymous<int>();

            var fieldData = new FieldData<int, string>(fieldMock, sourceValue);
            Assert.That(fieldData, Is.Not.Null);
            Assert.That(fieldData.SourceValue, Is.EqualTo(sourceValue));
            Assert.That(fieldData.TargetValue, Is.Not.Null);
            Assert.That(fieldData.TargetValue, Is.Not.Empty);
            Assert.That(fieldData.TargetValue, Is.EqualTo(sourceValue.ToString(CultureInfo.InvariantCulture)));
            Assert.That(fieldData.Mapping, Is.False);
        }

        /// <summary>
        /// Test that the setter of Id raises the PropertyChanged event.
        /// </summary>
        [Test]
        public void TestThatIdSetterRaisesPropertyChangedEvent()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();

            var fieldData = new FieldData<int, string>(fieldMock, fixture.CreateAnonymous<int>());
            Assert.That(fieldData, Is.Not.Null);

            var eventCalled = false;
            fieldData.PropertyChanged += (s, e) =>
                                             {
                                                 Assert.That(s, Is.Not.Null);
                                                 Assert.That(e, Is.Not.Null);
                                                 Assert.That(e.PropertyName, Is.Not.Null);
                                                 Assert.That(e.PropertyName, Is.Not.Empty);
                                                 Assert.That(e.PropertyName, Is.EqualTo("SourceValue"));
                                                 eventCalled = true;
                                             };

            fieldData.SourceValue = fieldData.SourceValue;
            Assert.That(eventCalled, Is.False);

            fieldData.SourceValue = fixture.CreateAnonymous<int>();
            Assert.That(eventCalled, Is.True);
        }

        /// <summary>
        /// Test that GetSourceValue throws an DeliveryEngineSystemException if the generic target type is invalid.
        /// </summary>
        [Test]
        public void TestThatGetSourceValueThrowsDeliveryEngineSystemExceptionIfGenericTargetTypeIsInvalid()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();

            var fieldData = new FieldData<int, string>(fieldMock, fixture.CreateAnonymous<int>());
            Assert.That(fieldData, Is.Not.Null);

            Assert.Throws<DeliveryEngineSystemException>(() => fieldData.GetSourceValue<decimal>());
        }

        /// <summary>
        /// Test that GetSourceValue get the source value.
        /// </summary>
        [Test]
        public void TestThatGetSourceValueGetsTheSourceValue()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();

            var fieldData = new FieldData<int, string>(fieldMock, fixture.CreateAnonymous<int>());
            Assert.That(fieldData, Is.Not.Null);

            var sourceValue = fieldData.GetSourceValue<int>();
            Assert.That(sourceValue, Is.EqualTo(fieldData.SourceValue));
        }

        /// <summary>
        /// Test that UpdateSourceValue throws an DeliveryEngineSystemException if the generic target type is invalid.
        /// </summary>
        [Test]
        public void TestThatUpdateSourceValueThrowsDeliveryEngineSystemExceptionIfGenericTargetTypeIsInvalid()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();

            var fieldData = new FieldData<int, string>(fieldMock, fixture.CreateAnonymous<int>());
            Assert.That(fieldData, Is.Not.Null);

            Assert.Throws<DeliveryEngineSystemException>(() => fieldData.UpdateSourceValue(fixture.CreateAnonymous<decimal>()));
        }

        /// <summary>
        /// Test that UpdateSourceValue updates the source value with a value of the same type.
        /// </summary>
        [Test]
        public void TestThatUpdateSourceValueUpdatesSourceValueWithValueOfSameType()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();

            var fieldData = new FieldData<int, string>(fieldMock, fixture.CreateAnonymous<int>());
            Assert.That(fieldData, Is.Not.Null);

            var newValue = fixture.CreateAnonymous<int>();
            fieldData.UpdateSourceValue(newValue);

            Assert.That(fieldData.GetSourceValue<int>(), Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that UpdateSourceValue updates the source value with a value by using ToString.
        /// </summary>
        [Test]
        public void TestThatUpdateSourceValueUpdatesSourceValueWithValueUsingToString()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();

            var fieldData = new FieldData<string, string>(fieldMock, fixture.CreateAnonymous<string>());
            Assert.That(fieldData, Is.Not.Null);

            var newValue = fixture.CreateAnonymous<int>();
            fieldData.UpdateSourceValue(newValue);

            Assert.That(fieldData.GetSourceValue<string>(), Is.EqualTo(newValue.ToString(CultureInfo.InvariantCulture)));
        }

        /// <summary>
        /// Test that UpdateSourceValue updates the source value with a value by using Parse.
        /// </summary>
        [Test]
        public void TestThatUpdateSourceValueUpdatesSourceValueWithValueUsingToParse()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();

            var fieldData = new FieldData<int, string>(fieldMock, fixture.CreateAnonymous<int>());
            Assert.That(fieldData, Is.Not.Null);

            var newValue = fixture.CreateAnonymous<int>().ToString(CultureInfo.InvariantCulture);
            fieldData.UpdateSourceValue(newValue);

            Assert.That(fieldData.GetSourceValue<int>(), Is.EqualTo(int.Parse(newValue)));
        }

        /// <summary>
        /// Test that UpdateSourceValue can set the source value to null.
        /// </summary>
        [Test]
        public void TestThatUpdateSourceValueCanSetSourceValueToNull()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();

            var fieldData = new FieldData<string, string>(fieldMock, fixture.CreateAnonymous<string>());
            Assert.That(fieldData, Is.Not.Null);

            fieldData.UpdateSourceValue<object>(null);

            Assert.That(fieldData.GetSourceValue<string>(), Is.Null);
        }

        /// <summary>
        /// Test that UpdateSourceValue can set the source value where the source value is nullable.
        /// </summary>
        [Test]
        public void TestThatUpdateSourceValueCanSetNullableSourceValue()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();

            var fieldData = new FieldData<int?, int?>(fieldMock, fixture.CreateAnonymous<int>());
            Assert.That(fieldData, Is.Not.Null);

            var newValue = fixture.CreateAnonymous<int>();
            fieldData.UpdateSourceValue(newValue.ToString(CultureInfo.InvariantCulture));

            Assert.That(fieldData.GetSourceValue<int?>(), Is.EqualTo(newValue));
        }

        /// <summary>
        /// Test that Clone clones the field data.
        /// </summary>
        [Test]
        public void TestThatCloneClonesFieldData()
        {
            var fixture = new Fixture();

            var fieldMock = MockRepository.GenerateMock<IField>();

            var fieldData = new FieldData<string, string>(fieldMock, fixture.CreateAnonymous<string>());
            Assert.That(fieldData, Is.Not.Null);

            var cloneFieldData = (IFieldData<string, string>) fieldData.Clone();
            Assert.That(cloneFieldData, Is.Not.Null);
            Assert.That(cloneFieldData.Field, Is.Not.Null);
            Assert.That(cloneFieldData.Field, Is.EqualTo(fieldData.Field));
            Assert.That(cloneFieldData.SourceValue, Is.Not.Null);
            Assert.That(cloneFieldData.SourceValue, Is.Not.Empty);
            Assert.That(cloneFieldData.SourceValue, Is.EqualTo(fieldData.SourceValue));
            Assert.That(cloneFieldData.Mapping, Is.EqualTo(fieldData.Mapping));
        }
    }
}
