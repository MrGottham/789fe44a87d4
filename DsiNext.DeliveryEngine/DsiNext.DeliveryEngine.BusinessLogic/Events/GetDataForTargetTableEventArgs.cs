using System;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Events;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.BusinessLogic.Events
{
    /// <summary>
    /// Arguments to the event raised before getting data for a target table.
    /// </summary>
    public class GetDataForTargetTableEventArgs : EventArgs, IGetDataForTargetTableEventArgs
    {
        #region Private variables

        private readonly IDataSource _dataSource;
        private readonly ITable _targetTable;
        private readonly int _dataBlock;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates arguments to the event raised before getting data for a target table.
        /// </summary>
        /// <param name="dataSource">Data source.</param>
        /// <param name="targetTable">Target table.</param>
        /// <param name="dataBlock">Data block number.</param>
        public GetDataForTargetTableEventArgs(IDataSource dataSource, ITable targetTable, int dataBlock)
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

        #endregion
    }
}
