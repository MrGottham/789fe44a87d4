using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Commands;

namespace DsiNext.DeliveryEngine.BusinessLogic.Commands
{
    /// <summary>
    /// Command executing the delivering engine.
    /// </summary>
    public class DeliveryEngineExecuteCommand : IDeliveryEngineExecuteCommand
    {
        #region Private variables

        private int _tablesHandledSimultaneity;
        private int _numberOfForeignTablesToCache;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a command for executing the delivering engine.
        /// </summary>
        public DeliveryEngineExecuteCommand()
        {
            _tablesHandledSimultaneity = 5;
            _numberOfForeignTablesToCache = 10;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Value for overriding the unique package ID for the archive.
        /// </summary>
        public virtual string OverrideArchiveInformationPackageId { get; set; }

        /// <summary>
        /// Indicates whether just to validate data.
        /// </summary>
        public virtual bool ValidationOnly { get; set; }

        /// <summary>
        /// Table name or regular expression for tables on which the delivery engine should be executed.
        /// </summary>
        public virtual string Table { get; set; }

        /// <summary>
        /// Number of tables which the delivery engine should handle simultaneity.
        /// </summary>
        public virtual int TablesHandledSimultaneity
        {
            get => _tablesHandledSimultaneity;
            set => _tablesHandledSimultaneity = value;
        }

        /// <summary>
        /// Indicates whether to remove records with missing relationship on foreign keys.
        /// </summary>
        public virtual bool RemoveMissingRelationshipsOnForeignKeys { get; set; }

        /// <summary>
        /// Number of foreign tables to be cached when validating foreign keys.
        /// </summary>
        public virtual int NumberOfForeignTablesToCache
        {
            get => _numberOfForeignTablesToCache;
            set => _numberOfForeignTablesToCache = value;
        }

        /// <summary>
        /// Indicates whether empty tables should be included in the delivery.
        /// </summary>
        public virtual bool IncludeEmptyTables { get; set; }

        #endregion
    }
}
