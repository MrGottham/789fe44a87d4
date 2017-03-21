using System;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Events;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.BusinessLogic.Events
{
    /// <summary>
    /// Arguments to the event raised before archiving data in a target table.
    /// </summary>
    public class ArchiveDataForTargetTableEventArgs : EventArgs, IArchiveDataForTargetTableEventArgs
    {
        #region Private variables

        private readonly IDataSource _dataSource;
        private readonly ITable _targetTable;
        private readonly int _dataBlock;
        private readonly int _rowsInDataBlock;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates arguments to the event raised before archiving data in a target table.
        /// </summary>
        /// <param name="dataSource">Data source.</param>
        /// <param name="targetTable">Target table.</param>
        /// <param name="dataBlock">Data block number.</param>
        /// <param name="rowsInDataBlock">Rows in the data block.</param>
        public ArchiveDataForTargetTableEventArgs(IDataSource dataSource, ITable targetTable, int dataBlock, int rowsInDataBlock)
        {
            if (dataSource == null)
            {
                throw new ArgumentNullException("dataSource");
            }
            if (targetTable == null)
            {
                throw new ArgumentNullException("targetTable");
            }
            _dataSource = dataSource;
            _targetTable = targetTable;
            _dataBlock = dataBlock;
            _rowsInDataBlock = rowsInDataBlock;
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// Data source.
        /// </summary>
        public virtual IDataSource DataSource
        {
            get
            {
                return _dataSource;
            }
        }

        /// <summary>
        /// Target table.
        /// </summary>
        public virtual ITable TargetTable
        {
            get
            {
                return _targetTable;
            }
        }

        /// <summary>
        /// Data block number.
        /// </summary>
        public virtual int DataBlock
        {
            get
            {
                return _dataBlock;
            }
        }

        /// <summary>
        /// Rows in the data block.
        /// </summary>
        public virtual int RowsInDataBlock
        {
            get
            {
                return _rowsInDataBlock;
            }
        }

        #endregion
    }
}
