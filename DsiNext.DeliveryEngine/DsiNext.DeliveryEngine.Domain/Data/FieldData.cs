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
    /// Data object for a field.
    /// </summary>
    /// <typeparam name="TSourceValue">Type of the source value.</typeparam>
    /// <typeparam name="TTargetValue">Type of the target value.</typeparam>
    public class FieldData<TSourceValue, TTargetValue> : DataObjectBase, IFieldData<TSourceValue, TTargetValue>
    {
        #region Private variables

        private TSourceValue _sourceValue;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates data object for a field.
        /// </summary>
        /// <param name="field">Field reference for the data object.</param>
        /// <param name="sourceValue">Source value.</param>
        public FieldData(IField field, TSourceValue sourceValue)
            : base(field)
        {
            _sourceValue = sourceValue;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Source value.
        /// </summary>
        public virtual TSourceValue SourceValue
        {
            get
            {
                return _sourceValue;
            }
            set
            {
                if (Equals(_sourceValue, value))
                {
                    return;
                }
                _sourceValue = value;
                RaisePropertyChanged(this, MethodBase.GetCurrentMethod().Name.Substring(4));
            }
        }

        /// <summary>
        /// Target value.
        /// </summary>
        public virtual TTargetValue TargetValue
        {
            get
            {
                return GetTargetValue<TTargetValue>();
            }
        }

        /// <summary>
        /// Indicates if the value is mapped.
        /// </summary>
        public virtual bool Mapping
        {
            get
            {
                return Field.Map != null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the source value for the field data.
        /// </summary>
        /// <typeparam name="TValue">Type of the source value.</typeparam>
        /// <returns>Source value for the field data.</returns>
        public override TValue GetSourceValue<TValue>()
        {
            if (typeof(TValue) != typeof(TSourceValue))
            {
                throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.TypeMismatch, typeof(TValue), typeof(TSourceValue), MethodBase.GetCurrentMethod().Name));
            }
            var property = GetType().GetProperty("SourceValue");
            return (TValue) property.GetValue(this, null);
        }

        /// <summary>
        /// Updates the source value on the field data.
        /// </summary>
        /// <typeparam name="TValue">Type of the source value.</typeparam>
        /// <param name="sourceValue">New source value.</param>
        public override void UpdateSourceValue<TValue>(TValue sourceValue)
        {
            var property = GetType().GetProperty("SourceValue");
            if (Equals(sourceValue, null) && property.PropertyType.IsValueType == false)
            {
                property.SetValue(this, null, null);
                return;
            }
            if (typeof (TValue) == typeof (TSourceValue))
            {
                property.SetValue(this, sourceValue, null);
                return;
            }
            if (typeof (TSourceValue) == typeof (string) && Equals(sourceValue, null) == false)
            {
                var sourceValueType = sourceValue.GetType();
                var method = sourceValueType.GetMethod("ToString", new Type[] {});
                if (method != null)
                {
                    property.SetValue(this, method.Invoke(sourceValue, null), null);
                    return;
                }
            }
            if (Equals(sourceValue, null) == false)
            {
                var sourceValueType = sourceValue.GetType();
                var parseValueType = typeof (TSourceValue);
                if (parseValueType.IsGenericType && parseValueType.GetGenericTypeDefinition() == typeof (Nullable<>))
                {
                    parseValueType = parseValueType.GetGenericArguments().ElementAt(0);
                }
                var method = parseValueType.GetMethod("Parse", new[] {sourceValueType});
                if (method != null)
                {
                    property.SetValue(this, method.Invoke(parseValueType, new object[] {sourceValue}), null);
                    return;
                }
            }
            throw new DeliveryEngineSystemException(Resource.GetExceptionMessage(ExceptionMessage.TypeMismatch, typeof(TValue), typeof(TSourceValue), MethodBase.GetCurrentMethod().Name));
        }

        /// <summary>
        /// Clone the field data.
        /// </summary>
        /// <returns>Cloned field data.</returns>
        public override object Clone()
        {
            if (SourceValue is ICloneable)
            {
                return new FieldData<TSourceValue, TTargetValue>(Field, (TSourceValue) (SourceValue as ICloneable).Clone());
            }
            return new FieldData<TSourceValue, TTargetValue>(Field, SourceValue);
        }

        #endregion
    }
}
