using System;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Resources;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Text = DsiNext.DeliveryEngine.Resources.Text;

namespace DsiNext.DeliveryEngine.Tests.Unittests.Resources
{
    /// <summary>
    /// Tests to resources for the delivery engine.
    /// </summary>
    [TestFixture]
    public class ResourceTests
    {
        /// <summary>
        /// Test that ExceptionMessage for IllegalValue is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForIllegalValueIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.IllegalValue);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.IllegalValue, fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for ApplicationSettingMissing is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForApplicationSettingMissingIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.ApplicationSettingMissing);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.ApplicationSettingMissing, fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for DirectoryNotFound is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForDirectoryNotFoundIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.DirectoryNotFound);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.DirectoryNotFound, fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for FileNotFound is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForFileNotFoundIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.FileNotFound);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.FileNotFound, fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for TableNotFound is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForTableNotFoundIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.TableNotFound);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.TableNotFound, fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for FieldNotFound is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForFieldNotFoundIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.FieldNotFound);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.FieldNotFound, fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for DirectoryNotFound is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForRepositoryErrorIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.RepositoryError);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.RepositoryError, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for ErrorReadingFieldValue is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForErrorReadingFieldValueIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.ErrorReadingFieldValue);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.ErrorReadingFieldValue, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for MissingChildNode is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForMissingChildNodeIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.MissingChildNode);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.MissingChildNode, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for TypeMismatch is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForTypeMismatchIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.TypeMismatch);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.TypeMismatch, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for UnableToFindMatchingKey is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForUnableToFindMatchingKeyIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.UnableToFindMatchingKey);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.UnableToFindMatchingKey, fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for InvalidChildNodeCount is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForInvalidChildNodeCountIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.InvalidChildNodeCount);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.InvalidChildNodeCount, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for InvalidCardinality is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForInvalidCardinalityIsReturned()
        {
            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.InvalidCardinality);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for MethodNotFoundOnType is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForMethodNotFoundOnTypeIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.MethodNotFoundOnType);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.MethodNotFoundOnType, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for FieldNotFoundOnType is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForFieldNotFoundOnTypeIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.FieldNotFoundOnType);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.FieldNotFoundOnType, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for PropertyNotFoundOnType is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForPropertyNotFoundOnTypeIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.PropertyNotFoundOnType);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.PropertyNotFoundOnType, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for UnableToCreateInstanceOfType is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForUnableToCreateInstanceOfTypeIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.UnableToCreateInstanceOfType);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.UnableToCreateInstanceOfType, fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for UnableToCreateCriteria is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForUnableToCreateCriteriaIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.UnableToCreateCriteria);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.UnableToCreateCriteria, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for ErrorHandleException is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForErrorHandleExceptionIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.ErrorHandleException);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.ErrorHandleException, fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for MissingCandidateKeysOnTable is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForMissingCandidateKeysOnTableIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.MissingCandidateKeysOnTable);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.MissingCandidateKeysOnTable, fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for MissingFieldsOnCandidateKey is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForMissingFieldsOnCandidateKeyIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.MissingFieldsOnCandidateKey);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.MissingFieldsOnCandidateKey, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for UniqueConstraintViolationOnCandidateKey is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForUniqueConstraintViolationOnCandidateKeyIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.UniqueConstraintViolationOnCandidateKey);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.UniqueConstraintViolationOnCandidateKey, fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for MissingCandidateKeyOnForeignKey is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForMissingCandidateKeyOnForeignKeyIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.MissingCandidateKeyOnForeignKey);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.MissingCandidateKeyOnForeignKey, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for UnableToMatchFieldsOnForeignKey is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForUnableToMatchFieldsOnForeignKeyIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.UnableToMatchFieldsOnForeignKey);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.UnableToMatchFieldsOnForeignKey, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for UnableToFindForeignKeyRelationship is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForUnableToFindForeignKeyRelationshipIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.UnableToFindForeignKeyRelationship);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.UnableToFindForeignKeyRelationship, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for TooManyForeignKeyRelationships is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForTooManyForeignKeyRelationshipsIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.TooManyForeignKeyRelationships);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.TooManyForeignKeyRelationships, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for NamedConnectionStringMissing is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForNamedConnectionStringMissingIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.NamedConnectionStringMissing);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.NamedConnectionStringMissing, fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for FieldNotFoundOnTable is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForFieldNotFoundOnTableIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.FieldNotFoundOnTable);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.FieldNotFoundOnTable, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for UnableToMapDataTypeFromDatabase is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForUnableToMapDataTypeFromDatabaseIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.UnableToMapDataTypeFromDatabase);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.UnableToMapDataTypeFromDatabase, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for InvalidFilterValue is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForInvalidFilterValueIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.InvalidFilterValue);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.InvalidFilterValue, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for InvokeError is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForInvokeErrorIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.InvokeError);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.InvokeError, typeof(DateTime), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for ParseError is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForParseErrorIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.ParseError);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.ParseError, typeof(DateTime), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for XmlValidationError is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForXmlValidationErrorIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.XmlValidationError);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.XmlValidationError, fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for UnableToMapValueForField is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForUnableToMapValueForFieldIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.UnableToMapValueForField);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.UnableToMapValueForField, fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for UnableToParseValueForField is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForUnableToParseValueForFieldIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.UnableToParseValueForField);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.UnableToParseValueForField, fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for UnableToGetValueForField is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForUnableToGetValueForFieldIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.UnableToGetValueForField);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.UnableToGetValueForField, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that ExceptionMessage for UnableToGetDataQueryer is returned.
        /// </summary>
        [Test]
        public void TestThatExceptionMessageForUnableToGetDataQueryerIsReturned()
        {
            var fixture = new Fixture();

            var exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.UnableToGetDataQueryer);
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));

            exceptionMessage = Resource.GetExceptionMessage(ExceptionMessage.UnableToGetDataQueryer, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(exceptionMessage, Is.Not.Null);
            Assert.That(exceptionMessage.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that an DeliveryEngineSystemException is throwed if the ExceptionMessage couldn't be found.
        /// </summary>
        [Test]
        public void TestThatDeliveryEngineSystemExceptionIsThrowedIfExceptionMessageCouldNotBeFound()
        {
            Assert.Throws<DeliveryEngineSystemException>(() => Resource.GetExceptionMessage((ExceptionMessage) 100));
            Assert.Throws<DeliveryEngineSystemException>(() => Resource.GetExceptionMessage((ExceptionMessage) 100, 1, 2, 3));
        }

        /// <summary>
        /// Test that Text for ArchiveMaker is returned.
        /// </summary>
        [Test]
        public void TestThatTextForArchiveMakerIsReturned()
        {
            var text = Resource.GetText(Text.ArchiveMaker);
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that Text for UsageArchiveMaker is returned.
        /// </summary>
        [Test]
        public void TestThatTextForUsageArchiveMakerIsReturned()
        {
            var fixture = new Fixture();

            var text = Resource.GetText(Text.UsageArchiveMaker);
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Length, Is.GreaterThan(0));

            text = Resource.GetText(Text.UsageArchiveMaker, fixture.CreateAnonymous<string>());
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that Text for ErrorMessage is returned.
        /// </summary>
        [Test]
        public void TestThatTextForErrorMessageIsReturned()
        {
            var fixture = new Fixture();

            var text = Resource.GetText(Text.ErrorMessage);
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Length, Is.GreaterThan(0));

            text = Resource.GetText(Text.ErrorMessage, fixture.CreateAnonymous<string>());
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that Text for BeforeGetDataSourceInformation is returned.
        /// </summary>
        [Test]
        public void TestThatTextForBeforeGetDataSourceInformationIsReturned()
        {
            var text = Resource.GetText(Text.BeforeGetDataSourceInformation);
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that Text for BeforeArchiveMetadataInformation is returned.
        /// </summary>
        [Test]
        public void TestThatTextForBeforeArchiveMetadataInformationIsReturned()
        {
            var fixture = new Fixture();

            var text = Resource.GetText(Text.BeforeArchiveMetadataInformation);
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Length, Is.GreaterThan(0));

            text = Resource.GetText(Text.BeforeArchiveMetadataInformation, fixture.CreateAnonymous<string>());
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that Text for BeforeGetDataForTargetTableInformation is returned.
        /// </summary>
        [Test]
        public void TestThatTextForBeforeGetDataForTargetTableInformationIsReturned()
        {
            var fixture = new Fixture();

            var text = Resource.GetText(Text.BeforeGetDataForTargetTableInformation);
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Length, Is.GreaterThan(0));

            text = Resource.GetText(Text.BeforeGetDataForTargetTableInformation, fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>());
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that Text for BeforeValidateDataInTargetTableInformation is returned.
        /// </summary>
        [Test]
        public void TestThatTextForBeforeValidateDataInTargetTableInformationIsReturned()
        {
            var fixture = new Fixture();

            var text = Resource.GetText(Text.BeforeValidateDataInTargetTableInformation);
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Length, Is.GreaterThan(0));

            text = Resource.GetText(Text.BeforeValidateDataInTargetTableInformation, fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>());
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that Text for BeforeArchiveDataForTargetTableInformation is returned.
        /// </summary>
        [Test]
        public void TestThatTextForBeforeArchiveDataForTargetTableInformationIsReturned()
        {
            var fixture = new Fixture();

            var text = Resource.GetText(Text.BeforeArchiveDataForTargetTableInformation);
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Length, Is.GreaterThan(0));

            text = Resource.GetText(Text.BeforeArchiveDataForTargetTableInformation, fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>());
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that Text for Type is returned.
        /// </summary>
        [Test]
        public void TestThatTextForTypeIsReturned()
        {
            var text = Resource.GetText(Text.Type);
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that Text for Information is returned.
        /// </summary>
        [Test]
        public void TestThatTextForInformationIsReturned()
        {
            var text = Resource.GetText(Text.Information);
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that Text for Warning is returned.
        /// </summary>
        [Test]
        public void TestThatTextForWarningIsReturned()
        {
            var text = Resource.GetText(Text.Warning);
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that Text for Assembly is returned.
        /// </summary>
        [Test]
        public void TestThatTextForAssemblyIsReturned()
        {
            var text = Resource.GetText(Text.Assembly);
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that Text for Class is returned.
        /// </summary>
        [Test]
        public void TestThatTextForClassIsReturned()
        {
            var text = Resource.GetText(Text.Class);
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that Text for Method is returned.
        /// </summary>
        [Test]
        public void TestThatTextForMethodIsReturned()
        {
            var text = Resource.GetText(Text.Method);
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that Text for Thread is returned.
        /// </summary>
        [Test]
        public void TestThatTextForThreadIsReturned()
        {
            var text = Resource.GetText(Text.Thread);
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that Text for PrimaryKeyAdderInformation is returned.
        /// </summary>
        [Test]
        public void TestThatTextForPrimaryKeyAdderInformationIsReturned()
        {
            var fixture = new Fixture();

            var text = Resource.GetText(Text.PrimaryKeyAdderInformation);
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Length, Is.GreaterThan(0));

            text = Resource.GetText(Text.PrimaryKeyAdderInformation, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that Text for ForeignKeyCleanerInformation is returned.
        /// </summary>
        [Test]
        public void TestThatTextForForeignKeyCleanerInformationIsReturned()
        {
            var fixture = new Fixture();

            var text = Resource.GetText(Text.ForeignKeyCleanerInformation);
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Length, Is.GreaterThan(0));

            text = Resource.GetText(Text.ForeignKeyCleanerInformation, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that Text for ForeignKeyDeleterInformation is returned.
        /// </summary>
        [Test]
        public void TestThatTextForForeignKeyDeleterInformationIsReturned()
        {
            var fixture = new Fixture();

            var text = Resource.GetText(Text.ForeignKeyDeleterInformation);
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Length, Is.GreaterThan(0));

            text = Resource.GetText(Text.ForeignKeyDeleterInformation, fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<int>(), fixture.CreateAnonymous<string>(), fixture.CreateAnonymous<string>());
            Assert.That(text, Is.Not.Null);
            Assert.That(text.Length, Is.GreaterThan(0));
        }

        /// <summary>
        /// Test that an DeliveryEngineSystemException is throwed if the text couldn't be found.
        /// </summary>
        [Test]
        public void TestThatDeliveryEngineSystemExceptionIsThrowedIfTextCouldNotBeFound()
        {
            Assert.Throws<DeliveryEngineSystemException>(() => Resource.GetText((Text) 100));
            Assert.Throws<DeliveryEngineSystemException>(() => Resource.GetText((Text) 100, 1, 2, 3));
        }
    }
}
