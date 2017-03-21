using System;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Events;

namespace DsiNext.DeliveryEngine.Repositories.Data.OldToNew.Events
{
    /// <summary>
    /// Arguments to the event raised when the data repository which converts old delivery format to the new delivery format is cloned.
    /// </summary>
    public class CloneOldToNewDataRepositoryEventArgs : EventArgs, ICloneDataRepositoryEventArgs
    {
        #region Private variables

        private readonly IDataRepository _clonedDataRepository;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates arguments to the event raised when the data repository which converts old delivery format to the new delivery format is cloned.
        /// </summary>
        /// <param name="clonedDataRepository">Cloned data repository which converts old delivery format to the new delivery format.</param>
        public CloneOldToNewDataRepositoryEventArgs(IDataRepository clonedDataRepository)
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
        /// Cloned data repository which converts old delivery format to the new delivery format.
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
