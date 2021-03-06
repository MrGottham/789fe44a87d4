﻿namespace DsiNext.DeliveryEngine.Resources
{
    /// <summary>
    /// Exception messages.
    /// </summary>
    public enum ExceptionMessage
    {
        IllegalValue,
        ApplicationSettingMissing,
        DirectoryNotFound,
        FileNotFound,
        TableNotFound,
        FieldNotFound,
        RepositoryError,
        ExistingMappingRule,
        MapByReflectionFailed,
        MapByTypeCastFailed,
        MapByDictionaryFailed,
        ErrorReadingFieldValue,
        MissingChildNode,
        InvalidFieldLength,
        FieldTableMismatch,
        TypeMismatch,
        UnableToFindMatchingKey,
        InvalidChildNodeCount,
        InvalidCardinality,
        MethodNotFoundOnType,
        FieldNotFoundOnType,
        PropertyNotFoundOnType,
        UnableToCreateInstanceOfType,
        UnableToCreateCriteria,
        ErrorHandleException,
        MissingCandidateKeysOnTable,
        MissingFieldsOnCandidateKey,
        UniqueConstraintViolationOnCandidateKey,
        MissingCandidateKeyOnForeignKey,
        UnableToMatchFieldsOnForeignKey,
        UnableToFindForeignKeyRelationship,
        TooManyForeignKeyRelationships,
        FileWriteError,
        DirectoryCreateError,
        ResourceNotFound,
        DataSourceNotSet,
        DataSourceAlreadySet,
        DataTypeNotSupported,
        FileCopyError,
        NamedConnectionStringMissing,
        FieldNotFoundOnTable,
        UnableToMapDataTypeFromDatabase,
        InvalidFilterValue,
        InvokeError,
        ParseError,
        XmlValidationError,
        UnableToMapValueForField,
        UnableToParseValueForField,
        UnableToGetValueForField,
        UnableToGetDataQueryer
    }
}
