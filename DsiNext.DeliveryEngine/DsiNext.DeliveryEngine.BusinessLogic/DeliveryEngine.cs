using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using DsiNext.DeliveryEngine.BusinessLogic.Events;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Commands;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.DataValidators;
using DsiNext.DeliveryEngine.BusinessLogic.Interfaces.Events;
using DsiNext.DeliveryEngine.Domain.Comparers;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Events;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.ExceptionHandling;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.Interfaces;
using DsiNext.DeliveryEngine.Repositories.Interfaces.Events;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.BusinessLogic
{
    /// <summary>
    /// Business logic for the delivery engine.
    /// </summary>
    public class DeliveryEngine : IDeliveryEngine
    {
        #region Private variables

        private readonly IConfigurationRepository _configurationRepository;
        private readonly IMetadataRepository _metadataRepository;
        private readonly IDataRepository _dataRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IDataValidators _dataValidators;
        private readonly IArchiveVersionRepository _archiveVersionRepository;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IDictionary<ITable, int> _tableDataBlock = new Dictionary<ITable, int>();
        private readonly IDictionary<ITable, Tuple<IDataSource, IDeliveryEngineExecuteCommand, BackgroundWorker>> _tableInformations = new Dictionary<ITable, Tuple<IDataSource, IDeliveryEngineExecuteCommand, BackgroundWorker>>();
        private readonly ICollection<Exception> _errors = new Collection<Exception>();
        private readonly object _syncRoot = new object();

        #endregion

        #region Constructor
        
        /// <summary>
        /// Creates business logic for the delivery engine.
        /// </summary>
        /// <param name="configurationRepository">Repository containing configuration.</param>
        /// <param name="metadataRepository">Repository containing metadata to get data from the source and creating the delivery format.</param>
        /// <param name="dataRepository">Repository containing functionality to get data from the source.</param>
        /// <param name="documentRepository">Repository containing functionality to get documents for the delivery format.</param>
        /// <param name="dataValidators">Data validators for the delivery engine.</param>
        /// <param name="archiveVersionRepository">Repository containing functionality to write the delivery format.</param>
        /// <param name="exceptionHandler">Exception handler for the delivery engine.</param>
        public DeliveryEngine(IConfigurationRepository configurationRepository, IMetadataRepository metadataRepository, IDataRepository dataRepository, IDocumentRepository documentRepository, IDataValidators dataValidators, IArchiveVersionRepository archiveVersionRepository, IExceptionHandler exceptionHandler)
        {
            if (configurationRepository == null)
            {
                throw new ArgumentNullException("configurationRepository");
            }
            if (metadataRepository == null)
            {
                throw new ArgumentNullException("metadataRepository");
            }
            if (dataRepository == null)
            {
                throw new ArgumentNullException("dataRepository");
            }
            if (documentRepository == null)
            {
                throw new ArgumentNullException("documentRepository");
            }
            if (dataValidators == null)
            {
                throw new ArgumentNullException("dataValidators");
            }
            if (archiveVersionRepository == null)
            {
                throw new ArgumentNullException("archiveVersionRepository");
            }
            if (exceptionHandler == null)
            {
                throw new ArgumentNullException("exceptionHandler");
            }
            _configurationRepository = configurationRepository;
            _metadataRepository = metadataRepository;
            _dataRepository = dataRepository;
            _documentRepository = documentRepository;
            _dataValidators = dataValidators;
            _archiveVersionRepository = archiveVersionRepository;
            _exceptionHandler = exceptionHandler;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Repository containing configuration.
        /// </summary>
        public virtual IConfigurationRepository ConfigurationRepository
        {
            get
            {
                return _configurationRepository;
            }
        }

        /// <summary>
        /// Repository containing metadata to get data from the source and creating the delivery format.
        /// </summary>
        public virtual IMetadataRepository MetadataRepository
        {
            get
            {
                return _metadataRepository;
            }
        }

        /// <summary>
        /// Repository containing functionality to get data from the source.
        /// </summary>
        public virtual IDataRepository DataRepository
        {
            get
            {
                return _dataRepository;
            }
        }

        /// <summary>
        /// Repository containing functionality to get documents for the delivery format.
        /// </summary>
        public virtual IDocumentRepository DocumentRepository
        {
            get
            {
                return _documentRepository;
            }
        }

        /// <summary>
        /// Data validators for the delivery engine.
        /// </summary>
        public virtual IDataValidators DataValidators
        {
            get
            {
                return _dataValidators;
            }
        }

        /// <summary>
        /// Repository containing functionality to write the delivery format.
        /// </summary>
        public virtual IArchiveVersionRepository ArchiveVersionRepository
        {
            get
            {
                return _archiveVersionRepository;
            }
        }

        /// <summary>
        /// Exception handler for the delivery engine.
        /// </summary>
        public virtual IExceptionHandler ExceptionHandler
        {
            get
            {
                return _exceptionHandler;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event raised before getting the data source.
        /// </summary>
        public virtual event DeliveryEngineEventHandler<IGetDataSourceEventArgs> BeforeGetDataSource;

        /// <summary>
        /// Event raised before archiving the metadata in the data source.
        /// </summary>
        public virtual event DeliveryEngineEventHandler<IArchiveMetadataEventArgs> BeforeArchiveMetadata;

        /// <summary>
        /// Event raised before getting data for a target table.
        /// </summary>
        public virtual event DeliveryEngineEventHandler<IGetDataForTargetTableEventArgs> BeforeGetDataForTargetTable;

        /// <summary>
        /// Event raised before validating data in a target table.
        /// </summary>
        public virtual event DeliveryEngineEventHandler<IValidateDataInTargetTableEventArgs> BeforeValidateDataInTargetTable;

        /// <summary>
        /// Event raised before archiving data for a target table.
        /// </summary>
        public virtual event DeliveryEngineEventHandler<IArchiveDataForTargetTableEventArgs> BeforeArchiveDataForTargetTable;

        #endregion

        #region Methods

        /// <summary>
        /// Execute the delivery engine to create and write the delivery format.
        /// </summary>
        /// <param name="command">Command for executing the delivery engine.</param>
        public virtual void Execute(IDeliveryEngineExecuteCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            try
            {
                // Get the data source.
                RaiseEvent(BeforeGetDataSource, new GetDataSourceEventArgs());
                var dataSource = MetadataRepository.DataSourceGet();
                if (!string.IsNullOrEmpty(command.OverrideArchiveInformationPackageId))
                {
                    dataSource.ArchiveInformationPackageId = command.OverrideArchiveInformationPackageId;
                }
                ArchiveVersionRepository.DataSource = dataSource;

                // Archive the metadata for the data source.
                if (command.ValidationOnly == false)
                {
                    RaiseEvent(BeforeArchiveMetadata, new ArchiveMetadataEventArgs(dataSource));
                    ArchiveVersionRepository.ArchiveMetaData();
                }

                // Handle and archive any target tables included in the data source.
                DataRepository.OnHandleData += HandleDataForTargetTable;
                DataRepository.OnClone += DataRepositoryCloned;
                var tableWorkers = new Collection<BackgroundWorker>();
                try
                {
                    var namedObjectComparer = new NameTargetComparer();
                    var targetTables = dataSource.Tables
                                                 .Where(m => string.IsNullOrEmpty(m.NameTarget) == false && (string.IsNullOrEmpty(command.Table) || Regex.IsMatch(m.NameTarget, command.Table, RegexOptions.Compiled)))
                                                 .Distinct(namedObjectComparer)
                                                 .OfType<ITable>()
                                                 .ToList();
                    foreach (var targetTable in targetTables)
                    {
                        while (tableWorkers.Count(m => m.IsBusy) >= (command.TablesHandledSimultaneity <= 0 ? 1 : command.TablesHandledSimultaneity) && _errors.Any() == false)
                        {
                            Thread.Sleep(250);
                        }
                        if (_errors.Any())
                        {
                            throw _errors.ElementAt(0);
                        }
                        var tableWorker = new BackgroundWorker
                            {
                                WorkerReportsProgress = false,
                                WorkerSupportsCancellation = true
                            };
                        tableWorker.DoWork += HandleTargetTableDoWork;
                        tableWorker.RunWorkerCompleted += WorkerCompleted;
                        tableWorker.Disposed += (sender, eventArgs) => tableWorkers.Remove((BackgroundWorker) sender);
                        tableWorkers.Add(tableWorker);
                        tableWorker.RunWorkerAsync(new Tuple<ITable, IDataSource, IDeliveryEngineExecuteCommand>(targetTable, dataSource, command));
                    }
                    while (tableWorkers.Any(m => m.IsBusy))
                    {
                        if (_errors.Any())
                        {
                            throw _errors.ElementAt(0);
                        }
                        Thread.Sleep(250);
                    }
                }
                finally
                {
                    foreach (var tableWorker in tableWorkers.Where(m => m.IsBusy))
                    {
                        tableWorker.CancelAsync();
                    }
                    while (tableWorkers.Any(m => m.IsBusy))
                    {
                        Thread.Sleep(250);
                    }
                    while (tableWorkers.Count > 0)
                    {
                        var tableWorker = tableWorkers.ElementAt(0);
                        tableWorker.Dispose();
                        tableWorkers.Remove(tableWorker);
                    }
                }
            }
            catch (DeliveryEngineAlreadyHandledException)
            {
            }
            catch (Exception ex)
            {
                lock (_syncRoot)
                {
                    ExceptionHandler.HandleException(ex);
                }
            }
            finally
            {
                lock (_syncRoot)
                {
                    while (_tableInformations.Count > 0)
                    {
                        _tableInformations.Clear();
                    }
                }
                while (_errors.Count > 0)
                {
                    _errors.Clear();
                }
            }
        }

        /// <summary>
        /// Raise an event for the delivery engine.
        /// </summary>
        /// <typeparam name="T">Type of the event arguments.</typeparam>
        /// <param name="eventHandler">Event handler to be raised.</param>
        /// <param name="eventArgs">Arguments to the event.</param>
        private void RaiseEvent<T>(DeliveryEngineEventHandler<T> eventHandler, T eventArgs) where T : IDeliveryEngineEventArgs
        {
            lock (_syncRoot)
            {
                if (eventHandler == null)
                {
                    return;
                }
                eventHandler.Invoke(this, eventArgs);
            }
        }

        /// <summary>
        /// Validates data for a given table with a data validator.
        /// </summary>
        /// <param name="dataValidator">Data validator which should validate the data.</param>
        /// <param name="table">Table on which to validate data.</param>
        /// <param name="tableData">Data to be validated.</param>
        /// <param name="endOfData">Indicates whether this is the last data for the table.</param>
        /// <param name="command">Execute command for the delivery engine.</param>
        private void Validate(IDataValidator dataValidator, ITable table, IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>> tableData, bool endOfData, IDeliveryEngineExecuteCommand command)
        {
            if (dataValidator == null)
            {
                throw new ArgumentNullException("dataValidator");
            }
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            if (tableData == null)
            {
                throw new ArgumentNullException("tableData");
            }
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            try
            {
                dataValidator.Validate(table, tableData, endOfData, command);
            }
            catch (ThreadInterruptedException)
            {
                throw;
            }
            catch (DeliveryEngineConvertException ex)
            {
                lock (_syncRoot)
                {
                    bool continueValidation;
                    ExceptionHandler.HandleException(ex, out continueValidation);
                    if (continueValidation == false)
                    {
                        throw new DeliveryEngineAlreadyHandledException(ex.Message, ex);
                    }
                }
                if (command.ValidationOnly)
                {
                    RemoveInvalidData(tableData, ex.Information.ConvertObjectData);
                }
                Validate(dataValidator, table, tableData, endOfData, command);
            }
            catch (DeliveryEngineMappingException ex)
            {
                lock (_syncRoot)
                {
                    bool continueValidation;
                    ExceptionHandler.HandleException(ex, out continueValidation);
                    if (continueValidation == false)
                    {
                        throw new DeliveryEngineAlreadyHandledException(ex.Message, ex);
                    }
                }
                if (command.ValidationOnly)
                {
                    RemoveInvalidData(tableData, ex.Information.MappingObjectData);
                }
                Validate(dataValidator, table, tableData, endOfData, command);
            }
            catch (DeliveryEngineValidateException ex)
            {
                lock (_syncRoot)
                {
                    bool continueValidation;
                    ExceptionHandler.HandleException(ex, out continueValidation);
                    if (continueValidation == false)
                    {
                        throw new DeliveryEngineAlreadyHandledException(ex.Message, ex);
                    }
                }
                if (command.ValidationOnly)
                {
                    RemoveInvalidData(tableData, ex.Information.ValidateObjectData);
                }
                Validate(dataValidator, table, tableData, endOfData, command);
            }
            catch (Exception ex)
            {
                var threadInterruptedException = ex.InnerException as ThreadInterruptedException;
                if (threadInterruptedException != null)
                {
                    throw threadInterruptedException;
                }
                lock (_syncRoot)
                {
                    bool continueValidation;
                    ExceptionHandler.HandleException(ex, out continueValidation);
                    if (continueValidation == false)
                    {
                        throw new DeliveryEngineAlreadyHandledException(ex.Message, ex);
                    }
                }
                Validate(dataValidator, table, tableData, endOfData, command);
            }
        }

        /// <summary>
        /// Remove the invalid data from the table data.
        /// </summary>
        /// <param name="tableData">Table data from where the invalid data should be removed.</param>
        /// <param name="invalidData">Data there should be removed.</param>
        private static void RemoveInvalidData(IEnumerable<KeyValuePair<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>> tableData, object invalidData)
        {
            if (tableData == null)
            {
                throw new ArgumentNullException("tableData");
            }
            var dataRow = invalidData as IEnumerable<IDataObjectBase>;
            if (dataRow != null)
            {
                foreach (var data in tableData.Select(m => m.Value))
                {
                    var removeMethod = data.GetType().GetMethod("Remove");
                    if (removeMethod == null)
                    {
                        throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.MethodNotFoundOnType, "Remove", data.GetType().Name));
                    }
                    removeMethod.Invoke(data, new object[] {dataRow});
                }
                return;
            }
            var invalidDataRows = invalidData as IEnumerable<IEnumerable<IDataObjectBase>>;
            if (invalidDataRows == null)
            {
                return;
            }
            var dataRows = new List<IEnumerable<IDataObjectBase>>(invalidDataRows);
            foreach (var data in tableData.Select(m => m.Value))
            {
                var removeMethod = data.GetType().GetMethod("Remove");
                if (removeMethod == null)
                {
                    throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.MethodNotFoundOnType, "Remove", data.GetType().Name));
                }
                foreach (var row in dataRows)
                {
                    removeMethod.Invoke(data, new object[] {row});
                }
            }
        }

        /// <summary>
        /// Event handler for handling data on a given target table.
        /// </summary>
        /// <param name="sender">Object which raises the event.</param>
        /// <param name="eventArgs">Arguments to the event.</param>
        private void HandleDataForTargetTable(object sender, IHandleDataEventArgs eventArgs)
        {
            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }
            if (eventArgs == null)
            {
                throw new ArgumentNullException("eventArgs");
            }
            var table = eventArgs.Table;
            var tableData = new Dictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>
                {
                    {table, eventArgs.Data}
                };
            var tuple = _tableInformations[table];
            var dataSource = tuple.Item1;
            var command = tuple.Item2;
            var tableWorker = tuple.Item3;
            if (tableWorker.CancellationPending)
            {
                throw new ThreadInterruptedException();
            }

            // Validate received data for the target table.
            if (DataValidators.Any())
            {
                IValidateDataInTargetTableEventArgs validateDataInTargetTableEventArgs;
                lock (_syncRoot)
                {
                    validateDataInTargetTableEventArgs = new ValidateDataInTargetTableEventArgs(dataSource, table, _tableDataBlock[table], tableData.Select(m => m.Value).Sum(m => m.Count()));
                }
                RaiseEvent(BeforeValidateDataInTargetTable, validateDataInTargetTableEventArgs);
                var validationWorkers = new Collection<BackgroundWorker>();
                try
                {
                    foreach (var dataValidator in DataValidators)
                    {
                        if (tableWorker.CancellationPending)
                        {
                            throw new ThreadInterruptedException();
                        }
                        var validationWorker = new BackgroundWorker
                            {
                                WorkerReportsProgress = false,
                                WorkerSupportsCancellation = true
                            };
                        validationWorker.DoWork += ValidationDoWork;
                        validationWorker.RunWorkerCompleted += WorkerCompleted;
                        validationWorker.Disposed += (s, e) => validationWorkers.Remove((BackgroundWorker) s);
                        validationWorkers.Add(validationWorker);
                        validationWorker.RunWorkerAsync(new Tuple<IDataValidator, ITable, IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>, bool, IDeliveryEngineExecuteCommand>(dataValidator, table, tableData, eventArgs.EndOfData, command));
                    }
                    while (validationWorkers.Any(m => m.IsBusy))
                    {
                        if (tableWorker.CancellationPending)
                        {
                            throw new ThreadInterruptedException();
                        }
                        Thread.Sleep(250);
                    }
                }
                finally
                {
                    foreach (var validationWorker in validationWorkers.Where(m => m.IsBusy))
                    {
                        validationWorker.CancelAsync();
                    }
                    while (validationWorkers.Any(m => m.IsBusy))
                    {
                        Thread.Sleep(250);
                    }
                    while (validationWorkers.Count > 0)
                    {
                        var validationWorker = validationWorkers.ElementAt(0);
                        validationWorker.Dispose();
                        validationWorkers.Remove(validationWorker);
                    }
                }
            }
            if (tableWorker.CancellationPending)
            {
                throw new ThreadInterruptedException();
            }

            // Archive the received data for the target table.
            if (command.ValidationOnly == false)
            {
                IArchiveDataForTargetTableEventArgs archiveDataForTargetTableEventArgs;
                lock (_syncRoot)
                {
                    archiveDataForTargetTableEventArgs = new ArchiveDataForTargetTableEventArgs(dataSource, table, _tableDataBlock[table], tableData.Select(m => m.Value).Sum(m => m.Count()));
                }
                RaiseEvent(BeforeArchiveDataForTargetTable, archiveDataForTargetTableEventArgs);
                lock (_syncRoot)
                {
                    ArchiveVersionRepository.ArchiveTableData(tableData);
                }
            }
            if (tableWorker.CancellationPending)
            {
                throw new ThreadInterruptedException();
            }
            if (eventArgs.EndOfData)
            {
                return;
            }
            IGetDataForTargetTableEventArgs getDataForTargetTableEventArgs;
            lock (_syncRoot)
            {
                _tableDataBlock[table] += 1;
                getDataForTargetTableEventArgs = new GetDataForTargetTableEventArgs(dataSource, table, _tableDataBlock[table]);
            }
            RaiseEvent(BeforeGetDataForTargetTable, getDataForTargetTableEventArgs);
        }

        /// <summary>
        /// Event handler raised when the data repository is cloned.
        /// </summary>
        /// <param name="sender">Object which raises the event.</param>
        /// <param name="eventArgs">Arguments to the event.</param>
        private void DataRepositoryCloned(object sender, ICloneDataRepositoryEventArgs eventArgs)
        {
            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }
            if (eventArgs == null)
            {
                throw new ArgumentNullException("eventArgs");
            }
            var clonedDataRepository = eventArgs.ClonedDataRepository;
            clonedDataRepository.OnHandleData += (s, e) =>
                {
                    if (_errors.Any())
                    {
                        throw new ThreadInterruptedException();
                    }
                };
        }

        /// <summary>
        /// Event handler for handling a given target table asynchronous.
        /// </summary>
        /// <param name="sender">Object which raises the event.</param>
        /// <param name="eventArgs">Arguments to the event.</param>
        private void HandleTargetTableDoWork(object sender, DoWorkEventArgs eventArgs)
        {
            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }
            if (eventArgs == null)
            {
                throw new ArgumentNullException("eventArgs");
            }
            var tableWorker = (BackgroundWorker) sender;
            var tuple = (Tuple<ITable, IDataSource, IDeliveryEngineExecuteCommand>) eventArgs.Argument;
            var table = tuple.Item1;
            var dataSource = tuple.Item2;
            var command = tuple.Item3;
            lock (_syncRoot)
            {
                _tableDataBlock.Add(table, 0);
                _tableInformations.Add(table, new Tuple<IDataSource, IDeliveryEngineExecuteCommand, BackgroundWorker>(dataSource, command, tableWorker));
            }
            try
            {
                IGetDataForTargetTableEventArgs getDataForTargetTableEventArgs;
                lock (_syncRoot)
                {
                    _tableDataBlock[table] += 1;
                    getDataForTargetTableEventArgs = new GetDataForTargetTableEventArgs(dataSource, table, _tableDataBlock[table]);
                }
                // Gets data for the target table.
                RaiseEvent(BeforeGetDataForTargetTable, getDataForTargetTableEventArgs);
                DataRepository.DataGetForTargetTable(table.NameTarget, dataSource);
            }
            catch (ThreadInterruptedException)
            {
                eventArgs.Cancel = true;
            }
            catch (DeliveryEngineAlreadyHandledException)
            {
                eventArgs.Cancel = false;
                throw;
            }
            catch (Exception ex)
            {
                var threadInterruptedException = ex.InnerException as ThreadInterruptedException;
                if (threadInterruptedException != null)
                {
                    eventArgs.Cancel = true;
                    return;
                }
                var deliveryEngineAlreadyHandledException = ex.InnerException as DeliveryEngineAlreadyHandledException;
                if (deliveryEngineAlreadyHandledException != null)
                {
                    eventArgs.Cancel = false;
                    throw deliveryEngineAlreadyHandledException;
                }
                lock (_syncRoot)
                {
                    eventArgs.Cancel = false;
                    ExceptionHandler.HandleException(ex);
                    throw new DeliveryEngineAlreadyHandledException(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Event handler for validation data on a given data validator.
        /// </summary>
        /// <param name="sender">Object which raises the event.</param>
        /// <param name="eventArgs">Arguments to the event.</param>
        private void ValidationDoWork(object sender, DoWorkEventArgs eventArgs)
        {
            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }
            if (eventArgs == null)
            {
                throw new ArgumentNullException("eventArgs");
            }
            var validationWorker = (BackgroundWorker) sender;
            var tuple = (Tuple<IDataValidator, ITable, IDictionary<ITable, IEnumerable<IEnumerable<IDataObjectBase>>>, bool, IDeliveryEngineExecuteCommand>) eventArgs.Argument;
            var dataValidator = tuple.Item1;
            var table = tuple.Item2;
            var tableData = tuple.Item3;
            var endOfData = tuple.Item4;
            var command = tuple.Item5;
            DeliveryEngineEventHandler<IDataValidatorEventArgs> onValidation = (s, e) =>
                {
                    if (validationWorker.CancellationPending)
                    {
                        throw new ThreadInterruptedException();
                    }
                };
            try
            {
                dataValidator.OnValidation += onValidation;
                Validate(dataValidator, table, tableData, endOfData, command);
            }
            catch (ThreadInterruptedException)
            {
                eventArgs.Cancel = true;
            }
            catch (DeliveryEngineAlreadyHandledException)
            {
                eventArgs.Cancel = false;
                throw;
            }
            catch (Exception ex)
            {
                var threadInterruptedException = ex.InnerException as ThreadInterruptedException;
                if (threadInterruptedException != null)
                {
                    eventArgs.Cancel = true;
                    return;
                }
                var deliveryEngineAlreadyHandledException = ex.InnerException as DeliveryEngineAlreadyHandledException;
                if (deliveryEngineAlreadyHandledException != null)
                {
                    eventArgs.Cancel = false;
                    throw deliveryEngineAlreadyHandledException;
                }
                lock (_syncRoot)
                {
                    eventArgs.Cancel = false;
                    ExceptionHandler.HandleException(ex);
                    throw new DeliveryEngineAlreadyHandledException(ex.Message, ex);
                }
            }
            finally
            {
                dataValidator.OnValidation -= onValidation;
            }
        }

        /// <summary>
        /// Event handler for completing work on a background worker.
        /// </summary>
        /// <param name="sender">Object which raises the event.</param>
        /// <param name="eventArgs">Arguments to the event.</param>
        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs eventArgs)
        {
            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }
            if (eventArgs == null)
            {
                throw new ArgumentNullException("eventArgs");
            }
            if (eventArgs.Cancelled || eventArgs.Error == null)
            {
                return;
            }
            _errors.Add(eventArgs.Error);
        }

        #endregion
    }
}
