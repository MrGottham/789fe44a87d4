using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Domain.Metadata
{
    /// <summary>
    /// Pool criteria.
    /// </summary>
    /// <typeparam name="TValue">Type of the values used by the criteria.</typeparam>
    public class PoolCriteria<TValue> : CriteriaBase, IPoolCriteria
    {
        #region Private variables

        private readonly IField _field;
        private readonly IEnumerable<TValue> _poolValues;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a pool criteria.
        /// </summary>
        /// <param name="field">Field where the value should be in the pool.</param>
        /// <param name="poolValues">Values for the pool.</param>
        public PoolCriteria(IField field, IEnumerable<TValue> poolValues)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }
            if (poolValues == null)
            {
                throw new ArgumentNullException("poolValues");
            }
            _field = field;
            _poolValues = poolValues;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Field for the pool criteria.
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
            var pattern = new StringBuilder();
            pattern.Append('^');
            foreach (var poolValue in _poolValues)
            {
                if (pattern.Length > 1)
                    pattern.Append('|');
                pattern.Append(poolValue);
            }
            pattern.Append('$');
            if (typeof(TValue) == typeof(string))
            {
                return string.Format("Regex.IsMatch({0}.Value, \"{1}\")", Field.NameSource, pattern);
            }
            return string.Format("Regex.IsMatch({0}.Value.ToString(), \"{1}\")", Field.NameSource, pattern);
        }

        /// <summary>
        /// SQL representation of the criteria.
        /// </summary>
        /// <returns>SQL representation of the criteria.</returns>
        public override string AsSql()
        {
            var pattern = new StringBuilder();
            pattern.Append('(');
            foreach (var poolValue in _poolValues)
            {
                if (pattern.Length > 1)
                    pattern.Append(", ");
                if (typeof(TValue) == typeof(string))
                    pattern.Append('\'');
                pattern.Append(poolValue);
                if (typeof(TValue) == typeof(string))
                    pattern.Append('\'');
            }
            pattern.Append(')');
            return string.Format("{0} IN {1}", Field.NameSource, pattern);
        }

        /// <summary>
        /// Indicates whether a value does not meet the criteria.
        /// </summary>
        /// <param name="value">Value to test against the criteria.</param>
        /// <returns>Indication of where the value does not meet the criteria.</returns>
        public override bool Exclude(object value)
        {
            return _poolValues.All(poolValue => Equals(value, poolValue) == false);
        }

        #endregion
    }
}
