using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Domain.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Repositories.Interfaces.DataManipulators;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Repositories.DataManipulators
{
    /// <summary>
    /// Basic data manipulator.
    /// </summary>
    public abstract class DataManipulatorBase : IDataManipulator
    {
        #region Private variables

        private readonly string _tableName;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a basic data manipulator.
        /// </summary>
        /// <param name="tableName">Table name for the table on which the data should be manipulated.</param>
        protected DataManipulatorBase(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("tableName");
            }
            _tableName = tableName;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Table name for the table on which the data should be manipulated.
        /// </summary>
        public virtual string TableName
        {
            get
            {
                return _tableName;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Manipulates data for a given table.
        /// </summary>
        /// <param name="table">Table for which data should be manipulated.</param>
        /// <param name="data">Data which should be manipulated.</param>
        /// <returns>Manipulated data for the table.</returns>
        public IEnumerable<IEnumerable<IDataObjectBase>> ManipulateData(ITable table, IEnumerable<IEnumerable<IDataObjectBase>> data)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            try
            {
                if (string.Compare(table.NameSource, TableName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var dataAsList = data as IList<IEnumerable<IDataObjectBase>> ?? new List<IEnumerable<IDataObjectBase>>(data);
                    return Manipulate(table, dataAsList);
                }
                if (string.Compare(table.NameTarget, TableName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var dataAsList = data as IList<IEnumerable<IDataObjectBase>> ?? new List<IEnumerable<IDataObjectBase>>(data);
                    return Manipulate(table, dataAsList);
                }
                return data;
            }
            finally 
            {
                GC.Collect();
            }
        }

        /// <summary>
        /// Finalize the data manipulation for a given table when the last data has been received.
        /// </summary>
        /// <param name="table">Table for which to finalize the data manipulation.</param>
        /// <param name="data">The last manipulated data which has been received.</param>
        /// <returns>Finalized and manipulated data for the table.</returns>
        public IEnumerable<IEnumerable<IDataObjectBase>> FinalizeDataManipulation(ITable table, IEnumerable<IEnumerable<IDataObjectBase>> data)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            try
            {
                if (string.Compare(table.NameSource, TableName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var dataAsList = data as IList<IEnumerable<IDataObjectBase>> ?? new List<IEnumerable<IDataObjectBase>>(data);
                    return Finalize(table, dataAsList);
                }
                if (string.Compare(table.NameTarget, TableName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var dataAsList = data as IList<IEnumerable<IDataObjectBase>> ?? new List<IEnumerable<IDataObjectBase>>(data);
                    return Finalize(table, dataAsList);
                }
                return data;
            }
            finally
            {
                GC.Collect();
            }
        }

        /// <summary>
        /// Indicates whether the data manipulator is manipulating a given table.
        /// </summary>
        /// <param name="tableName">Name of the table on which to exam for use in the data manipulator.</param>
        /// <returns>True if the data manipulator use the table otherwise false.</returns>
        public bool IsManipulatingTable(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("tableName");
            }
            return string.Compare(TableName, tableName, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Indicates whether the data manipulator is manipulating a given field.
        /// </summary>
        /// <param name="fieldName">Name of the field on which to exam for use in the data manipulator.</param>
        /// <returns>True if the data manipulator use the field otherwise false.</returns>
        public bool IsManipulatingField(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException("fieldName");
            }
            return ManipulatingField(fieldName);
        }

        /// <summary>
        /// Manipulates data for the table used by the data manipulator.
        /// </summary>
        /// <param name="table">Table on which to manipulate data.</param>
        /// <param name="dataToManipulate">Data which sould be manipulated.</param>
        /// <returns>Manipulated data for the table.</returns>
        protected abstract IEnumerable<IEnumerable<IDataObjectBase>> Manipulate(ITable table, IList<IEnumerable<IDataObjectBase>> dataToManipulate);

        /// <summary>
        /// Finalize data manipulation for the table used by the data manipulator.
        /// </summary>
        /// <param name="table">Table on which to finalize data manipulation.</param>
        /// <param name="dataToManipulate">The last manipulated data which has been received.</param>
        /// <returns>Finalized and manipulated data for the table.</returns>
        protected virtual IEnumerable<IEnumerable<IDataObjectBase>> Finalize(ITable table, IList<IEnumerable<IDataObjectBase>> dataToManipulate)
        {
            return dataToManipulate;
        }

        /// <summary>
        /// Indicates whether the data manipulator is manipulating a given field.
        /// </summary>
        /// <param name="fieldName">Name of the field on which to exam for use in the data manipulator.</param>
        /// <returns>True if the data manipulator use the field otherwise false.</returns>
        protected abstract bool ManipulatingField(string fieldName);

        /// <summary>
        /// Generates a filter for the table using the criteria configurations.
        /// </summary>
        /// <param name="table">Table on which to generate the filter.</param>
        /// <param name="criteriaConfigurations">Configuration for the criterias.</param>
        /// <returns>Filter for the table.</returns>
        protected virtual IFilter GenerateFilter(ITable table, IEnumerable<Tuple<Type, string, object>> criteriaConfigurations)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            if (criteriaConfigurations == null)
            {
                throw new ArgumentNullException("criteriaConfigurations");
            }
            var filter = new Filter();
            foreach (var criteriaConfiguration in criteriaConfigurations)
            {
                var criteriaField = table.Fields.SingleOrDefault(m => string.Compare(m.NameSource, criteriaConfiguration.Item2, StringComparison.OrdinalIgnoreCase) == 0);
                if (criteriaField == null)
                {
                    throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.FieldNotFound, criteriaConfiguration.Item2));
                }
                if (criteriaConfiguration.Item1 == typeof (EqualCriteria<>))
                {
                    filter.AddCriteria(GenerateEqualCriteria(criteriaConfiguration.Item1, criteriaField, criteriaConfiguration.Item3));
                    continue;
                }
                if (criteriaConfiguration.Item1 == typeof (PoolCriteria<>))
                {
                    var poolValues = (string[]) criteriaConfiguration.Item3;
                    filter.AddCriteria(GeneratePoolCriteria(criteriaConfiguration.Item1, criteriaField, poolValues));
                    continue;
                }
                if (criteriaConfiguration.Item1 == typeof (IntervalCriteria<>))
                {
                    var values = (Tuple<string, string>) criteriaConfiguration.Item3;
                    filter.AddCriteria(GenerateIntervalCriteria(criteriaConfiguration.Item1, criteriaField, values.Item1, values.Item2));
                    continue;
                }
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.DataTypeNotSupported, criteriaConfiguration.Item1.Name));
            }
            return filter;
        }

        /// <summary>
        /// Generates an equal criteria.
        /// </summary>
        /// <param name="criteriaType">Type for the equal criteria.</param>
        /// <param name="criteriaField">Field for the equal criteria.</param>
        /// <param name="value">Value for the criteria.</param>
        /// <returns>Equal criteria.</returns>
        private static IEqualCriteria GenerateEqualCriteria(Type criteriaType, IField criteriaField, object value)
        {
            if (criteriaType == null)
            {
                throw new ArgumentNullException("criteriaType");
            }
            if (criteriaField == null)
            {
                throw new ArgumentNullException("criteriaField");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            var criteriaFieldType = criteriaField.DatatypeOfSource;
            var type = criteriaType.MakeGenericType(new[] {criteriaFieldType});
            if (criteriaFieldType == value.GetType())
            {
                return Activator.CreateInstance(type, new[] {criteriaField, value}) as IEqualCriteria;
            }
            if (criteriaFieldType == typeof (string))
            {
                return Activator.CreateInstance(type, new object[] {criteriaField, value.ToString()}) as IEqualCriteria;
            }
            MethodInfo parseMethod;
            if (criteriaFieldType.IsGenericType && criteriaFieldType.GetGenericTypeDefinition() == typeof (Nullable<>))
            {
                var valueType = criteriaFieldType.GetGenericArguments().ElementAt(0);
                if (valueType == value.GetType())
                {
                    return Activator.CreateInstance(type, new[] {criteriaField, value}) as IEqualCriteria;
                }
                if (valueType == typeof (string))
                {
                    return Activator.CreateInstance(type, new object[] {criteriaField, value.ToString()}) as IEqualCriteria;
                }
                parseMethod = valueType.GetMethod("Parse", new[] {value.GetType()});
                if (parseMethod == null)
                {
                    throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.MethodNotFoundOnType, "Parse", valueType.Name));
                }
                return Activator.CreateInstance(type, new[] {criteriaField, parseMethod.Invoke(valueType, new[] {value})}) as IEqualCriteria;
            }
            parseMethod = criteriaFieldType.GetMethod("Parse", new[] {value.GetType()});
            if (parseMethod == null)
            {
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.MethodNotFoundOnType, "Parse", criteriaFieldType.Name));
            }
            return Activator.CreateInstance(type, new[] {criteriaField, parseMethod.Invoke(criteriaFieldType, new[] {value})}) as IEqualCriteria;
        }

        /// <summary>
        /// Generates a pool criteria.
        /// </summary>
        /// <param name="criteriaType">Type for the pool criteria.</param>
        /// <param name="criteriaField">Field for the pool criteria.</param>
        /// <param name="poolValues">Pool values for the criteria.</param>
        /// <returns>Pool criteria.</returns>
        private static IPoolCriteria GeneratePoolCriteria(Type criteriaType, IField criteriaField, IEnumerable<string> poolValues)
        {
            if (criteriaType == null)
            {
                throw new ArgumentNullException("criteriaType");
            }
            if (criteriaField == null)
            {
                throw new ArgumentNullException("criteriaField");
            }
            if (poolValues == null)
            {
                throw new ArgumentNullException("poolValues");
            }
            var criteriaFieldType = criteriaField.DatatypeOfSource;
            var type = criteriaType.MakeGenericType(new[] {criteriaFieldType});
            if (criteriaFieldType == typeof (string))
            {
                return Activator.CreateInstance(type, new object[] {criteriaField, poolValues.ToList()}) as IPoolCriteria;
            }
            var poolValueCollection = Activator.CreateInstance(typeof (Collection<>).MakeGenericType(new[] {criteriaFieldType}));
            var addMethod = poolValueCollection.GetType().GetMethod("Add", new[] {criteriaFieldType});
            if (addMethod == null)
            {
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.MethodNotFoundOnType, "Add", poolValueCollection.GetType().Name));
            }
            foreach (var poolValue in poolValues.Where(m => m != null))
            {
                MethodInfo parseMethod;
                if (criteriaFieldType.IsGenericType && criteriaFieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var valueType = criteriaFieldType.GetGenericArguments().ElementAt(0);
                    if (valueType == typeof (string))
                    {
                        addMethod.Invoke(poolValueCollection, new object[] {poolValue});
                        continue;
                    }
                    parseMethod = valueType.GetMethod("Parse", new[] { poolValue.GetType() });
                    if (parseMethod == null)
                    {
                        throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.MethodNotFoundOnType, "Parse", valueType.Name));
                    }
                    addMethod.Invoke(poolValueCollection, new[] {parseMethod.Invoke(valueType, new object[] {poolValue})});
                    continue;
                }
                parseMethod = criteriaFieldType.GetMethod("Parse", new[] { poolValue.GetType() });
                if (parseMethod == null)
                {
                    throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.MethodNotFoundOnType, "Parse", criteriaFieldType.Name));
                }
                addMethod.Invoke(poolValueCollection, new[] {parseMethod.Invoke(criteriaFieldType, new object[] {poolValue})});
            }
            return Activator.CreateInstance(type, new[] {criteriaField, poolValueCollection}) as IPoolCriteria;
        }

        /// <summary>
        /// Generates a pool criteria.
        /// </summary>
        /// <param name="criteriaType">Type for the pool criteria.</param>
        /// <param name="criteriaField">Field for the pool criteria.</param>
        /// <param name="fromValue">Beginning for the interval.</param>
        /// <param name="toValue">Ending for the interval.</param>
        /// <returns>Pool criteria.</returns>
        private static IIntervalCriteria GenerateIntervalCriteria(Type criteriaType, IField criteriaField, string fromValue, string toValue)
        {
            if (criteriaType == null)
            {
                throw new ArgumentNullException("criteriaType");
            }
            if (criteriaField == null)
            {
                throw new ArgumentNullException("criteriaField");
            }
            if (string.IsNullOrEmpty(fromValue))
            {
                throw new ArgumentNullException("fromValue");
            }
            if (string.IsNullOrEmpty(toValue))
            {
                throw new ArgumentNullException("toValue");
            }
            var criteriaFieldType = criteriaField.DatatypeOfSource;
            Type type;
            MethodInfo parseMethod;
            if (criteriaFieldType.IsGenericType && criteriaFieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var valueType = criteriaFieldType.GetGenericArguments().ElementAt(0);
                type = criteriaType.MakeGenericType(new[] {valueType});
                if (valueType == typeof (string))
                {
                    return Activator.CreateInstance(type, new object[] {criteriaField, fromValue, toValue}) as IIntervalCriteria;
                }
                parseMethod = valueType.GetMethod("Parse", new[] { typeof(string) });
                if (parseMethod == null)
                {
                    throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.MethodNotFoundOnType, "Parse", valueType.Name));
                }
                return Activator.CreateInstance(type, new[] {criteriaField, parseMethod.Invoke(valueType, new object[] {fromValue}), parseMethod.Invoke(valueType, new object[] {toValue})}) as IIntervalCriteria;
            }
            type = criteriaType.MakeGenericType(new[] {criteriaFieldType});
            if (criteriaFieldType == typeof (string))
            {
                return Activator.CreateInstance(type, new object[] {criteriaField, fromValue, toValue}) as IIntervalCriteria;
            }
            parseMethod = criteriaFieldType.GetMethod("Parse", new[] { typeof(string) });
            if (parseMethod == null)
            {
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.MethodNotFoundOnType, "Parse", criteriaFieldType.Name));
            }
            return Activator.CreateInstance(type, new[] {criteriaField, parseMethod.Invoke(criteriaFieldType, new object[] {fromValue}), parseMethod.Invoke(criteriaFieldType, new object[] {toValue})}) as IIntervalCriteria;
        }

        #endregion
    }
}
