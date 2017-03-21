using System;
using System.Linq;
using System.Text;
using DsiNext.DeliveryEngine.Domain.Interfaces.Metadata;

namespace DsiNext.DeliveryEngine.Domain.Metadata
{
    /// <summary>
    /// Not null criteria.
    /// </summary>
    public class NotNullCriteria : CriteriaBase, INotNullCriteria
    {
        #region Private variables

        private readonly IField _field;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a not null criteria.
        /// </summary>
        /// <param name="field">Field that should not be null.</param>
        public NotNullCriteria(IField field)
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
        /// Field for the not null criteria.
        /// </summary>
        public virtual IField Field
        {
            get
            {
                return _field;
            }
        }

        #endregion

        /// <summary>
        /// String representation of the criteria.
        /// </summary>
        /// <returns>String representation of the criteria.</returns>
        public override string AsString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("Equals({0}.Value, null) == false", Field.NameSource);
            var dataType = Field.DatatypeOfSource;
            if (dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof (Nullable<>))
            {
                dataType = dataType.GetGenericArguments().ElementAt(0);
            }
            if (dataType == typeof (string))
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendFormat("Equals({0}.Value, string.Empty) == false", Field.NameSource);
            }
            if (dataType == typeof (int) || dataType == typeof (long))
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendFormat("Equals({0}.Value, 0) == false", Field.NameSource);
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// SQL representation of the criteria.
        /// </summary>
        /// <returns>SQL representation of the criteria.</returns>
        public override string AsSql()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0} IS NOT NULL", Field.NameSource);
            var dataType = Field.DatatypeOfSource;
            if (dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof (Nullable<>))
            {
                dataType = dataType.GetGenericArguments().ElementAt(0);
            }
            if (dataType == typeof (string))
            {
                stringBuilder.AppendFormat(" AND LENGTH({0})>0", Field.NameSource);
            }
            if (dataType == typeof (int) || dataType == typeof (long))
            {
                stringBuilder.AppendFormat(" AND {0}<>0", Field.NameSource);
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Indicates whether a value does not meet the criteria.
        /// </summary>
        /// <param name="value">Value to test against the criteria.</param>
        /// <returns>Indication of where the value does not meet the criteria.</returns>
        public override bool Exclude(object value)
        {
            if (Equals(value, null))
            {
                return true;
            }
            var dataType = Field.DatatypeOfSource;
            if (dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof (Nullable<>))
            {
                dataType = dataType.GetGenericArguments().ElementAt(0);
            }
            if (dataType == typeof (string))
            {
                return Equals(value, string.Empty);
            }
            if (dataType == typeof (int) || dataType == typeof (long))
            {
                return Equals(value, 0);
            }
            return false;
        }
    }
}
