using System;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Events;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.BusinessLogic.Events
{
    /// <summary>
    /// Arguments to the event raised before archiving the metadata.
    /// </summary>
    public class ArchiveMetadataEventArgs : EventArgs, IArchiveMetadataEventArgs
    {
        #region Private variables

        private readonly IDataSource _dataSource;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates arguments to the event raised before archiving the metadata.
        /// </summary>
        /// <param name="dataSource">Data source for the metadata to archive.</param>
        public ArchiveMetadataEventArgs(IDataSource dataSource)
        {
            if (dataSource == null)
            {
                throw new ArgumentNullException("dataSource");
            }
            _dataSource = dataSource;
        }

        #endregion

        #region Propterties

        /// <summary>
        /// Data source for the metadata to archive.
        /// </summary>
        public virtual IDataSource DataSource
        {
            get
            {
                return _dataSource;
            }
        }

        #endregion
    }
}
