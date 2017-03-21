namespace DsiNext.DeliveryEngine.Domain.Interfaces.Metadata
{
    /// <summary>
    /// Categories for context documents.
    /// </summary>
    public enum ContextDocumentCategories
    {
        SystemPurpose,
        SystemRegulations,
        SystemContent,
        SystemAdministrativeFunctions,
        SystemPresentationStructure,
        SystemDataProvision,
        SystemDataTransfer,
        SystemPreviousSubsequentFunctions,
        SystemAgencyQualityControl,
        SystemPublication,
        SystemInformationOther,
        OperationalSystemInformation,
        OperationalSystemConvertedInformation,
// ReSharper disable InconsistentNaming
        OperationalSystemSOA,
// ReSharper restore InconsistentNaming
        OperationalSystemInformationOther,
        ArchivalProvisions,
        ArchivalTransformationInformation,
        ArchivistNotes,
        ArchivalTestNotes,
        ArchivalMigrationInformation,
        InformationOther,
        ArchivalSubmissionInformationOther,
        ArchivalIngestInformationOther,
        ArchivalMigrationInformationOther
    }
}
