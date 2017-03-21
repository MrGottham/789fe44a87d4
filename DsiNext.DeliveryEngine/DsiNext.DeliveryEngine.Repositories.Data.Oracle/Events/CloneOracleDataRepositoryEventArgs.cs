using System;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Events;

namespace DsiNext.DeliveryEngine.Repositories.Data.Oracle.Events
{
    /// <summary>
    /// Arguments to the event raised when the oracle data repository is cloned.
    /// </summary>
    public class CloneOracleDataRepositoryEventArgs : EventArgs, ICloneDataRepositoryEventArgs
    {
        #region Private variables

        private readonly IDataRepository _clonedDataRepository;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates arguments to the event raised when the oracle data repository is cloned.
        /// </summary>
        /// <param name="clonedDataRepository">Cloned oracle data repository.</param>
        public CloneOracleDataRepositoryEventArgs(IDataRepository clonedDataRepository)
        {
            if (clonedDataRepository == null)
            {
                throw new ArgumentNullException("clonedDataRepository");
            }
            _clonedDataRepository = clonedDataRepository;
        }

        #endregion

        #region ICloneDataRepositoryEventArgs Members

        /// <summary>
        /// Cloned oracle data repository.
        /// </summary>
        public virtual IDataRepository ClonedDataRepository
        {
            get
            {
                return _clonedDataRepository;
            }
        }

        #endregion
    }
}
