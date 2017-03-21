using System;
using System.Collections.Generic;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Events;

namespace DsiNext.DeliveryEngine.Repositories.Data.OldToNew.Events
{
    /// <summary>
    /// Arguments to the event raised when to handle data from a data repository which converts old delivery format to the new delivery format.
    /// </summary>
    public class HandleOldToNewDataEventArgs : EventArgs, IHandleDataEventArgs
    {
        #region Private variables

        private readonly ITable _table;
        private readonly IEnumerable<IEnumerable<IDataObjectBase>> _data;
        private readonly bool _endOfData;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates arguments to the event raised when to handle data from a oracle data repository.
        /// </summary>
        /// <param name="table">Table for which to handle the data.</param>
        /// <param name="data">Data to be handled.</param>
        /// <param name="endOfData">Indicates whether all data has been readed.</param>
        public HandleOldToNewDataEventArgs(ITable table, IEnumerable<IEnumerable<IDataObjectBase>> data, bool endOfData)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            _table = table;
            _data = data;
            _endOfData = endOfData;
        }

        #endregion

        #region IHandleDataEventArgs Members

        /// <summary>
        /// Table for which to handle the data.
        /// </summary>
        public virtual ITable Table
        {
            get
            {
                return _table;
            }
        }

        /// <summary>
        /// Data to be handled.
        /// </summary>
        public virtual IEnumerable<IEnumerable<IDataObjectBase>> Data
        {
            get
            {
                return _data;
            }
        }

        /// <summary>
        /// Indicates whether all data has been readed.
        /// </summary>
        public virtual bool EndOfData
        {
            get
            {
                return _endOfData;
            }
        }

        #endregion
    }
}
