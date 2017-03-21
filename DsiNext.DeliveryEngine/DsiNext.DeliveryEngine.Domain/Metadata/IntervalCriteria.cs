using System;
using System.Text;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Domain.Metadata
{
    /// <summary>
    /// Interval criteria.
    /// </summary>
    /// <typeparam name="TValue">Type of the values used by the criteria.</typeparam>
    public class IntervalCriteria<TValue> : CriteriaBase, IIntervalCriteria where TValue : IComparable
    {
        #region Private variables

        private readonly IField _field;
        private readonly TValue _fromValue;
        private readonly TValue _toValue;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates an interval criteria.
        /// </summary>
        /// <param name="field">Field where the value should be in the interval.</param>
        /// <param name="fromValue">The beginning of the interval.</param>
        /// <param name="toValue">The end of the interval.</param>
        public IntervalCriteria(IField field, TValue fromValue, TValue toValue)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }
            if (Equals(fromValue, null))
            {
                throw new ArgumentNullException("fromValue");
            }
            if (Equals(toValue, null))
            {
                throw new ArgumentNullException("toValue");
            }
            _field = field;
            _fromValue = fromValue;
            _toValue = toValue;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Field for the interval criteria.
        /// </summary>
        public virtual IField Field
        {
            get
            {
                return _field;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// String representation of the criteria.
        /// </summary>
        /// <returns>String representation of the criteria.</returns>
        public override string AsString()
        {
            var stringBuilder = new StringBuilder();
            if (typeof(TValue) == typeof(string))
            {
                stringBuilder.AppendFormat("{0}.Value.CompareTo(\"{1}\") >= 0", Field.NameSource, _fromValue);
                stringBuilder.AppendLine();
                stringBuilder.AppendFormat("{0}.Value.CompareTo(\"{1}\") <= 0", Field.NameSource, _toValue);
                return stringBuilder.ToString();
            }
            stringBuilder.AppendFormat("{0}.Value.CompareTo({1}) >= 0", Field.NameSource, _fromValue);
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("{0}.Value.CompareTo({1}) <= 0", Field.NameSource, _toValue);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// SQL representation of the criteria.
        /// </summary>
        /// <returns>SQL representation of the criteria.</returns>
        public override string AsSql()
        {
            if (typeof(TValue) == typeof(string))
            {
                return string.Format("{0} BETWEEN '{1}' AND '{2}'", Field.NameSource, _fromValue, _toValue);
            }
            return string.Format("{0} BETWEEN {1} AND {2}", Field.NameSource, _fromValue, _toValue);
        }

        /// <summary>
        /// Indicates whether a value does not meet the criteria.
        /// </summary>
        /// <param name="value">Value to test against the criteria.</param>
        /// <returns>Indication of where the value does not meet the criteria.</returns>
        public override bool Exclude(object value)
        {
            return _fromValue.CompareTo(value) > 0 || _toValue.CompareTo(value) < 0;
        }

        #endregion
    }
}
