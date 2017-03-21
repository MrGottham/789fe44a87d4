using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Domstolene.JFS.CommonLibrary.IoC.Interfaces;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Repositories.DataManipulators
{
    /// <summary>
    /// Data manipulator to handle missing foreign keys.
    /// </summary>
    public class MissingForeignKeyHandler : DataManipulatorBase, IMissingForeignKeyHandler
    {
        #region Private variables

        private static IDataSource _currentDataSource;
        private readonly IMissingForeignKeyWorker _worker;
        private readonly IContainer _container;
        private readonly static object SyncRoot = new object();

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a data manipulator to handle missing foreign keys.
        /// </summary>
        /// <param name="tableName">Table name for the table on which the data should be manipulated.</param>
        /// <param name="worker">Worker which manipulates data for missing foreign key values.</param>
        /// <param name="container">Container </param>
        public MissingForeignKeyHandler(string tableName, IMissingForeignKeyWorker worker, IContainer container)
            : base(tableName)
        {
            if (worker == null)
            {
                throw new ArgumentNullException("container");
            }
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            _worker = worker;
            _container = container;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Repository for getting data.
        /// </summary>
        public virtual IDataRepository DataRepository
        {
            get
            {
                return _container.Resolve<IDataRepository>();
            }
        }

        /// <summary>
        /// Repository for getting metadata.
        /// </summary>
        public virtual IMetadataRepository MetadataRepository
        {
            get
            {
                return _container.Resolve<IMetadataRepository>();
            }
        }

        /// <summary>
        /// Worker which manipulates data for missing foreign key values.
        /// </summary>
        public virtual IMissingForeignKeyWorker Worker
        {
            get
            {
                return _worker;
           }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Manipulates data for the table used by the data manipulator.
        /// </summary>
        /// <param name="table">Table on which to manipulate data.</param>
        /// <param name="dataToManipulate">Data which sould be manipulated.</param>
        /// <returns>Manipulated data for the table.</returns>
        protected override IEnumerable<IEnumerable<IDataObjectBase>> Manipulate(ITable table, IList<IEnumerable<IDataObjectBase>> dataToManipulate)
        {
            return _worker.ManipulateData(table, DataRepository, dataToManipulate);
        }

        /// <summary>
        /// Finalize data manipulation for the table used by the data manipulator.
        /// </summary>
        /// <param name="table">Table on which to finalize data manipulation.</param>
        /// <param name="dataToManipulate">The last manipulated data which has been received.</param>
        /// <returns>Finalized and manipulated data for the table.</returns>
        protected override IEnumerable<IEnumerable<IDataObjectBase>> Finalize(ITable table, IList<IEnumerable<IDataObjectBase>> dataToManipulate)
        {
            return _worker.FinalizeDataManipulation(table, DataRepository, dataToManipulate);
        }

        /// <summary>
        /// Indicates whether the data manipulator is manipulating a given field.
        /// </summary>
        /// <param name="fieldName">Name of the field on which to exam for use in the data manipulator.</param>
        /// <returns>True if the data manipulator use the field otherwise false.</returns>
        protected override bool ManipulatingField(string fieldName)
        {
            lock (SyncRoot)
            {
                if (_currentDataSource == null)
                {
                    _currentDataSource = MetadataRepository.DataSourceGet();
                }
                try
                {
                    ITable dataManipulatorTable;
                    try
                    {
                        dataManipulatorTable = _currentDataSource.Tables.Single(table => String.Compare(TableName, table.NameSource, StringComparison.OrdinalIgnoreCase) == 0);
                    }
                    catch (InvalidOperationException)
                    {
                        dataManipulatorTable = _currentDataSource.Tables.SingleOrDefault(table => String.Compare(TableName, table.NameTarget, StringComparison.OrdinalIgnoreCase) == 0);
                    }
                    if (dataManipulatorTable == null)
                    {
                        throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.TableNotFound, TableName));
                    }
                    try
                    {
                        return _worker.IsManipulatingField(fieldName, dataManipulatorTable);
                    }
                    finally
                    {
                        dataManipulatorTable = null;
                        Debug.Assert(dataManipulatorTable == null);
                    }
                }
                finally
                {
                    GC.Collect();
                }
            }
        }

        #endregion
    }
}
