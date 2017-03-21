using System;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Events;

namespace DsiNext.DeliveryEngine.BusinessLogic.Events
{
    /// <summary>
    /// Arguments to events raised by data validators.
    /// </summary>
    public class DataValidatorEventArgs : EventArgs, IDataValidatorEventArgs
    {
        #region Private variables

        private readonly object _data;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates arguments to events raised by data validators.
        /// </summary>
        public DataValidatorEventArgs(object data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            _data = data;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Data to the data validator event.
        /// </summary>
        public virtual object Data
        {
            get
            {
                return _data;
            }
        }

        #endregion
    }
}
