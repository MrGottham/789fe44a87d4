using System;
using System.Linq;
using System.Reflection;
using DsiNext.DeliveryEngine.Domain.Interfaces.Data;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;
using DsiNext.DeliveryEngine.Infrastructure.Interfaces.Exceptions;
using DsiNext.DeliveryEngine.Resources;

namespace DsiNext.DeliveryEngine.Domain.Data
{
    /// <summary>
    ///  Basic data object in the delivery engine.
    /// </summary>
    public abstract class DataObjectBase : DomainObjectBase, IDataObjectBase
    {
        #region Private variables

        private readonly IField _field;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a basic data object for the delivery engine.
        /// </summary>
        /// <param name="field">Field reference for the data object.</param>
        protected DataObjectBase(IField field)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }
            _field = field;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Field reference for the data object.
        /// </summary>
        public virtual IField Field
        {
            get
            {
                return _field;
            }
        }

        #endregion

        #region Members

        /// <summary>
        /// Get the source value for the data object.
        /// </summary>
        /// <typeparam name="TSourceValue">Type of the source value.</typeparam>
        /// <returns>Source value.</returns>
        public abstract TSourceValue GetSourceValue<TSourceValue>();

        /// <summary>
        /// Get the target (mapped) value for the data object.
        /// </summary>
        /// <typeparam name="TTargetValue">Type of the target value.</typeparam>
        /// <returns>Target value.</returns>
        public virtual TTargetValue GetTargetValue<TTargetValue>()
        {
            var getSourceValueMethod = GetType().GetMethod("GetSourceValue");
            if (getSourceValueMethod == null)
            {
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.MethodNotFoundOnType, "GetSourceValue", GetType()));
            }
            var sourceValue = getSourceValueMethod.MakeGenericMethod(new[] {Field.DatatypeOfSource}).Invoke(this, null);

            var map = Field.Map;
            if (map == null)
            {
                if (sourceValue is TTargetValue)
                {
                    return (TTargetValue) sourceValue;
                }
                var targetValueType = typeof(TTargetValue);
                if (targetValueType == typeof(string))
                {
                    if (sourceValue == null)
                    {
                        // ReSharper disable ExpressionIsAlwaysNull
                        return (TTargetValue) sourceValue;
                        // ReSharper restore ExpressionIsAlwaysNull
                    }
                    var toStringMethod = sourceValue.GetType().GetMethod("ToString", new Type[] {});
                    if (toStringMethod == null)
                    {
                        throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.MethodNotFoundOnType, "ToString", sourceValue.GetType()));
                    }
                    try
                    {
                        return (TTargetValue) toStringMethod.Invoke(sourceValue, new object[] {});
                    }
                    catch (TargetInvocationException ex)
                    {
                        var deliveryEngineException = ex.InnerException as DeliveryEngineExceptionBase;
                        if (deliveryEngineException != null)
                        {
                            throw deliveryEngineException;
                        }
                        throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.UnableToGetValueForField, Field.NameTarget, Field.Table.NameTarget, ex.InnerException.Message), ex.InnerException);
                    }
                }
                if (targetValueType.IsGenericType && targetValueType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    if (Equals(sourceValue, null))
                    {
                        // ReSharper disable ExpressionIsAlwaysNull
                        return (TTargetValue)sourceValue;
                        // ReSharper restore ExpressionIsAlwaysNull
                    }
                    var valueType = targetValueType.GetGenericArguments().ElementAt(0);
                    var getTargetValueMethod = GetType().GetMethod("GetTargetValue", new Type[] {}).MakeGenericMethod(new[] {valueType});
                    try
                    {
                        return (TTargetValue) getTargetValueMethod.Invoke(this, null);
                    }
                    catch (TargetInvocationException ex)
                    {
                        var deliveryEngineException = ex.InnerException as DeliveryEngineExceptionBase;
                        if (deliveryEngineException != null)
                        {
                            throw deliveryEngineException;
                        }
                        throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.UnableToGetValueForField, Field.NameTarget, Field.Table.NameTarget, ex.InnerException.Message), ex.InnerException);
                    }
                }
                var parseMethod = targetValueType.GetMethod("Parse", new[] {typeof (string)});
                if (parseMethod == null)
                {
                    throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.MethodNotFoundOnType, "Parse", targetValueType));
                }
                try
                {
                    return (TTargetValue) parseMethod.Invoke(targetValueType, new object[] {sourceValue.ToString()});
                }
                catch (TargetInvocationException ex)
                {
                    var formatException = ex.InnerException as FormatException;
                    if (formatException != null)
                    {
                        if (Equals(sourceValue, string.Empty) && targetValueType.IsValueType && (targetValueType == typeof (int) || targetValueType == typeof (long) || targetValueType == typeof (decimal)))
                        {
                            return (TTargetValue) parseMethod.Invoke(targetValueType, new object[] {"0"});
                        }
                    }
                    var deliveryEngineException = ex.InnerException as DeliveryEngineExceptionBase;
                    if (deliveryEngineException != null)
                    {
                        throw deliveryEngineException;
                    }
                    throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.UnableToParseValueForField, Equals(sourceValue, null) ? "{null}" : sourceValue, Field.NameTarget, Field.Table.NameTarget, ex.InnerException.Message), ex.InnerException);
                }
            }

            var mapValueMethod = map.GetType().GetMethods().SingleOrDefault(m => m.Name.Equals("MapValue") && m.IsGenericMethod && m.GetGenericArguments().Count() == 2);
            if (mapValueMethod == null)
            {
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.MethodNotFoundOnType, "MapValue", map.GetType()));
            }
            try
            {
                var mapValueTo = typeof (TTargetValue);
                return (TTargetValue) mapValueMethod.MakeGenericMethod(new[] {Field.DatatypeOfSource, mapValueTo}).Invoke(map, new[] {sourceValue});
            }
            catch (TargetInvocationException ex)
            {
                var deliveryEngineException = ex.InnerException as DeliveryEngineExceptionBase;
                if (deliveryEngineException != null)
                {
                    throw deliveryEngineException;
                }
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.UnableToMapValueForField, Equals(sourceValue, null) ? "{null}" : sourceValue, Field.NameTarget, Field.Table.NameTarget, ex.InnerException.Message), ex.InnerException);
            }
        }

        /// <summary>
        /// Get an mapped target value for the data object.
        /// </summary>
        /// <typeparam name="TTargetValue">Type of the target value.</typeparam>
        /// <param name="mapper">Mapper used to map the target value.</param>
        /// <returns>Mapped target value.</returns>
        public virtual TTargetValue GetTargetValue<TTargetValue>(IMap mapper)
        {
            var value = GetTargetValue<TTargetValue>();
            if (mapper == null)
            {
                return value;
            }
            try
            {
                return mapper.MapValue<TTargetValue, TTargetValue>(value);
            }
            catch (DeliveryEngineMappingException)
            {
                return value;
            }
        }

        /// <summary>
        /// Updates the source value on the data object.
        /// </summary>
        /// <typeparam name="TSourceValue">Type of the source value.</typeparam>
        /// <param name="sourceValue">New source value.</param>
        public abstract void UpdateSourceValue<TSourceValue>(TSourceValue sourceValue);

        /// <summary>
        /// Clone the data object.
        /// </summary>
        /// <returns>Cloned data object.</returns>
        public abstract object Clone();

        #endregion
    }
}
