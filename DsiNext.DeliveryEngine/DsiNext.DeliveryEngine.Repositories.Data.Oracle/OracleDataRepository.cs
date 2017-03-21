using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Events;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.Data.Oracle.Events;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Events;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Repositories.Data.Oracle
{
    /// <summary>
    /// Data repository for reading data from an Oracle database.
    /// </summary>
    public class OracleDataRepository : IDataRepository
    {
        #region Private variables

        private readonly IOracleClientFactory _oracleClientFactory;
        private readonly IDataManipulators _dataManipulators;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a data repository for reading data from an Oracle database.
        /// </summary>
        /// <param name="oracleClientFactory">Factory to create Oracle clients used by the delivery engine.</param>
        /// <param name="dataManipulators">Collection of data manipulators for data repositories.</param>
        public OracleDataRepository(IOracleClientFactory oracleClientFactory, IDataManipulators dataManipulators)
        {
            if (oracleClientFactory == null)
            {
                throw new ArgumentNullException("oracleClientFactory");
            }
            if (dataManipulators == null)
            {
                throw new ArgumentNullException("dataManipulators");
            }
            _oracleClientFactory = oracleClientFactory;
            _dataManipulators = dataManipulators;
        }

        #endregion

        #region Events

        /// <summary>
        /// Event raised when the data repository must handle data.
        /// </summary>
        public virtual event DeliveryEngineEventHandler<IHandleDataEventArgs> OnHandleData;

        /// <summary>
        /// Event raised when the data repository is cloned.
        /// </summary>
        public virtual event DeliveryEngineEventHandler<ICloneDataRepositoryEventArgs> OnClone;

        #endregion

        #region IDataRepository Members

        /// <summary>
        /// Gets data for a specific target table where data can be merged from one or more source tables.
        /// </summary>
        /// <param name="targetTableName">Name of the target table where data should be returned.</param>
        /// <param name="dataSource">Data source from where to get the data.</param>
        public virtual void DataGetForTargetTable(string targetTableName, IDataSource dataSource)
        {
            if (string.IsNullOrEmpty(targetTableName))
            {
                throw new ArgumentNullException("targetTableName");
            }
            if (dataSource == null)
            {
                throw new ArgumentNullException("dataSource");
            }
            try
            {
                dataSource.Tables.Where(m => String.Compare(targetTableName, m.NameTarget, StringComparison.Ordinal) == 0)
                          .ToList()
                          .ForEach(DataGetFromTable);
            }
            catch (DeliveryEngineMetadataException)
            {
                throw;
            }
            catch (DeliveryEngineSystemException)
            {
                throw;
            }
            catch (DeliveryEngineRepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.RepositoryError, MethodBase.GetCurrentMethod().Name, ex.Message), ex);
            }
        }

        /// <summary>
        /// Gets data from a table.
        /// </summary>
        /// <param name="table">Table from where data should be returned.</param>
        public virtual void DataGetFromTable(ITable table)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            try
            {
                using (var oracleClient = _oracleClientFactory.Create())
                {
                    oracleClient.ValidateTable(table);
                    oracleClient.GetData(table, OnHandleOracleData);
                    oracleClient.Dispose();
                }
            }
            catch (DeliveryEngineMetadataException)
            {
                throw;
            }
            catch (DeliveryEngineSystemException)
            {
                throw;
            }
            catch (DeliveryEngineRepositoryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DeliveryEngineRepositoryException(Resource.GetExceptionMessage(ExceptionMessage.RepositoryError, MethodBase.GetCurrentMethod().Name, ex.Message), ex);
            }
        }

        /// <summary>
        /// Gets a data queryer for executing queries.
        /// </summary>
        /// <returns>Data queryer to execute queries.</returns>
        public virtual IDataQueryer GetDataQueryer()
        {
            return _oracleClientFactory.CreateDataQueryer(_dataManipulators);
        }

        /// <summary>
        /// Event handler for handling data for the oracle data repository.
        /// </summary>
        /// <param name="sender">Object which raises the event.</param>
        /// <param name="eventArgs">Arguments to the event.</param>
        private void OnHandleOracleData(object sender, IHandleDataEventArgs eventArgs)
        {
            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }
            if (eventArgs == null)
            {
                throw new ArgumentNullException("eventArgs");
            }
            var recordFilters = eventArgs.Table.RecordFilters;
            if (OnHandleData == null)
            {
                return;
            }
            if (recordFilters.Count == 0)
            {
                var data = new List<IEnumerable<IDataObjectBase>>(_dataManipulators.ManipulateData(eventArgs.Table, eventArgs.Data, eventArgs.EndOfData));
                try
                {
                    OnHandleData.Invoke(this, new HandleOracleDataEventArgs(eventArgs.Table, data, eventArgs.EndOfData));
                    return;
                }
                finally
                {
                    while (data.Count > 0)
                    {
                        data.Clear();
                    }
                }
            }
            var result = new List<IEnumerable<IDataObjectBase>>(eventArgs.Data.Where(m => recordFilters.Any(recordFilter => !recordFilter.Exclude(m))));
            try
            {
                var data = new List<IEnumerable<IDataObjectBase>>(_dataManipulators.ManipulateData(eventArgs.Table, result, eventArgs.EndOfData));
                try
                {
                    OnHandleData.Invoke(this, new HandleOracleDataEventArgs(eventArgs.Table, data, eventArgs.EndOfData));
                }
                finally
                {
                    while (data.Count > 0)
                    {
                        data.Clear();
                    }
                }
            }
            finally
            {
                while (result.Count > 0)
                {
                    result.Clear();
                }
            }
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Clones the data repository.
        /// </summary>
        /// <returns>Cloned data repository.</returns>
        public virtual object Clone()
        {
            var clonedDataRepository = new OracleDataRepository(_oracleClientFactory, _dataManipulators);
            if (OnClone != null)
            {
                OnClone.Invoke(this, new CloneOracleDataRepositoryEventArgs(clonedDataRepository));
            }
            return clonedDataRepository;
        }

        #endregion
    }
}
