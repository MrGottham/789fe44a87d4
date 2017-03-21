using System;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Domain.Metadata
{
    /// <summary>
    /// Equal criteria.
    /// </summary>
    /// <typeparam name="TValue">Type of the value used by the criteria.</typeparam>
    public class EqualCriteria<TValue> : CriteriaBase, IEqualCriteria
    {
        #region Private variables

        private readonly IField _field;
        private readonly TValue _equalTo;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates an equal criteria.
        /// </summary>
        /// <param name="field">Field that should be equal to a value.</param>
        /// <param name="equalTo">Value for the criteria.</param>
        public EqualCriteria(IField field, TValue equalTo)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }
            _field = field;
            _equalTo = equalTo;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Field for the equal criteria.
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
            if (Equals(_equalTo, null))
            {
                return string.Format("{0}.Value == null", Field.NameSource);
            }
            if (_equalTo is string)
            {
                return string.Format("Regex.IsMatch({0}.Value, \"^{1}$\")", Field.NameSource, _equalTo);
            }
            return string.Format("Regex.IsMatch({0}.Value.ToString(), \"^{1}$\")", Field.NameSource, _equalTo);
        }

        /// <summary>
        /// SQL representation of the criteria.
        /// </summary>
        /// <returns>SQL representation of the criteria.</returns>
        public override string AsSql()
        {
            if (Equals(_equalTo, null))
            {
                return string.Format("{0} IS NULL", Field.NameSource);
            }
            if (_equalTo is string)
            {
                return string.Format("{0}='{1}'", Field.NameSource, _equalTo);
            }
            return string.Format("{0}={1}", Field.NameSource, _equalTo);
        }

        /// <summary>
        /// Indicates whether a value does not meet the criteria.
        /// </summary>
        /// <param name="value">Value to test against the criteria.</param>
        /// <returns>Indication of where the value does not meet the criteria.</returns>
        public override bool Exclude(object value)
        {
            return Equals(value, _equalTo) == false;
        }

        #endregion
    }
}
