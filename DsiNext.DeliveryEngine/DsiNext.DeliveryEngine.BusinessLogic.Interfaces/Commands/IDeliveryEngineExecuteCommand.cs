namespace DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Commands
{
    /// <summary>
    /// Interface for the command executing the delivering engine.
    /// </summary>
    public interface IDeliveryEngineExecuteCommand : IForeignKeysValidationCommand
    {
        /// <summary>
        /// Value for overriding the unique package ID for the archive.
        /// </summary>
        string OverrideArchiveInformationPackageId
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates whether just to validate data.
        /// </summary>
        bool ValidationOnly
        {
            get;
            set;
        }

        /// <summary>
        /// Table name or regular expression for tables on which the delivery engine should be executed.
        /// </summary>
        string Table
        {
            get;
            set;
        }

        /// <summary>
        /// Number of tables which the delivery engine should handle simultaneity.
        /// </summary>
        int TablesHandledSimultaneity
        {
            get;
            set;
        }
    }
}
