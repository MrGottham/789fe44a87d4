namespace DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Commands
{
    /// <summary>
    /// Interface for the command that validates foreign keys.
    /// </summary>
    public interface IForeignKeysValidationCommand : ICommand
    {
        /// <summary>
        /// Indicates whether to remove records with missing relationship on foreign keys.
        /// </summary>
        bool RemoveMissingRelationshipsOnForeignKeys
        {
            get;
            set;
        }

        /// <summary>
        /// Number of foreign tables to be cached when validating foreign keys.
        /// </summary>
        int NumberOfForeignTablesToCache
        {
            get;
            set;
        }
    }
}
